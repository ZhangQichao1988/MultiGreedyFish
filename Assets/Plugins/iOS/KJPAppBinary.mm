/**
 * Jackpot
 * Copyright(c) 2014 KLab, Inc. All Rights Reserved.
 * Proprietary and Confidential - This source code is not for redistribution
 *
 * Subject to the prior written consent of KLab, Inc(Licensor) and its terms and
 * conditions, Licensor grants to you, and you hereby accept nontransferable,
 * nonexclusive limited right to access, obtain, use, copy and/or download
 * a copy of this product only for requirement purposes. You may not rent,
 * lease, loan, time share, sublicense, transfer, make generally available,
 * license, disclose, disseminate, distribute or otherwise make accessible or
 * available this product to any third party without the prior written approval
 * of Licensor. Unauthorized copying of this product, including modifications
 * of this product or programs in which this product has been merged or included
 * with other software products is expressly forbidden.
 */

#include "KJPAppBinary.h"

#include <dlfcn.h>
#include <mach-o/dyld.h>
#include <mach-o/fat.h>
#include <iomanip>
#include <iostream>
#include <sstream>
#include <string>
#include <type_traits>
#include <valarray>

#import <CommonCrypto/CommonCrypto.h>
#import <Foundation/Foundation.h>

// 関数の難読化目的の置換
#define DecodeObfuscatedString getLogTextString101
#define ToHexString getLogTextString102
#define ToBase64String getLogTextString103
#define GetBaseAddr getLogTextString109
#define FindTextSectionPointer getLogTextString104
#define Sum getLogTextString105
#define CalculateTextBlockHash getLogTextString106
#define SumDylibCommand getLogTextString107
#define CalculatDylibLoadCommandHash getLogTextString108
#define GetTextBlockHash getLogTextString110
#define GetLoadCommandHash getLogTextString111
#define SelectAlgoV2 getLogTextString112

namespace
{
using ByteArray = std::valarray<uint8_t>;
using RangeHasher = ByteArray (*)(char*, uint32_t);
using CommandHasher = ByteArray (*)(load_command*, uint32_t);

enum class HashAlgorithm { MD5 = 0, SHA1 = 1, SHA256 = 2 };

constexpr int APP_BINARY_KEY_NULL_DIGEST = 47;
const auto APP_BINARY_STR_NULL_DIGEST_APP = {
    78, 95, 95, 77, 70, 65, 78, 93, 86, 75, 70, 72, 74, 92, 91,
    88, 78, 92, 65, 64, 91, 78, 89, 78, 70, 67, 78, 77, 67, 74};  // appbinarydigestwasnotavailable
const auto APP_BINARY_STR_NULL_DIGEST_BIN = {
    78, 93, 64, 90, 65, 75, 77, 70, 65, 75, 70, 72, 74, 92, 91,
    88, 78, 92, 65, 64, 91, 78, 89, 78, 70, 67, 78, 77, 67, 74};  // aroundbindigestwasnotavailable
constexpr int APP_BINARY_STR_KEY = 47;
const auto APP_BINARY_STR_TEXT_TAG1 = {112, 112, 123, 106, 119, 123};  // "__TEXT"
const auto APP_BINARY_STR_TEXT_TAG2 = {112, 112, 91, 74, 87, 91};      // "__text"

inline std::string DecodeObfuscatedString(int key, std::initializer_list<int> l)
{
    std::string s(l.size(), 0);
    int i = 0;
    for (int c : l) {
        s[i++] = static_cast<char>(c ^ key);
    }
    return s;
}

struct Xorshift {
    uint64_t x, y, z, w;

    Xorshift(uint64_t seed)
    {
        x = 316190518 ^ seed;
        y = 539795106;
        z = 329098117;
        w = 735408775;
        for (int i = 0; i < 10; ++i)
            next();
    }

    uint64_t next()
    {
        const uint64_t t = x ^ (x << 11);
        x = y;
        y = z;
        z = w;
        w = (w ^ (w >> 19)) ^ (t ^ (t >> 8));
        return w;
    }

    uint8_t next8()
    {
        return static_cast<uint8_t>(next() & 0xFF);
    }
};

std::string ToHexString(const ByteArray& v)
{
    std::stringstream ss;
    const int n = static_cast<int>(v.size());
    for (int i = 0; i < n; ++i) {
        ss << std::hex << std::setfill('0') << std::setw(2) << std::nouppercase << static_cast<int>(v[i]);
    }
    auto result = ss.str();
    ss.str("");
    return result;
}

// ByteArray を Base64String にエンコード
// Base Implementation (Public Domain)
// https://en.wikibooks.org/wiki/Algorithm_Implementation/Miscellaneous/Base64#C.2B.2B
inline std::string ToBase64String(const ByteArray& input)
{
    static const char table[] = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
    std::string result;
    result.reserve(((input.size() / 3) + (input.size() % 3 > 0)) * 4);
    int idx = 0;

    for (size_t i = 0; i < input.size() / 3; ++i) {
        // Convert to big endian
        const auto temp = static_cast<uint32_t>(input[idx]) << 16 | static_cast<uint32_t>(input[idx + 1]) << 8 |
                          static_cast<uint32_t>(input[idx + 2]);
        idx += 3;
        result.append(1, table[(temp & 0x00FC0000) >> 18]);
        result.append(1, table[(temp & 0x0003F000) >> 12]);
        result.append(1, table[(temp & 0x00000FC0) >> 6]);
        result.append(1, table[(temp & 0x0000003F)]);
    }

    if (input.size() % 3 == 1) {
        const auto temp = static_cast<uint32_t>(input[idx]) << 16;
        idx += 1;
        result.append(1, table[(temp & 0x00FC0000) >> 18]);
        result.append(1, table[(temp & 0x0003F000) >> 12]);
        result.append(2, '=');
    } else if (input.size() % 3 == 2) {
        const auto temp = static_cast<uint32_t>(input[idx]) << 16 | static_cast<uint32_t>(input[idx + 1]) << 8;
        idx += 2;
        result.append(1, table[(temp & 0x00FC0000) >> 18]);
        result.append(1, table[(temp & 0x0003F000) >> 12]);
        result.append(1, table[(temp & 0x00000FC0) >> 6]);
        result.append(1, '=');
    }

    return result;
}

template <typename T>
inline void FillZero(T& x)
{
    std::fill(begin(x), end(x), 0);
}

void* GetBaseAddr()
{
    Dl_info dl_info;
    if (dladdr((const void*) ToHexString, &dl_info) == 0) return nullptr;
    return dl_info.dli_fbase;
}

template <HashAlgorithm>
class HashImpl final
{
};

template <>
class HashImpl<HashAlgorithm::MD5>
{
   public:
    HashImpl()
    {
        CC_MD5_Init(&ctx_);
    }

    void Update(char* begin, uint32_t size)
    {
        CC_MD5_Update(&ctx_, begin, size);
    }

    ByteArray Final()
    {
        unsigned char digest[CC_MD5_DIGEST_LENGTH];
        CC_MD5_Final(digest, &ctx_);
        return ByteArray(digest, sizeof(digest));
    }

   private:
    CC_MD5_CTX ctx_;
};

template <>
class HashImpl<HashAlgorithm::SHA1>
{
   public:
    HashImpl()
    {
        CC_SHA1_Init(&ctx_);
    }

    void Update(char* begin, uint32_t size)
    {
        CC_SHA1_Update(&ctx_, begin, size);
    }

    ByteArray Final()
    {
        unsigned char digest[CC_SHA1_DIGEST_LENGTH];
        CC_SHA1_Final(digest, &ctx_);
        return ByteArray(digest, sizeof(digest));
    }

   private:
    CC_SHA1_CTX ctx_;
};

template <>
class HashImpl<HashAlgorithm::SHA256>
{
   public:
    HashImpl()
    {
        CC_SHA256_Init(&ctx_);
    }

    void Update(char* begin, uint32_t size)
    {
        CC_SHA256_Update(&ctx_, begin, size);
    }

    ByteArray Final()
    {
        unsigned char digest[CC_SHA256_DIGEST_LENGTH];
        CC_SHA256_Final(digest, &ctx_);
        return ByteArray(digest, sizeof(digest));
    }

   private:
    CC_SHA256_CTX ctx_;
};

template <typename T>
struct IsMachOHeaderType final {
    typedef void TySection;
    typedef void TySegmentCommand;
    static constexpr int LoadSegmentCommand = 0;
    static constexpr bool Value = false;
};

template <>
struct IsMachOHeaderType<mach_header> {
    typedef section TySection;
    typedef segment_command TySegmentCommand;
    static constexpr int LoadSegmentCommand = LC_SEGMENT;
    static constexpr bool Value = true;
};

template <>
struct IsMachOHeaderType<mach_header_64> {
    typedef section_64 TySection;
    typedef segment_command_64 TySegmentCommand;
    static constexpr int LoadSegmentCommand = LC_SEGMENT_64;
    static constexpr bool Value = true;
};

template <typename T>
typename IsMachOHeaderType<T>::TySection* FindTextSectionPointer(T* header)
{
    static_assert(IsMachOHeaderType<T>::Value, "Non-mach header type given");

    auto seg_name = DecodeObfuscatedString(APP_BINARY_STR_KEY, APP_BINARY_STR_TEXT_TAG1);   // "__TEXT"
    auto sect_name = DecodeObfuscatedString(APP_BINARY_STR_KEY, APP_BINARY_STR_TEXT_TAG2);  // "__text"

    load_command* cmd = reinterpret_cast<load_command*>(header + 1);

    for (int i = 0; cmd != nullptr && i < header->ncmds; i++) {
        if (cmd->cmd != IsMachOHeaderType<T>::LoadSegmentCommand) {
            cmd = reinterpret_cast<load_command*>(reinterpret_cast<char*>(cmd) + cmd->cmdsize);
            continue;
        }
        auto segment_p = reinterpret_cast<typename IsMachOHeaderType<T>::TySegmentCommand*>(cmd);
        if (seg_name.compare(segment_p->segname)) {
            cmd = reinterpret_cast<load_command*>(reinterpret_cast<char*>(cmd) + cmd->cmdsize);
            continue;
        }
        auto section_p = reinterpret_cast<typename IsMachOHeaderType<T>::TySection*>(segment_p + 1);
        for (int j = 0; section_p != nullptr && j < segment_p->nsects; j++) {
            if (!sect_name.compare(section_p->sectname)) {
                FillZero(seg_name);
                FillZero(sect_name);
                return section_p;
            }
            section_p = reinterpret_cast<typename IsMachOHeaderType<T>::TySection*>(segment_p + 1);
        }
    }

    FillZero(seg_name);
    FillZero(sect_name);

    return nullptr;
}

template <HashAlgorithm Algo>
ByteArray SumRange(char* begin, uint32_t size)
{
    HashImpl<Algo> h;
    h.Update(begin, size);
    return h.Final();
}

template <HashAlgorithm Algo>
ByteArray SumDylibCommand(load_command* cmd, uint32_t cmd_count)
{
    HashImpl<Algo> h;

    for (int i = 0; cmd != nullptr && i < cmd_count; i++) {
        switch (cmd->cmd) {
            case LC_LOAD_DYLIB:
            case LC_ID_DYLIB:
            case LC_LOAD_WEAK_DYLIB:
            case LC_REEXPORT_DYLIB:
            case LC_LAZY_LOAD_DYLIB:
            case LC_LOAD_UPWARD_DYLIB:
                h.Update(reinterpret_cast<char*>(cmd), cmd->cmdsize);
        }
        cmd = reinterpret_cast<load_command*>(reinterpret_cast<char*>(cmd) + cmd->cmdsize);
    }

    return h.Final();
}

constexpr RangeHasher SectionHashers[] = {SumRange<HashAlgorithm::MD5>, SumRange<HashAlgorithm::SHA1>,
                                          SumRange<HashAlgorithm::SHA256>};

constexpr CommandHasher DylibCommandHashers[] = {
    SumDylibCommand<HashAlgorithm::MD5>, SumDylibCommand<HashAlgorithm::SHA1>, SumDylibCommand<HashAlgorithm::SHA256>};

template <typename T>
ByteArray CalculateTextBlockHash(HashAlgorithm algo, T* header)
{
    static_assert(IsMachOHeaderType<T>::Value, "Non-mach header type given");

    auto section_ptr = FindTextSectionPointer(header);  // auto section_ptr is section or section_64
    if (!section_ptr) return ByteArray();

    char* text_section_ptr = reinterpret_cast<char*>(header) + section_ptr->offset;
    return SectionHashers[static_cast<int>(algo)](text_section_ptr, (uint32_t)section_ptr->size);
}

template <typename T>
ByteArray CalculatDylibLoadCommandHash(HashAlgorithm algo, T* header)
{
    auto cmd = reinterpret_cast<load_command*>(header + 1);
    return DylibCommandHashers[static_cast<int>(algo)](cmd, header->ncmds);
}

ByteArray GetTextBlockHash(HashAlgorithm algo, void* base_addr)
{
    if (base_addr == nullptr) return ByteArray();

    uint32_t magic = *static_cast<uint32_t*>(base_addr);
    switch (magic) {
        case MH_MAGIC:
        case MH_CIGAM:
            return CalculateTextBlockHash(algo, static_cast<mach_header*>(base_addr));
        case MH_MAGIC_64:
        case MH_CIGAM_64:
            return CalculateTextBlockHash(algo, static_cast<mach_header_64*>(base_addr));
    }

    return ByteArray();
}

ByteArray GetLoadCommandHash(HashAlgorithm algo, void* base_addr)
{
    if (base_addr == nullptr) return ByteArray();

    uint32_t magic = *static_cast<uint32_t*>(base_addr);
    switch (magic) {
        case MH_MAGIC:
        case MH_CIGAM:
            return CalculatDylibLoadCommandHash(algo, static_cast<mach_header*>(base_addr));
        case MH_MAGIC_64:
        case MH_CIGAM_64:
            return CalculatDylibLoadCommandHash(algo, static_cast<mach_header_64*>(base_addr));
    }

    return ByteArray();
}

inline HashAlgorithm SelectAlgoV2(int num)
{
    return static_cast<HashAlgorithm>(abs(num) % 3);
}
}

std::string GenerateAppBinaryHashV2(const char* seed)
{
    const int order = seed[0];
    const auto algo1 = SelectAlgoV2(seed[1]);
    const auto algo2 = SelectAlgoV2(seed[2]);
    auto null_digest_str_app = DecodeObfuscatedString(APP_BINARY_KEY_NULL_DIGEST, APP_BINARY_STR_NULL_DIGEST_APP);
    auto null_digest_str_bin = DecodeObfuscatedString(APP_BINARY_KEY_NULL_DIGEST, APP_BINARY_STR_NULL_DIGEST_BIN);

    std::string s1, s2;

    if (order % 2 == 0) {
        s1 = ToHexString(GetTextBlockHash(algo1, GetBaseAddr()));
        s2 = ToHexString(GetLoadCommandHash(algo2, GetBaseAddr()));
        if (s1.empty()) s1 = std::string(null_digest_str_app);
        if (s2.empty()) s2 = std::string(null_digest_str_bin);
    } else {
        s1 = ToHexString(GetLoadCommandHash(algo1, GetBaseAddr()));
        s2 = ToHexString(GetTextBlockHash(algo2, GetBaseAddr()));
        if (s1.empty()) s1 = std::string(null_digest_str_bin);
        if (s2.empty()) s2 = std::string(null_digest_str_app);
    }

    auto digest = s1 + "-" + s2;
    Xorshift rnd(static_cast<uint32_t>(seed[0]) | static_cast<uint32_t>(seed[1]) << 8 |
                 static_cast<uint32_t>(seed[2]) << 16 | static_cast<uint32_t>(seed[3]) << 24);
    ByteArray encrypted(digest.size());
    for (int i = 0; i < static_cast<int>(digest.size()); ++i) {
        encrypted[i] = static_cast<uint8_t>(digest[i]) ^ rnd.next8();
    }

    const auto result = ToBase64String(encrypted);

    FillZero(s1);
    FillZero(s2);
    FillZero(digest);
    FillZero(null_digest_str_app);
    FillZero(null_digest_str_bin);
    FillZero(encrypted);

    return result;
}

#ifdef APP_BINARY_LOCAL_DEBUG
// These codes are for local debugging and testing.
// You can use following commands to build / run for testing as OSX binary.
// $clang++ -std=c++11 -DAPP_BINARY_LOCAL_DEBUG -framework Foundation
// KJPAppBinary.mm -o KJPAppBinary.out

int main(int argc, char* argv[])
{
    for (int i = 1; i < argc; ++i) {
        if (std::string(argv[i]) == "-macho") {
            FILE* f = fopen(argv[i + 1], "rb");
            fseek(f, 0, SEEK_END);
            auto fsize = ftell(f);
            fseek(f, 0, SEEK_SET);

            char* buf = static_cast<char*>(malloc(fsize));
            fread(buf, fsize, 1, f);
            fclose(f);

            std::cout << "TEXT section hash of this binary" << std::endl;
            std::cout << "MD5:" << ToHexString(GetTextBlockHash(HashAlgorithm::MD5, buf)) << std::endl;
            std::cout << "SHA1:" << ToHexString(GetTextBlockHash(HashAlgorithm::SHA1, buf)) << std::endl;
            std::cout << "SHA256:" << ToHexString(GetTextBlockHash(HashAlgorithm::SHA256, buf)) << std::endl;

            free(buf);
        }
    }

    std::cout << "AppBinaryHash(1234):" << GenerateAppBinaryHashV2("1234") << std::endl;
    std::cout << "AppBinaryHash(0000):" << GenerateAppBinaryHashV2("0000") << std::endl;
    std::cout << "AppBinaryHash(1111):" << GenerateAppBinaryHashV2("1111") << std::endl;
    std::cout << "AppBinaryHash(2222):" << GenerateAppBinaryHashV2("2222") << std::endl;

    std::cout << "TextBlockHash:" << std::endl;
    std::cout << "MD5:" << ToHexString(GetTextBlockHash(HashAlgorithm::MD5, GetBaseAddr())) << std::endl;
    std::cout << "SHA1:" << ToHexString(GetTextBlockHash(HashAlgorithm::SHA1, GetBaseAddr())) << std::endl;
    std::cout << "SHA256:" << ToHexString(GetTextBlockHash(HashAlgorithm::SHA256, GetBaseAddr())) << std::endl;

    std::cout << "LoadCommandHash:" << std::endl;
    std::cout << "MD5:" << ToHexString(GetLoadCommandHash(HashAlgorithm::MD5, GetBaseAddr())) << std::endl;
    std::cout << "SHA1:" << ToHexString(GetLoadCommandHash(HashAlgorithm::SHA1, GetBaseAddr())) << std::endl;
    std::cout << "SHA256:" << ToHexString(GetLoadCommandHash(HashAlgorithm::SHA256, GetBaseAddr())) << std::endl;
}
#endif

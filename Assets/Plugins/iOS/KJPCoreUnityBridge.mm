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

//
// KJPCoreUnityBridge.mm
// Jackpot::Core向けUnityBridge
//

#import <CommonCrypto/CommonCrypto.h>
#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>
#import <UIKit/UIKit.h>
#import <mach/mach.h>
#import <sys/sysctl.h>
#import "KJPAppBinary.h"
#import "KJPUnityBridgeCommon.h"
#import <Foundation/NSProcessInfo.h>

#pragma mark Bridge Interface

#ifdef __cplusplus
extern "C" {
#endif

/*!
 @function  _KJPCore_GetFreeSpace
 @abstract  端末の空き領域を取得します
 @return    端末の空き領域を返却(単位はKB)
 */
long long _KJPCore_GetFreeSpace();

/*!
 @function  _KJPCore_GetCurrentLocaleIdentifier()
 @abstract  端末のロケールIDを返却します
 @return    端末のロケールID
 */
const char* _KJPCore_GetCurrentLocaleIdentifier();

/*!
 @function  _KJPCore_GetCurrentLocaleCountryCode()
 @abstract  端末の国コードを返却します
 @return    端末の国コード
 */
const char* _KJPCore_GetCurrentLocaleCountryCode();

/*!
 @function  _KJPCore_GetCurrentLocaleLanguageCode()
 @abstract  端末の言語を返却します
 @return    端末の言語
 */
const char* _KJPCore_GetCurrentLocaleLanguageCode();

/*!
 @function   _KJPCore_GetDeviceTimeZone()
 @abstract   端末のタイムゾーン情報を返します。(e.g. "Asia/Tokyo")
 @return     端末のタイムゾーン情報
 */
const char* _KJPCore_GetDeviceTimeZone();

/*!
 @function  _KJPCore_GetBundleIdentifier()
 @abstract  bundle identifierを返却します
 @return    bundle identifier
 */
const char* _KJPCore_GetBundleIdentifier();

/*!
 @function  _KJPCore_GetMainBundlePlistInfo()
 @abstract  info.plistの、指定したキーの内容を返却します
 @param     key     info.plistに指定したkey
 @return    info
 */
const char* _KJPCore_GetMainBundlePlistInfo(const char* key);

/*!
 @function  _KJPCore_GetApplicationHome()
 @abstract  NSHomeDirectory()を返却します
 @return    NSHomeDirectory()
 */
const char* _KJPCore_GetHomeDirectory();

/*
 @function  _KJPCore_VerifyExcludedPathFromBackup()
 @abstract  指定したディレクトリがバックアップ対象から外されているか検証します(先に、ディレクトリを作る必要があります)
 @param     cFullPath    検証ディレクトリのフルパス
 @return    外されていれば空文字列, 外されてなければ"NotExcluded", 検証時にエラーが発生した場合はerror.domain
 */
const char* _KJPCore_VerifyExcludedPathFromBackup(const char* cFullPath);

/*!
 @function  _KJPCore_ExcludePathFromBackup()
 @abstract  指定したディレクトリをバックアップ対象から外します(先に、ディレクトリを作る必要があります)
 @param     cFullPath    バックアップ対象から外すディレクトリのフルパス
 @return    エラーが発生していたら、error.domain, エラーなしの場合は空文字列
 */
const char* _KJPCore_ExcludePathFromBackup(const char* cFullPath);

/*!
 @function  _KJPCore_Log
 @abstract  NSLogを通してログ出力をおこないます
 @param     message ログ出力するメッセージを指定します
 */
void _KJPCore_Log(const char* message);

/*!
 @function  _KJPCore_GetOSVersion
 @abstract  端末のOSVersionを返却します
 @return    端末のOSVersion
 */
const char* _KJPCore_GetOSVersion();

/*!
 @function  _KJPCore_GetUsedMemory
 @abstract  Appのメモリ使用量を返します
 @return    Appのメモリ使用量
 */
unsigned int _KJPCore_GetUsedMemory();

/*!
 @function  _KJPCore_GetFreeMemory
 @abstract  Appの未使用メモリ量を返します
 @return    Appの未使用メモリ量
 */
unsigned long _KJPCore_GetFreeMemory();

/*!
 @function  _KJPCore_GetTotalMemory
 @abstract  iPhoneの全メモリ量を返します
 @return    iPhoneの全メモリ量
 */
unsigned long long _KJPCore_GetTotalMemory();

/*!
 @function  _KJPCore_LoadResemaraDetectionIdentifier
 @abstract  リセマラ判定用端末固有IDを返却します
 @return    リセマラ判定用端末固有ID
 */
const char* _KJPCore_LoadResemaraDetectionIdentifier();

/*!
 @function  _KJPCore_SubscribeData
 @abstract  自プロセスが他プロセスにアタッチされているか否かを示します。名前はわざとです
 @param     output  アタッチされているか否かを示します
 @param     err     検出時にエラーが発生したか否かを示します
 */
void _KJPCore_SubscribeData(BOOL* output, BOOL* err);

/*!
 @function  _KJPCore_GetBatteryStatus
 @abstract  バッテリーの充電状態を返却します
 @return    バッテリーの充電状態
 */
int _KJPCore_GetBatteryStatus();

/*!
 @function  _KJPCore_GetBatteryStatus
 @abstract  バッテリーの残量を返却します
 @return    バッテリーの残量
 */
const float _KJPCore_GetBatteryLevel();

/*!
 @function  _KJPCore_LaunchApp
 @abstract  指定されたURLスキーマでアプリを起動します。　端末にインストールされていない場合はFlaseを返却します
 @param     urlScheme  起動するアプリのURLスキーマ
 @param     appId      AppStoreで遷移するアプリのAppId
 @return    指定されたURLスキーマでアプリ起動したかを返却します
*/
bool _KJPCore_LaunchApp(const char* urlScheme, const char* appId);

/*!
 @function  _KJPCore_StoreReview
 @abstract  iOS標準のレビュー誘導ダイアログを表示します。　iOS10.3未満の端末を使用している場合はFlaseを返却します
 @return    iOS10.3未満の端末を使用している場合はFlaseを返却します。それ以外であればtrueを返却します
*/
bool _KJPCore_StoreReview();

/*!
 @function  _KJACore_AssetStateLogGenerateV2
 @abstract  iOSのアプリ改竄対策としてのバイナリのハッシュ値を取得します。
 @return    バイナリのハッシュ値を返します
 */
void _KJACore_AssetStateLogGenerateV2(char* buf, int bufsize, const char* seed);

/*!
 @function  _KJACore_GetThermalState
 @abstract  iOS端末のシステム温度状態のしきい値を取得します。
 @return    iOS11.0以上の端末を使用している場合はシステム温度状態のしきい値をを返却します、それ以外の端末は、-1の値を返却します。
 */
int _KJPCore_GetThermalState();

/*!
 @function  _KJPCore_GetMaximumFramesPerSecond
 @abstract  iOS端末の最高FPSを取得します。
 @return    FPS値を返します。
 */
int _KJPCore_GetMaximumFramesPerSecond();

#ifdef __cplusplus
}
#endif

#pragma mark Bridge Implementations

long long _KJPCore_GetFreeSpace()
{
    NSError* error = nil;
    NSArray* paths = NSSearchPathForDirectoriesInDomains(NSLibraryDirectory, NSUserDomainMask, YES);
    long long freeSpace = 0;
    if (@available(iOS 11.0, *)) {
        NSURL *fileURL = [[NSURL alloc] initFileURLWithPath: [paths lastObject]];
        NSDictionary* dictionary =
        [fileURL resourceValuesForKeys:@[NSURLVolumeAvailableCapacityForImportantUsageKey] error:&error];
        if (dictionary) {
            freeSpace =
            [[dictionary objectForKey:NSURLVolumeAvailableCapacityForImportantUsageKey] longLongValue] / 1024;
        }
    } else {
        NSDictionary* dictionary =
        [[NSFileManager defaultManager] attributesOfFileSystemForPath:[paths lastObject] error:&error];
        if (dictionary) {
            freeSpace = [[dictionary objectForKey:NSFileSystemFreeSize] longLongValue] / 1024;
        }
    }
    return freeSpace;
}

const char* _KJPCore_GetCurrentLocaleIdentifier()
{
    NSString* identifier = [[NSLocale currentLocale] objectForKey:NSLocaleIdentifier];
    return identifier != nil ? KJPMakeStringCopy([identifier UTF8String]) : KJPMakeStringCopy([@"" UTF8String]);
}

const char* _KJPCore_GetCurrentLocaleCountryCode()
{
    NSString* country = [[NSLocale currentLocale] objectForKey:NSLocaleCountryCode];
    return country != nil ? KJPMakeStringCopy([country UTF8String]) : KJPMakeStringCopy([@"" UTF8String]);
}

const char* _KJPCore_GetCurrentLocaleLanguageCode()
{
    NSString* language;
    if (KJPOsVersion() >= 11.0) {
        NSString *localeID = NSLocale.preferredLanguages.firstObject;
        NSLocale *deviceLocale = [NSLocale localeWithLocaleIdentifier:localeID];
        language = [deviceLocale objectForKey:NSLocaleLanguageCode];
    } else {
        language = [[NSLocale currentLocale] objectForKey:NSLocaleLanguageCode];
    }
    return language != nil ? KJPMakeStringCopy(language.UTF8String) : KJPMakeStringCopy(@"".UTF8String);
}

const char* _KJPCore_GetDeviceTimeZone()
{
    NSString* currentRegion = [[NSTimeZone systemTimeZone] name];
    return KJPMakeStringCopy([currentRegion UTF8String]);
}

const char* _KJPCore_GetBundleIdentifier()
{
    NSString* bundleIdentifier = [[NSBundle mainBundle] bundleIdentifier];
    return KJPMakeStringCopy([bundleIdentifier UTF8String]);
}

const char* _KJPCore_GetMainBundlePlistInfo(const char* key)
{
    NSString* keyStr = KJPStringWithCString(key);
    NSString* value = [[NSBundle mainBundle] objectForInfoDictionaryKey:keyStr];
    return KJPMakeStringCopy([value UTF8String]);
}

const char* _KJPCore_GetHomeDirectory()
{
    NSString* homeDirectory = NSHomeDirectory();
    return KJPMakeStringCopy([homeDirectory UTF8String]);
}

const char* _KJPCore_VerifyExcludedPathFromBackup(const char* cFullPath)
{
    NSString* fullPath = KJPStringWithCString(cFullPath);
    NSError* error = nil;
    NSURL* fullUrl = [NSURL fileURLWithPath:fullPath];

    // verify do not needed
    NSNumber* excluded = nil;
    [fullUrl getResourceValue:&excluded forKey:NSURLIsExcludedFromBackupKey error:&error];
    if (error != nil) {
        // OMG! :(
        return KJPMakeStringCopy([error.domain UTF8String]);
    }
    return [excluded boolValue] ? KJPMakeStringCopy([@"" UTF8String]) : KJPMakeStringCopy([@"NotExcluded" UTF8String]);
}

const char* _KJPCore_ExcludePathFromBackup(const char* cFullPath)
{
    NSString* fullPath = KJPStringWithCString(cFullPath);
    NSError* error = nil;
    NSURL* fullUrl = [NSURL fileURLWithPath:fullPath];

    [fullUrl setResourceValue:[NSNumber numberWithBool:YES] forKey:NSURLIsExcludedFromBackupKey error:&error];
    return error != nil ? KJPMakeStringCopy([error.domain UTF8String]) : KJPMakeStringCopy([@"" UTF8String]);
}

void _KJPCore_Log(const char* message)
{
    NSString* messageStr = KJPStringWithCString(message);
    NSLog(@"%@", messageStr);
}

const char* _KJPCore_GetOSVersion()
{
    NSString* version = [[UIDevice currentDevice] systemVersion];
    return version != nil ? KJPMakeStringCopy([version UTF8String]) : KJPMakeStringCopy([@"" UTF8String]);
}

unsigned int _KJPCore_GetUsedMemory()
{
    struct task_basic_info basic_info;
    mach_msg_type_number_t t_info_count = TASK_BASIC_INFO_COUNT;
    kern_return_t status;

    status = task_info(current_task(), TASK_BASIC_INFO, (task_info_t) &basic_info, &t_info_count);

    if (status != KERN_SUCCESS) {
        NSLog(@"%s(): Error in task_info(): %s", __FUNCTION__, strerror(errno));
        return 0;
    }

    return (unsigned int) basic_info.resident_size;
}

unsigned long _KJPCore_GetFreeMemory()
{
    mach_port_t host_port;
    mach_msg_type_number_t host_size;
    vm_size_t pagesize;
    
    host_port = mach_host_self();
    host_size = sizeof(vm_statistics_data_t) / sizeof(integer_t);
    host_page_size(host_port, &pagesize);
    vm_statistics_data_t vm_stat;
    
    if (host_statistics(host_port, HOST_VM_INFO, (host_info_t)&vm_stat, &host_size) != KERN_SUCCESS) {
        return 0;
    }
    
    return (unsigned long)vm_stat.free_count * (unsigned long)pagesize;
}

unsigned long long _KJPCore_GetTotalMemory()
{
    NSProcessInfo *processInfo = [NSProcessInfo processInfo];
    return (unsigned long long) processInfo.physicalMemory;
}

const char* _KJPCore_LoadResemaraDetectionIdentifier()
{
    NSString* const UIApplication_UIID_Key = @"jackpotUniversallyUniqueIdentifier";
    NSString* resemaraDetectionIdString = nil;

    NSDictionary* query = @{
        (__bridge id) kSecClass : (__bridge id) kSecClassGenericPassword,
        (__bridge id) kSecAttrGeneric : UIApplication_UIID_Key,
        (__bridge id) kSecAttrAccount : UIApplication_UIID_Key,
        (__bridge id) kSecAttrService : [[NSBundle mainBundle] bundleIdentifier],
        (__bridge id) kSecMatchLimit : (__bridge id) kSecMatchLimitOne,
        (__bridge id) kSecReturnData : (__bridge id) kCFBooleanTrue
    };

    CFTypeRef dataRef = NULL;
    OSStatus result = SecItemCopyMatching((__bridge CFDictionaryRef) query, &dataRef);
    if (result == noErr) {
        NSData* passwordData = (__bridge NSData*) dataRef;
        resemaraDetectionIdString = [[NSString alloc] initWithBytes:[passwordData bytes]
                                                             length:[passwordData length]
                                                           encoding:NSUTF8StringEncoding];
    }

    if (dataRef) {
        CFRelease(dataRef);
    }

    if (resemaraDetectionIdString == nil) {
        NSUUID* uuid = [NSUUID UUID];
        NSString* bundleId = [[NSBundle mainBundle] bundleIdentifier];
        NSString* resemaraBundleId = [NSString stringWithFormat:@"%@ %@", uuid.UUIDString, bundleId];

        // MD5ハッシュ値の取得
        const char* data = [resemaraBundleId UTF8String];
        if (uuid.UUIDString.length == 0) {
            return nil;
        }
        CC_LONG len = (CC_LONG) strlen(data);
        unsigned char result[CC_MD5_DIGEST_LENGTH];
        CC_MD5(data, len, result);
        NSMutableString* ms = @"".mutableCopy;
        for (int i = 0; i < 16; i++) {
            [ms appendFormat:@"%02X", result[i]];
        }
        resemaraDetectionIdString = ms;

        // keychainに保存
        NSDictionary* query = @{
            (__bridge id) kSecClass : (__bridge id) kSecClassGenericPassword,
            (__bridge id) kSecAttrGeneric : UIApplication_UIID_Key,
            (__bridge id) kSecAttrAccount : UIApplication_UIID_Key,
            (__bridge id) kSecAttrService : [[NSBundle mainBundle] bundleIdentifier],
            (__bridge id) kSecAttrAccessible : (__bridge id) kSecAttrAccessibleAfterFirstUnlock,
            (__bridge id) kSecValueData : [resemaraDetectionIdString dataUsingEncoding:NSUTF8StringEncoding]
        };

        OSStatus resultOSStatus = SecItemAdd((__bridge CFDictionaryRef) query, NULL);
        if (resultOSStatus != noErr) {
            NSLog(@"%s(): Error in task_info(): %s", __FUNCTION__, strerror(errno));
            return nil;
        }
    }
    return KJPMakeStringCopy([resemaraDetectionIdString UTF8String]);
}

void _KJPCore_SubscribeData(BOOL* output, BOOL* err)
{
    struct kinfo_proc info;
    size_t size_info = sizeof(info);
    pid_t pid = getpid();
    int name[] = {CTL_KERN, KERN_PROC, KERN_PROC_PID, pid};
    if (sysctl(name, 4, &info, &size_info, NULL, 0) == -1) {
        *err = YES;
        *output = NO;
        return;
    }
    *err = NO;
    *output = (info.kp_proc.p_flag & P_TRACED) != 0 ? YES : NO;
}

int _KJPCore_GetBatteryStatus()
{
    [UIDevice currentDevice].batteryMonitoringEnabled = YES;
    return static_cast<int>([UIDevice currentDevice].batteryState);
}

const float _KJPCore_GetBatteryLevel()
{
    [UIDevice currentDevice].batteryMonitoringEnabled = YES;
    return [UIDevice currentDevice].batteryLevel;
}

bool _KJPCore_LaunchApp(const char* urlScheme, const char* appId)
{
    NSString* url = KJPStringWithCString(urlScheme);
    BOOL canOpen = [[UIApplication sharedApplication] canOpenURL:[NSURL URLWithString:url]];

    if (canOpen) {
        // アプリ起動
        [[UIApplication sharedApplication] openURL:[NSURL URLWithString:url]];
        return TRUE;
    } else {
        return FALSE;
    }
}

bool _KJPCore_StoreReview()
{
    if (KJPOsVersion() >= 10.3) {
        [SKStoreReviewController requestReview];
        return TRUE;
    } else {
        return FALSE;
    }
}

void _KJACore_AssetStateLogGenerateV2(char* buf, int bufsize, const char* seed)
{
    if (bufsize <= 0) return;

    std::string ret = GenerateAppBinaryHashV2(seed);
    if (ret.length() > 0 && ret.length() < bufsize) {
        memcpy(buf, ret.c_str(), ret.length());
        buf[ret.length()] = 0;
    } else {
        buf[0] = 0;
    }
    std::fill(ret.begin(), ret.end(), 0);
}

int _KJPCore_GetThermalState(){
    if (KJPOsVersion() >= 11.0) {
        NSProcessInfoThermalState processInfoThermalState = [[NSProcessInfo processInfo] thermalState];
        return processInfoThermalState;
    }
    else{
        return -1;
    }
}

int _KJPCore_GetMaximumFramesPerSecond() {
    if (@available(iOS 10.3, *)) {
         return UIScreen.mainScreen.maximumFramesPerSecond;
    }
    else{
        return -1;
    }
}

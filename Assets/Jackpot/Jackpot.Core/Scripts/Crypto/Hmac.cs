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
using System;
using System.Security.Cryptography;

namespace Jackpot
{
    public static class Hmac
    {
#region Factory methods

        /// <summary>
        /// The md5 factory.
        /// </summary>
        static HMAC Md5Factory(byte[] key)
        {
            if (key == null)
            {
                return new HMACMD5();
            }
            return new HMACMD5(key);
        }

#if !NET_STANDARD_2_0
        /// <summary>
        /// The ripemd160 factory.
        /// </summary>
        static HMAC Ripemd160Factory(byte[] key)
        {
            if (key == null)
            {
                return new HMACRIPEMD160();
            }
            return new HMACRIPEMD160(key);
        }
#endif

        /// <summary>
        /// The sha1 factory.
        /// </summary>
        static HMAC Sha1Factory(byte[] key)
        {
            if (key == null)
            {
                return new HMACSHA1();
            }
            return new HMACSHA1(key, false);
        }

        /// <summary>
        /// The sha256 factory.
        /// </summary>
        static HMAC Sha256Factory(byte[] key)
        {
            if (key == null)
            {
                return new HMACSHA256();
            }
            return new HMACSHA256(key);
        }

        /// <summary>
        /// The sha384 factory.
        /// </summary>
        static HMAC Sha384Factory(byte[] key)
        {
            if (key == null)
            {
                return new HMACSHA384();
            }
            return new HMACSHA384(key);
        }

        /// <summary>
        /// The sha512 factory.
        /// </summary>
        static HMAC Sha512Factory(byte[] key)
        {
            if (key == null)
            {
                return new HMACSHA512();
            }
            return new HMACSHA512(key);
        }

#endregion

#region Public methods

        /// <summary>
        /// Factory Methodを指定してHMAC方式を使用したハッシュ値を生成します。
        /// </summary>
        /// <returns><c>true</c> if hash hmacFactory key data; otherwise, <c>false</c>.</returns>
        /// <param name="hmacFactory">Hmac factory.</param>
        /// <param name="key">Key.</param>
        /// <param name="data">Data.</param>
        public static string Hash(Func<byte[], HMAC> hmacFactory, byte[] key, byte[] data)
        {
            byte[] hashed;
            using (var hmac = hmacFactory(key))
            {
                hashed = hmac.ComputeHash(data);
            }
            return BitConverter.ToString(hashed).ToLower().Replace("-", "");
        }

        /// <summary>
        /// MD5を利用してHMAC方式のハッシュ値を生成します。
        /// </summary>
        /// <returns>The hash.</returns>
        /// <param name="key">Key.</param>
        /// <param name="data">Data.</param>
        public static string Md5Hash(byte[] key, byte[] data)
        {
            return Hash(Md5Factory, key, data);
        }

#if !NET_STANDARD_2_0
        /// <summary>
        /// RIPEMD-160を利用してHMAC方式のハッシュ値を生成します。
        /// </summary>
        /// <returns>The hash.</returns>
        /// <param name="key">Key.</param>
        /// <param name="data">Data.</param>
        public static string Ripemd160Hash(byte[] key, byte[] data)
        {
            return Hash(Ripemd160Factory, key, data);
        }
#endif

        /// <summary>
        /// SHA-1を利用してHMAC方式のハッシュ値を生成します。
        /// </summary>
        /// <returns>The hash.</returns>
        /// <param name="key">Key.</param>
        /// <param name="data">Data.</param>
        public static string Sha1Hash(byte[] key, byte[] data)
        {
            return Hash(Sha1Factory, key, data);
        }

        /// <summary>
        /// SHA-256を利用してHMAC方式のハッシュ値を生成します。
        /// </summary>
        /// <returns>The hash.</returns>
        /// <param name="key">Key.</param>
        /// <param name="data">Data.</param>
        public static string Sha256Hash(byte[] key, byte[] data)
        {
            return Hash(Sha256Factory, key, data);
        }

        /// <summary>
        /// SHA-384を利用してHMAC方式のハッシュ値を生成します。
        /// </summary>
        /// <returns>The hash.</returns>
        /// <param name="key">Key.</param>
        /// <param name="data">Data.</param>
        public static string Sha384Hash(byte[] key, byte[] data)
        {
            return Hash(Sha384Factory, key, data);
        }

        /// <summary>
        /// SHA-512を利用してHMAC方式のハッシュ値を生成します。
        /// </summary>
        /// <returns>The hash.</returns>
        /// <param name="key">Key.</param>
        /// <param name="data">Data.</param>
        public static string Sha512Hash(byte[] key, byte[] data)
        {
            return Hash(Sha512Factory, key, data);
        }

#endregion
    }
}

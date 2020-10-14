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
using System.Text;

namespace Jackpot
{
    /// <summary>
    /// URI エンコードの為のヘルパーユーティリティです。
    /// </summary>
    public static class UriHelper
    {
        static readonly string hexLowerChar = "0123456789abcdef";

        /// <summary>
        /// 文字列をLowerCaseでパーセントエンコードします
        /// </summary>
        /// <param name="toEscape">To escape.</param>
        public static string Escape(string toEscape)
        {
            if (toEscape == null)
            {
                throw new ArgumentNullException("toEscape");
            }

            var builder = new StringBuilder(toEscape.Length);

            foreach (var b in Encoding.UTF8.GetBytes(toEscape))
            {
                var c = (char) b;
                builder.Append(NeedToEscapeDataChar(c) ? HexEscape(c) : c);
            }

            return builder.ToString();
        }

        /// <summary>
        /// 渡された文字がURIエンコードのエスケープ処理を必要とするか否かを判定します
        /// </summary>
        /// <remarks>
        /// <see cref="UnityEngine.WWW.EscapeURL"/> の仕様に準拠した判定を行います。
        /// そのため、System.Uriとはエスケープ処理の結果が異なります 
        /// </remarks>
        /// <returns><c>true</c>, if to escape data char was needed, <c>false</c> otherwise.</returns>
        /// <param name="b">The blue component.</param>
        static bool NeedToEscapeDataChar(char b)
        {
            return !((b >= 'A' && b <= 'Z') ||
                    (b >= 'a' && b <= 'z') ||
                    (b >= '0' && b <= '9') ||
                    // (b >= '&' && b <= ';') ||
                    b == '_' || b == '~' || b == '-'
                    // System.Uri.EscapeUriString() では以下のASCII文字は変換しない文字ですが
                    // UnityEngine.WWW.EscapeURLではエスケープの対象となります
                    // b == '!' || b == '#' || b == '$' ||
                    // b == '=' || b == '?' || b == '@'
                    );
        }

        static object HexEscape(char character)
        {
            if (character > 255)
            {
                throw new ArgumentOutOfRangeException ("character");
            }

            return "%" + hexLowerChar[((character & 0xf0) >> 4)] + hexLowerChar[((character & 0x0f))];
        }
    }
}

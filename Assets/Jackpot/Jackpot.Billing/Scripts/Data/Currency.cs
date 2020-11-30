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
using System.Collections.Generic;
using System.Globalization;
using Jackpot.Extensions;

namespace Jackpot.Billing
{
    /// <summary>
    /// 通貨に関連する情報を持つクラスです.
    /// </summary>
    public partial class Currency
    {
#region Properties

        /// <summary>
        /// 通貨単位を示します。e.g. JPY, USD
        /// </summary>
        /// <value>The code.</value>
        public string Code { get; private set; }

        /// <summary>
        /// 通貨単位シンボルを示します。e.g. ￥, $
        /// </summary>
        /// <value>The symbol.</value>
        public string Symbol { get; private set; }

#endregion

#region Factory

        /// <summary>
        /// currencyCodeを使用してCurrencyオブジェクトを返却します
        /// </summary>
        /// <remarks>
        /// currencySymbolが不正な場合はnullが返却されます。
        /// 存在しない場合に別のCurrencyを返却したい場合は、第二引数を指定します
        /// </remarks>
        /// <param name="currencyCode">Currency code.</param>
        public static Currency Resolve(string currencyCode)
        {
            return Resolve(currencyCode, new Currency(currencyCode, ""));
        }

        /// <summary>
        /// currencyCodeを使用してCurrencyオブジェクトを返却します
        /// </summary>
        /// <param name="currencyCode">Currency code.</param>
        /// <param name="defaultValue">Default value.</param>
        public static Currency Resolve(string currencyCode, Currency defaultValue)
        {
            if (string.IsNullOrEmpty(currencyCode))
            {
                return defaultValue;
            }
            if (!currencySymbolMap.ContainsKey(currencyCode))
            {
                return defaultValue;
            }
            var currencySymbol = currencySymbolMap[currencyCode];
            if (string.IsNullOrEmpty(currencySymbol))
            {
                return defaultValue;
            }
            return new Currency(currencyCode, currencySymbol);
        }

#endregion

#region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.Billing.Currency"/> class.
        /// </summary>
        /// <param name="code">Code.</param>
        /// <param name="symbol">Symbol.</param>
        public Currency(string code, string symbol)
        {
            Code = code;
            Symbol = symbol;
        }

#endregion

#region Generated

        /// <summary>
        /// CurrencySymbolとCurrencyCodeのMap。自動生成したもの
        /// </summary>
        /// <remarks>
        /// 本当はランタイムでとりたいんだけど、Unityのmonoのバージョンが古すぎてまともな値が取れない。
        /// (っていうかCurrencySymbolとISOCurrencySymbolが逆だったりだいぶ話にならない。)
        /// ので最新のmonoで以下のコードを実行して自動生成した。文字化けはもうなんかしょうがない
        /// <code>
        /// var map = CultureInfo.GetCultures(CultureTypes.AllCultures)
        /// .Where(c => !c.IsNeutralCulture)
        /// .Select(c =>
        /// {
        ///     try
        ///     {
        ///         return new RegionInfo(c.LCID);
        ///     }
        ///     catch (Exception)
        ///     {
        ///         return null;
        ///     }
        /// })
        /// .Where(ri => ri != null)
        /// .GroupBy(r => r.ISOCurrencySymbol)
        /// .ToDictionary(x => x.Key, x => x.First().CurrencySymbol);
        ///
        /// var sorted = new SortedDictionary<string, string>(map);
        /// return sorted;
        /// </code>
        /// </remarks>
        static readonly Dictionary<string, string> currencySymbolMap = new Dictionary<string, string>() {
            { "AED", "د.إ.‏" },
            { "AFN", "؋" },
            { "ALL", "Lekë" },
            { "AMD", "դր." },
            { "ARS", "$" },
            { "AUD", "$" },
            { "AZN", "man." },
            { "BAM", "KM" },
            { "BDT", "৳" },
            { "BGN", "лв." },
            { "BHD", "د.ب.‏" },
            { "BOB", "Bs" },
            { "BRL", "R$" },
            { "BYR", "р." },
            { "BZD", "$" },
            { "CAD", "$" },
            { "CHF", "CHF" },
            { "CLP", "$" },
            { "CNY", "¥" },
            { "COP", "$" },
            { "CRC", "₡" },
            { "CZK", "Kč" },
            { "DKK", "kr" },
            { "DOP", "$" },
            { "DZD", "د.ج.‏" },
            { "EGP", "ج.م.‏" },
            { "ETB", "ብር" },
            { "EUR", "€" },
            { "GBP", "£" },
            { "GEL", "" },
            { "GTQ", "Q" },
            { "HKD", "$" },
            { "HNL", "L" },
            { "HRK", "kn" },
            { "HUF", "Ft" },
            { "IDR", "Rp" },
            { "ILS", "₪" },
            { "INR", "₹" },
            { "IQD", "د.ع.‏" },
            { "IRR", "ریال" },
            { "ISK", "kr" },
            { "JMD", "$" },
            { "JOD", "د.أ.‏" },
            { "JPY", "￥" },
            { "KES", "Ksh" },
            { "KHR", "៛" },
            { "KRW", "₩" },
            { "KWD", "د.ك.‏" },
            { "KZT", "₸" },
            { "LAK", "₭" },
            { "LBP", "ل.ل.‏" },
            { "LKR", "රු." },
            { "LTL", "Lt" },
            { "LYD", "د.ل.‏" },
            { "MAD", "د.م.‏" },
            { "MKD", "ден" },
            { "MOP", "MOP$" },
            { "MXN", "$" },
            { "MYR", "RM" },
            { "NGN", "₦" },
            { "NIO", "C$" },
            { "NOK", "kr" },
            { "NPR", "नेरू" },
            { "NZD", "$" },
            { "OMR", "ر.ع.‏" },
            { "PAB", "B/." },
            { "PEN", "S/." },
            { "PHP", "₱" },
            { "PKR", "Rs" },
            { "PLN", "zł" },
            { "PYG", "₲" },
            { "QAR", "ر.ق.‏" },
            { "RON", "" },
            { "RSD", "din." },
            { "RUB", "руб." },
            { "RUR", "р." },
            { "RWF", "RF" },
            { "SAR", "ر.س.‏" },
            { "SEK", "kr" },
            { "SGD", "$" },
            { "SYP", "ل.س.‏" },
            { "THB", "฿" },
            { "TJS", "сом" },
            { "TND", "د.ت.‏" },
            { "TRL", "TL" },
            { "TRY", "₺" },
            { "TTD", "$" },
            { "TWD", "NT$" },
            { "UAH", "₴" },
            { "USD", "$" },
            { "UYU", "$" },
            { "UZS", "soʻm" },
            { "VEF", "Bs." },
            { "VND", "₫" },
            { "YER", "ر.ي.‏" },
            { "ZAR", "R" },
        };

#endregion
    }
}

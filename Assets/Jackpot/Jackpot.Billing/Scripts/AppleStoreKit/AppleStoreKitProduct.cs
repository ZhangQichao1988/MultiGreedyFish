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

namespace Jackpot.Billing
{
    /// <summary>
    /// SKProductクラスのデータを格納するクラスです。
    /// </summary>
    /// <remarks>
    /// see. https://developer.apple.com/library/ios/documentation/StoreKit/Reference/SKProduct_Reference/index.html
    /// </remarks>
    public class AppleStoreKitProduct
    {
        /// <summary>
        /// SKProduct.localizedDescription
        /// </summary>
        /// <value>The localized description.</value>
        public string LocalizedDescription { get; private set; }

        /// <summary>
        /// SKProduct.localizedTitle
        /// </summary>
        /// <value>The localized title.</value>
        public string LocalizedTitle { get; private set; }

        /// <summary>
        /// SKProduct.price
        /// </summary>
        /// <value>The price.</value>
        public decimal Price { get; private set; }

        /// <summary>
        /// 通貨記号を含めた書式でフォーマットされた価格情報
        /// </summary>
        /// <remarks>
        /// NSNumberFormatter *numberFormatter = [[NSNumberFormatter alloc] init];<br/>
        /// [numberFormatter setFormatterBehavior:NSNumberFormatterBehavior10_4];<br/>
        /// [numberFormatter setNumberStyle:NSNumberFormatterCurrencyStyle];<br/>
        /// [numberFormatter setLocale:product.priceLocale];<br/>
        /// @"formattedPrice":[numberFormatter stringFromNumber:product.price]
        /// </remarks>
        /// <value>The formatted price.</value>
        public string FormattedPrice { get; private set; }

        /// <summary>
        /// 通貨コード
        /// </summary>
        /// <remarks>
        /// @"currencyCode":[product.priceLocale objectForKey:NSLocaleCurrencyCode]
        /// </remarks>
        /// <value>The currency code.</value>
        public string CurrencyCode { get; private set; }

        /// <summary>
        /// 通貨記号
        /// </summary>
        /// <remarks>
        /// @"currencySymbol":[product.priceLocale objectForKey:NSLocaleCurrencySymbol]
        /// </remarks>
        /// <value>The currency symbol.</value>
        public string CurrencySymbol { get; private set; }

        /// <summary>
        /// 国コード
        /// </summary>
        /// <remarks>
        /// @"countryCode":[product.priceLocale objectForKey:NSLocaleCountryCode]
        /// </remarks>
        /// <value>The country code.</value>
        public string CountryCode { get; private set; }

        /// <summary>
        /// SKProduct.productIdentifier
        /// </summary>
        /// <value>The product identifier.</value>
        public string ProductIdentifier { get; private set; }

        ILogger logger;

        public AppleStoreKitProduct(Dictionary<string, object> dict)
        {
            logger = Jackpot.Logger.Get<AppleStoreKitProduct>();
            if (dict.ContainsKey("localizedDescription"))
            {
                LocalizedDescription = dict["localizedDescription"] as string;
            }

            if (dict.ContainsKey("localizedTitle"))
            {
                LocalizedTitle = dict["localizedTitle"] as string;
            }

            if (dict.ContainsKey("price"))
            {
                Price = Convert.ToDecimal(dict["price"]);
                if (dict["price"] != null && typeof(double) == dict["price"].GetType() && (double) dict["price"] != Convert.ToDouble(Price))
                {
                    //doubleで扱えない桁数の浮動小数はdecimalに正しくキャストできません。
                    logger.Error(
                        "invalid price double:{0}, decimal:{1}",
                        Convert.ToDouble(dict["price"]).ToString("G17"), Price.ToString("G17")
                    );
                    Price = 0;
                }
            }

            if (dict.ContainsKey("formattedPrice"))
            {
                FormattedPrice = dict["formattedPrice"] as string;
            }

            if (dict.ContainsKey("currencyCode"))
            {
                CurrencyCode = dict["currencyCode"] as string;
            }

            if (dict.ContainsKey("currencySymbol"))
            {
                CurrencySymbol = dict["currencySymbol"] as string;
            }

            if (dict.ContainsKey("countryCode"))
            {
                CountryCode = dict["countryCode"] as string;
            }

            if (dict.ContainsKey("productIdentifier"))
            {
                ProductIdentifier = dict["productIdentifier"] as string;
            }
        }

        public override string ToString()
        {
            return string.Format(
                "LocalizedDescription:{0}\nLocalizedTitle:{1}\nPrice:{2}\nFormattedPrice:{3}\nCurrencyCode:{4}\nCurrencySymbol:{5}\nCountryCode:{6}\nProductIdentifier:{7}",
                LocalizedDescription
                , LocalizedTitle
                , Price
                , FormattedPrice
                , CurrencyCode
                , CurrencySymbol
                , CountryCode
                , ProductIdentifier
            );
        }
    }
}

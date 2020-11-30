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
using System.Collections.Generic;

namespace Jackpot.Billing
{
    /// <summary>
    /// In-app BillingのgetSkuDetails()で取得できるデータを格納するクラスです。
    /// </summary>
    /// <remarks>
    /// see. http://developer.android.com/google/play/billing/billing_reference.html <br/>
    /// </remarks>
    public class GoogleIabSkuDetail
    {
        /// <summary>
        /// DETAILS_LIST:productId
        /// </summary>
        public string ProductId { get; private set; }

        /// <summary>
        /// DETAILS_LIST:type
        /// </summary>
        /// <remarks>
        /// Value must be “inapp” for an in-app product or "subs" for subscriptions.
        /// </remarks>
        public string Type { get; private set; }

        /// <summary>
        /// 商品の実価格を数値で表した値
        /// </summary>
        /// <remarks>
        /// Price = (PriceAmountMicros / 1000000m).ToString("F6");
        /// </remarks>
        public string Price { get; private set; }

        /// <summary>
        /// DETAILS_LIST:price
        /// </summary>
        public string FormattedPrice { get; private set; }

        /// <summary>
        /// DETAILS_LIST:price_amount_micros
        /// </summary>
        public long PriceAmountMicros { get; private set; }

        /// <summary>
        /// DETAILS_LIST:price_currency_code
        /// </summary>
        public string PriceCurrencyCode { get; private set; }

        /// <summary>
        /// DETAILS_LIST:title
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// DETAILS_LIST:description
        /// </summary>
        public string Description { get; private set; }

        public GoogleIabSkuDetail(Dictionary<string, object> dict)
        {
            if (dict.ContainsKey("productId"))
            {
                ProductId = dict["productId"] as string;
            }

            if (dict.ContainsKey("type"))
            {
                Type = dict["type"] as string;
            }

            if (dict.ContainsKey("price"))
            {
                FormattedPrice = dict["price"] as string;
            }

            if (dict.ContainsKey("price_amount_micros"))
            {
                PriceAmountMicros = System.Convert.ToInt64(dict["price_amount_micros"]);
                Price = (PriceAmountMicros / 1000000m).ToString("F6");
            }

            if (dict.ContainsKey("price_currency_code"))
            {
                PriceCurrencyCode = dict["price_currency_code"] as string;
            }

            if (dict.ContainsKey("title"))
            {
                Title = dict["title"] as string;
            }

            if (dict.ContainsKey("description"))
            {
                Description = dict["description"] as string;
            }
        }

        public override string ToString()
        {
            return string.Format(
                "ProductId:{0}\nType:{1}\nPrice:{2}\nPriceAmountMicros:{3}\nPriceCurrencyCode:{4}\nTitle:{5}\nDescription:{6}",
                ProductId
                , Type
                , Price
                , PriceAmountMicros
                , PriceCurrencyCode
                , Title
                , Description
            );
        }
    }
}

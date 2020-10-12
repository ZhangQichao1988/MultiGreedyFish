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
    /// 商品情報を格納するクラスです
    /// </summary>
    public partial class Product
    {
#region properties

        /// <summary>
        /// 商品IDを示します。Jackpot.Billing.Product.Idに等しい値が入ります
        /// </summary>
        /// <value>The Identifier of Product to purchase</value>
        public string Id { get; private set; }

        /// <summary>
        /// 商品の種別を示します
        /// </summary>
        /// <value>The type.</value>
        public string Type { get; private set; }

        /// <summary>
        /// 商品名を示します
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; private set; }

        /// <summary>
        /// 商品説明を示します
        /// </summary>
        /// <value>The receipt.</value>
        public string Description { get; private set; }

        /// <summary>
        /// 商品価格を示します
        /// </summary>
        /// <value>The receipt.</value>
        public string Price { get; private set; }

        /// <summary>
        /// フォーマットされた商品価格を示します
        /// </summary>
        /// <value>The receipt.</value>
        public string FormattedPrice { get; private set; }

        /// <summary>
        /// 通貨単位を示します
        /// </summary>
        /// <value>The currency code.</value>
        public string CurrencyCode { get { return currency.Code; } }

        /// <summary>
        /// 通貨単位記号を示します
        /// </summary>
        /// <value>The currency symbol.</value>
        public string CurrencySymbol { get { return currency.Symbol; } }

#endregion

#region Fields

        Currency currency;

#endregion

#region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.Billing.Product"/> class.
        /// </summary>
        public Product(string id, string type, string title, string description, string price, string formattedPrice, Currency currency)
        {
            Id = id;
            Type = type;
            Title = title;
            Description = description;
            Price = price;
            FormattedPrice = formattedPrice;
            this.currency = currency;
        }

#endregion

#region Public Methods

        public override string ToString()
        {
            return string.Format("[Product: Id={0}, Type={1}, Title={2}, Description={3}, Price={4}, FormattedPrice={5}, CurrencyCode={6}, CurrencySymbol={7}]", Id, Type, Title, Description, Price, FormattedPrice, CurrencyCode,
                CurrencySymbol);
        }

#endregion
    }
}

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
    /// 購入中から購入済の商品情報の取り扱いに使用するDTOです。
    /// </summary>
    /// <remarks>
    /// 購入済商品に関連する様々なプロパティを保持していますが
    /// PurchasedProductのプロパティをトレース目的がない限り、
    /// 基本的に利用者は各プロパティを意識する必要はなく、
    /// ToDictionary()を使用してDictionaryを生成し、
    /// リクエストパラメータにDictionaryのkey-valueを追加すれば良いです。
    /// </remarks>
    public class PurchasedProduct
    {
#region isset

        [Serializable]
        public struct Isset
        {
            public bool Id;
            public bool Type;
            public bool Receipt;
            public bool SignedData;
            public bool Signature;
            public bool OrderId;
            public bool PackageName;
            public bool PurchaseTime;
            public bool PurchaseToken;
            public bool AutoRenewing;
            public bool Acknowledged;
        }

        public Isset __isset;

#endregion

#region properties

        /// <summary>
        /// 商品IDを示します。Jackpot.Billing.Product.Idに等しい値が入ります
        /// </summary>
        /// <value>The Identifier of Product to purchase</value>
        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                __isset.Id = true;
                id = value;
            }
        }

        /// <summary>
        /// [iOS] Original Transaction IDを示します。ToDictionary()では出力されません(不要な為)。
        /// </summary>
        /// <value>The original transaction identifier.</value>
        public string OriginalTransactionId { get; private set; }

        /// <summary>
        /// [iOS]Transaction IDを示します。ToDictionary()では出力されません(不要な為)。
        /// </summary>
        /// <remarks>
        /// ToDictionary()で出力して欲しい場合はご連絡ください
        /// </remarks>
        /// <value>The transaction identifier.</value>
        public string TransactionId { get; private set; }

        /// <summary>
        /// [iOS]Transaction Dateを示します。ToDictionary()では出力されません(不要な為)。
        /// </summary>
        /// <remarks>
        /// ToDictionary()で出力して欲しい場合はご連絡ください
        /// </remarks>
        /// <value>The transaction date.</value>
        public DateTime TransactionDate { get; private set; }

        /// <summary>
        /// [iOS]購入した際のレシートを示します
        /// </summary>
        /// <value>The receipt.</value>
        public string Receipt
        {
            get
            {
                return receipt;
            }
            set
            {
                __isset.Receipt = true;
                receipt = value;
            }
        }

        /// <summary>
        /// [Android]商品の種別を示します
        /// </summary>
        /// <value>The type.</value>
        public string Type
        {
            get
            {
                return type;
            }
            set
            {
                __isset.Type = true;
                type = value;
            }
        }

        /// <summary>
        /// [Android]署名済みデータを示します
        /// </summary>
        /// <value>The signed data.</value>
        public string SignedData
        {
            get
            {
                return signedData;
            }
            set
            {
                __isset.SignedData = true;
                signedData = value;
            }
        }

        /// <summary>
        /// [Android]購入済商品のSignatureを示します
        /// </summary>
        /// <value>The signature.</value>
        public string Signature
        {
            get
            {
                return signature;
            }
            set
            {
                __isset.Signature = true;
                signature = value;
            }
        }

        /// <summary>
        /// [Android]購入済商品の注文IDを示します
        /// </summary>
        /// <value>The order identifier.</value>
        public string OrderId
        {
            get
            {
                return orderId;
            }
            set
            {
                __isset.OrderId = true;
                orderId = value;
            }
        }

        /// <summary>
        /// [Android]商品を管理しているアプリケーションのパッケージ名を示します
        /// </summary>
        /// <value>The name of the package.</value>
        public string PackageName
        {
            get
            {
                return packageName;
            }
            set
            {
                __isset.PackageName = true;
                packageName = value;
            }
        }

        /// <summary>
        /// [Android]購入された時間(ms)を示します
        /// </summary>
        /// <value>The purchase time.</value>
        public long PurchaseTime
        {
            get
            {
                return purchaseTime;
            }
            set
            {
                __isset.PurchaseTime = true;
                purchaseTime = value;
            }
        }

        /// <summary>
        /// [Android]購入済み商品と、購入ユーザを紐づけるトークン文字列を示します
        /// </summary>
        /// <value>The purchase token.</value>
        public string PurchaseToken
        {
            get
            {
                return purchaseToken;
            }
            set
            {
                __isset.PurchaseToken = true;
                purchaseToken = value;
            }
        }

        /// <summary>
        /// [Android]定期購入が自動更新されたかどうかを示します
        /// </summary>
        /// <value><c>true</c> if auto renewing; otherwise, <c>false</c>.</value>
        public bool AutoRenewing
        {
            get
            {
                return autoRenewing;
            }
            set
            {
                __isset.AutoRenewing = true;
                autoRenewing = value;
            }
        }

        public bool Acknowledged
        {
            get
            {
                return acknowledged;
            }
            set
            {
                __isset.Acknowledged = true;
                acknowledged = value;
            }
        }

        /// <summary>
        /// レシート更新を行った回数
        /// </summary>
        public int RetryRefreshReceiptCount { get; set; }

#endregion

#region Fields

        string id;
        string type;
        string receipt;
        string signedData;
        string signature;
        string orderId;
        string packageName;
        long purchaseTime;
        string purchaseToken;
        bool autoRenewing;
        bool acknowledged;

        bool isIos;
        bool isAndroid;
        bool isUnityEditor;

#endregion

#region Constructor

        /// <summary>
        /// [iOS]Initializes a new instance of the <see cref="Jackpot.Billing.PurchasedProduct"/> class.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="transactionId">Transaction identifier.</param>
        /// <param name="receipt">Receipt.</param>
        /// <param name="transactionDate">Transaction Data.</param>
        /// <param name="originalTransactionId">Original transaction identifier.</param>
        public PurchasedProduct(string id, string transactionId, string receipt, long transactionDate, string originalTransactionId)
        {
            Id = id;
            TransactionId = transactionId;
            Receipt = receipt;
            TransactionDate = UnixEpoch.ToDateTime(transactionDate);
            OriginalTransactionId = originalTransactionId;
            isIos = true;
        }

        /// <summary>
        /// [Android]Initializes a new instance of the <see cref="Jackpot.Billing.PurchasedProduct"/> class.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="type">Type.</param>
        /// <param name="signedData">Signed data.</param>
        /// <param name="signature">Signature.</param>
        /// <param name="orderId">Order identifier.</param>
        /// <param name="packageName">Package name.</param>
        /// <param name="purchaseTime">Purchase time.</param>
        /// <param name="purchaseToken">Purchase token.</param>
        /// <param name="autoRenewing">Auto renewing.</param>
        /// <param name="acknowledged">Acknowledged.</param>
        public PurchasedProduct(
            string id,
            string type,
            string signedData,
            string signature,
            string orderId,
            string packageName,
            long purchaseTime,
            string purchaseToken,
            bool autoRenewing,
            bool acknowledged)
        {
            Id = id;
            Type = type;
            SignedData = signedData;
            Signature = signature;
            OrderId = orderId;
            PackageName = packageName;
            PurchaseTime = purchaseTime;
            PurchaseToken = purchaseToken;
            AutoRenewing = autoRenewing;
            Acknowledged = acknowledged;
            isAndroid = true;
        }

        /// <summary>
        /// [UnityEditor]Initializes a new instance of the <see cref="Jackpot.Billing.PurchasedProduct"/> class.
        /// </summary>
        /// <param name="id">Identifier.</param>
        public PurchasedProduct(string id)
        {
            Id = id;
            isUnityEditor = true;
        }

#endregion

#region Public Methods

        /// <summary>
        /// Platformを判断しつつPurchasedProductをDictionaryとして出力します
        /// </summary>
        /// <returns>The dictionary.</returns>
        public Dictionary<string, string> ToDictionary()
        {
            if (isIos)
            {
                return ToDictionary("product_id", "receipt");
            }
            if (isAndroid)
            {
                return ToDictionary(
                    "sku",
                    "type",
                    "signed_data",
                    "base64_signature",
                    "order_id",
                    "package_name",
                    "purchase_time",
                    "purchase_token",
                    "autoRenewing",
                    "acknowledged"
                );
            }
            if (isUnityEditor)
            {
                return ToDictionary("product_id");
            }
            throw new InvalidOperationException("Unsupported platform");
        }

        /// <summary>
        /// [iOS]PurchasedProductをDictionaryとして出力します。
        /// </summary>
        /// <remarks>
        /// iOS Platformでの利用に特化した形で出力されるので、Androidでは使用する価値はないです。
        /// 通常はPurchaseProduct.ToDictionary()の使用を推奨します
        /// キーのカスタマイズの為にPublic Methodとして用意していますが、RingoameやAndounughtを使用している案件では、
        /// カスタマイズする必要がないはずなので、将来的にPrivate Methodになると思います
        /// </remarks>
        /// <returns>The dictionary.</returns>
        /// <param name="idKey">Identifier key.</param>
        /// <param name="receiptKey">Receipt key.</param>
        public Dictionary<string, string> ToDictionary(string idKey, string receiptKey)
        {
            var results = new Dictionary<string, string>();
            if (__isset.Id && !string.IsNullOrEmpty(Id))
            {
                results[idKey] = Id;
            }
            if (__isset.Receipt && !string.IsNullOrEmpty(Receipt))
            {
                results[receiptKey] = Receipt;
            }
            return results;
        }

        /// <summary>
        /// [Android]PurchasedProductをDictionaryとして出力します。
        /// </summary>
        /// <remarks>
        /// Android Platformでの利用に特化した形で出力されるので、iOSでは使用する価値はないです。
        /// 通常はPurchaseProduct.ToDictionary()の使用を推奨します
        /// キーのカスタマイズの為にPublic Methodとして用意していますが、RingoameやAndounughtを使用している案件では、
        /// カスタマイズする必要がないはずなので、将来的にPrivate Methodになると思います
        /// </remarks>
        /// <returns>The dictionary.</returns>
        /// <param name="idKey">Identifier key.</param>
        /// <param name="typeKey">Type key.</param>
        /// <param name="signedDataKey">Signed data key.</param>
        /// <param name="signatureKey">Signature key.</param>
        /// <param name="orderIdKey">Order identifier key.</param>
        /// <param name="packageNameKey">Package name key.</param>
        /// <param name="purchaseTimeKey">Purchase time key.</param>
        /// <param name="purchaseTokenKey">Purchase token key.</param>
        /// <param name="autoRenewingKey">Auto renewing key.</param>
        /// <param name="acknowledgedKey">Acknowledged key.</param>
        public Dictionary<string, string> ToDictionary(
            string idKey,
            string typeKey,
            string signedDataKey,
            string signatureKey,
            string orderIdKey,
            string packageNameKey,
            string purchaseTimeKey,
            string purchaseTokenKey,
            string autoRenewingKey,
            string acknowledgedKey)
        {
            var results = new Dictionary<string, string>();
            if (__isset.Id && !string.IsNullOrEmpty(Id))
            {
                results[idKey] = Id;
            }
            if (__isset.Type && !string.IsNullOrEmpty(Type))
            {
                results[typeKey] = Type;
            }
            if (__isset.SignedData && !string.IsNullOrEmpty(SignedData))
            {
                results[signedDataKey] = SignedData;
            }
            if (__isset.Signature && !string.IsNullOrEmpty(Signature))
            {
                results[signatureKey] = Signature;
            }
            if (__isset.OrderId && !string.IsNullOrEmpty(OrderId))
            {
                results[orderIdKey] = OrderId;
            }
            if (__isset.PackageName && !string.IsNullOrEmpty(PackageName))
            {
                results[packageNameKey] = PackageName;
            }
            if (__isset.PurchaseTime && PurchaseTime >= 0)
            {
                results[purchaseTimeKey] = PurchaseTime.ToString();
            }
            if (__isset.PurchaseToken && !string.IsNullOrEmpty(PurchaseToken))
            {
                results[purchaseTokenKey] = PurchaseToken;
            }
            if (__isset.AutoRenewing)
            {
                results[autoRenewingKey] = AutoRenewing.ToString();
            }
            if (__isset.Acknowledged)
            {
                results[acknowledgedKey] = Acknowledged.ToString();
            }
            return results;
        }

        /// <summary>
        /// [UnityEditor]PurchasedProductをDictionaryとして出力します。
        /// </summary>
        /// <returns>The dictionary.</returns>
        /// <param name="idKey">Identifier key.</param>
        public Dictionary<string, string> ToDictionary(string idKey)
        {
            var results = new Dictionary<string, string>();
            if (__isset.Id && !string.IsNullOrEmpty(Id))
            {
                results[idKey] = Id;
            }
            return results;
        }

#endregion
    }
}

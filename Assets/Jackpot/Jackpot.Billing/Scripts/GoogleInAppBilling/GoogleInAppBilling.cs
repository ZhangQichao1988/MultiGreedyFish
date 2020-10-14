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
using UnityEngine;
using System;
using System.Collections.Generic;

#if UNITY_ANDROID
namespace Jackpot.Billing
{
    /// <summary>
    /// Jackpotが提供するIn-App Billingネイティブプラグインの処理を呼び出すためのクラスです。
    /// </summary>
    public partial class GoogleInAppBilling
    {
        /// <summary>
        /// GoogleInAppBilling.StartSetup()が失敗した時の処理をカスタマイズする際に設定するeventです。
        /// </summary>
        public static event Action<GoogleIabResult> SetupFailed;

        /// <summary>
        /// GoogleInAppBilling.StartSetup()が成功した時の処理をカスタマイズする際に設定するeventです。
        /// </summary>
        public static event Action SetupSucceeded;

        /// <summary>
        /// GoogleInAppBilling.QueryInventory()が失敗した時の処理をカスタマイズする際に設定するeventです。
        /// </summary>
        public static event Action<GoogleIabResult> QueryInventoryFailed;

        /// <summary>
        /// GoogleInAppBilling.QueryInventory()が成功した時の処理をカスタマイズする際に設定するeventです。
        /// </summary>
        public static event Action<List<GoogleIabSkuDetail>> QueryInventorySucceeded;

        /// <summary>
        /// GoogleInAppBilling.Purchase()が失敗した時の処理をカスタマイズする際に設定するeventです。
        /// </summary>
        public static event Action<GoogleIabResult> PurchaseFailed;

        /// <summary>
        /// GoogleInAppBilling.Purchase()が成功した時の処理をカスタマイズする際に設定するeventです。
        /// </summary>
        public static event Action<GoogleIabPurchase> PurchaseSucceeded;

        /// <summary>
        /// GoogleInAppBilling.Consume()が失敗した時の処理をカスタマイズする際に設定するeventです。
        /// </summary>
        public static event Action<GoogleIabResult> ConsumeFailed;

        /// <summary>
        /// GoogleInAppBilling.Consume()が成功した時の処理をカスタマイズする際に設定するeventです。
        /// </summary>
        public static event Action<GoogleIabPurchase> ConsumeSucceeded;

        /// <summary>
        /// GoogleInAppBilling.Acknowledge()が失敗した時の処理をカスタマイズする際に設定するeventです。
        /// </summary>
        public static event Action<GoogleIabResult> AcknowledgeFailed;

        /// <summary>
        /// GoogleInAppBilling.Acknowledge()が成功した時の処理をカスタマイズする際に設定するeventです。
        /// </summary>
        public static event Action<GoogleIabPurchase> AcknowledgeSucceeded;

        /// <summary>
        /// Occurs when restore failed.
        /// </summary>
        public static event Action<GoogleIabResult> RestoreFailed;

        /// <summary>
        /// Occurs when restore succeeded.
        /// </summary>
        public static event Action<List<GoogleIabPurchase>> RestoreSucceeded;

        /// <summary>
        /// Occurs when query purchases failed.
        /// </summary>
        public static event Action<GoogleIabResult> QueryPurchasesFailed;

        /// <summary>
        /// Occurs when query purchases succeeded.
        /// </summary>
        public static event Action<List<GoogleIabPurchase>, List<GoogleIabSkuDetail>> QueryPurchasesSucceeded;

        static AndroidJavaObject plugin;

        static GoogleInAppBilling()
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                return;
            }

            plugin = new AndroidJavaClass("com.klab.jackpot.billing.UnityBridge");
        }

        /// <summary>
        /// プラグインが出力するデバッグログの有無を設定します
        /// </summary>
        /// <param name="enable">If set to <c>true</c> enable.</param>
        public static void EnableLogging(bool enable)
        {
            plugin.CallStatic("enableLogging", new object[] { enable });
        }

        /// <summary>
        /// プラグインの初期化処理を実施します。
        /// </summary>
        /// <remarks>
        /// Googleが提供するBillingLibraryを初期化します。
        /// </remarks>
        public static void StartSetup()
        {
            plugin.CallStatic("startSetup", new object[] { new GoogleInAppBillingListener() });
        }

        /// <summary>
        /// プラグインを開放します。
        /// </summary>
        /// <remarks>
        /// BillingClientの破棄処理を実行します。
        /// </remarks>
        public static void Dispose()
        {
            plugin.CallStatic("dispose", new object[] { });
        }

        /// <summary>
        /// 商品一覧を取得します。
        /// </summary>
        /// <remarks>
        /// BillingClient.querySkuDetailsAsync()を実行します。
        /// </remarks>
        public static void QueryInventory(string[] itemSkus, string[] subsSkus)
        {
            plugin.CallStatic("queryInventory", new object[] { itemSkus, subsSkus });
        }

        /// <summary>
        /// 購入処理を実施します。
        /// </summary>
        /// <remarks>
        /// BillingClient.launchBillingFlow()を実行します
        /// </remarks>
        /// <param name="sku">Sku.</param>
        /// <param name="accountId">Account ID.</param>
        /// <param name="profileId">Profile ID.</param>
        public static void Purchase(string sku, string accountId, string profileId)
        {
            plugin.CallStatic("purchase", new object[] { sku, accountId, profileId });
        }

        /// <summary>
        /// 定期購入におけるアップグレード・ダウングレード処理を実施します。
        /// </summary>
        /// <remarks>
        /// BillingClient.launchBillingFlow()を実行します
        /// </remarks>
        /// <param name="sku">Sku.</param>
        /// <param name="accountId">Account ID.</param>
        /// <param name="profileId">Profile ID.</param>
        /// <param name="oldSku">Old Sku.</param>
        public static void PurchaseWithTimeProration(string sku, string accountId, string profileId, string oldSku)
        {
            plugin.CallStatic("purchaseWithTimeProration", new object[] { sku, accountId, profileId, oldSku });
        }
        
        /// <summary>
        /// 定期購入におけるアップグレード・ダウングレード処理を実施します。
        /// </summary>
        /// <remarks>
        /// BillingClient.launchBillingFlow()を実行します
        /// </remarks>
        /// <param name="sku">Sku.</param>
        /// <param name="accountId">Account ID.</param>
        /// <param name="profileId">Profile ID.</param>
        /// <param name="oldSku">Old Sku.</param>
        public static void PurchaseWithChargeProratedPrice(string sku, string accountId, string profileId, string oldSku)
        {
            plugin.CallStatic("purchaseWithChargeProratedPrice", new object[] { sku, accountId, profileId, oldSku });
        }
        
        /// <summary>
        /// 定期購入におけるアップグレード・ダウングレード処理を実施します。
        /// </summary>
        /// <remarks>
        /// BillingClient.launchBillingFlow()を実行します
        /// </remarks>
        /// <param name="sku">Sku.</param>
        /// <param name="accountId">Account ID.</param>
        /// <param name="profileId">Profile ID.</param>
        /// <param name="oldSku">Old Sku.</param>
        public static void PurchaseWithoutProration(string sku, string accountId, string profileId, string oldSku)
        {
            plugin.CallStatic("purchaseWithoutProration", new object[] { sku, accountId, profileId, oldSku });
        }

        /// <summary>
        /// 定期購入におけるアップグレード・ダウングレード処理を実施します。
        /// </summary>
        /// <remarks>
        /// BillingClient.launchBillingFlow()を実行します
        /// </remarks>
        /// <param name="sku">Sku.</param>
        /// <param name="accountId">Account ID.</param>
        /// <param name="profileId">Profile ID.</param>
        /// <param name="oldSku">Old Sku.</param>
        public static void PurchaseWithDeferred(string sku, string accountId, string profileId, string oldSku)
        {
            plugin.CallStatic("purchaseWithDeferred", new object[] { sku, accountId, profileId, oldSku });
        }

        /// <summary>
        /// 購入後のConsume処理を実施します。
        /// </summary>
        /// <param name="sku">Sku.</param>
        /// <param name="purchaseToken">Purchase Token.</param>
        /// <remarks>
        /// BillingClient.consumeAsync()を実行します。
        /// </remarks>
        public static void Consume(string sku, string purchaseToken)
        {
            plugin.CallStatic("consume", new object[] { sku, purchaseToken });
        }

        /// <summary>
        /// 購入後のDeveloperPayloadを引数にもつConsume処理を実施します。
        /// </summary>
        /// <param name="sku">Sku.</param>
        /// <param name="purchaseToken">Purchase Token.</param>
        /// <param name="developerPayload">Developer Payload.</param>
        /// <remarks>
        /// BillingClient.consumeAsync()を実行します。
        /// </remarks>
        public static void Consume(string sku, string purchaseToken, string developerPayload)
        {
            plugin.CallStatic("consume", new object[] { sku, purchaseToken, developerPayload });
        }

        /// <summary>
        /// 定期購入商品購入後の承認処理を実施します。
        /// </summary>
        /// <param name="sku">Sku.</param>
        /// <param name="purchaseToken">Purchase Token.</param>
        /// <remarks>
        /// BillingClient.acknowledgePurchase()を実行します。
        /// </remarks>
        public static void Acknowledge(string sku, string purchaseToken)
        {
            plugin.CallStatic("acknowledge", new object[] { sku, purchaseToken });
        }

        /// <summary>
        /// 定期購入商品購入後のDeveloperPayloadを引数にもつ承認処理を実施します。
        /// </summary>
        /// <param name="sku">Sku.</param>
        /// <param name="purchaseToken">Purchase Token.</param>
        /// <param name="developerPayload">Developer Payload.</param>
        /// <remarks>
        /// BillingClient.acknowledgePurchase()を実行します。
        /// </remarks>
        public static void Acknowledge(string sku, string purchaseToken, string developerPayload)
        {
            plugin.CallStatic("acknowledge", new object[] { sku, purchaseToken, developerPayload });
        }

        /// <summary>
        /// 中断された購入処理があった場合に処理を完了させます。
        /// </summary>
        public static bool EnsureFinishPausedPurchase()
        {
            return plugin.CallStatic<bool>("ensureFinishPausedPurchase", new object[] { });
        }

        /// <summary>
        /// Consumeされていない購入済み商品を取得します
        /// </summary>
        public static void QueryPurchases()
        {
            plugin.CallStatic("queryPurchases", new object[] { });
        }

        /// <summary>
        /// Consumeされていない購入済み商品を取得します
        /// </summary>
        /// <param name="skuType">Sku Type.</param>
        public static void QueryPurchases(string skuType)
        {
            plugin.CallStatic("queryPurchases", new object[] { skuType });
        }
    }
}
#endif

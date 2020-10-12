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
    /// In-app BillingのgetBuyIntent()で取得できるデータを格納するクラスです。
    /// </summary>
    /// <remarks>
    /// see. http://developer.android.com/google/play/billing/billing_reference.html <br/>
    /// </remarks>
    public class GoogleIabPurchase
    {
        /// <summary>
        /// getSkuDetails()で取得したtype
        /// </summary>
        public string ItemType { get; private set; }

        /// <summary>
        /// INAPP_PURCHASE_DATA:orderId
        /// </summary>
        public string OrderId { get; private set; }

        /// <summary>
        /// INAPP_PURCHASE_DATA:packageName
        /// </summary>
        public string PackageName { get; private set; }

        /// <summary>
        /// INAPP_PURCHASE_DATA:productId
        /// </summary>
        public string ProductId { get; private set; }

        /// <summary>
        /// INAPP_PURCHASE_DATA:accountId
        /// </summary>
        public string AccountId { get; private set; }

        /// <summary>
        /// INAPP_PURCHASE_DATA:profileId
        /// </summary>
        public string ProfileId { get; private set; }

        /// <summary>
        /// INAPP_PURCHASE_DATA:purchaseTime
        /// </summary>
        public long PurchaseTime { get; private set; }

        /// <summary>
        /// INAPP_PURCHASE_DATA:developerPayload
        /// </summary>
        public string DeveloperPayload { get; private set; }

        /// <summary>
        /// INAPP_PURCHASE_DATA:purchaseToken
        /// </summary>
        public string PurchaseToken { get; private set; }

        /// <summary>
        /// getBuyIntent()で取得したINAPP_PURCHASE_DATAのオリジナルJSON
        /// </summary>
        public string OriginalJson { get; private set; }

        /// <summary>
        /// INAPP_DATA_SIGNATURE
        /// </summary>
        public string Signature { get; private set; }

        /// <summary>
        /// INAPP_PURCHASE_DATA:autoRenewing
        /// </summary>
        /// <value><c>true</c> if auto renewing; otherwise, <c>false</c>.</value>
        public bool AutoRenewing { get; private set; }

        /// <summary>
        /// INAPP_PURCHASE_DATA:acknowledged
        /// </summary>
        /// <value><c>true</c> if acknowledged; otherwise, <c>false</c>.</value>
        public bool Acknowledged { get; private set; }

        public bool IsPending { get; private set; }

        public GoogleIabPurchase(Dictionary<string, object> dict)
        {
            if (dict.ContainsKey("orderId"))
            {
                OrderId = dict["orderId"] as string;
            }

            if (dict.ContainsKey("packageName"))
            {
                PackageName = dict["packageName"] as string;
            }

            if (dict.ContainsKey("productId"))
            {
                ProductId = dict["productId"] as string;
            }

            if (dict.ContainsKey("purchaseTime"))
            {
                PurchaseTime = System.Convert.ToInt64(dict["purchaseTime"]);
            }

            if (dict.ContainsKey("developerPayload"))
            {
                DeveloperPayload = dict["developerPayload"] as string;
            }

            if (dict.ContainsKey("purchaseToken"))
            {
                PurchaseToken = dict["purchaseToken"] as string;
            }

            if (dict.ContainsKey("itemType"))
            {
                ItemType = dict["itemType"] as string;
            }

            if (dict.ContainsKey("signature"))
            {
                Signature = dict["signature"] as string;
            }

            if (dict.ContainsKey("autoRenewing"))
            {
                AutoRenewing = System.Convert.ToBoolean(dict["autoRenewing"]);
            }

            if (dict.ContainsKey("acknowledged"))
            {
                Acknowledged = System.Convert.ToBoolean(dict["acknowledged"]);
            }

            if (dict.ContainsKey("originalJson"))
            {
                OriginalJson = dict["originalJson"] as string;
            }

            if (dict.ContainsKey("isPending"))
            {
                IsPending = System.Convert.ToBoolean(dict["isPending"]);
            }

            if (dict.ContainsKey("accountId"))
            {
                AccountId = dict["accountId"] as string;
            }

            if (dict.ContainsKey("profileId"))
            {
                ProfileId = dict["profileId"] as string;
            }
        }

        public override string ToString()
        {
            return string.Format(
                "ItemType:{0}\nOrderId:{1}\nPackageName:{2}\nProductId:{3}\nAccountId:{4}\nProfileId:{5}\nPurchaseTime:{6}\nDeveloperPayload:{7}\nPurchaseToken:{8}\nSignature:{9}\nAutoRenewing:{10}\nAcknowledged:{11}\nOriginalJson:{12}\nIsPending:{13}",
                ItemType
                , OrderId
                , PackageName
                , ProductId
                , AccountId
                , ProfileId
                , PurchaseTime
                , DeveloperPayload
                , PurchaseToken
                , Signature
                , AutoRenewing
                , Acknowledged
                , OriginalJson
                , IsPending
            );
        }
    }
}

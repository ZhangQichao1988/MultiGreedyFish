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
using System.Collections.Generic;
using MiniJSON;

#if UNITY_ANDROID
namespace Jackpot.Billing
{
    public partial class GoogleInAppBilling
    {
        /// <summary>
        /// Jackpotが提供するIn-App Billingネイティブプラグインからのメッセージを受信するためのクラスです。
        /// </summary>
        class GoogleInAppBillingListener : AndroidJavaProxy
        {
            public GoogleInAppBillingListener()
                : base("com.klab.jackpot.billing.BillingListener")
            {
            }

            /// <summary>
            /// GoogleInAppBilling.StartSetup()が失敗した時のメッセージを受信します。
            /// </summary>
            /// <param name="response">Response.</param>
            /// <param name="message">Message.</param>
            public void setupFailed(int response, string message)
            {
                var result = new GoogleIabResult(response, message);
                MainThreadDispatcher.Post(() =>
                {
                    if (SetupFailed != null)
                    {
                        SetupFailed(result);
                    }
                });
            }

            /// <summary>
            /// GoogleInAppBilling.StartSetup()が成功した時のメッセージを受信します。
            /// </summary>
            public void setupSucceeded()
            {
                MainThreadDispatcher.Post(() =>
                {
                    if (SetupSucceeded != null)
                    {
                        SetupSucceeded();
                    }
                });
            }

            /// <summary>
            /// GoogleInAppBilling.QueryInventory()が失敗した時のメッセージを受信します。
            /// </summary>
            /// <param name="response">Response.</param>
            /// <param name="message">Message.</param>
            public void queryInventoryFailed(int response, string message)
            {
                var result = new GoogleIabResult(response, message);
                MainThreadDispatcher.Post(() =>
                {
                    if (QueryInventoryFailed != null)
                    {
                        QueryInventoryFailed(result);
                    }
                });
            }

            /// <summary>
            /// GoogleInAppBilling.QueryInventory()が成功した時のメッセージを受信します。
            /// </summary>
            /// <param name="json">Json.</param>
            public void queryInventorySucceeded(string json)
            {
                var skus = new List<GoogleIabSkuDetail>();
                var dict = Json.Deserialize(json) as Dictionary<string, object>;

                if (dict.ContainsKey("Skus"))
                {
                    foreach (var skuObj in (List<object>) dict["Skus"])
                    {
                        var skuDic = (Dictionary<string, object>) skuObj;
                        skus.Add(new GoogleIabSkuDetail(skuDic));
                    }
                }

                MainThreadDispatcher.Post(() =>
                {
                    if (QueryInventorySucceeded != null)
                    {
                        QueryInventorySucceeded(skus);
                    }
                });
            }

            /// <summary>
            /// GoogleInAppBilling.Purchase()が失敗した時のメッセージを受信します。
            /// </summary>
            /// <param name="response">Response.</param>
            /// <param name="message">Message.</param>
            public void purchaseFailed(int response, string message)
            {
                var result = new GoogleIabResult(response, message);
                MainThreadDispatcher.Post(() =>
                {
                    if (PurchaseFailed != null)
                    {
                        PurchaseFailed(result);
                    }
                });
            }

            /// <summary>
            /// GoogleInAppBilling.Purchase()が成功した時のメッセージを受信します。
            /// </summary>
            /// <param name="json">Json.</param>
            public void purchaseSucceeded(string json)
            {
                UnityEngine.Debug.Log("google play result json:" + json);
                var dict = Json.Deserialize(json) as Dictionary<string, object>;
                var purchase = new GoogleIabPurchase(dict);
                MainThreadDispatcher.Post(() =>
                {
                    if (PurchaseSucceeded != null)
                    {
                        PurchaseSucceeded(purchase);
                    }
                });
            }

            /// <summary>
            /// GoogleInAppBilling.Purchase()が成功した時のメッセージを受信します。
            /// ProrationモードがDeferredなどのケース(購入情報がないケース)限定です。
            /// </summary>
            public void purchaseSucceeded()
            {
                MainThreadDispatcher.Post(() =>
                {
                    if (PurchaseSucceeded != null)
                    {
                        PurchaseSucceeded(null);
                    }
                });
            }

            /// <summary>
            /// GoogleInAppBilling.Consume()が失敗した時のメッセージを受信します。
            /// </summary>
            /// <param name="response">Response.</param>
            /// <param name="message">Message.</param>
            public void consumeFailed(int response, string message)
            {
                var result = new GoogleIabResult(response, message);
                MainThreadDispatcher.Post(() =>
                {
                    if (ConsumeFailed != null)
                    {
                        ConsumeFailed(result);
                    }
                });
            }

            /// <summary>
            /// GoogleInAppBilling.Consume()が成功した時のメッセージを受信します。
            /// </summary>
            /// <param name="json">Json.</param>
            public void consumeSucceeded(string json)
            {
                var dict = Json.Deserialize(json) as Dictionary<string, object>;
                var purchase = new GoogleIabPurchase(dict);
                MainThreadDispatcher.Post(() =>
                {
                    if (ConsumeSucceeded != null)
                    {
                        ConsumeSucceeded(purchase);
                    }
                });
            }

            /// <summary>
            /// GoogleInAppBilling.Acknowledge()が失敗した時のメッセージを受信します。
            /// </summary>
            /// <param name="response">Response.</param>
            /// <param name="message">Message.</param>
            public void acknowledgeFailed(int response, string message)
            {
                var result = new GoogleIabResult(response, message);
                MainThreadDispatcher.Post(() =>
                {
                    if (AcknowledgeFailed != null)
                    {
                        AcknowledgeFailed(result);
                    }
                });
            }

            /// <summary>
            /// GoogleInAppBilling.Acknowledge()が成功した時のメッセージを受信します。
            /// </summary>
            public void acknowledgeSucceeded(string json)
            {
                var dict = Json.Deserialize(json) as Dictionary<string, object>;
                var purchase = new GoogleIabPurchase(dict);
                MainThreadDispatcher.Post(() =>
                {
                    if (AcknowledgeSucceeded != null)
                    {
                        AcknowledgeSucceeded(purchase);
                    }
                });
            }

            /// <summary>
            /// GoogleInAppBilling.QueryPurchases()が失敗した時のメッセージを受信します。
            /// </summary>
            /// <param name="response">Response.</param>
            /// <param name="message">Message.</param>
            public void queryPurchasesFailed(int response, string message)
            {
                var result = new GoogleIabResult(response, message);
                MainThreadDispatcher.Post(() =>
                {
                    if (QueryPurchasesFailed != null)
                    {
                        QueryPurchasesFailed(result);
                    }
                });
            }

            /// <summary>
            /// GoogleInAppBilling.QueryPurchases()が成功した時のメッセージを受信します。
            /// </summary>
            /// <param name="json"></param>
            public void queryPurchasesSucceeded(string json)
            {
                var dict = Json.Deserialize(json) as Dictionary<string, object>;

                var purchases = new List<GoogleIabPurchase>();
                if (dict.ContainsKey("Purchases"))
                {
                    foreach (var purchaseObj in (List<object>) dict["Purchases"])
                    {
                        var purchaseDic = (Dictionary<string, object>) purchaseObj;
                        purchases.Add(new GoogleIabPurchase(purchaseDic));
                    }
                }

                var skus = new List<GoogleIabSkuDetail>();
                if (dict.ContainsKey("Skus"))
                {
                    foreach (var skuObj in (List<object>) dict["Skus"])
                    {
                        var skuDic = (Dictionary<string, object>) skuObj;
                        skus.Add(new GoogleIabSkuDetail(skuDic));
                    }
                }

                MainThreadDispatcher.Post(() =>
                {
                    if (QueryPurchasesSucceeded != null)
                    {
                        QueryPurchasesSucceeded(purchases, skus);
                    }
                });
            }
        }
    }
}
#endif

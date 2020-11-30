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
using MiniJSON;

#if UNITY_IOS
namespace Jackpot.Billing
{
    /// <summary>
    /// Jackpotが提供するStoreKitプラグインからのメッセージを受信するためのクラスです。
    /// </summary>
    public class AppleStoreKitListener : MonoBehaviour
    {
        /// <summary>
        /// ネイティブのObserverから通知を受けて利用者側に通知するevent
        /// </summary>
        /// <remarks>
        /// 以下の様にテストURLを外部（ブラウザ、メモ帳など）から叩くと、IAPプロモの通知を取得できます。
        /// itms-services://?action=purchaseIntent&bundleId=jp.klab.jackpot&productIdentifier=jp.klab.jackpot.item10
        /// 
        /// see. https://developer.apple.com/documentation/storekit/in-app_purchase/testing_promoted_in-app_purchases?language=objc
        /// </remarks>>
        public event Action<AppleStoreKitProduct> ShouldAddStorePayment;

        /// <summary>
        /// AppleStoreKit.StartProductsRequest()が成功した時の処理をカスタマイズする際に設定するeventです。
        /// </summary>
        public event Action<List<AppleStoreKitProduct>,List<string>> ProductsRequestSucceeded;

        /// <summary>
        /// AppleStoreKit.StartProductsRequest()が失敗した時の処理をカスタマイズする際に設定するeventです。
        /// </summary>
        public event Action<AppleStoreKitError> ProductsRequestFailed;

        /// <summary>
        /// トランザクションが更新された時の処理をカスタマイズする際に設定するeventです。
        /// </summary>
        /// <remarks>
        /// デリゲートメソッドpaymentQueue:updatedTransactions:が呼び出された時の処理を設定できます。
        /// </remarks>
        public event Action<AppleStoreKitTransaction> TransactionUpdated;

        /// <summary>
        /// トランザクションが削除された時の処理をカスタマイズする際に設定するeventです。
        /// </summary>
        /// <remarks>
        /// デリゲートメソッドpaymentQueue:removedTransactions:が呼び出された時の処理を設定できます。
        /// </remarks>
        public event Action<AppleStoreKitTransaction> TransactionRemoved;

        /// <summary>
        /// 未完了のトランザクションの取得に成功した時の処理をカスタマイズする際に設定するeventです。
        /// </summary>
        public event Action<List<AppleStoreKitTransaction>> ResumeCompletedTransactionSucceeded;

        /// <summary>
        /// トランザクションの復元に成功した時の処理をカスタマイズする際に設定するeventです。
        /// </summary>
        public event Action<List<AppleStoreKitTransaction>> RestoreCompletedTransactionSucceeded;

        /// <summary>
        /// トランザクションの復元に失敗した時の処理をカスタマイズする際に設定するeventです。
        /// </summary>
        /// <remarks>
        /// デリゲートメソッドpaymentQueue:restoreCompletedTransactionsFailedWithError:が呼び出された時の処理を設定できます。
        /// </remarks>
        public event Action<AppleStoreKitError> RestoreCompletedTransactionFailed;

        /// <summary>
        /// AppleStoreKit.StartPayment()が失敗した時の処理をカスタマイズする際に設定するeventです。
        /// </summary>
        public event Action<AppleStoreKitError> PurchaseFailed;

        /// <summary>
        /// AppleStoreKit.RefreshReceipt()が成功した時の処理をカスタマイズする際に設定するeventです。
        /// </summary>
        public event Action<string> RefreshReceiptSucceeded;

        /// <summary>
        /// AppleStoreKit.RefreshReceipt()が失敗した時の処理をカスタマイズする際に設定するeventです。
        /// </summary>
        public event Action<AppleStoreKitError> RefreshReceiptFailed;

        static AppleStoreKitListener instance;

        public static AppleStoreKitListener Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject("AppleStoreKitListener").AddComponent<AppleStoreKitListener>();
                    DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }

        void HandleSucceededProductsRequest(string json)
        {
            if (ProductsRequestSucceeded != null)
            {
                var products = new List<AppleStoreKitProduct>();
                var invalidProductIds = new List<string>();

                var dict = Json.Deserialize(json) as Dictionary<string,object>;

                if (dict.ContainsKey("products"))
                {
                    foreach (var productObj in (List<object>) dict["products"])
                    {
                        var productDic = (Dictionary<string,object>) productObj;
                        products.Add(new AppleStoreKitProduct(productDic));
                    }
                }

                if (dict.ContainsKey("invalidProductIdentifiers"))
                {
                    foreach (var invalidProductId in (List<object>) dict["invalidProductIdentifiers"])
                    {
                        invalidProductIds.Add(invalidProductId as string);
                    }
                }

                ProductsRequestSucceeded(products, invalidProductIds);
            }
        }

        void HandleFailedProductsRequest(string json)
        {
            if (ProductsRequestFailed != null)
            {
                var dict = Json.Deserialize(json) as Dictionary<string,object>;

                ProductsRequestFailed(new AppleStoreKitError(dict));
            }
        }

        void HandleUpdatedTransaction(string json)
        {
            if (TransactionUpdated != null)
            {
                var dict = Json.Deserialize(json) as Dictionary<string,object>;

                TransactionUpdated(new AppleStoreKitTransaction(dict));
            }
        }

        void HandleRemovedTransaction(string json)
        {
            if (TransactionRemoved != null)
            {
                var dict = Json.Deserialize(json) as Dictionary<string,object>;

                TransactionRemoved(new AppleStoreKitTransaction(dict));
            }
        }

        void HandleSucceededResumeCompletedTransaction(string json)
        {
            if (ResumeCompletedTransactionSucceeded != null) {
                var transactions = new List<AppleStoreKitTransaction>();

                var dict = Json.Deserialize(json) as Dictionary<string,object>;

                if (dict.ContainsKey("transactions"))
                {
                    foreach (var tranObj in (List<object>) dict["transactions"])
                    {
                        var tranDic = (Dictionary<string,object>)tranObj;
                        transactions.Add (new AppleStoreKitTransaction (tranDic));
                    }
                }

                ResumeCompletedTransactionSucceeded (transactions);
            }
        }

        void HandleSucceededRestoreCompletedTransaction(string json)
        {
            if (RestoreCompletedTransactionSucceeded != null) {
                var transactions = new List<AppleStoreKitTransaction>();

                var dict = Json.Deserialize(json) as Dictionary<string,object>;

                if (dict.ContainsKey("transactions"))
                {
                    foreach (var tranObj in (List<object>) dict["transactions"])
                    {
                        var tranDic = (Dictionary<string,object>)tranObj;
                        transactions.Add (new AppleStoreKitTransaction (tranDic));
                    }
                }

                RestoreCompletedTransactionSucceeded (transactions);
            }
        }

        void HandleFailedRestoreCompletedTransaction(string json)
        {
            if (RestoreCompletedTransactionFailed != null)
            {
                var dict = Json.Deserialize(json) as Dictionary<string,object>;

                RestoreCompletedTransactionFailed(new AppleStoreKitError(dict));
            }
        }

        void HandleFailedToMakePayment(string json)
        {
            if (PurchaseFailed != null)
            {
                var dict = Json.Deserialize(json) as Dictionary<string,object>;

                PurchaseFailed(new AppleStoreKitError(dict));
            }
        }

        void HandleSucceededRefreshReceipt(string receipt)
        {
            if (RefreshReceiptSucceeded != null)
            {
                RefreshReceiptSucceeded(receipt);
            }
        }

        void HandleFailedRefreshReceipt(string json)
        {
            if (RefreshReceiptFailed != null)
            {
                var dict = Json.Deserialize(json) as Dictionary<string,object>;
                RefreshReceiptFailed(new AppleStoreKitError(dict));
            }
        }

        void HandleShouldAddStorePayment(string json)
        {
            if(ShouldAddStorePayment != null)
            {
                var dict = Json.Deserialize(json) as Dictionary<string,object>;
                ShouldAddStorePayment(new AppleStoreKitProduct(dict));
            }
        }
    }
}
#endif

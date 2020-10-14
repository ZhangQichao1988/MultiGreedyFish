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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MiniJSON;
using System.Threading;

#if UNITY_IOS
namespace Jackpot.Billing
{
    /// <summary>
    /// Jackpotが提供するStoreKitネイティブプラグインの処理を呼び出すためのクラスです。
    /// </summary>
    public class AppleStoreKit
    {
        [DllImport("__Internal")]
        private static extern void _KJPBilling_EnableDebugging();

        [DllImport("__Internal")]
        private static extern bool _KJPBilling_canMakePayments();

        [DllImport("__Internal")]
        private static extern void _KJPBilling_startProductsRequest(string productIds);

        [DllImport("__Internal")]
        private static extern void _KJPBilling_startPayment(string productId);

        [DllImport("__Internal")]
        private static extern void _KJPBilling_finishTransaction(string transactionId);

        [DllImport("__Internal")]
        private static extern string _KJPBilling_getPendingTransactions();
        
        [DllImport("__Internal")]
        private static extern string _KJPBilling_getSavedIAPProductId();

        [DllImport("__Internal")]
        private static extern void _KJPBilling_refreshReceipt();

        [DllImport("__Internal")]
        private static extern void _KJPBilling_resume();
        
        [DllImport("__Internal")]
        private static extern void _KJPBilling_restore();

        /// <summary>
        /// プラグインを初期化します。
        /// </summary>
        /// <remarks>
        /// 内部的にはcanMakePayments()を実行しているだけです。
        /// </remarks>
        /// <returns><c>true</c>, if billing can make payments was KJPed, <c>false</c> otherwise.</returns>
        public static bool Init()
        {
            return _KJPBilling_canMakePayments();
        }

        /// <summary>
        /// デバッグログを表示します。
        /// </summary>
        public static void EnableDebugging()
        {
            _KJPBilling_EnableDebugging();
        }

        /// <summary>
        /// ユーザが支払いを行えるかどうかを判断します。
        /// </summary>
        /// <remarks>
        /// SKPaymentQueueクラスのcanMakePaymentsメソッドを呼び出します。
        /// </remarks>
        /// <returns><c>true</c>, if billing can make payments was KJPed, <c>false</c> otherwise.</returns>
        public static bool CanMakePayments()
        {
            return _KJPBilling_canMakePayments();
        }

        /// <summary>
        /// プロダクト情報を取得します。
        /// </summary>
        /// <remarks>
        /// SKProductsRequestクラスのstartメソッドを呼び出します。
        /// </remarks>
        /// <param name="productIds">Product identifiers.</param>
        public static void StartProductsRequest(string[] productIds)
        {
            _KJPBilling_startProductsRequest(string.Join(",", productIds));
        }

        /// <summary>
        /// 支払い要求を開始します。
        /// </summary>
        /// <remarks>
        /// SKPaymentQueueクラスのaddPaymentメソッドを呼び出します。
        /// </remarks>
        /// <param name="productId">Product identifier.</param>
        public static void StartPayment(string productId)
        {
            _KJPBilling_startPayment(productId);
        }

        /// <summary>
        /// トランザクションを終了します。
        /// </summary>
        /// <remarks>
        /// SKPaymentQueueクラスのfinishTransactionメソッドを呼び出します。
        /// </remarks>
        /// <param name="transactionId">Transaction identifier.</param>
        public static void FinishTransaction(string transactionId)
        {
            _KJPBilling_finishTransaction(transactionId);
        }

        /// <summary>
        /// 保留中のトランザクションの一覧を取得します。
        /// </summary>
        /// <remarks>
        /// SKPaymentQueueクラスのtransactionsプロパティを取得します。
        /// </remarks>
        public static List<AppleStoreKitTransaction> GetPendingTransactions()
        {
            var transactions = new List<AppleStoreKitTransaction>();
            var json = _KJPBilling_getPendingTransactions();
            foreach (var obj in Json.Deserialize(json) as List<object>)
            {
                var dict = obj as Dictionary<string,object>;
                transactions.Add(new AppleStoreKitTransaction(dict));
            }
            return transactions;
        }

        /// <summary>
        /// IAPプロモの商品IDの保存の有無を取得
        /// </summary>
        /// <returns>string</returns>
        public static string GetSavedIAPProductId()
        {
            return _KJPBilling_getSavedIAPProductId();
        }
        
        /// <summary>
        /// レシート情報の更新を行います。
        /// </summary>
        /// <remarks>
        /// SKReceiptRefreshRequestクラスのstartメソッドを呼び出します
        /// </remarks>
        public static void RefreshReceipt()
        {
            _KJPBilling_refreshReceipt();
        }

        public static void Resume()
        {
            _KJPBilling_resume();
        }

        public static void Restore()
        {
            _KJPBilling_restore();
        }
    }
}
#endif

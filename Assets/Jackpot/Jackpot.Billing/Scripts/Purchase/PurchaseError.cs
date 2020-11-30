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

namespace Jackpot.Billing
{
    /// <summary>
    /// 購入時のエラー情報を持つクラスです
    /// </summary>
    public class PurchaseError
    {
#region Properties

        /// <summary>
        /// 指定の端末では購入できない、あるいは購入制限がかかっている事を示します
        /// </summary>
        /// <value><c>true</c> if this instance is billing not supported; otherwise, <c>false</c>.</value>
        public bool IsBillingNotSupported { get; private set; }

        /// <summary>
        /// PrePurchaseRequestが失敗した事を示します
        /// </summary>
        /// <value><c>true</c> if this instance is failed on pre purchase; otherwise, <c>false</c>.</value>
        public bool IsFailedOnPrePurchase { get; private set; }

        /// <summary>
        /// プラットフォームの購入処理がDeferredされた事を示します
        /// </summary>
        /// <value><c>true</c> if this instance is failed on platform purchase; otherwise, <c>false</c>.</value>
        public bool IsDeferredOnPlatformPurchase { get; private set; }

        /// <summary>
        /// プラットフォームの購入処理が失敗した事を示します
        /// </summary>
        /// <value><c>true</c> if this instance is failed on platform purchase; otherwise, <c>false</c>.</value>
        public bool IsFailedOnPlatformPurchase { get; private set; }

        /// <summary>
        /// PurchasedRequestDelegateが失敗した事を示します
        /// </summary>
        /// <value><c>true</c> if this instance is failed on purchased request; otherwise, <c>false</c>.</value>
        public bool IsFailedOnPurchasedRequest { get; private set; }

        /// <summary>
        /// 各プラットフォームの購入終了処理が失敗した事を示します
        /// </summary>
        /// <value><c>true</c> if this instance is failed on platform finish purchase; otherwise, <c>false</c>.</value>
        public bool IsFailedOnPlatformFinishPurchase { get; private set; }

        /// <summary>
        /// [Resumeのみ] Productの再取得に失敗した事を示します
        /// </summary>
        /// <value><c>true</c> if this instance is failed on re query; otherwise, <c>false</c>.</value>
        public bool IsFailedOnReQuery { get; private set; }

        /// <summary>
        /// キャンセルされたことを示します
        /// </summary>
        /// <value><c>true</c> if this instance is cancelled; otherwise, <c>false</c>.</value>
        public bool IsCancelled { get; private set; }

        /// <summary>
        /// 実装上の問題である事を示します
        /// </summary>
        /// <value><c>true</c> if this instance is invalid operation; otherwise, <c>false</c>.</value>
        public bool IsInvalidOperation { get; private set; }

        /// <summary>
        /// アプリサーバーがメンテナンスである事を示します
        /// </summary>
        /// <value><c>true</c> if this instance is maintenance on purchased request; otherwise, <c>false</c>.</value>
        public bool IsMaintenanceAppServer { get; private set; }

        /// <summary>
        /// レシート更新回数が最大リトライ回数に達したことを示します
        /// </summary>
        public bool IsFailedOnRefreshReceipt { get; private set; }

        /// <summary>
        /// アプリケーションが一時停止したことによって課金処理が中断した事を示します
        /// </summary>
        public bool IsCanceledByAppPausing { get; private set; }

        /// <summary>
        /// IAPプロモ商品の取得に失敗した事を示します
        /// </summary>
        public bool IsFailedIAPPromoProduct { get; private set; }

        /// <summary>
        /// エラーが発生した商品IDを示します
        /// </summary>
        /// <value>The product identifier.</value>
        public string ProductId { get; private set; }

        /// <summary>
        /// エラーメッセージを示します
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; private set; }

        /// <summary>
        /// レシート検証結果を示します
        /// </summary>
        public PurchasedResponseKind PurchasedResponseKind { get; set; }

        /// <summary>
        /// ReQueryでエラーになった場合のエラー情報
        /// </summary>
        public QueryError QueryError { get; set; }

#endregion

#region Factory

        public static PurchaseError BillingNotSupported(string productId, string message)
        {
            var result = new PurchaseError(productId, message);
            result.IsBillingNotSupported = true;
            return result;
        }

        public static PurchaseError FailedOnPrePurchase(string productId, string message)
        {
            var result = new PurchaseError(productId, message);
            result.IsFailedOnPrePurchase = true;
            return result;
        }

        public static PurchaseError DeferredOnPlatformPurchase(string productId, string message)
        {
            var result = new PurchaseError(productId, message);
            result.IsDeferredOnPlatformPurchase = true;
            return result;
        }

        public static PurchaseError FailedOnPlatformPurchase(string productId, string message)
        {
            var result = new PurchaseError(productId, message);
            result.IsFailedOnPlatformPurchase = true;
            return result;
        }

        public static PurchaseError FailedOnPurchasedRequest(string productId, string message)
        {
            return FailedOnPurchasedRequest(productId, message, PurchasedResponseKind.Unknown);
        }

        public static PurchaseError FailedOnPurchasedRequest(
            string productId,
            string message,
            PurchasedResponseKind purchasedResponseKind)
        {
            var result = new PurchaseError(productId, message);
            result.IsFailedOnPurchasedRequest = true;
            result.PurchasedResponseKind = purchasedResponseKind;
            return result;
        }

        public static PurchaseError FailedOnPlatformFinishPurchase(string productId, string message)
        {
            var result = new PurchaseError(productId, message);
            result.IsFailedOnPlatformFinishPurchase = true;
            return result;
        }

        public static PurchaseError FailedOnReQuery(string productId, string message)
        {
            var result = new PurchaseError(productId, message);
            result.IsFailedOnReQuery = true;
            return result;
        }

        public static PurchaseError FailedOnReQuery(string productId, QueryError queryError)
        {
            var result = FailedOnReQuery(productId, queryError.Message);
            result.QueryError = queryError;
            return result;
        }

        public static PurchaseError Cancelled(string productId, string message)
        {
            var result = new PurchaseError(productId, message);
            result.IsCancelled = true;
            return result;
        }

        public static PurchaseError InvalidOperation(string productId, string message)
        {
            var result = new PurchaseError(productId, message);
            result.IsInvalidOperation = true;
            return result;
        }

        public static PurchaseError MaintenaceAppServer(string productId, string message)
        {
            var result = new PurchaseError(productId, message);
            result.IsMaintenanceAppServer = true;
            return result;
        }

        public static PurchaseError FailedOnRefreshReceipt(string productId, string message)
        {
            var result = new PurchaseError(productId, message);
            result.IsFailedOnRefreshReceipt = true;
            return result;
        }

        public static PurchaseError CanceledByAppPausing(string productId, string message)
        {
            var result = new PurchaseError(productId, message);
            result.IsCanceledByAppPausing = true;
            return result;
        }

        public static PurchaseError FailedIAPPromoProduct(string productId, string message)
        {
            var result = new PurchaseError(productId, message);
            result.IsFailedIAPPromoProduct = true;
            return result;
        }

#endregion

#region Constructor

        PurchaseError(string productId, string message)
        {
            ProductId = productId;
            Message = message;
        }

#endregion

#region Public Methods

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="Jackpot.Billing.PurchaseError"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="Jackpot.Billing.PurchaseError"/>.</returns>
        public override string ToString()
        {
            if (IsBillingNotSupported)
            {
                return string.Format("BillingNotSupported({0}): {1}", ProductId, Message);
            }
            if (IsFailedOnPrePurchase)
            {
                return string.Format("FailedOnPrePurchase({0}): {1}", ProductId, Message);
            }
            if (IsFailedOnPlatformPurchase)
            {
                return string.Format("FailedOnPlatformPurchase({0}): {1}", ProductId, Message);
            }
            if (IsFailedOnPurchasedRequest)
            {
                return string.Format("FailedOnPurchasedRequest({0}): {1}", ProductId, Message);
            }
            if (IsFailedOnPlatformFinishPurchase)
            {
                return string.Format("FailedOnPlatformConsume({0}): {1}", ProductId, Message);
            }
            if (IsFailedOnReQuery)
            {
                return string.Format("FailedOnRequery({0}): {1}", ProductId, Message);
            }
            if (IsCancelled)
            {
                return string.Format("Cancelled({0}): {1}", ProductId, Message);
            }
            if (IsInvalidOperation)
            {
                return string.Format("InvalidOperation({0}): {1}", ProductId, Message);
            }
            if (IsMaintenanceAppServer)
            {
                return string.Format("IsMaintenanceAppServer({0}): {1}", ProductId, Message);
            }
            if (IsFailedOnRefreshReceipt)
            {
                return string.Format("IsFailedOnRefreshReceipt({0}): {1}", ProductId, Message);
            }
            if (IsCanceledByAppPausing)
            {
                return string.Format("IsCanceledByAppPausing({0}): {1}", ProductId, Message);
            }
            if (IsFailedIAPPromoProduct)
            {
                return string.Format("IsFailedIAPPromoProduct({0}): {1}", ProductId, Message);
            }
            return Message;
        }

#endregion
    }
}

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
namespace Jackpot.Billing
{
    /// <summary>
    /// Jackpot.Billing Androidプラットフォームプラグインが返すレスポンス結果を格納するクラスです。
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// レスポンスコードは以下の2種類に分類されます。
    /// <item><description>In-App Billingで定義されたコード(-3..8)</description></item>
    /// <item><description>Jackpotが提供するBillingServiceで定義されたコード(100, 102)</description></item>
    /// </list>
    /// </remarks>
    public class GoogleIabResult
    {
        /// <summary>
        /// In-App Billingで定義されたレスポンスコード：0 | BILLING_RESPONSE_RESULT_OK
        /// </summary>
        public bool IsBillingResponseResultOk { get; private set; }

        /// <summary>
        /// In-App Billingで定義されたレスポンスコード：1 | BILLING_RESPONSE_RESULT_USER_CANCELED
        /// </summary>
        public bool IsBillingResponseResultUserCanceled { get; private set; }

        /// <summary>
        /// In-App Billingで定義されたレスポンスコード：2 | BILLING_RESPONSE_RESULT_SERVICE_UNAVAILABLE
        /// </summary>
        public bool IsBillingResponseResultServiceUnavailable { get; private set; }

        /// <summary>
        /// In-App Billingで定義されたレスポンスコード：3 | BILLING_RESPONSE_RESULT_BILLING_UNAVAILABLE
        /// </summary>
        public bool IsBillingResponseResultBillingUnavailable { get; private set; }

        /// <summary>
        /// In-App Billingで定義されたレスポンスコード：4 | BILLING_RESPONSE_RESULT_ITEM_UNAVAILABLE
        /// </summary>
        public bool IsBillingResponseResultItemUnavailable { get; private set; }

        /// <summary>
        /// In-App Billingで定義されたレスポンスコード：5 | BILLING_RESPONSE_RESULT_DEVELOPER_ERROR
        /// </summary>
        public bool IsBillingResponseResultDeveloperError { get; private set; }

        /// <summary>
        /// In-App Billingで定義されたレスポンスコード：6 | BILLING_RESPONSE_RESULT_ERROR
        /// </summary>
        public bool IsBillingResponseResultError { get; private set; }

        /// <summary>
        /// In-App Billingで定義されたレスポンスコード：7 | BILLING_RESPONSE_RESULT_ITEM_ALREADY_OWNED
        /// </summary>
        public bool IsBillingResponseResultItemAlreadyOwned { get; private set; }

        /// <summary>
        /// In-App Billingで定義されたレスポンスコード：8 | BILLING_RESPONSE_RESULT_ITEM_NOT_OWNED
        /// </summary>
        public bool IsBillingResponseResultItemNotOwned { get; private set; }

        /// <summary>
        /// In-App Billingで定義されたレスポンスコード：-1 | BILLING_RESPONSE_RESULT_SERVICE_DISCONNECTED
        /// </summary>
        public bool IsBillingResponseResultServiceDisconnected { get; private set; }

        /// <summary>
        /// In-App Billingで定義されたレスポンスコード：-2 | BILLING_RESPONSE_RESULT_FEATURE_NOT_SUPPORTED
        /// </summary>
        public bool IsBillingResponseResultFeatureNotSupported { get; private set; }

        /// <summary>
        /// In-App Billingで定義されたレスポンスコード：-3 | BILLING_RESPONSE_RESULT_SERVICE_TIMEOUT
        /// </summary>
        public bool IsBillingResponseResultServiceTimeout { get; private set; }

        /// <summary>
        /// BillingServiceで定義されたレスポンスコード：100 | BILLING_SERVICE_ERROR
        /// </summary>
        public bool IsBillingServiceError { get; private set; }

        /// <summary>
        /// BillingServiceで定義されたレスポンスコード：102 | PURCHASE_CANCELED_BY_APP_PAUSING
        /// </summary>
        public bool IsPurchaseCanceledByAppPausing{ get; private set; }

        /// <summary>
        /// 不明なレスポンス
        /// </summary>
        public bool IsUnknown { get; private set; }

        /// <summary>
        /// レスポンスコード
        /// </summary>
        public int Response { get; private set; }

        /// <summary>
        /// レスポンスメッセージ
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <see cref="https://developer.android.com/reference/com/android/billingclient/api/BillingClient.BillingResponseCode.html"/>
        /// <param name="response">In-App Billingにおけるレスポンスコード</param>
        /// <param name="message">レスポンスメッセージ</param>
        public GoogleIabResult(int response, string message)
        {
            Response = response;
            Message = message;
            switch (Response)
            {
                case 0:
                    IsBillingResponseResultOk = true;
                    break;
                case 1:
                    IsBillingResponseResultUserCanceled = true;
                    break;
                case 2:
                    IsBillingResponseResultServiceUnavailable = true;
                    break;
                case 3:
                    IsBillingResponseResultBillingUnavailable = true;
                    break;
                case 4:
                    IsBillingResponseResultItemUnavailable = true;
                    break;
                case 5:
                    IsBillingResponseResultDeveloperError = true;
                    break;
                case 6:
                    IsBillingResponseResultError = true;
                    break;
                case 7:
                    IsBillingResponseResultItemAlreadyOwned = true;
                    break;
                case 8:
                    IsBillingResponseResultItemNotOwned = true;
                    break;
                case -1:
                    IsBillingResponseResultServiceDisconnected = true;
                    break;
                case -2:
                    IsBillingResponseResultFeatureNotSupported = true;
                    break;
                case -3:
                    IsBillingResponseResultServiceTimeout = true;
                    break;
                case 100:
                    IsBillingServiceError = true;
                    break;
                case 102:
                    IsPurchaseCanceledByAppPausing = true;
                    break;
                default:
                    IsUnknown = true;
                    break;
            }
        }

        public override string ToString()
        {
            return string.Format("Response:{0},Message:{1}", Response, Message);
        }
    }
}

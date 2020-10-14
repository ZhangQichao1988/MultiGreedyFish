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
using UnityEngine;
using Jackpot.Extensions;

namespace Jackpot.Billing
{
    /// <summary>
    /// 課金に関連する処理を司るクラスです
    /// </summary>
    public class BillingService : MonoBehaviour
    {
#region Properties

        /// <summary>
        /// BillingServiceのシングルトンインスタンスを取得します
        /// </summary>
        /// <value>The instance.</value>
        public static BillingService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject("BillingService").AddComponent<BillingService>();
                }
                return instance;
            }
        }

        /// <summary>
        /// BillingServiceが初期化済か否かを示します
        /// </summary>
        /// <value><c>true</c> if this instance is initialized; otherwise, <c>false</c>.</value>
        public bool IsInitialized { get; private set; }

#endregion

#region Fields

        static BillingService instance;
        ILogger logger;
        AbstractBilling billing;
        ContextQueue<QueryContext> queryQueue;
        ContextQueue<PurchaseContext> purchaseQueue;
        ContextQueue<ResumeContext> resumeQueue;
        ContextQueue<RestoreContext> restoreQueue;

#endregion

#region Unity Methods

        void Awake()
        {
            MainThreadDispatcher.Initialize();
            DontDestroyOnLoad(gameObject);
            logger = Logger.Get<BillingService>();
            billing = null;
            queryQueue = null;
            purchaseQueue = null;
            resumeQueue = null;
            restoreQueue = null;
            IsInitialized = false;
        }

        void Update()
        {
            if (!IsInitialized)
            {
                return;
            }
            queryQueue.ProcessQueue();
            purchaseQueue.ProcessQueue();
            resumeQueue.ProcessQueue();
            restoreQueue.ProcessQueue();
        }

        void OnDestroy()
        {
            if (IsInitialized)
            {
                billing.Dispose();
            }
            instance = null;
        }

        void OnApplicationPause(bool pauseStatus)
        {
#if UNITY_ANDROID
            //Androidのみ、中断された購入処理を完了させるための対応
            if (pauseStatus)
            {
                return;
            }
            if (!IsInitialized)
            {
                return;
            }
            billing.OnAfterPlatformPurchase();
#endif
        }

#endregion

#region Public Methods

        /// <summary>
        /// BillingServiceを初期化します。初期化済の場合はスルーされます
        /// </summary>
        /// <param name="prePurchaseRequest">Pre purchase request.</param>
        /// <param name="purchasedRequest">Purchased request.</param>
        /// <param name="successPurchase">Success purchase.</param>
        /// <param name="failurePurchase">Failure purchase.</param>
        /// <param name="refreshReceiptPurchase">Refresh receipt purchase.</param>
        public void Initialize(
            PrePurchaseRequestDelegate prePurchaseRequest,
            PurchasedRequestDelegate purchasedRequest,
            SuccessPurchaseDelegate successPurchase,
            FailurePurchaseDelegate failurePurchase,
            RefreshReceiptDelegate refreshReceiptPurchase
        )
        {
            Initialize(
                prePurchaseRequest,
                purchasedRequest,
                successPurchase,
                failurePurchase,
                refreshReceiptPurchase,
                null,
                false
            );
        }

        /// <summary>
        /// BillingServiceを初期化します。初期化済の場合はスルーされます
        /// </summary>
        /// <param name="prePurchaseRequest">Pre purchase request.</param>
        /// <param name="purchasedRequest">Purchased request.</param>
        /// <param name="successPurchase">Success purchase.</param>
        /// <param name="failurePurchase">Failure purchase.</param>
        /// <param name="refreshReceiptPurchase">Refresh receipt purchase.</param>
        /// <param name="shouldAddStorePayment">should Add Store Payment.</param>
        public void Initialize(
            PrePurchaseRequestDelegate prePurchaseRequest,
            PurchasedRequestDelegate purchasedRequest,
            SuccessPurchaseDelegate successPurchase,
            FailurePurchaseDelegate failurePurchase,
            RefreshReceiptDelegate refreshReceiptPurchase,
            ShouldAddStorePaymentDelegate shouldAddStorePayment
        )
        {
            Initialize(
                prePurchaseRequest,
                purchasedRequest,
                successPurchase,
                failurePurchase,
                refreshReceiptPurchase,
                shouldAddStorePayment,
                false
            );
        }

        /// <summary>
        /// BillingServiceを初期化します。初期化済の場合はスルーされます
        /// </summary>
        /// <param name="prePurchaseRequest">Pre purchase request.</param>
        /// <param name="purchasedRequest">Purchased request.</param>
        /// <param name="successPurchase">Success purchase.</param>
        /// <param name="failurePurchase">Failure purchase.</param>
        /// <param name="refreshReceiptPurchase">Refresh receipt purchase.</param>
        /// <param name="enableClientAcknowledge">
        /// （Androidでのみ利用されるパラメータ）
        /// クライアント（Jackpot.Billing）における承認処理の有効化フラグ
        /// 有効化する（true）場合：サーバー側での承認処理を実施しないこと
        /// 有効化しない（false）場合：サーバー側での承認処理実施が必須となる
        /// </param>
        public void Initialize(
            PrePurchaseRequestDelegate prePurchaseRequest,
            PurchasedRequestDelegate purchasedRequest,
            SuccessPurchaseDelegate successPurchase,
            FailurePurchaseDelegate failurePurchase,
            RefreshReceiptDelegate refreshReceiptPurchase,
            bool enableClientAcknowledge
        )
        {
            Initialize(
                prePurchaseRequest,
                purchasedRequest,
                successPurchase,
                failurePurchase,
                refreshReceiptPurchase,
                null,
                enableClientAcknowledge
            );
        }

        /// <summary>
        /// BillingServiceを初期化します。初期化済の場合はスルーされます
        /// </summary>
        /// <param name="prePurchaseRequest">Pre purchase request.</param>
        /// <param name="purchasedRequest">Purchased request.</param>
        /// <param name="successPurchase">Success purchase.</param>
        /// <param name="failurePurchase">Failure purchase.</param>
        /// <param name="refreshReceiptPurchase">Refresh receipt purchase.</param>
        /// <param name="shouldAddStorePayment">should Add Store Payment.</param>
        /// <param name="enableClientAcknowledge">
        /// （Androidでのみ利用されるパラメータ）
        /// クライアント（Jackpot.Billing）における承認処理の有効化フラグ
        /// 有効化する（true）場合：サーバー側での承認処理を実施しないこと
        /// 有効化しない（false）場合：サーバー側での承認処理実施が必須となる
        /// </param>
        public void Initialize(
            PrePurchaseRequestDelegate prePurchaseRequest,
            PurchasedRequestDelegate purchasedRequest,
            SuccessPurchaseDelegate successPurchase,
            FailurePurchaseDelegate failurePurchase,
            RefreshReceiptDelegate refreshReceiptPurchase,
            ShouldAddStorePaymentDelegate shouldAddStorePayment,
            bool enableClientAcknowledge
        )
        {
            if (IsInitialized)
            {
                if (IsBillingSupported())
                {
                    logger.Debug("Already initialized");
                    return;
                }
            }
            logger.Debug("Initialize BillingService");
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    billing = new BillingAndroid(
                        prePurchaseRequest,
                        purchasedRequest,
                        successPurchase,
                        failurePurchase,
                        refreshReceiptPurchase,
                        enableClientAcknowledge
                    );
                    break;
                case RuntimePlatform.IPhonePlayer:
                    billing = new BillingIos(
                        prePurchaseRequest,
                        purchasedRequest,
                        successPurchase,
                        failurePurchase,
                        refreshReceiptPurchase,
                        shouldAddStorePayment
                    );
                    break;
                default:
                    billing = new BillingEditor(
                        prePurchaseRequest,
                        purchasedRequest,
                        successPurchase,
                        failurePurchase,
                        refreshReceiptPurchase
                    );
                    break;
            }
            queryQueue = new ContextQueue<QueryContext>(billing);
            purchaseQueue = new ContextQueue<PurchaseContext>(billing);
            resumeQueue = new ContextQueue<ResumeContext>(billing);
            restoreQueue = new ContextQueue<RestoreContext>(billing);
            IsInitialized = true;
        }

        /// <summary>
        /// Resume処理を実施します
        /// </summary>
        public void Resume()
        {
            if (!IsInitialized)
            {
                logger.Error("BillingService has not been initialized yet.");
                return;
            }
            billing.Resume(null);
        }

        /// <summary>
        /// Resume処理を実施します
        /// 消費アイテム/サブスクリプション共に同一のAPIを利用して処理する
        /// </summary>
        /// <param name="onGetResumeProducts">Resumeすべき商品取得時の処理を定義します</param>
        public void Resume(
            Action<List<PurchasedProduct>, Action<List<PurchasedProduct>>> onGetResumeProducts)
        {
            if (!IsInitialized)
            {
                logger.Error("BillingService has not been initialized yet.");
                return;
            }
            billing.Resume(onGetResumeProducts);
        }

        /// <summary>
        /// Resume処理を実施します
        /// 消費アイテム/サブスクリプション共に同一のAPIを利用して処理する
        /// </summary>
        /// <param name="onGetResumeProducts">Resumeすべき商品取得時の処理を定義します</param>
        /// <param name="onFailureGetResumeProducts">Resumeすべき商品取得失敗時の処理を定義します</param>
        public void Resume(
            Action<List<PurchasedProduct>, Action<List<PurchasedProduct>>> onGetResumeProducts,
            Action<ResumeError> onFailureGetResumeProducts)
        {
            if (!IsInitialized)
            {
                logger.Error("BillingService has not been initialized yet.");
                return;
            }
            billing.Resume(
                onGetResumeProducts,
                onFailureGetResumeProducts
            );
        }

        /// <summary>
        /// [Internal] Query the specified context.
        /// </summary>
        /// <param name="context">Context.</param>
        internal void Query(QueryContext context)
        {
            if (!IsInitialized)
            {
                logger.Error("BillingService has not been initialized yet.");
                return;
            }
            queryQueue.Enqueue(context);
        }

        /// <summary>
        /// [Internal] Purchase the specified context.
        /// </summary>
        /// <param name="context">Context.</param>
        internal void Purchase(PurchaseContext context)
        {
            if (!IsInitialized)
            {
                logger.Error("BillingService has not been initialized yet.");
                return;
            }
            purchaseQueue.Enqueue(context);
        }

        /// <summary>
        /// Refresh Receipt
        /// </summary>
        public void RefreshReceipt()
        {
            billing.RefreshReceipt();
        }

        /// <summary>
        /// Canecl Refresh Receipt
        /// </summary>
        public void CaneclRefresh()
        {
            billing.CancelRefreshReceipt();
        }

        public bool IsBillingSupported()
        {
            return IsInitialized ? billing.IsBillingSupported().Unpack((result, _) => result) : false;
        }

        /// <summary>
        /// [Internal] Restore the specified context.
        /// </summary>
        /// <param name="context">Context.</param>
        internal void Restore(RestoreContext context)
        {
            if (!IsInitialized)
            {
                logger.Error("BillingService has not been initialized yet.");
                return;
            }
            restoreQueue.Enqueue(context);
        }

        /// <summary>
        /// [Internal] Resume the specified context.
        /// </summary>
        /// <param name="context">Context.</param>
        internal void Resume(ResumeContext context)
        {
            if (!IsInitialized)
            {
                logger.Error("BillingService has not been initialized yet.");
                return;
            }
            resumeQueue.Enqueue(context);
        }

        /// <summary>
        /// IAPプロモの商品IDの保存の有無を取得
        /// </summary>
        /// <returns>string</returns>
        public string GetSavedIAPPromoProductId()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return AppleStoreKit.GetSavedIAPProductId();
#else
            return string.Empty;
#endif
        }


#if UNITY_ANDROID
        public void CloseAndroidProudct()
        {
            (billing as BillingAndroid).CloseResumeProudct();
        }
#endif

#endregion
    }
}

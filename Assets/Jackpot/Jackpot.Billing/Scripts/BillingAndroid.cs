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
    /// [Internal] Androidでの課金処理を実装するクラスです
    /// </summary>
    internal class BillingAndroid : AbstractBilling
    {

#region Fields

        /// <summary>
        /// Resume時のProductを一時的に保持するList
        /// </summary>
        readonly List<ResumeProduct> resumedProducts;

        /// <summary>
        /// The is billing supported.
        /// </summary>
        bool isBillingSupported;

        /// <summary>
        /// The billing not supported message.
        /// </summary>
        string billingNotSupportedMessage;

        /// <summary>
        /// クライアント（Jackpot.Billing）における承認処理の有効化フラグ
        /// 有効化する（true）場合：サーバー側での承認処理を実施しないこと
        /// 有効化しない（false）場合：サーバー側での承認処理実施が必須となる
        /// </summary>
        bool enableClientAcknowledge;

#endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.Billing.BillingAndroid"/> class.
        /// </summary>
        /// <param name="defaultPrePurchaseRequest">Default pre purchase request.</param>
        /// <param name="defaultPurchasedRequest">Default purchased request.</param>
        /// <param name="defaultSuccessPurchase">Default success purchase.</param>
        /// <param name="defaultFailurePurchase">Default failure purchase.</param>
        /// <param name="defaultRefreshReceipt">Default refresh purchase.</param>
        /// <param name="enableClientAcknowledge">
        /// クライアント（Jackpot.Billing）における承認処理の有効化フラグ
        /// 有効化する（true）場合：サーバー側での承認処理を実施しないこと
        /// 有効化しない（false）場合：サーバー側での承認処理実施が必須となる
        /// </param>
        public BillingAndroid(
            PrePurchaseRequestDelegate defaultPrePurchaseRequest,
            PurchasedRequestDelegate defaultPurchasedRequest,
            SuccessPurchaseDelegate defaultSuccessPurchase,
            FailurePurchaseDelegate defaultFailurePurchase,
            RefreshReceiptDelegate defaultRefreshReceipt,
            bool enableClientAcknowledge) : base(
            Jackpot.Logger.Get<BillingAndroid>(),
            defaultPrePurchaseRequest,
            defaultPurchasedRequest,
            defaultSuccessPurchase,
            defaultFailurePurchase,
            defaultRefreshReceipt)
        {
#if UNITY_ANDROID
            resumedProducts = new List<ResumeProduct>();
            isBillingSupported = false;
            billingNotSupportedMessage = string.Empty;
            this.enableClientAcknowledge = enableClientAcknowledge;

            // init GoogleInAppBilling
            GoogleInAppBilling.EnableLogging(UnityEngine.Debug.isDebugBuild);
            GoogleInAppBilling.SetupSucceeded += HandleSetupSucceeded;
            GoogleInAppBilling.SetupFailed += HandleSetupFailed;
            GoogleInAppBilling.QueryInventorySucceeded += HandleQueryInventorySucceeded;
            GoogleInAppBilling.QueryInventoryFailed += HandleQueryInventoryFailed;
            GoogleInAppBilling.PurchaseSucceeded += HandlePurchaseSucceed;
            GoogleInAppBilling.PurchaseFailed += HandlePurchaseFailed;
            GoogleInAppBilling.ConsumeSucceeded += HandleConsumeSucceeded;
            GoogleInAppBilling.ConsumeFailed += HandleConsumeFailed;
            GoogleInAppBilling.AcknowledgeSucceeded += HandleAcknowledgeSucceeded;
            GoogleInAppBilling.AcknowledgeFailed += HandleAcknowledgeFailed;
            GoogleInAppBilling.RestoreSucceeded += HandleRestoreSucceeded;
            GoogleInAppBilling.RestoreFailed += HandleRestoreFailed;
            GoogleInAppBilling.QueryPurchasesSucceeded += HandleQueryPurchasesSucceeded;
            GoogleInAppBilling.QueryPurchasesFailed += HandleQueryPurchasesFailed;

            GoogleInAppBilling.StartSetup();
#endif
        }

#region Override Methods

        public override void Dispose()
        {
            if (Disposed)
            {
                return;
            }
            Disposed = true;

#if UNITY_ANDROID
            GoogleInAppBilling.SetupSucceeded -= HandleSetupSucceeded;
            GoogleInAppBilling.SetupFailed -= HandleSetupFailed;
            GoogleInAppBilling.QueryInventorySucceeded -= HandleQueryInventorySucceeded;
            GoogleInAppBilling.QueryInventoryFailed -= HandleQueryInventoryFailed;
            GoogleInAppBilling.PurchaseSucceeded -= HandlePurchaseSucceed;
            GoogleInAppBilling.PurchaseFailed -= HandlePurchaseFailed;
            GoogleInAppBilling.ConsumeSucceeded -= HandleConsumeSucceeded;
            GoogleInAppBilling.ConsumeFailed -= HandleConsumeFailed;
            GoogleInAppBilling.AcknowledgeSucceeded -= HandleAcknowledgeSucceeded;
            GoogleInAppBilling.AcknowledgeFailed -= HandleAcknowledgeFailed;
            GoogleInAppBilling.RestoreSucceeded -= HandleRestoreSucceeded;
            GoogleInAppBilling.RestoreFailed -= HandleRestoreFailed;
            GoogleInAppBilling.QueryPurchasesSucceeded -= HandleQueryPurchasesSucceeded;
            GoogleInAppBilling.QueryPurchasesFailed -= HandleQueryPurchasesFailed;

            GoogleInAppBilling.Dispose();
#endif
        }

        /// <summary>
        /// Resume処理を実施します
        /// <param name="onGetResumeProducts">Resumeすべき商品の取得時の処理を定義します</param>
        /// </summary>
        public override void Resume(Action<List<PurchasedProduct>, Action<List<PurchasedProduct>>> onGetResumeProducts)
        {
            Resume(onGetResumeProducts, null);
        }

        /// <summary>
        /// Resume処理を実施します
        /// </summary>
        /// <param name="onGetResumeProducts">Resumeすべき商品取得成功時の処理を定義します</param>
        /// <param name="onFailureGetResumeProduct">Resumeすべき商品取得失敗時の処理を定義します</param>
        public override void Resume(
            Action<List<PurchasedProduct>, Action<List<PurchasedProduct>>> onGetResumeProducts,
            Action<ResumeError> onFailureGetResumeProduct)
        {
#if UNITY_ANDROID
            new ResumeContext()
                .OnSuccessResume((products) =>
                {
                    Logger.Debug("Resume success.");

                    if (onGetResumeProducts != null)
                    {
                        onGetResumeProducts(
                            products,
                            ResumeExecute);
                    }
                    else
                    {
                        ResumeExecute(products);
                    }
                })
                .OnFailureResume((error) =>
                {
                    Logger.Debug("Resume failed.");
                    if (onFailureGetResumeProduct != null)
                    {
                        onFailureGetResumeProduct(error);
                    }
                })
                .Execute();
#endif
        }

        /// <summary>
        /// Resume処理として購入後リクエストを実施します
        /// </summary>
        /// <param name="products">Resume対象の商品リスト</param>
        void ResumeExecute(ICollection<PurchasedProduct> products)
        {
            foreach (var product in products)
            {
                Logger.Debug("Resume purchased request OrderID:{0} ProductID:{1}", product.OrderId, product.Id);

                // QueryPurchases実行によって得られた情報を元にPurchaseを実施する
                HandlePurchasedRequest(product);
            }
        }

        /// <summary>
        /// Resume処理として、ネイティブ側のQueryPurchasesを実施する
        /// </summary>
        protected override void Resume()
        {
#if UNITY_ANDROID
            GoogleInAppBilling.QueryPurchases();
#endif
        }

        /// <summary>
        /// 商品情報取得処理を呼び出す
        /// </summary>
        /// <param name="productIds">商品IDリスト</param>
        protected override void QueryProductsCore(List<string> productIds)
        {
            QueryProductsCore(productIds, null);
        }

        /// <summary>
        /// 商品情報取得処理を呼び出す
        /// </summary>
        /// <param name="productIds">消費型アイテムの商品IDリスト</param>
        /// <param name="subsIds">非消費型（定期購入）アイテムの商品IDリスト</param>
        protected override void QueryProductsCore(List<string> productIds, List<string> subsIds)
        {
#if UNITY_ANDROID
            if (productIds == null)
            {
                productIds = new List<string>();
            }
            if (subsIds == null)
            {
                subsIds = new List<string>();
            }
            GoogleInAppBilling.QueryInventory(productIds.ToArray(), subsIds.ToArray());
#endif
        }

        /// <summary>
        /// 課金機能がサポートされているアプリ・端末かを判断
        /// </summary>
        /// <returns>サポートの有無・サポートされていない場合のメッセージ（サポートされていた場合は空文字列）をもつタプル</returns>
        public override Tuple<bool, string> IsBillingSupported()
        {
            var isSupported = true;
            var message = string.Empty;
#if UNITY_ANDROID
            isSupported = isBillingSupported;
            message = billingNotSupportedMessage;
#endif
            return Tuple.Create(isSupported, message);
        }

        /// <summary>
        /// 購入処理を呼び出す
        /// </summary>
        /// <param name="prePurchaseProduct">購入対象の商品情報</param>
        protected override void PurchaseCore(PrePurchaseProduct prePurchaseProduct)
        {
#if UNITY_ANDROID
            if (prePurchaseProduct.SkuToReplace != null)
            {
                // Google側現行のデフォルト動作は「ImmediateWithTimeProration」である
                switch (prePurchaseProduct.ProrationMode)
                {
                    case(GoogleIabProrationMode.ImmediateWithTimeProration):
                        GoogleInAppBilling.PurchaseWithTimeProration(
                            prePurchaseProduct.Id,
                            prePurchaseProduct.AccountId,
                            prePurchaseProduct.ProfileId,
                            prePurchaseProduct.SkuToReplace
                        );
                        break;
                    case(GoogleIabProrationMode.ImmediateAndChargeProratedPrice):
                        GoogleInAppBilling.PurchaseWithChargeProratedPrice(
                            prePurchaseProduct.Id,
                            prePurchaseProduct.AccountId,
                            prePurchaseProduct.ProfileId,
                            prePurchaseProduct.SkuToReplace
                        );
                        break;
                    case(GoogleIabProrationMode.ImmediateWithoutProration):
                        GoogleInAppBilling.PurchaseWithoutProration(
                            prePurchaseProduct.Id,
                            prePurchaseProduct.AccountId,
                            prePurchaseProduct.ProfileId,
                            prePurchaseProduct.SkuToReplace
                        );
                        break;
                    case(GoogleIabProrationMode.Deferred):
                        GoogleInAppBilling.PurchaseWithDeferred(
                            prePurchaseProduct.Id,
                            prePurchaseProduct.AccountId,
                            prePurchaseProduct.ProfileId,
                            prePurchaseProduct.SkuToReplace
                        );
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                // 通常の購入処理（消費型・非消費型の双方に対応）
                GoogleInAppBilling.Purchase(
                    prePurchaseProduct.Id,
                    prePurchaseProduct.AccountId,
                    prePurchaseProduct.ProfileId
                );
            }
#endif
        }

        /// <summary>
        /// 購入処理完了後の処理を呼び出す
        /// </summary>
        /// <param name="purchasedProduct">購入した商品</param>
        protected override void FinishPurchaseCore(PurchasedProduct purchasedProduct)
        {
#if UNITY_ANDROID
            if (purchasedProduct.Type == ProductType.Subscription)
            {
                // 承認済みのレシートが来る可能性がある
                // それらに対して承認処理を実施するとエラーが発生するケースがあるため承認処理を行わない
                if (purchasedProduct.Acknowledged)
                {
                    // 承認済みの場合は、承認処理は実施せずに購入処理成功時のハンドラーを呼ぶ
                    Logger.Debug("Already acknowledged OrderID: {0} ProductID: {1}", purchasedProduct.OrderId, purchasedProduct.Id);
                    HandleSuccessPurchase(purchasedProduct, PurchaseSuccessTypes.Normal);
                }
                else
                {
                    // enableClientAcknowledgeがtrueである場合にのみ承認処理を実施する
                    if (enableClientAcknowledge)
                    {
                        // クライアントでの承認処理を実施する
                        // このケースでは、サーバー側での承認処理を行わないこと
                        GoogleInAppBilling.Acknowledge(purchasedProduct.Id, purchasedProduct.PurchaseToken);
                    }
                    else
                    {
                        Logger.Debug("Disable acknowledge.");

                        // クライアントでの承認処理を実施せずに、購入処理成功時のハンドラーを呼ぶ
                        // このケースでは、サーバー側での承認処理が必須となる
                        HandleSuccessPurchase(purchasedProduct, PurchaseSuccessTypes.Normal);
                    }
                }
            }
            else
            {
                // 承認処理を含む消費処理を実施する
                GoogleInAppBilling.Consume(purchasedProduct.Id, purchasedProduct.PurchaseToken);
            }
#endif
        }

        /// <summary>
        /// Resumeによる購入処理の終了時の処理
        /// </summary>
        /// <param name="purchasedProduct">Resumeによる購入処理の対象となった商品情報</param>
        /// <param name="completeReQuery">Resumeによる購入処理完了時の処理</param>
        protected override void ReQueryProductsCore(PurchasedProduct purchasedProduct, Action<Product, PurchasedProduct, PurchaseSuccessTypes> completeReQuery)
        {
            // Androidの場合、QueryPurchases実施で対応するProductを保存しているため、このタイミングで対応するProductIdのProductを返却
#if UNITY_ANDROID
            var resumedProduct = resumedProducts.Find((product) => product.Id == purchasedProduct.Id);
            if (resumedProduct != null)
            {
                resumedProducts.Remove(resumedProduct);
            }
            completeReQuery(resumedProduct, purchasedProduct, PurchaseSuccessTypes.Normal);
#endif
        }

        public override void RefreshReceipt()
        {
            CurrentPurchasedProduct = null;
        }

        public override void OnAfterPlatformPurchase()
        {
#if UNITY_ANDROID
            if (CurrentPurchaseContext == null)
            {
                return;
            }
            //購入処理が正常に終了した場合、途中でアプリから離れ購入処理が中断した場合
            //どちらにおいてもBillingService.OnApplicationPause()から本メソッドが実行されます。
            //本メソッドが呼ばれた時点ではUnity上で購入処理が完了(or 中断)した事を確認することができません。
            //中断した購入処理があった場合は以下のEnsureFinishPausedPurchase()の結果がtrueになります。
            //その場合も結果は HandlePurchaseFailedで処理されます。
            var result = GoogleInAppBilling.EnsureFinishPausedPurchase();
            Logger.Debug("EnsureFinishPausedPurchase:" + result);
#endif
        }

        /// <summary>
        /// Restore処理として、ネイティブ側のQueryPurchasesを実施する
        /// </summary>
        public override void Restore()
        {
#if UNITY_ANDROID
            // Restoreの対象は定期購入のみのため、ProductTypeにSubscriptionを指定する
            GoogleInAppBilling.QueryPurchases(ProductType.Subscription);
#endif
        }

        /// <summary>
        /// リストア完了時の処理
        /// </summary>
        /// <param name="restoredProducts"></param>
        protected override void FinishRestoredProducts(List<PurchasedProduct> restoredProducts)
        {
            // 購入正常終了時、またはResume時にのみAcknowledgeを行うようにするため、ここではAcknowledgeしない
            // (消費アイテムのConsumeと同様に、PurchaseContextを通してAcknowledgeを行うようにする)
        }

#endregion

#if UNITY_ANDROID

        void HandleQueryInventorySucceeded(List<GoogleIabSkuDetail> skus)
        {
            Logger.Debug("QueryInventorySucceededEvent");

            // GoogleIabSkuInfoをDataModelに
            var products = new List<Product>();
            skus.ForEach((sku) =>
            {
                products.Add(
                    new Product(
                        sku.ProductId,
                        sku.Type,
                        sku.Title,
                        sku.Description,
                        sku.Price,
                        sku.FormattedPrice,
                        Currency.Resolve(sku.PriceCurrencyCode)
                    )
                );
            });

            HandleSuccessQuery(products);
        }

        void HandleQueryInventoryFailed(GoogleIabResult result)
        {
            Logger.Debug("ProductListRequestFailed: " + result);

            HandleFailureQuery(QueryError.FailedOnPlatformRequest(result.Message));
        }

        void HandleSetupSucceeded()
        {
            isBillingSupported = true;
        }

        void HandleSetupFailed(GoogleIabResult result)
        {
            isBillingSupported = false;
            billingNotSupportedMessage = result.Message;
        }

        void HandlePurchaseSucceed(GoogleIabPurchase purchase)
        {
            Logger.Debug("PurchaseSucceededEvent");
            if (purchase != null)
            {
                Logger.Debug("OriginalJson:" + purchase.OriginalJson);
            }

            if (purchase == null)
            {
                HandleSuccessPurchase(null, PurchaseSuccessTypes.NoPurchases);
            }
            else if (purchase.IsPending)
            {
                HandleSuccessPurchase(
                    new PurchasedProduct(
                        purchase.ProductId,
                        purchase.ItemType,
                        purchase.OriginalJson,
                        purchase.Signature,
                        purchase.OrderId,
                        purchase.PackageName,
                        purchase.PurchaseTime,
                        purchase.PurchaseToken,
                        purchase.AutoRenewing,
                        purchase.Acknowledged),
                    PurchaseSuccessTypes.Pending);
            }
            else
            {
                HandlePurchasedRequest(
                    new PurchasedProduct(
                        purchase.ProductId,
                        purchase.ItemType,
                        purchase.OriginalJson,
                        purchase.Signature,
                        purchase.OrderId,
                        purchase.PackageName,
                        purchase.PurchaseTime,
                        purchase.PurchaseToken,
                        purchase.AutoRenewing,
                        purchase.Acknowledged
                    )
                );
            }
        }

        /// <summary>
        /// 購入処理失敗時の処理
        /// </summary>
        /// <param name="result">購入処理結果</param>
        void HandlePurchaseFailed(GoogleIabResult result)
        {
            Logger.Debug("purchase failed with error: " + result);

            var productId = CurrentPurchaseContext == null
                ? "Unknown"
                : CurrentPurchaseContext.Product.Id;
            PurchaseError error;

            if (result.IsBillingResponseResultUserCanceled)
            {
                error = PurchaseError.Cancelled(productId, result.Message);
            }
            else if (result.IsPurchaseCanceledByAppPausing)
            {
                error = PurchaseError.CanceledByAppPausing(productId, result.Message);
            }
            else
            {
                error = PurchaseError.FailedOnPlatformPurchase(productId, result.Message);
            }

            HandleFailurePurchase(error);
        }

        /// <summary>
        /// 消費処理成功時の処理を呼び出す
        /// </summary>
        /// <param name="googlePurchase"></param>
        void HandleConsumeSucceeded(GoogleIabPurchase googlePurchase)
        {
            Logger.Debug("HandleConsumeSucceeded: {0}", googlePurchase.ProductId);

            var purchasedProduct = new PurchasedProduct(
                googlePurchase.ProductId,
                googlePurchase.ItemType,
                googlePurchase.OriginalJson,
                googlePurchase.Signature,
                googlePurchase.OrderId,
                googlePurchase.PackageName,
                googlePurchase.PurchaseTime,
                googlePurchase.PurchaseToken,
                googlePurchase.AutoRenewing,
                googlePurchase.Acknowledged
            );

            HandleSuccessPurchase(purchasedProduct, PurchaseSuccessTypes.Normal);
        }

        /// <summary>
        /// 消費処理失敗時の処理を呼び出す
        /// </summary>
        /// <param name="result">消費処理結果</param>
        void HandleConsumeFailed(GoogleIabResult result)
        {
            Logger.Debug("HandleConsumeFailed: " + result);
            HandleFailurePurchase(
                PurchaseError.FailedOnPlatformFinishPurchase(
                    CurrentPurchaseContext == null
                        ? "Unknown"
                        : CurrentPurchaseContext.Product.Id,
                    result.Message
                )
            );
        }

        /// <summary>
        /// 承認処理成功時の処理を呼び出す
        /// </summary>
        void HandleAcknowledgeSucceeded(GoogleIabPurchase googlePurchase)
        {
            Logger.Debug("HandleAcknowledgeSucceeded: {0}", googlePurchase.ProductId);

            var purchasedProduct = new PurchasedProduct(
                googlePurchase.ProductId,
                googlePurchase.ItemType,
                googlePurchase.OriginalJson,
                googlePurchase.Signature,
                googlePurchase.OrderId,
                googlePurchase.PackageName,
                googlePurchase.PurchaseTime,
                googlePurchase.PurchaseToken,
                googlePurchase.AutoRenewing,
                googlePurchase.Acknowledged
            );

            HandleSuccessPurchase(purchasedProduct, PurchaseSuccessTypes.Normal);
        }

        /// <summary>
        /// 承認処理失敗時の処理を呼び出す
        /// </summary>
        /// <param name="result">承認処理結果</param>
        void HandleAcknowledgeFailed(GoogleIabResult result)
        {
            Logger.Debug("HandleAcknowledgeFailed: " + result);
            HandleFailurePurchase(
                PurchaseError.FailedOnPlatformFinishPurchase(
                    CurrentPurchaseContext == null
                        ? "Unknown"
                        : CurrentPurchaseContext.Product.Id,
                    result.Message
                )
            );
        }

        void HandleRestoreSucceeded(List<GoogleIabPurchase> purchases)
        {
            Logger.Debug("RestoreRequestSuccessful");
            var products = new List<PurchasedProduct>();
            if (purchases.Count > 0)
            {
                foreach (var purchase in purchases)
                {
                    products.Add(new PurchasedProduct(
                        purchase.ProductId,
                        purchase.ItemType,
                        purchase.OriginalJson,
                        purchase.Signature,
                        purchase.OrderId,
                        purchase.PackageName,
                        purchase.PurchaseTime,
                        purchase.PurchaseToken,
                        purchase.AutoRenewing,
                        purchase.Acknowledged
                    ));
                }
            }
            HandleSuccessRestore(products);
        }

        void HandleRestoreFailed(GoogleIabResult result)
        {
            Logger.Debug("RestoreRequestFailed: " + result);
            HandleFailureRestore(RestoreError.FailedOnPlatformRequest(result.Message));
        }

        /// <summary>
        /// QueryPurchases成功時の処理
        /// ※ QueryPurchasesはResume/Restoreから実施される
        /// </summary>
        /// <param name="purchases">QueryPurchasesによって取得された購入情報リスト</param>
        /// <param name="skuDetails">purchases内の購入に紐づく商品詳細情報リスト</param>
        void HandleQueryPurchasesSucceeded(List<GoogleIabPurchase> purchases, List<GoogleIabSkuDetail> skuDetails)
        {
            Logger.Debug("HandleQueryPurchasesSucceeded");

            var products = new List<PurchasedProduct>();
            foreach (var purchase in purchases)
            {
                var resumedSkuDetail = skuDetails.Find((skuDetail) => skuDetail.ProductId == purchase.ProductId);
                if (resumedSkuDetail == null)
                {
                    HandleFailureResume(ResumeError.FailedOnPlatformRequest("resume product sku detail is not exist."));
                    return;
                }

                // QueryPurchases実行によって得られた情報を元にPurchaseを実施する
                products.Add(
                    new PurchasedProduct(
                        purchase.ProductId,
                        purchase.ItemType,
                        purchase.OriginalJson,
                        purchase.Signature,
                        purchase.OrderId,
                        purchase.PackageName,
                        purchase.PurchaseTime,
                        purchase.PurchaseToken,
                        purchase.AutoRenewing,
                        purchase.Acknowledged
                    )
                );

                resumedProducts.Add(
                    new ResumeProduct(
                        resumedSkuDetail.ProductId,
                        resumedSkuDetail.Type,
                        resumedSkuDetail.Title,
                        resumedSkuDetail.Description,
                        resumedSkuDetail.Price,
                        resumedSkuDetail.FormattedPrice,
                        Currency.Resolve(resumedSkuDetail.PriceCurrencyCode),
                        purchase.PurchaseToken
                    )
                );
            }

            if (CurrentRestoreContext != null)
            {
                // リストア処理によって実行されたQueryPurchasesである場合はリストアのハンドラーを利用する
                HandleSuccessRestore(products);
            }
            else
            {
                HandleSuccessResume(products);
            }
        }

        /// <summary>
        /// QueryPurchases失敗時の処理
        /// ※ QueryPurchasesはResume/Restoreから実施される
        /// </summary>
        /// <param name="result">QueryPurchases処理結果</param>
        void HandleQueryPurchasesFailed(GoogleIabResult result)
        {
            Logger.Debug("HandleQueryPurchasesFailed: " + result);

            if (CurrentRestoreContext != null)
            {
                // リストア処理によって実行されたQueryPurchasesである場合はリストアのハンドラーを利用する
                HandleFailureRestore(RestoreError.FailedOnPlatformRequest(result.Message));
            }
            else
            {
                HandleFailureResume(ResumeError.FailedOnPlatformRequest(result.Message));
            }
        }

        public void CloseResumeProudct()
        {
            if (resumedProducts != null)
            {
                if (resumedProducts.Count > 0)
                {
                    foreach (var item in resumedProducts)
                    {
                        GoogleInAppBilling.Consume(item.Id, item.PurchaseToken);
                    }
                }
                resumedProducts.Clear();
            }
        }
#endif
    }
}

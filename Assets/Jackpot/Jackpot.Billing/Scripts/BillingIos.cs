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
    /// [Internal]iOSでの課金処理を実装するクラスです
    /// </summary>
    internal class BillingIos : AbstractBilling
    {
#region Properties

        private ShouldAddStorePaymentDelegate shouldAddStorePayment;

#endregion

#region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.Billing.BillingIos"/> class.
        /// </summary>
        /// <param name="defaultPrePurchaseRequest">Default pre purchase request.</param>
        /// <param name="defaultPurchasedRequest">Default purchased request.</param>
        /// <param name="defaultSuccessPurchase">Default success purchase.</param>
        /// <param name="defaultFailurePurchase">Default failure purchase.</param>
        /// <param name="defaultRefreshReceipt">Default refresh purchase.</param>
        /// <param name="shouldAddStorePayment">should Add Store Payment.</param>
        public BillingIos(
            PrePurchaseRequestDelegate defaultPrePurchaseRequest,
            PurchasedRequestDelegate defaultPurchasedRequest,
            SuccessPurchaseDelegate defaultSuccessPurchase,
            FailurePurchaseDelegate defaultFailurePurchase,
            RefreshReceiptDelegate defaultRefreshReceipt,
            ShouldAddStorePaymentDelegate shouldAddStorePayment) : base(
            Jackpot.Logger.Get<BillingIos>(),
            defaultPrePurchaseRequest,
            defaultPurchasedRequest,
            defaultSuccessPurchase,
            defaultFailurePurchase,
            defaultRefreshReceipt)
        {
#if UNITY_IOS
            this.shouldAddStorePayment = shouldAddStorePayment;

            AppleStoreKitListener.Instance.ProductsRequestSucceeded += ProductsReceived;
            AppleStoreKitListener.Instance.ProductsRequestFailed += ProductsRequestFailed;
            AppleStoreKitListener.Instance.TransactionUpdated += HandleTransactionUpdated;
            AppleStoreKitListener.Instance.TransactionRemoved += HandleTransactionRemoved;
            AppleStoreKitListener.Instance.PurchaseFailed += PurchaseFailed;
            AppleStoreKitListener.Instance.RefreshReceiptSucceeded += RefreshReceiptSucceeded;
            AppleStoreKitListener.Instance.RefreshReceiptFailed += RefreshReceiptFailed;
            AppleStoreKitListener.Instance.ResumeCompletedTransactionSucceeded += HandleResumeSucceeded;
            AppleStoreKitListener.Instance.RestoreCompletedTransactionSucceeded += HandleRestoreSucceeded;
            AppleStoreKitListener.Instance.RestoreCompletedTransactionFailed += HandleRestoreFailed;
            AppleStoreKitListener.Instance.ShouldAddStorePayment += HandleShouldAddStorePayment;

            AppleStoreKit.Init();
            if (UnityEngine.Debug.isDebugBuild)
            {
                AppleStoreKit.EnableDebugging();
            }
            //StoreKitManager.productPurchaseAwaitingConfirmationEvent += HandleProductPurchaseAwaitingConfirmation;
#endif
        }

#endregion

#region Override Methods

        public override void Dispose()
        {
            if (Disposed)
            {
                return;
            }
            Disposed = true;


#if UNITY_IOS
            AppleStoreKitListener.Instance.ProductsRequestSucceeded -= ProductsReceived;
            AppleStoreKitListener.Instance.ProductsRequestFailed -= ProductsRequestFailed;
            AppleStoreKitListener.Instance.TransactionUpdated -= HandleTransactionUpdated;
            AppleStoreKitListener.Instance.TransactionRemoved -= HandleTransactionRemoved;
            AppleStoreKitListener.Instance.PurchaseFailed -= PurchaseFailed;
            AppleStoreKitListener.Instance.RefreshReceiptSucceeded -= RefreshReceiptSucceeded;
            AppleStoreKitListener.Instance.RefreshReceiptFailed -= RefreshReceiptFailed;
            AppleStoreKitListener.Instance.ResumeCompletedTransactionSucceeded -= HandleResumeSucceeded;
            AppleStoreKitListener.Instance.RestoreCompletedTransactionSucceeded -= HandleRestoreSucceeded;
            AppleStoreKitListener.Instance.RestoreCompletedTransactionFailed -= HandleRestoreFailed;
            AppleStoreKitListener.Instance.ShouldAddStorePayment -= HandleShouldAddStorePayment;
#endif
        }

        /// <summary>
        /// Resume処理を実施します
        /// <param name="onGetResumeProducts">Resumeすべき商品の取得時の処理を定義します</param>
        /// </summary>
        public override void Resume(
            Action<List<PurchasedProduct>, Action<List<PurchasedProduct>>> onGetResumeProducts)
        {
            Resume(onGetResumeProducts, null);
        }

        /// <summary>
        /// Resume処理を実施します
        /// </summary>
        /// <param name="onGetResumeProducts">Resumeすべき商品の取得時の処理を定義します</param>
        /// <param name="onFailureGetResumeProduct">Resumeすべき商品取得失敗時の処理を定義します</param>
        public override void Resume(
            Action<List<PurchasedProduct>, Action<List<PurchasedProduct>>> onGetResumeProducts,
            Action<ResumeError> onFailureGetResumeProduct)
        {
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
        }

        /// <summary>
        /// Resume処理として購入後リクエストを実施します
        /// </summary>
        /// <param name="products">Resume対象の商品リスト</param>
        void ResumeExecute(ICollection<PurchasedProduct> products)
        {
            foreach (var product in products)
            {
                Logger.Debug("Resume purchased request TransactionID:{0} ProductID:{1}", product.TransactionId, product.Id);
                HandlePurchasedRequest(product);
            }
        }

        protected override void Resume()
        {
#if UNITY_IOS
            AppleStoreKit.Resume();
#endif
        }

        protected override void QueryProductsCore(List<string> productIds)
        {
            QueryProductsCore(productIds, null);
        }

        protected override void QueryProductsCore(List<string> productIds, List<string> subsIds)
        {
#if UNITY_IOS
            var products = new List<string>();
            if (productIds != null)
            {
                products.AddRange(productIds);
            }
            if (subsIds != null)
            {
                products.AddRange(subsIds);
            }
            if (products.Count <= 0)
            {
                HandleSuccessQuery(new List<Product>());
                return;
            }
            AppleStoreKit.StartProductsRequest(products.ToArray());
#endif
        }

        public override Tuple<bool, string> IsBillingSupported()
        {
            var supported = true;
            var message = string.Empty;
#if UNITY_IOS
            supported = AppleStoreKit.CanMakePayments();
            message = supported ? string.Empty : "Cannot make payments";
#endif
            return Tuple.Create<bool, string>(supported, message);
        }

        protected override void PurchaseCore(PrePurchaseProduct prePurchaseProduct)
        {
#if UNITY_IOS
            AppleStoreKit.StartPayment(prePurchaseProduct.Id);
#endif
        }

        protected override void FinishPurchaseCore(PurchasedProduct purchasedProduct)
        {
            // iOSの場合は普通にtransactionを終了する
#if UNITY_IOS
            AppleStoreKit.FinishTransaction(purchasedProduct.TransactionId);
            this.Logger.Debug("PurchaseSuccessful");
            HandleSuccessPurchase(purchasedProduct, PurchaseSuccessTypes.Normal);
#endif
        }

        protected override void ReQueryProductsCore(PurchasedProduct purchasedProduct, Action<Product, PurchasedProduct, PurchaseSuccessTypes> completeReQuery)
        {
            // iOSの場合はrequestProductを実施
            // 現在購入中のものとコンテキストが重複するとデータの整合性とれなくて死ぬので
            // BillingServiceを使用してキューイングする
#if UNITY_IOS
            new QueryContext()
                .OnQueryRequest((startQuery, failQuery) => startQuery(new List<string>() { purchasedProduct.Id }))
                .OnSuccess(products =>
                {
                    if (products.Count == 0 || products[0].Id != purchasedProduct.Id)
                    {
                        HandleFailurePurchase(
                            PurchaseError.InvalidOperation(
                                purchasedProduct.Id,
                                string.Format(
                                    "Excepted ProductId {0}, but was {1}.",
                                    purchasedProduct.Id,
                                    products.Count == 0 ? "empty" : products[0].Id
                                )
                            )
                        );
                        return;
                    }
                    completeReQuery(products[0], purchasedProduct, PurchaseSuccessTypes.Normal);
                })
                .OnFailure(error => HandleFailurePurchase(PurchaseError.FailedOnReQuery(purchasedProduct.Id, error)))
                .Execute();
#endif
        }

        public override void RefreshReceipt()
        {
#if UNITY_IOS
            AppleStoreKit.RefreshReceipt();
#endif
        }

        public override void OnAfterPlatformPurchase()
        {
            // 特になにもしない
        }

        public override void Restore()
        {
#if UNITY_IOS
            AppleStoreKit.Restore();
#endif
        }

#endregion

#if UNITY_IOS
        void HandleShouldAddStorePayment(AppleStoreKitProduct storeKitProduct)
        {
            if (this.shouldAddStorePayment != null)
            {
                this.shouldAddStorePayment(new Product(
                    storeKitProduct.ProductIdentifier,
                    null,
                    storeKitProduct.LocalizedTitle,
                    storeKitProduct.LocalizedDescription,
                    storeKitProduct.Price.ToString(),
                    storeKitProduct.FormattedPrice,
                    Currency.Resolve(storeKitProduct.CurrencyCode)));
            }
        }

        void ProductsReceived(List<AppleStoreKitProduct> storeKitProductList, List<string> invalidProductIds)
        {
            this.Logger.Debug("ProductListReceived. total products received: " + storeKitProductList.Count);

            var products = new List<Product>();
            foreach (var storeKitProduct in storeKitProductList)
            {
                this.Logger.Debug(storeKitProduct.ToString());

                products.Add(
                    new Product(
                        storeKitProduct.ProductIdentifier,
                        null,
                        storeKitProduct.LocalizedTitle,
                        storeKitProduct.LocalizedDescription,
                        storeKitProduct.Price.ToString(),
                        storeKitProduct.FormattedPrice,
                        Currency.Resolve(storeKitProduct.CurrencyCode)
                    )
                );
            }
            HandleSuccessQuery(products);
        }

        void ProductsRequestFailed(AppleStoreKitError error)
        {
            this.Logger.Debug("ProductListRequestFailed: " + error.ToString());

            HandleFailureQuery(QueryError.FailedOnPlatformRequest(error.Description));
        }

        void HandleTransactionUpdated(AppleStoreKitTransaction transaction)
        {
            this.Logger.Debug("TransactionUpdated: " + transaction.TransactionState);
            switch (transaction.TransactionState)
            {
                case AppleStoreKitTransactionState.Deferred:
                    HandlePurchaseDeferred(transaction.ProductIdentifier);
                    break;
                case AppleStoreKitTransactionState.Purchased:
                    HandleProductPurchaseAwaitingConfirmation(transaction);
                    break;
                case AppleStoreKitTransactionState.Failed:
                    if (transaction.Error != null)
                    {
                        if (transaction.Error.IsPaymentCancelled)
                        {
                            HandlePurchaseCancelled(transaction);
                        }
                        else
                        {
                            HandlePurchaseFailed(transaction);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        void HandleTransactionRemoved(AppleStoreKitTransaction transaction)
        {
            this.Logger.Debug("HandleTransactionRemoved: " + transaction.TransactionState);
        }

        void HandleProductPurchaseAwaitingConfirmation(AppleStoreKitTransaction transaction)
        {
            this.Logger.Debug("PurchaseSuccessful");
            var purchasingProduct = new PurchasedProduct(
                transaction.ProductIdentifier,
                transaction.TransactionIdentifier,
                transaction.Base64EncodedReceipt,
                transaction.TransactionDate,
                transaction.OriginalTransactionIdentifier
            );
            HandlePurchasedRequest(purchasingProduct);
        }

        void HandlePurchaseDeferred(string productId)
        {
            HandleFailurePurchase(PurchaseError.DeferredOnPlatformPurchase(
                productId,
                "DeferredPrePurchaseDelegate was not set."
            ));
        }

        void HandlePurchaseCancelled(AppleStoreKitTransaction transaction)
        {
            HandleFailurePurchase(
                PurchaseError.Cancelled(
                    CurrentPurchaseContext == null
                        ? "Unknown"
                        : CurrentPurchaseContext.Product.Id,
                    transaction.Error.Description
                )
            );
        }

        void HandlePurchaseFailed(AppleStoreKitTransaction transaction)
        {
            this.Logger.Debug("purchase failed with error: " + transaction.ToString());
            HandleFailurePurchase(
                PurchaseError.FailedOnPlatformPurchase(
                    CurrentPurchaseContext == null
                        ? "Unknown"
                        : CurrentPurchaseContext.Product.Id,
                    transaction.Error.Description
                )
            );
        }

        void PurchaseFailed(AppleStoreKitError error)
        {
            this.Logger.Debug("PurchaseFailed: " + error.ToString());
            HandleFailurePurchase(
                PurchaseError.FailedOnPlatformPurchase(
                    CurrentPurchaseContext == null
                        ? "Unknown"
                        : CurrentPurchaseContext.Product.Id,
                    error.Description
                )
            );
        }

        void RefreshReceiptSucceeded(string refreshReceipt)
        {
            this.Logger.Debug("RefreshSuccessful.");
            var purchasedProduct = CurrentPurchasedProduct;
            CurrentPurchasedProduct = null;
            purchasedProduct.Receipt = refreshReceipt;
            HandlePurchasedRequest(purchasedProduct);
        }

        void RefreshReceiptFailed(AppleStoreKitError error)
        {
            this.Logger.Debug("RefreshRequestFailed: " + error.ToString());
            var purchasedProduct = CurrentPurchasedProduct;
            CurrentPurchasedProduct = null;
            HandleFailurePurchase(
                PurchaseError.FailedOnRefreshReceipt(
                    CurrentPurchaseContext == null
                        ? "Unknown"
                        : purchasedProduct.Id,
                    error.Description
                )
            );
        }

        void HandleResumeSucceeded(List<AppleStoreKitTransaction> transactions)
        {
            this.Logger.Debug("ResumeRequestSuccessful");
            var products = new List<PurchasedProduct>();
            foreach (var tran in transactions)
            {
                var product = new PurchasedProduct(
                    tran.ProductIdentifier,
                    tran.TransactionIdentifier,
                    tran.Base64EncodedReceipt,
                    tran.TransactionDate,
                    tran.OriginalTransactionIdentifier
                );
                products.Add (product);
            }
            HandleSuccessResume(products);
        }

        void HandleRestoreSucceeded(List<AppleStoreKitTransaction> transactions)
        {
            this.Logger.Debug("RestoreRequestSuccessful.");
            var products = new List<PurchasedProduct>();
            foreach (var tran in transactions)
            {
                var product = new PurchasedProduct(
                    tran.ProductIdentifier,
                    tran.TransactionIdentifier,
                    tran.Base64EncodedReceipt,
                    tran.TransactionDate,
                    tran.OriginalTransactionIdentifier
                );
                products.Add(product);
            }
            HandleSuccessRestore(products);
        }

        void HandleRestoreFailed(AppleStoreKitError error)
        {
            this.Logger.Debug("RestoreRequestFailed: " + error.ToString());
            HandleFailureRestore(RestoreError.FailedOnPlatformRequest(error.Description));
        }
#endif

        protected override void FinishRestoredProducts(List<PurchasedProduct> restoredProducts)
        {
#if UNITY_IOS
// 利用者側の処理が終了したらfinishする
            foreach (var product in restoredProducts)
            {
                AppleStoreKit.FinishTransaction(product.TransactionId);
            }
#endif
        }
    }
}

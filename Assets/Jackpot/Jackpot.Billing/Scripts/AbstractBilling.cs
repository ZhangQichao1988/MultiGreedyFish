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
    /// プラットフォーム毎の課金処理を抽象化するクラスです
    /// </summary>
    internal abstract class AbstractBilling : IContextExecutor<QueryContext>, IContextExecutor<PurchaseContext>, IContextExecutor<RestoreContext>, IContextExecutor<ResumeContext>, IDisposable
    {
#region Properties

        protected ILogger Logger { get; private set; }

        protected QueryContext CurrentQueryContext { get; private set; }

        protected PurchaseContext CurrentPurchaseContext { get; private set; }

        protected PurchasedProduct CurrentPurchasedProduct { get; set; }

        protected RestoreContext CurrentRestoreContext { get; private set; }

        protected ResumeContext CurrentResumeContext { get; private set; }

        protected bool Disposed { get; set; }

#endregion

#region Fields

        PrePurchaseRequestDelegate defaultPrePurchaseRequest;
        PurchasedRequestDelegate defaultPurchasedRequest;
        SuccessPurchaseDelegate defaultSuccessPurchase;
        FailurePurchaseDelegate defaultFailurePurchase;
        RefreshReceiptDelegate defaultRefreshReceipt;

#endregion

#region Constructor

        protected AbstractBilling(
            ILogger logger,
            PrePurchaseRequestDelegate defaultPrePurchaseRequest,
            PurchasedRequestDelegate defaultPurchasedRequest,
            SuccessPurchaseDelegate defaultSuccessPurchase,
            FailurePurchaseDelegate defaultFailurePurchase,
            RefreshReceiptDelegate defaultRefreshReceipt)
        {
            Disposed = false;
            Logger = logger;
            this.defaultPrePurchaseRequest = defaultPrePurchaseRequest;
            this.defaultPurchasedRequest = defaultPurchasedRequest;
            this.defaultSuccessPurchase = defaultSuccessPurchase;
            this.defaultFailurePurchase = defaultFailurePurchase;
            this.defaultRefreshReceipt = defaultRefreshReceipt;
        }

        ~AbstractBilling()
        {
            if (Disposed)
            {
                return;
            }

            Dispose();
        }

#endregion

#region Public Methods

        public bool CanExecute(QueryContext context)
        {
            return CurrentQueryContext == null;
        }

        public bool CanExecute(PurchaseContext context)
        {
            return CurrentPurchaseContext == null;
        }

        public bool CanExecute(RestoreContext context)
        {
            // resumeとrestoreはAndroidでは同じネイティブ側メソッドを利用するため、排他制御しておく
            return CurrentRestoreContext == null && CurrentResumeContext == null;
        }

        public bool CanExecute(ResumeContext context)
        {
            // resumeとrestoreはAndroidでは同じネイティブ側メソッドを利用するため、排他制御しておく
            return CurrentResumeContext == null && CurrentRestoreContext == null;
        }

        public void Execute(QueryContext context)
        {
            if (context == null)
            {
                return;
            }
            Logger.Debug("Execute Query: {0}", context.Id);
            CurrentQueryContext = context;
            if (context.QueryRequest == null && context.QueryItemsRequest == null)
            {
                HandleFailureQuery(QueryError.InvalidOperation("QueryRequestDelegate and QueryItemsRequestDelegate was not set."));
                return;
            }

            if (CurrentQueryContext.QueryRequest != null)
            {
                Logger.Debug("Start QueryRequest: {0}", context.Id);
                CurrentQueryContext.QueryRequest(
                    QueryProductsCore,
                    (message) => HandleFailureQuery(message)
                );
            }
            else if (CurrentQueryContext.QueryItemsRequest != null)
            {
                Logger.Debug("Start QueryItemRequest: {0}", context.Id);
                CurrentQueryContext.QueryItemsRequest(
                    QueryProductsCore,
                    (message) => HandleFailureQuery(message)
                );
            }
        }

        public void Execute(RestoreContext context)
        {
            if (context == null)
            {
                return;
            }
            Logger.Debug("Execute Restore: {0}", context.Id);
            CurrentRestoreContext = context;
            Restore();
        }

        public void Execute(ResumeContext context)
        {
            if (context == null)
            {
                return;
            }
            Logger.Debug("Execute Resume: {0}", context.Id);
            CurrentResumeContext = context;
            Resume();
        }

        public void Execute(PurchaseContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("Expected context was not null");
            }
            if (context.Product == null)
            {
                throw new ArgumentException("Expected product was not null");
            }
            Logger.Debug("Execute Purchase: {0}, {1}", context.Id, context.Product.Id);
            CurrentPurchaseContext = context;
            var supported = IsBillingSupported();
            if (!supported.Item1)
            {
                HandleFailurePurchase(PurchaseError.BillingNotSupported(
                    CurrentPurchaseContext.Product.Id,
                    supported.Item2
                ));
                return;
            }
            var prePurchaseProduct = new PrePurchaseProduct() {
                Id = CurrentPurchaseContext.Product.Id,
                Type = CurrentPurchaseContext.Product.Type,
            };
            if (CurrentPurchaseContext.PrePurchaseRequest == null)
            {
                if (defaultPrePurchaseRequest == null)
                {
                    HandleFailurePurchase(
                        PurchaseError.InvalidOperation(
                            CurrentPurchaseContext.Product.Id,
                            "PrePurchaseRequestDelegate was not set."
                        )
                    );
                    return;
                }
                Logger.Debug("Start Default PrePurchase: {0}, {1}", context.Id, context.Product.Id);
                defaultPrePurchaseRequest(
                    prePurchaseProduct,
                    PurchaseCore,
                    (purchaseError) => HandleFailurePurchase(purchaseError)
                );
                return;
            }
            Logger.Debug("Start Custom PrePurchase: {0}, {1}", context.Id, context.Product.Id);
            CurrentPurchaseContext.PrePurchaseRequest(
                prePurchaseProduct,
                PurchaseCore,
                (purchaseError) => HandleFailurePurchase(purchaseError)
            );
        }

        public void CancelRefreshReceipt()
        {
            var purchasedProduct = CurrentPurchasedProduct;
            var error = PurchaseError.FailedOnRefreshReceipt(
                purchasedProduct.Id,
                "refresh receipt canceled."
            );
            HandleFailurePurchase(error);

            CurrentPurchasedProduct = null;
        }

#endregion

#region Abstract Methods

        public abstract void Dispose();

        public abstract void Resume(Action<List<PurchasedProduct>, Action<List<PurchasedProduct>>> onGetResumeProducts);

        public abstract void Resume(Action<List<PurchasedProduct>, Action<List<PurchasedProduct>>> onGetResumeProducts, Action<ResumeError> onFailureGetResumeProducts);

        public abstract Tuple<bool, string> IsBillingSupported();

        protected abstract void QueryProductsCore(List<string> productIds);

        protected abstract void QueryProductsCore(List<string> productIds, List<string> subsIds);

        protected abstract void PurchaseCore(PrePurchaseProduct prePurchaseProduct);

        protected abstract void FinishPurchaseCore(PurchasedProduct purchasedProduct);

        protected abstract void ReQueryProductsCore(PurchasedProduct purchasedProduct, Action<Product, PurchasedProduct, PurchaseSuccessTypes> completeReQuery);

        protected abstract void FinishRestoredProducts(List<PurchasedProduct> restoredProducts);

        public abstract void RefreshReceipt();

        public abstract void OnAfterPlatformPurchase();

        public abstract void Restore();

        protected abstract void Resume();

#endregion

#region Query Methods

        protected void HandleSuccessQuery(List<Product> products)
        {
            Logger.Debug("Handle Success Query");
            var context = CurrentQueryContext;
            CurrentQueryContext = null;
            if (context == null || context.SuccessQuery == null)
            {
                Logger.Warn("SuccessQueryDelegate was not set.");
                return;
            }
            context.SuccessQuery(products);
        }

        protected void HandleFailureQuery(QueryError error)
        {
            Logger.Debug("Handle Failure Query");
            var context = CurrentQueryContext;
            CurrentQueryContext = null;
            if (context == null || context.FailureQuery == null)
            {
                Logger.Warn("FailureQueryDelegate was not set. Error:\n{0}", error.ToString());
                return;
            }
            context.FailureQuery(error);
        }

#endregion

#region Purchase Methods

        protected void HandlePurchasedRequest(PurchasedProduct purchasedProduct)
        {
            Logger.Debug("Handle PurchasedRequest Query: {0}", purchasedProduct.Id);
            if (CurrentPurchaseContext != null
                && CurrentPurchaseContext.Product.Id == purchasedProduct.Id
                && CurrentPurchaseContext.PurchasedRequest != null)
            {
                Logger.Debug("Current Context: {0}", purchasedProduct.Id);
                CurrentPurchaseContext.PurchasedRequest(
                    purchasedProduct,
                    FinishPurchaseCore,
                    (purchaseError) =>
                    {
                        if (purchaseError.IsFailedOnPurchasedRequest
                            && purchaseError.PurchasedResponseKind == PurchasedResponseKind.ReceiptOutOfDate
                            && purchasedProduct.RetryRefreshReceiptCount < Config.MaxRetryRefreshReceiptCount)
                        {
                            CurrentPurchasedProduct = purchasedProduct;
                            CurrentPurchaseContext.RefreshReceipt(
                                purchasedProduct,
                                RefreshReceipt,
                                CancelRefreshReceipt
                            );
                            purchasedProduct.RetryRefreshReceiptCount++;
                        }
                        else
                        {
                            if (purchaseError.PurchasedResponseKind == PurchasedResponseKind.ReceiptOutOfDate)
                            {
                                purchaseError = PurchaseError.FailedOnRefreshReceipt(
                                    purchasedProduct.Id,
                                    "refresh receipt failed."
                                );
                            }
                            HandleFailurePurchase(purchaseError);
                        }
                    }
                );
                return;
            }
            if (defaultPurchasedRequest != null)
            {
                Logger.Debug("Default Context: {0}", purchasedProduct.Id);
                defaultPurchasedRequest(
                    purchasedProduct,
                    FinishPurchaseCore,
                    (purchaseError) =>
                    {
                        if (purchaseError.IsFailedOnPurchasedRequest
                            && purchaseError.PurchasedResponseKind == PurchasedResponseKind.ReceiptOutOfDate
                            && purchasedProduct.RetryRefreshReceiptCount < Config.MaxRetryRefreshReceiptCount)
                        {
                            CurrentPurchasedProduct = purchasedProduct;
                            defaultRefreshReceipt(
                                purchasedProduct,
                                RefreshReceipt,
                                CancelRefreshReceipt
                            );
                            purchasedProduct.RetryRefreshReceiptCount++;
                        }
                        else
                        {
                            if (purchaseError.PurchasedResponseKind == PurchasedResponseKind.ReceiptOutOfDate)
                            {
                                purchaseError = PurchaseError.FailedOnRefreshReceipt(
                                    purchasedProduct.Id,
                                    "refresh receipt failed."
                                );
                            }
                            HandleFailurePurchase(purchaseError);
                        }
                    }
                );
                return;
            }
            FinishPurchaseCore(purchasedProduct);
        }

        protected void HandleSuccessPurchase(PurchasedProduct purchasedProduct, PurchaseSuccessTypes successType)
        {
            if (CurrentPurchaseContext != null)
            {
                HandleSuccessPurchase(CurrentPurchaseContext.Product, purchasedProduct, successType);
                return;
            }
            Logger.Debug("Start ReQuery: {0}", purchasedProduct.Id);
            ReQueryProductsCore(purchasedProduct, HandleSuccessPurchase);
        }

        protected void HandleSuccessPurchase(Product product, PurchasedProduct purchasedProduct, PurchaseSuccessTypes successType)
        {
            if (purchasedProduct != null)
            {
                Logger.Debug("Handle Success Purchase: {0}", purchasedProduct.Id);
            }

            var context = CurrentPurchaseContext;
            CurrentPurchaseContext = null;
            if (context != null && context.SuccessPurchase != null)
            {
                if (purchasedProduct != null && context.Product.Id == purchasedProduct.Id)
                {
                    context.SuccessPurchase(product, purchasedProduct, successType);
                    return;
                }
                if (purchasedProduct == null && successType == PurchaseSuccessTypes.NoPurchases)
                {
                    context.SuccessPurchase(product, null, successType);
                    return;
                }
            }

            if (defaultSuccessPurchase != null)
            {
                defaultSuccessPurchase(product, purchasedProduct, successType);
                return;
            }
            HandleFailurePurchase(PurchaseError.InvalidOperation(purchasedProduct.Id, "SuccessPurchaseDelegate was not set."));
        }

        protected void HandleFailurePurchase(PurchaseError error)
        {
            var context = CurrentPurchaseContext;
            CurrentPurchaseContext = null;
            if (context == null || context.FailurePurchase == null)
            {
                if (defaultFailurePurchase == null)
                {
                    return;
                }
                defaultFailurePurchase(error);
                return;
            }
            context.FailurePurchase(error);
        }

#endregion

#region Restore Methods

        protected void HandleSuccessRestore(List<PurchasedProduct> products)
        {
            Logger.Debug("Handle Success Restore");
            var context = CurrentRestoreContext;
            CurrentRestoreContext = null;
            if (context == null || context.SuccessRestore == null)
            {
                this.Logger.Warn("SuccessRestoreDelegate was not set.");
                return;
            }
            context.SuccessRestore(products, restoredProducts =>
            {
                FinishRestoredProducts(restoredProducts);
            });
        }

        protected void HandleFailureRestore(RestoreError error)
        {
            Logger.Debug("Handle Failure Restore");
            var context = CurrentRestoreContext;
            CurrentRestoreContext = null;
            if (context == null || context.FailureRestore == null)
            {
                Logger.Warn("FailureRestoreDelegate was not set. Error:\n{0}", error.ToString());
                return;
            }
            context.FailureRestore(error);
        }

#endregion

#region Resume Methods

        protected void HandleSuccessResume(List<PurchasedProduct> products)
        {
            Logger.Debug("Handle Success Resume");
            var context = CurrentResumeContext;
            CurrentResumeContext = null;
            if (context == null || context.SuccessResume == null)
            {
                Logger.Warn("SuccessResumeDelegate was not set.");
                return;
            }
            context.SuccessResume(products);
        }

        protected void HandleFailureResume(ResumeError error)
        {
            Logger.Debug("Handle Failure Restore");
            var context = CurrentResumeContext;
            CurrentResumeContext = null;
            if (context == null || context.FailureResume == null)
            {
                Logger.Warn("FailureResumeDelegate was not set. Error:\n{0}", error.ToString());
                return;
            }
            context.FailureResume(error);
        }

#endregion
    }
}

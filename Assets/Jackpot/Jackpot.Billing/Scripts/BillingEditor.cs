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
using System.Threading;

namespace Jackpot.Billing
{
    /// <summary>
    /// UnityEditorでの課金処理を実装するクラスです
    /// </summary>
    internal class BillingEditor : AbstractBilling
    {
        public static QueryError StubQueryError = null;
        public static PurchaseError StubPurchaseError = null;

        public BillingEditor(
            PrePurchaseRequestDelegate defaultPrePurchaseRequest,
            PurchasedRequestDelegate defaultPurchasedRequest,
            SuccessPurchaseDelegate defaultSuccessPurchase,
            FailurePurchaseDelegate defaultFailurePurchase,
            RefreshReceiptDelegate defaultRefreshReceipt) : base(
            Jackpot.Logger.Get<BillingEditor>(),
            defaultPrePurchaseRequest,
            defaultPurchasedRequest,
            defaultSuccessPurchase,
            defaultFailurePurchase,
            defaultRefreshReceipt)
        {
        }

        public override void Dispose()
        {
            if (Disposed)
            {
                return;
            }
            Disposed = true;
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
            Logger.Debug("Resume success.");

            onGetResumeProducts(
                new List<PurchasedProduct>(),
                ResumeExecute);
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

        protected override void QueryProductsCore(List<string> productIds)
        {
            QueryProductsCore(productIds, null);
        }

        protected override void QueryProductsCore(List<string> productIds, List<string> subsIds)
        {
            if (StubQueryError != null)
            {
                HandleFailureQuery(StubQueryError);
                return;
            }
            var stubProducts = new List<Product>();
            if (productIds != null)
            {
                productIds.ForEach(productId =>
                {
                    stubProducts.Add(new Product(
                        productId,
                        "inapp",
                        "Title:" + productId,
                        "DescriptionDescriptionDescriptionDescriptionDescription",
                        "10500",
                        "¥10,500",
                        Currency.Resolve("JPY")
                    ));
                });
            }
            if (subsIds != null)
            {
                subsIds.ForEach(productId =>
                {
                    stubProducts.Add(new Product(
                        productId,
                        "inapp",
                        "Title:" + productId,
                        "DescriptionDescriptionDescriptionDescriptionDescription",
                        "10500",
                        "¥10,500",
                        Currency.Resolve("JPY")
                    ));
                });
            }
            if (stubProducts.Count <= 0)
            {
                HandleSuccessQuery(new List<Product>());
                return;
            }

            MainThreadDispatcher.Post(() =>
            {
                HandleSuccessQuery(stubProducts);
            });
        }

        public override Tuple<bool, string> IsBillingSupported()
        {
            if (StubPurchaseError != null && StubPurchaseError.IsBillingNotSupported)
            {
                return Tuple.Create<bool, string>(false, StubPurchaseError.Message);
            }
            return Tuple.Create<bool, string>(true, string.Empty);
        }

        protected override void PurchaseCore(PrePurchaseProduct prePurchaseProduct)
        {
            if (StubPurchaseError != null && StubPurchaseError.IsFailedOnPlatformPurchase)
            {
                HandleFailurePurchase(StubPurchaseError);
                return;
            }
            HandlePurchasedRequest(new PurchasedProduct(prePurchaseProduct.Id));
        }

        protected override void FinishPurchaseCore(PurchasedProduct purchasedProduct)
        {
            if (StubPurchaseError != null && StubPurchaseError.IsFailedOnPlatformFinishPurchase)
            {
                HandleFailurePurchase(StubPurchaseError);
                return;
            }
            HandleSuccessPurchase(purchasedProduct, PurchaseSuccessTypes.Normal);
        }

        protected override void ReQueryProductsCore(PurchasedProduct purchasedProduct, Action<Product, PurchasedProduct, PurchaseSuccessTypes> completeReQuery)
        {
            if (StubPurchaseError != null && StubPurchaseError.IsFailedOnReQuery)
            {
                HandleFailurePurchase(StubPurchaseError);
                return;
            }
            completeReQuery(
                new Product(
                    purchasedProduct.Id,
                    "inapp",
                    "TitleTitleTitleTitleTitle",
                    "DescriptionDescriptionDescriptionDescriptionDescription",
                    "10500",
                    "¥10,500",
                    Currency.Resolve("JPY")
                ),
                purchasedProduct,
                PurchaseSuccessTypes.Normal
            );
        }

        public override void RefreshReceipt()
        {
            CurrentPurchasedProduct = null;
        }

        public override void OnAfterPlatformPurchase()
        {
            // 特になにもしない
        }

        public override void Restore()
        {
            var products = new List<PurchasedProduct>();
            for (int i = 0; i < 3; i++)
            {
                products.Add(new PurchasedProduct(i.ToString()));
            }

            MainThreadDispatcher.Post(() =>
            {
                HandleSuccessRestore(products);
            });
        }

        protected override void FinishRestoredProducts(List<PurchasedProduct> restoredProducts)
        {
            // 特に何もしない
        }

        protected override void Resume()
        {
            // 特に何もしない
        }
    }
}

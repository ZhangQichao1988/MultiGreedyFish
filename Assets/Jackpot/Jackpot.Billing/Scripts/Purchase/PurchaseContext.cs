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
    public class PurchaseContext : Context
    {
        public Product Product { get { return product; } }

        public PrePurchaseRequestDelegate PrePurchaseRequest { get { return prePurchaseRequest; } }

        public PurchasedRequestDelegate PurchasedRequest { get { return purchasedRequest; } }

        public SuccessPurchaseDelegate SuccessPurchase { get { return successPurchase; } }

        public FailurePurchaseDelegate FailurePurchase { get { return failurePurchase; } }

        public RefreshReceiptDelegate RefreshReceipt { get { return refreshReceipt; } }

        readonly Product product;
        readonly PrePurchaseRequestDelegate prePurchaseRequest;
        readonly PurchasedRequestDelegate purchasedRequest;
        readonly SuccessPurchaseDelegate successPurchase;
        readonly FailurePurchaseDelegate failurePurchase;
        readonly RefreshReceiptDelegate refreshReceipt;

        public PurchaseContext(Product product) : this(product, null, null, null, null, null)
        {
        }

        public PurchaseContext(Product product,
            PrePurchaseRequestDelegate prePurchaseRequestDelegate,
            PurchasedRequestDelegate purchasedRequestDelegate,
            SuccessPurchaseDelegate successPurchaseDelegate,
            FailurePurchaseDelegate failurePurchaseDelegate,
            RefreshReceiptDelegate refreshReceiptDelegate) : base()
        {
            this.product = product;
            this.prePurchaseRequest = prePurchaseRequestDelegate;
            this.purchasedRequest = purchasedRequestDelegate;
            this.successPurchase = successPurchaseDelegate;
            this.failurePurchase = failurePurchaseDelegate;
            this.refreshReceipt = refreshReceiptDelegate;
        }

        public PurchaseContext(
            int id,
            Product product,
            PrePurchaseRequestDelegate prePurchaseRequestDelegate,
            PurchasedRequestDelegate purchasedRequestDelegate,
            SuccessPurchaseDelegate successPurchaseDelegate,
            FailurePurchaseDelegate failurePurchaseDelegate,
            RefreshReceiptDelegate refreshReceiptDelegate) : base(id)
        {
            this.product = product;
            this.prePurchaseRequest = prePurchaseRequestDelegate;
            this.purchasedRequest = purchasedRequestDelegate;
            this.successPurchase = successPurchaseDelegate;
            this.failurePurchase = failurePurchaseDelegate;
            this.refreshReceipt = refreshReceiptDelegate;
        }

        public PurchaseContext OnPrePurchaseRequest(PrePurchaseRequestDelegate prePurchaseRequestDelegate)
        {
            return new PurchaseContext(
                Id,
                Product,
                prePurchaseRequestDelegate,
                PurchasedRequest,
                SuccessPurchase,
                FailurePurchase,
                RefreshReceipt
            );
        }

        public PurchaseContext OnPurchasedRequest(PurchasedRequestDelegate purchasedRequestDelegate)
        {
            return new PurchaseContext(
                Id,
                Product,
                PrePurchaseRequest,
                purchasedRequestDelegate,
                SuccessPurchase,
                FailurePurchase,
                RefreshReceipt
            );
        }

        public PurchaseContext OnSuccessPurchase(SuccessPurchaseDelegate successPurchaseDelegate)
        {
            return new PurchaseContext(
                Id,
                Product,
                PrePurchaseRequest,
                PurchasedRequest,
                successPurchaseDelegate,
                FailurePurchase,
                RefreshReceipt
            );
        }

        public PurchaseContext OnFailurePurchase(FailurePurchaseDelegate failurePurchaseDelegate)
        {
            return new PurchaseContext(
                Id,
                Product,
                PrePurchaseRequest,
                PurchasedRequest,
                SuccessPurchase,
                failurePurchaseDelegate,
                RefreshReceipt
            );
        }

        public PurchaseContext OnRefreshReceipt(RefreshReceiptDelegate refreshReceiptDelegate)
        {
            return new PurchaseContext(
                Id,
                Product,
                PrePurchaseRequest,
                PurchasedRequest,
                SuccessPurchase,
                FailurePurchase,
                refreshReceiptDelegate
            );
        }

        public override void Execute()
        {
            BillingService.Instance.Purchase(this);
        }
    }
}

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
    /// SKPaymentTransactionStateに対応するenum値です。
    /// </summary>
    public enum AppleStoreKitTransactionState
    {
        Purchasing, // Transaction is being added to the server queue.
        Purchased, // Transaction is in queue, user has been charged.  Client should complete the transaction.
        Failed, // Transaction was cancelled or failed before being added to the server queue.
        Restored, // Transaction was restored from user's purchase history.  Client should complete the transaction.
        Deferred, // The transaction is in the queue, but its final status is pending external action.
    }
}

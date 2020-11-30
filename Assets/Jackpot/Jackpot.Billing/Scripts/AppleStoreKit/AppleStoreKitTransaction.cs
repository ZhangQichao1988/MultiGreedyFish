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
    /// SKPaymentTransactionクラスのデータを格納するクラスです。
    /// </summary>
    /// <remarks>
    /// see. https://developer.apple.com/library/ios/documentation/StoreKit/Reference/SKPaymentTransaction_Class/index.html
    /// </remarks>
    public class AppleStoreKitTransaction
    {
        /// <summary>
        /// SKProduct.productIdentifier
        /// </summary>
        /// <remarks>
        /// @"productIdentifier":transaction.payment.productIdentifier
        /// </remarks>
        /// <value>The product identifier.</value>
        public string ProductIdentifier { get; private set; }

        /// <summary>
        /// SKPayment.quantity
        /// </summary>
        /// <remarks>
        /// @"quantity":[NSNumber numberWithInt:transaction.payment.quantity
        /// </remarks>
        /// <value>The quantity.</value>
        public string Quantity { get; private set; }

        /// <summary>
        /// SKPayment.applicationUsername
        /// </summary>
        /// <remarks>
        /// @"applicationUsername":transaction.payment.applicationUsername
        /// </remarks>
        /// <value>The application username.</value>
        public string ApplicationUsername { get; private set; }

        /// <summary>
        /// SKPaymentTransactionState
        /// </summary>
        /// <remarks>
        /// @"transactionState":[NSNumber numberWithInt:transaction.transactionState]
        /// </remarks>
        /// <value>The state of the transaction.</value>
        public AppleStoreKitTransactionState TransactionState { get; private set; }

        /// <summary>
        /// SKPaymentTransaction.originalTransaction.transactionIdentifier
        /// </summary>
        /// <value>The original transaction identifier.</value>
        public string OriginalTransactionIdentifier { get; private set; }

        /// <summary>
        /// SKPaymentTransaction.transactionIdentifier
        /// </summary>
        /// <remarks>
        /// @"transactionIdentifier":transaction.transactionIdentifier
        /// </remarks>
        /// <value>The transaction identifier.</value>
        public string TransactionIdentifier { get; private set; }

        /// <summary>
        /// SKPaymentTransaction.transactionDate
        /// </summary>
        /// <remarks>
        /// @"transactionDate":[[NSNumber alloc] initWithDouble:[transaction.transactionDate timeIntervalSince1970]]
        /// </remarks>
        /// <value>The transaction date.</value>
        public long TransactionDate { get; private set; }

        /// <summary>
        /// SKPaymentTransaction.transactionReceipt
        /// </summary>
        /// <remarks>
        /// @"base64EncodedReceipt":[transaction.transactionReceipt base64EncodedStringWithOptions:0]
        /// </remarks>
        /// <value>The base64 encoded receipt.</value>
        public string Base64EncodedReceipt { get; private set; }

        /// <summary>
        /// SKPaymentTransaction.error
        /// </summary>
        /// <value>The error.</value>
        public AppleStoreKitError Error { get; private set; }

        public AppleStoreKitTransaction(Dictionary<string, object> dict)
        {
            if (dict.ContainsKey("productIdentifier"))
            {
                ProductIdentifier = dict["productIdentifier"] as string;
            }

            if (dict.ContainsKey("quantity"))
            {
                Quantity = dict["quantity"] as string;
            }

            if (dict.ContainsKey("applicationUsername"))
            {
                ApplicationUsername = dict["applicationUsername"] as string;
            }

            if (dict.ContainsKey("transactionState"))
            {
                var state = System.Convert.ToInt32(dict["transactionState"]);
                TransactionState = (AppleStoreKitTransactionState) Enum.ToObject(
                    typeof(AppleStoreKitTransactionState),
                    state
                );
            }

            if (dict.ContainsKey("originalTransactionIdentifier"))
            {
                OriginalTransactionIdentifier = dict["originalTransactionIdentifier"] as string;
            }

            if (dict.ContainsKey("transactionIdentifier"))
            {
                TransactionIdentifier = dict["transactionIdentifier"] as string;
            }

            if (dict.ContainsKey("transactionDate"))
            {
                TransactionDate = System.Convert.ToInt64(dict["transactionDate"]);
            }

            if (dict.ContainsKey("base64EncodedReceipt"))
            {
                Base64EncodedReceipt = dict["base64EncodedReceipt"] as string;
            }

            if (dict.ContainsKey("errorCode") && dict.ContainsKey("errorDomain") && dict.ContainsKey("errorDescription"))
            {
                Error = new AppleStoreKitError(
                    System.Convert.ToInt64(dict["errorCode"]),
                    dict["errorDomain"] as string,
                    dict["errorDescription"] as string
                );
            }
        }

        public override string ToString()
        {
            return string.Format(
                "ProductIdentifier:{0}\nQuantity:{1}\nApplicationUsername:{2}\nTransactionState:{3}\nOriginalTransactionIdentifier:{4}\nTransactionIdentifier:{5}\nTransactionDate:{6}\nBase64EncodedReceipt:{7}",
                ProductIdentifier
                , Quantity
                , ApplicationUsername
                , TransactionState
                , OriginalTransactionIdentifier
                , TransactionIdentifier
                , TransactionDate
                , Base64EncodedReceipt
            );
        }
    }
}

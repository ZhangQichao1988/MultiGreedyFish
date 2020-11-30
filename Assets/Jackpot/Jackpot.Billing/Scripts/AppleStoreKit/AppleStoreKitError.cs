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
using System.Collections.Generic;

namespace Jackpot.Billing
{
    /// <summary>
    /// StoreKitプラグインで発生したエラーに関するデータを格納するクラスです。
    /// </summary>
    public class AppleStoreKitError
    {
        /// <summary>
        /// NSErrorクラスのプロパティcodeの値
        /// </summary>
        /// <value>The code.</value>
        public long Code { get; private set; }

        /// <summary>
        /// NSErrorクラスのプロパティdomainの値
        /// </summary>
        /// <value>The domain.</value>
        public string Domain { get; private set; }

        /// <summary>
        /// NSErrorクラスのプロパティdescriptionの値
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; private set; }

        /// <summary>
        /// SKError.hで定義されているエラー：0 | SKErrorUnknown
        /// </summary>
        /// <value><c>true</c> if this instance is client invalid; otherwise, <c>false</c>.</value>
        public bool IsUnknown { get; private set; }

        /// <summary>
        /// SKError.hで定義されているエラー：1 | SKErrorClientInvalid
        /// </summary>
        /// <remarks>
        /// client is not allowed to issue the request, etc.
        /// </remarks>
        /// <value><c>true</c> if this instance is payment cancelled; otherwise, <c>false</c>.</value>
        public bool IsClientInvalid { get; private set; }

        //
        /// <summary>
        /// SKError.hで定義されているエラー：2 | SKErrorPaymentCancelled
        /// </summary>
        /// <remarks>
        /// user cancelled the request, etc.
        /// </remarks>
        /// <value><c>true</c> if this instance is payment invalid; otherwise, <c>false</c>.</value>
        public bool IsPaymentCancelled { get; private set; }

        /// <summary>
        /// SKError.hで定義されているエラー：3 | SKErrorPaymentInvalid
        /// </summary>
        /// <remarks>
        /// purchase identifier was invalid, etc.
        /// </remarks>
        /// <value><c>true</c> if this instance is payment invalid; otherwise, <c>false</c>.</value>
        public bool IsPaymentInvalid { get; private set; }

        /// <summary>
        /// SKError.hで定義されているエラー：4 | SKErrorPaymentNotAllowed
        /// </summary>
        /// <remarks>
        /// this device is not allowed to make the payment
        /// </remarks>
        /// <value><c>true</c> if this instance is payment not allowed; otherwise, <c>false</c>.</value>
        public bool IsPaymentNotAllowed { get; private set; }

        // Product is not available in the current storefront
        /// <summary>
        /// SKError.hで定義されているエラー：5 | SKErrorStoreProductNotAvailable
        /// </summary>
        /// <value><c>true</c> if this instance is not available; otherwise, <c>false</c>.</value>
        public bool IsNotAvailable { get; private set; }

        /// <summary>
        /// KJBillingService.hで定義されているエラー：0 | KJPBillingErrorUnknown
        /// </summary>
        /// <value><c>true</c> if this instance is KJP billing error unknown; otherwise, <c>false</c>.</value>
        public bool IsKJPBillingErrorUnknown { get; private set; }

        /// <summary>
        /// KJBillingService.hで定義されているエラー：1 | KJPBillingErrorPurchaseFailed
        /// </summary>
        /// <value><c>true</c> if KJP billing error purchase failed; otherwise, <c>false</c>.</value>
        public bool KJPBillingErrorPurchaseFailed { get; private set; }

        public AppleStoreKitError(Dictionary<string, object> dict)
        {
            if (dict.ContainsKey("code"))
            {
                Code = System.Convert.ToInt64(dict["code"]);
            }
            if (dict.ContainsKey("domain"))
            {
                Domain = dict["domain"] as string;
            }
            if (dict.ContainsKey("description"))
            {
                Description = dict["description"] as string;
            }

            CaseAction
                .When("KJPBillingErrorDomain", SetKJPBillingErrorType)
                .Default(SetErrorType)
                .Apply(Domain);
        }

        public AppleStoreKitError(long code, string domain, string description)
        {
            Code = code;
            Domain = domain;
            Description = description;
            CaseAction
                .When("KJPBillingErrorDomain", SetKJPBillingErrorType)
                .Default(SetErrorType)
                .Apply(Domain);
        }

        void SetErrorType()
        {
            switch (Code)
            {
                case 1:
                    IsClientInvalid = true;
                    break;
                case 2:
                    IsPaymentCancelled = true;
                    break;
                case 3:
                    IsPaymentInvalid = true;
                    break;
                case 4:
                    IsPaymentNotAllowed = true;
                    break;
                case 5:
                    IsNotAvailable = true;
                    break;
                default:
                    IsUnknown = true;
                    break;
            }
        }

        void SetKJPBillingErrorType()
        {
            switch (Code)
            {
                case 1:
                    KJPBillingErrorPurchaseFailed = true;
                    break;
                default:
                    IsKJPBillingErrorUnknown = true;
                    break;
            }
        }

        public override string ToString()
        {
            if (IsClientInvalid)
            {
                return string.Format("IsClientInvalid({0}),domain:{1},description:{2}", Code, Domain, Description);
            }
            if (IsPaymentCancelled)
            {
                return string.Format("IsPaymentCancelled({0}),domain:{1},description:{2}", Code, Domain, Description);
            }
            if (IsPaymentInvalid)
            {
                return string.Format("IsPaymentInvalid({0}),domain:{1},description:{2}", Code, Domain, Description);
            }
            if (IsPaymentNotAllowed)
            {
                return string.Format("IsPaymentNotAllowed({0}),domain:{1},description:{2}", Code, Domain, Description);
            }
            if (IsNotAvailable)
            {
                return string.Format("IsNotAvailable({0}),domain:{1},description:{2}", Code, Domain, Description);
            }
            if (IsUnknown)
            {
                return string.Format("IsUnknown({0}),domain:{1},description:{2}", Code, Domain, Description);
            }
            if (IsKJPBillingErrorUnknown)
            {
                return string.Format("IsKJPBillingErrorUnknown({0}),domain:{1},description:{2}", Code, Domain, Description);
            }
            if (KJPBillingErrorPurchaseFailed)
            {
                return string.Format("KJPBillingErrorPurchaseFailed({0}),domain:{1},description:{2}", Code, Domain, Description);
            }
            return string.Format("code:{0},domain:{1},description:{2}", Code, Domain, Description);
        }
    }
}

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

namespace Jackpot.Billing
{
    /// <summary>
    /// 購入中の商品情報の取り扱いに必要なDTOです
    /// </summary>
    public class PrePurchaseProduct
    {
#region Fields

        GoogleIabProrationMode prorationMode = GoogleIabProrationMode.ImmediateWithTimeProration;

#endregion

#region properties

        /// <summary>
        /// 商品IDを示します。Jackpot.Billing.Product.Idに等しい値が入ります
        /// </summary>
        /// <value>The Identifier of Product to purchase</value>
        public string Id { get; set; }

        public string Type { get; set; }

        /// <summary>
        /// 定期購入のグレード変更時に指定できるグレード変更前のSku(Androidのみ)
        /// </summary>
        /// <value>The before grade change product sku(Android Only).</value>
        public string SkuToReplace { get; set; }

        /// <summary>
        /// 定期購入のグレード変更時に指定できる比例分配モード(Androidのみ)
        /// </summary>
        /// <value>The proration mode at grade change(Android Only).</value>
        public GoogleIabProrationMode ProrationMode
        {
            get
            {
                return prorationMode;
            }
            set
            {
                prorationMode = value;
            }
        }

        /// <summary>
        /// 購入時に指定できるaccountId(Androidのみ)
        /// accountIdの最大長は64文字
        /// メールアドレスなのど個人情報を含んでいる場合、購入がブロックされる可能性があるので注意。
        /// </summary>
        /// <value>The Identifier of Account.</value>
        public string AccountId { get; set; }

        /// <summary>
        /// 購入時に指定できるprofileId(Androidのみ)
        /// profileIdの最大長は64文字
        /// メールアドレスなのど個人情報を含んでいる場合、購入がブロックされる可能性があるので注意。
        /// </summary>
        /// <value>The Identifier of Profile.</value>
        public string ProfileId { get; set; }
#endregion
    }
}

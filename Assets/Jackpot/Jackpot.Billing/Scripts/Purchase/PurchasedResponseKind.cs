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
    public enum PurchasedResponseKind
    {
        /// <summary>
        /// 不明なエラー
        /// </summary>
        Unknown = -1,
        /// <summary>
        /// アイテム付与成功
        /// </summary>
        ProcessedSuccessfully = 0,
        /// <summary>
        /// アイテム付与済み
        /// </summary>
        AlreadyProcessed = 1,
        /// <summary>
        /// 不正な値
        /// </summary>
        InvalidParam = 10,
        /// <summary>
        /// 不正なレシート
        /// </summary>
        InvalidReceipt = 11,
        /// <summary>
        /// Appleサーバー接続不可
        /// </summary>
        FailedToConnectToAppStore = 12,
        /// <summary>
        /// 不正なプロダクトID
        /// </summary>
        InvalidProductId = 13,
        /// <summary>
        /// 古いレシート
        /// </summary>
        ReceiptOutOfDate = 14,
        /// <summary>
        /// レシート検証中のバリデーションエラー
        /// </summary>
        UnexpectedValidastionError = 19,
        /// <summary>
        /// コンテンツデリバリー中のエラー
        /// </summary>
        ContentDeliverFailed = 30,
    }
}

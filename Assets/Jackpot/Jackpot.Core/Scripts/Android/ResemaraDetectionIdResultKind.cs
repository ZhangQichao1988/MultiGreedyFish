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
namespace Jackpot
{
    /// <summary>
    /// Androidの端末固有IDの取得結果を示すenumです。
    /// </summary>
    public enum ResemaraDetectionIdResultKind
    {
        /// <summary>
        /// ID取得に成功した
        /// </summary>
        /// <remarks>
        /// プラットフォームプラグインとの連携で(int)ResemaraDetectionIdResultKindを使うのでint valueを明示的に設定しています。
        /// </remarks>
        Succeeded = 0,
        /// <summary>
        /// IDの取得に失敗した
        /// </summary>
        Failed = 1,
        /// <summary>
        /// GooglePlayが端末にインストールされていない為IDの取得に失敗した(広告ID取得時のみ)
        /// </summary>
        GooglePlayServicesNotAvailable = 2,
        /// <summary>
        /// GooglePlayServices接続失敗の為取得に失敗した(広告ID取得時のみ)
        /// </summary>
        GooglePlayServicesDisconnect = 3,
        /// <summary>
        /// GooglePlayServices上で何かしらエラーが発生した為IDの取得に失敗した(広告ID取得時のみ)
        /// </summary>
        GooglePlayServicesRepairable = 4,
        /// <summary>
        /// 上記以外の理由でIDの取得に失敗した(広告ID取得時のみ)
        /// </summary>
        UnknownError = 5,
    }
}

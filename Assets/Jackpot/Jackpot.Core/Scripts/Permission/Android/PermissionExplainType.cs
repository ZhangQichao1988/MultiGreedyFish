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
    /// 権限が必要な理由を説明するダイアログの種類を定義します
    /// </summary>
    public enum PermissionExplainType
    {
        /// <summary>
        /// 初回に権限が拒否された際に権限が必要な理由を説明します
        /// </summary>
        Normal = 1,

        /// <summary>
        /// 権限が必要な理由を説明しても拒否された際に
        /// 権限がないとアプリが実行できない旨を説明します
        /// </summary>
        AppFinish = 2,

        /// <summary>
        /// 権限取得ダイアログの「今後表示しない」にチェックされた際に
        /// 権限を許可してもらう為に設定画面へ誘導する旨を説明します
        /// </summary>
        OpenSettings = 3,
    }
}

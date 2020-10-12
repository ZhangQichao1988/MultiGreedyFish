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

namespace Jackpot
{
    /// <summary>
    /// PlayerPrefsの変更の種類を示す列挙型です。
    /// </summary>
    public enum PlayerPrefsChangedEventKind
    {
        /// <summary>
        /// あるキーで登録された、または登録されていなかった設定項目が更新されたイベントであることを示します。
        /// 新規の項目の登録、既存の項目の更新を含めます
        /// </summary>
        Update,

        /// <summary>
        /// あるキーで登録された設定項目が削除されたイベントであることを示します。
        /// </summary>
        Delete,

        /// <summary>
        /// 登録されている全ての設定項目が削除された際のイベントであることを示します。
        /// </summary>
        DeletedAll
    }
}

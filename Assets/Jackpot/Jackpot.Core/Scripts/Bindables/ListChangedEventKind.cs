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
    /// ListChangedEventの詳細を示します
    /// </summary>
    /// <remarks>
    /// 各イベント毎に渡されるパラメータの詳細は、<see cref="Jackpot.ListChangedEventArgs{T}"/>を参考にしてください。
    /// </remarks>
    public enum ListChangedEventKind
    {
        /// <summary>
        /// ForceUpdateが実施された事を示します
        /// </summary>
        ForceUpdate,
        /// <summary>
        /// Swap, Update, UpdateAll, index accessorによってデータの変更があった事を示します
        /// </summary>
        Update,
        /// <summary>
        /// Insert, Add等でデータが追加された事を示します
        /// </summary>
        Add,
        /// <summary>
        /// Remove, Clear等でデータが削除された事を示します
        /// </summary>
        Remove,
        /// <summary>
        /// Editによって変更された事を示します
        /// </summary>
        Edit,
    }
}


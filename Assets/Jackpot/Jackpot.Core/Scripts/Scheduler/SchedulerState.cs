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
    /// Schedulerの状態を表す列挙型です
    /// </summary>
    public enum SchedulerState
    {
        /// <summary>
        /// 待機状態を示します
        /// </summary>
        Waiting,

        /// <summary>
        /// タスクを実行している状態を示します
        /// </summary>
        Working,

        /// <summary>
        /// 完了させた状態を示します
        /// </summary>
        Done,
    }
}

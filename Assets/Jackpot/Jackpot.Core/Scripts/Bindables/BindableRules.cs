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
    /// BindableObjectのデータ変更時の制約を示します
    /// </summary>
    public enum BindableRules : int
    {
        /// <summary>
        /// 値の変更通知のコールスタック中にBindlableObjectの値を変更する事を許可しません
        /// </summary>
        NotAllowCallStackChanging,
        /// <summary>
        /// 値の変更通知のコールスタック中にBindlableObjectの値を変更する事を許可します
        /// </summary>
        /// <remarks>
        /// 変更が全て通知されます
        /// スタックオーバーフローに注意して使用してください。。。
        /// </remarks>
        AllowCallStackChanging,
        /// <summary>
        /// 値の変更通知のコールスタック中にBindlableObjectの値を変更する事を許可しますが、通知されません
        /// </summary>
        ThroughCallStackChanging,
    }
}

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
    /// <see cref="Jackpot.ICacheAdapter.GetAsync"/>利用時の、非同期なfallback処理を行うdelegateです
    /// </summary>
    /// <param name="onSuccess">fallback成功時には、onSuccess(result)を実行してください</param>
    /// <param name="onFailure">fallback失敗時には、onFailure(error)を実行してください</param>
    public delegate void AsyncFallbackDelegate<TValue>(Action<TValue> onSuccess, Action<object> onFailure);
}

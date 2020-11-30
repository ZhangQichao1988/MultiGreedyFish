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
    /// 権限が拒否された時の処理をカスタマイズする際に指定するdelegateです。
    /// </summary>
    /// <param name="deniedAfterAction">ユーザーによって権限が拒否された際に再度要求理由ダイアログを表示する為のActionです。</param>
    /// <param name="finishAction">権限確認処理を正常に終了させる為のActionです。権限確認終了時に必ず実行して下さい。</param>
    public delegate void DeniedPermissionDelegate(Action deniedAfterAction, Action finishAction);
}


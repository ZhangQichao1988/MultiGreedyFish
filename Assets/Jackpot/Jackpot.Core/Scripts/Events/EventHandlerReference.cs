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
    /// EventHandlerを保持する抽象クラスです
    /// </summary>
    public abstract class EventHandlerReference<TEventArgs> where TEventArgs : EventArgs
    {
        /// <summary>
        /// 一度だけイベントハンドリングを実施するかを示します
        /// </summary>
        /// <value><c>true</c> if this instance is handle once; otherwise, <c>false</c>.</value>
        public bool IsHandleOnce { get; private set; }

        /// <summary>
        /// Handlerが生存しているか否かを示します
        /// </summary>
        /// <value><c>true</c> if this instance is alive; otherwise, <c>false</c>.</value>
        public abstract bool IsAlive { get; }

        /// <summary>
        /// Handlerを取得します
        /// </summary>
        /// <value>The handler.</value>
        public abstract EventHandler<TEventArgs> Handler { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.EventHandlerReference"/> class.
        /// </summary>
        /// <param name="isHandleOnce">If set to <c>true</c> is handle once.</param>
        protected EventHandlerReference(bool isHandleOnce)
        {
            IsHandleOnce = isHandleOnce;
        }
    }
}


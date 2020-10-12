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
using Microsoft.Win32;

namespace Jackpot
{
    /// <summary>
    /// 強参照でEventHandlerを保持するクラスです
    /// </summary>
    public class HardEventHandlerReference<TEventArgs> : EventHandlerReference<TEventArgs> where TEventArgs : EventArgs
    {
        public override bool IsAlive { get { return Handler != null; } }

        public override EventHandler<TEventArgs> Handler { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.HardEventHandler`1"/> class.
        /// </summary>
        /// <param name="handler">Handler.</param>
        /// <param name="isHandleOnce">If set to <c>true</c> is handle once.</param>
        public HardEventHandlerReference(EventHandler<TEventArgs> handler, bool isHandleOnce) : base(isHandleOnce)
        {
            Handler = handler;
        }
    }
}


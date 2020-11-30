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
using UnityEngine;

namespace Jackpot
{
    /// <summary>
    /// 弱参照でEventHandlerを保持するクラスです
    /// </summary>
    public class WeakEventHandlerReference<TEventArgs> : EventHandlerReference<TEventArgs> where TEventArgs : EventArgs
    {
        public override bool IsAlive
        {
            get
            {
                var areAliveReference = targetReference.IsAlive && handlerReference.IsAlive;

                if (!areAliveReference)
                {
                    return false;
                }

                // UnityObject は Mono の GC とは別のロジックでメモリが回収されるため
                // WearkRefernce#IsAlive が true を返したとしても、Unity上からは参照が外れている可能性がある
                // そのため、MissingReferenceException の有無で参照の死活を見ている

                var target = targetReference.Target;
                var monoBehaviour = target as MonoBehaviour;
                if (monoBehaviour != null)
                {
                    try
                    {
                        monoBehaviour.gameObject.Equals(null);
                    }
                    catch (MissingReferenceException)
                    {
                        return false;
                    }
                    return true;
                }

                var gameObject = target as GameObject;
                if (gameObject != null)
                {
                    try
                    {
                        gameObject.Equals(null);
                    }
                    catch (MissingReferenceException)
                    {
                        return false;
                    }
                    return true;
                }

                // UnityObject の独特の死活チェックここまで

                return true;
            }
        }

        public override EventHandler<TEventArgs> Handler
        {
            get
            {
                return (EventHandler<TEventArgs>) handlerReference.Target;
            }
            protected set
            {
                handlerReference = new WeakReference(value);
            }
        }

        WeakReference handlerReference;
        readonly WeakReference targetReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.WeakEventHandlerReference"/> class.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="handler">Handler.</param>
        /// <param name="isHandleOnce">If set to <c>true</c> is handle once.</param>
        public WeakEventHandlerReference(object target, EventHandler<TEventArgs> handler, bool isHandleOnce) : base(isHandleOnce)
        {
            targetReference = new WeakReference(target);
            Handler = handler;
        }
    }
}

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
using System.Collections;
using System.Collections.Generic;

namespace Jackpot
{
    /// <summary>
    /// Provides a set of static methods for creating Disposables.
    /// </summary>
    public static class Disposable
    {
        #region Inner classes

        class EmptyDisposable : IDisposable
        {
            public void Dispose() { }
        }

        class AnonymousDisposable : IDisposable
        {
            Action onDispose;

            public AnonymousDisposable(Action onDispose)
            {
                this.onDispose = onDispose;
            }

            ~AnonymousDisposable()
            {
                Dispose();
            }

            public void Dispose()
            {
                if (null == onDispose)
                {
                    return;
                }

                onDispose();
                onDispose = null;
            }
        }

        #endregion

        #region Members

        static readonly IDisposable empty = new EmptyDisposable();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the disposable that does nothing when disposed.
        /// </summary>
        public static IDisposable Empty { get { return empty; } }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the disposable that invokes the specified action when disposed.
        /// </summary>
        /// <param name="dispose">The action to run during IDisposable.Dispose.</param>
        /// <returns>The disposable object that runs the given action upon disposal.</returns>
        public static IDisposable Create(Action dispose)
        {
            return new AnonymousDisposable(dispose);
        }

        #endregion
    }
}


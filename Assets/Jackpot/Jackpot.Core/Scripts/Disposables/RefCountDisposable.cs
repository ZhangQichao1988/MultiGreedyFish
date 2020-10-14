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
    /// Represents a disposable that only disposes its underlying disposable when all dependent disposables have been disposed.
    /// </summary>
    public class RefCountDisposable : IDisposable
    {
        IDisposable disposable = null;
        int refCount = 0;

        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Initializes a new instance of the RefCountDisposable class with the specified disposable.
        /// </summary>
        /// <param name="disposable">The underlying disposable.</param>
        public RefCountDisposable(IDisposable disposable)
        {
            this.disposable = disposable;
            this.IsDisposed = false;
        }

        /// <summary>
        /// Returns a dependent disposable that when disposed decreases the refcount on the underlying disposable.
        /// </summary>
        /// <returns>A dependent disposable contributing to the reference count that manages the underlying disposable's lifetime.</returns>
        public IDisposable GetDisposable()
        {
            if (IsDisposed)
            {
                return Disposable.Empty;
            }

            ++refCount;
            return Disposable.Create(OnDisposed);
        }

        void OnDisposed()
        {
            if (--refCount <= 0)
            {
                Dispose();
            }
        }

        /// <summary>
        /// Releases all resource used by the <see cref="Jackpot.RefCountDisposable"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Jackpot.RefCountDisposable"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="Jackpot.RefCountDisposable"/> in an unusable state. After
        /// calling <see cref="Dispose"/>, you must release all references to the
        /// <see cref="Jackpot.RefCountDisposable"/> so the garbage collector can reclaim the memory that the
        /// <see cref="Jackpot.RefCountDisposable"/> was occupying.</remarks>
        public void Dispose()
        {
            if (IsDisposed || 0 < refCount)
            {
                return;
            }

            disposable.Dispose();
            disposable = null;
            IsDisposed = true;
        }
    }
}


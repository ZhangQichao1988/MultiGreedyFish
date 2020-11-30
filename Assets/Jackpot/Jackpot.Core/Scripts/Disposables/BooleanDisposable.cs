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
    /// Represents an IDisposable that can be checked for status.
    /// </summary>
    public class BooleanDisposable : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets a value that indicates whether the object is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the BooleanDisposable class.
        /// </summary>
        public BooleanDisposable()
        {
            IsDisposed = false;
        }

        #endregion

        #region Implement IDisposable

        /// <summary>
        /// Sets the status to Disposed.
        /// </summary>
        public void Dispose()
        {
            IsDisposed = true;
        }

        #endregion
    }
}


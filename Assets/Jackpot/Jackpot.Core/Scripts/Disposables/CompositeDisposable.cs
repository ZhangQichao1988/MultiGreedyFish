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
    /// Represents a group of Disposables that are disposed together.
    /// </summary>
    public class CompositeDisposable : ICollection<IDisposable>, IEnumerable<IDisposable>, IEnumerable, IDisposable
    {
        #region Members

        List<IDisposable> disposables;

        #endregion

        #region Properties

        public bool IsDisposed { get; set; }

        #region Implement ICollection<T>

        public int Count { get { return disposables.Count; } }
        public bool IsReadOnly { get { return false; } }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the CompositeDisposable class from a group of disposables.
        /// </summary>
        public CompositeDisposable()
        {
            disposables = new List<IDisposable>();
            IsDisposed = false;
        }

        /// <summary>
        /// Initializes a new instance of the CompositeDisposable class from a group of disposables.
        /// </summary>
        /// <param name="disposables">The disposables that will be disposed together.</param>
        public CompositeDisposable(IEnumerable<IDisposable> disposables)
            : this()
        {
            foreach (var x in disposables)
            {
                Add(x);
            }
        }

        /// <summary>
        /// Initializes a new instance of the CompositeDisposable class from a group of disposables.
        /// </summary>
        /// <param name="disposables">The disposables that will be disposed together.</param>
        public CompositeDisposable(params IDisposable[] disposables)
            : this(disposables.Length)
        {
            foreach (var x in disposables)
            {
                Add(x);
            }
        }

        /// <summary>
        /// Initializes a new instance of the CompositeDisposable class with the specified number of disposables.
        /// </summary>
        /// <param name="capacity">The number of disposables that the new CompositeDisposable can initially store.</param>
        public CompositeDisposable(int capacity)
        {
            disposables = new List<IDisposable>(capacity);
            IsDisposed = false;
        }

        #endregion

        #region Destructor

        ~CompositeDisposable()
        {
            Dispose();
        }

        #endregion

        #region Methods

        #region Implement ICollection<T>

        /// <summary>
        /// Adds a disposable to the CompositeDisposable or disposes the disposable if the CompositeDisposable is disposed.
        /// </summary>
        /// <param name="item">The disposable to add.</param>
        public void Add(IDisposable item)
        {
            if (IsDisposed)
            {
                item.Dispose();
                return;
            }

            disposables.Add(item);
        }

        /// <summary>
        /// Removes and disposes the first occurrence of a disposable from the CompositeDisposable.
        /// </summary>
        /// <param name="item">The disposable to remove.</param>
        /// <returns></returns>
        public bool Remove(IDisposable item)
        {
            return disposables.Remove(item);
        }

        /// <summary>
        /// Removes and disposes all disposables from the GroupDisposable, but does not dispose the CompositeDisposable.
        /// </summary>
        public void Clear()
        {
            foreach (var d in disposables)
            {
                d.Dispose();
            }
            disposables.Clear();
        }

        /// <summary>
        /// Determines whether the CompositeDisposable contains a specific disposable.
        /// </summary>
        /// <param name="item">The disposable to search for.</param>
        /// <returns>true if the disposable was found; otherwise, false</returns>
        public bool Contains(IDisposable item)
        {
            return disposables.Contains(item);
        }

        /// <summary>
        /// Copies the disposables contained in the CompositeDisposable to an array, starting at a particular array index.
        /// </summary>
        /// <param name="array">The array to copy the contained disposables to.</param>
        /// <param name="arrayIndex">The target index at which to copy the first disposable of the group.</param>
        public void CopyTo(IDisposable[] array, int arrayIndex)
        {
            disposables.CopyTo(array, arrayIndex);
        }

        #endregion

        #region Implement IDisposable

        /// <summary>
        /// Disposes all disposables in the group and removes them from the group.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            Clear();
            IsDisposed = true;
        }

        #endregion

        #region Implement IEnumerable<T>

        /// <summary>
        /// Returns an enumerator that iterates through the CompositeDisposable
        /// </summary>
        /// <returns>An enumerator to iterate over the disposables.</returns>
        public IEnumerator<IDisposable> GetEnumerator()
        {
            return disposables.GetEnumerator();
        }

        #endregion

        #region Implement IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through the CompositeDisposable.
        /// </summary>
        /// <returns>An enumerator to iterate over the disposables.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #endregion
    }
}


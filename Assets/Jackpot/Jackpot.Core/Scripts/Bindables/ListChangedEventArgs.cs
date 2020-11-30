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
using System.Collections.Generic;

namespace Jackpot
{
    /// <summary>
    /// BindableListのイベント送出時に渡されるEventArgs
    /// </summary>
    public class ListChangedEventArgs<T> : EventArgs
    {
        #region Properties

        /// <summary>
        /// ListChangedEventの詳細を示します
        /// </summary>
        /// <see cref="Jackpot.ListChangedEventKind"/> 
        /// <value>The kind.</value>
        public ListChangedEventKind Kind { get; private set; }

        /// <summary>
        /// Update, Add, Removeによって変更のあったインデックスを示します。
        /// </summary>
        /// <remarks>
        /// 複数存在する場合、1つ目のIndexを示します。
        /// ForceUpdate, Edit時は-1が返却されます。
        /// </remarks>
        /// <value>The index.</value>
        public int Index
        {
            get
            {
                if (KindIsAnyOf(
                        ListChangedEventKind.ForceUpdate,
                        ListChangedEventKind.Edit))
                {
                    return -1;
                }
                if (KindIsAnyOf(
                        ListChangedEventKind.Update,
                        ListChangedEventKind.Add,
                        ListChangedEventKind.Remove))
                {
                    return Indexes[0];
                }
                return -1;
            } 
        }

        /// <summary>
        /// Update, Add, Removeによって変更のあったインデックスの一覧を示します
        /// </summary>
        /// <remarks>
        /// ForceUpdate, Edit時は空のリストが返却されます。
        /// </remarks>
        /// <value>The indexes.</value>
        public List<int> Indexes { get; private set; }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <remarks>
        /// ForceUpdate, Edit時は-1が返却されます。
        /// </remarks>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                if (KindIsAnyOf(ListChangedEventKind.ForceUpdate, ListChangedEventKind.Edit))
                {
                    return -1;
                }
                if (KindIsAnyOf(ListChangedEventKind.Update, ListChangedEventKind.Add, ListChangedEventKind.Remove))
                {
                    return Indexes.Count;
                }
                return 0;
            }
        }

        /// <summary>
        /// 変更前のListを返却します。使用するにはBindableList.OldValueEnabledプロパティをtrueにする必要があります
        /// </summary>
        /// <value>The old value.</value>
        /// <exception cref="System.InvalidOperationException">OldValueがnullだった時にスローされます</exception>
        public List<T> OldValue
        {
            get
            {
                if (oldValue == null)
                {
                    throw new InvalidOperationException("Missing OldValue. Did you set BindableList`1 OldValueEnabled to true?");
                }
                return oldValue;
            }
        }

        #endregion

        #region Fields

        List<T> oldValue;

        #endregion

        #region Factory

        public static ListChangedEventArgs<T> ForceUpdate(List<T> oldValue)
        {
            return new ListChangedEventArgs<T>(ListChangedEventKind.ForceUpdate, new List<int>(), oldValue);
        }

        public static ListChangedEventArgs<T> Update(int index, List<T> oldValue)
        {
            return Update(new List<int>() { index }, oldValue);
        }

        public static ListChangedEventArgs<T> Update(List<int> indexes, List<T> oldValue)
        {
            return new ListChangedEventArgs<T>(ListChangedEventKind.Update, indexes, oldValue);
        }

        public static ListChangedEventArgs<T> Add(int index, List<T> oldValue)
        {
            return Add(new List<int>() { index }, oldValue);
        }

        public static ListChangedEventArgs<T> Add(List<int> indexes, List<T> oldValue)
        {
            return new ListChangedEventArgs<T>(ListChangedEventKind.Add, indexes, oldValue);
        }

        public static ListChangedEventArgs<T> Remove(int index, List<T> oldValue)
        {
            return Remove(new List<int>() { index }, oldValue);
        }

        public static ListChangedEventArgs<T> Remove(List<int> indexes, List<T> oldValue)
        {
            return new ListChangedEventArgs<T>(ListChangedEventKind.Remove, indexes, oldValue);
        }

        public static ListChangedEventArgs<T> Edit(List<T> oldValue)
        {
            return new ListChangedEventArgs<T>(ListChangedEventKind.Edit, new List<int>(), oldValue);
        }

        #endregion

        #region Constructor

        ListChangedEventArgs(ListChangedEventKind kind, List<int> indexes, List<T> oldValue)
        {
            Kind = kind;
            Indexes = indexes;
            this.oldValue = oldValue;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Kindプロパティが指定のListChangedEventKindのいずれかに一致する事を示します
        /// </summary>
        /// <returns><c>true</c>, if is any of was kinded, <c>false</c> otherwise.</returns>
        /// <param name="kind">Kind.</param>
        /// <param name="moreKinds">More kinds.</param>
        public bool KindIsAnyOf(ListChangedEventKind kind, params ListChangedEventKind[] moreKinds)
        {
            if (Kind == kind)
            {
                return true;
            }
            if (moreKinds != null && moreKinds.Length > 0)
            {
                for (var i = 0; i < moreKinds.Length; i++)
                {
                    var moreKind = moreKinds[i];
                    if (Kind == moreKind)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

    }
}


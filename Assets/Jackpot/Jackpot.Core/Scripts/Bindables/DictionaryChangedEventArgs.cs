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
    using EventKind = DictionaryChangedEventKind;

    /// <summary>
    /// BindableDictionaryからのイベントに渡されるEventArgsオブジェクトです
    /// </summary>
    public class DictionaryChangedEventArgs<TKey, TValue> : EventArgs
    {
        #region Fields

        /// <summary>
        /// イベントが実行される前の状態を持っています
        /// </summary>
        Dictionary<TKey, TValue> oldValue;

        #endregion

        #region Properties

        /// <summary>
        /// イベントの種類を示します
        /// </summary>
        /// <see cref="Jackpot.DictionaryChangedEventKind"/> 
        /// <value>The kind.</value>
        public EventKind Kind { get; private set; }

        /// <summary>
        /// 変更された要素のキーのリストを示します
        /// </summary>
        /// <remarks>
        /// ForceUpdate, Edit では空のリストになります。null ではありません
        /// あとReadOnlyです
        /// </remarks>
        /// <value>The changes.</value>
        public List<TKey> Keys { get; private set; }

        /// <summary>
        /// 変更されたレコードの要素数を示します
        /// </summary>
        /// <remarks>
        /// ForceUpdate, Edit では -1 となります
        /// </remarks>
        /// <value>The count.</value>
        public int Count { get { return KindIsAnyOf(EventKind.ForceUpdate, EventKind.Edit) ? -1 : Keys.Count; } }

        /// <summary>
        /// 変更される前のレコードをすべて返します。使用する場合は BindableDictionary.OldValueEnabledプロパティをtrueとしてください
        /// </summary>
        /// <value>The old value.</value>
        /// <exception cref="System.InvalidOperationException">
        /// OldValue が null の場合
        /// </exception>
        public Dictionary<TKey, TValue> OldValue
        {
            get
            {
                if (null == oldValue)
                {
                    throw new InvalidOperationException("Missing OldValue. Did you set BindableDictionary`1 OldValueEnabled to true?");
                }
                return oldValue;
            }
        }

        #endregion

        #region Factory

        public static DictionaryChangedEventArgs<TKey, TValue> ForceUpdate(Dictionary<TKey, TValue> oldValue)
        {
            return new DictionaryChangedEventArgs<TKey, TValue>(EventKind.ForceUpdate, new List<TKey>(), oldValue);
        }

        public static DictionaryChangedEventArgs<TKey, TValue> Update(TKey key, Dictionary<TKey, TValue> oldValue)
        {
            return Update(new List<TKey>() { key }, oldValue);
        }

        public static DictionaryChangedEventArgs<TKey, TValue> Update(List<TKey> keys, Dictionary<TKey, TValue> oldValue)
        {
            return new DictionaryChangedEventArgs<TKey, TValue>(EventKind.Update, keys, oldValue);
        }

        public static DictionaryChangedEventArgs<TKey, TValue> Add(TKey key, Dictionary<TKey, TValue> oldValue)
        {
            return Add(new List<TKey>() { key }, oldValue);
        }

        public static DictionaryChangedEventArgs<TKey, TValue> Add(List<TKey> keys, Dictionary<TKey, TValue> oldValue)
        {
            return new DictionaryChangedEventArgs<TKey, TValue>(EventKind.Add, keys, oldValue);
        }

        public static DictionaryChangedEventArgs<TKey, TValue> Remove(TKey key, Dictionary<TKey, TValue> oldValue)
        {
            return Remove(new List<TKey>() { key }, oldValue);
        }

        public static DictionaryChangedEventArgs<TKey, TValue> Remove(List<TKey> keys, Dictionary<TKey, TValue> oldValue)
        {
            return new DictionaryChangedEventArgs<TKey, TValue>(EventKind.Remove, keys, oldValue);
        }

        public static DictionaryChangedEventArgs<TKey, TValue> Edit(Dictionary<TKey, TValue> oldValue)
        {
            return new DictionaryChangedEventArgs<TKey, TValue>(EventKind.Edit, new List<TKey>(), oldValue);
        }

        #endregion

        #region Constructor

        DictionaryChangedEventArgs(EventKind kind, List<TKey> keys, Dictionary<TKey, TValue> oldValue)
        {
            Kind = kind;
            Keys = keys;
            this.oldValue = oldValue;
        }

        #endregion

        #region Public Method

        /// <summary>
        /// Kindプロパティが引数の中から一致するものがあるかどうかを返します
        /// </summary>
        /// <returns><c>true</c>, if is any of was kinded, <c>false</c> otherwise.</returns>
        /// <param name="kind">Kind.</param>
        /// <param name="more">More.</param>
        public bool KindIsAnyOf(EventKind kind, params EventKind[] more)
        {
            if (Kind == kind)
            {
                return true;
            }

            if (null == more)
            {
                return false;
            }

            foreach (var k in more)
            {
                if (Kind == k)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}

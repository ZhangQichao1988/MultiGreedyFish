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
    /// BindableObjectがデータの変更通知時に送出するEventArgsです
    /// </summary>
    public class PropertyChangedEventArgs<T> : EventArgs
    {
        #region Properties

        /// <summary>
        /// PropertyChangedEventがどういう原因で発生したか、詳細を示します
        /// </summary>
        /// <see cref="Jackpot.PropertyChangedEventKind"/> 
        /// <value>The kind.</value>
        public PropertyChangedEventKind Kind { get; private set; }

        /// <summary>
        /// 変更前の値を示します
        /// </summary>
        /// <value>The old value.</value>
        public T OldValue { get; private set; }

        /// <summary>
        /// 変更後の値を示します
        /// </summary>
        /// <value>The new value.</value>
        public T NewValue { get; private set; }

        #endregion

        #region Factory

        /// <summary>
        /// PropertyChangedEventKind.ForceUpdate用のEventArgsを返却します
        /// </summary>
        /// <returns>The update.</returns>
        /// <param name="value">Value.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static PropertyChangedEventArgs<T> ForceUpdate(T value)
        {
            var result = new PropertyChangedEventArgs<T>(PropertyChangedEventKind.ForceUpdate, value, value);
            return result;
        }

        /// <summary>
        /// BindableObject用のPropertyChangedEventKind.UpdateのEventArgsを返却します
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static PropertyChangedEventArgs<T> Update(T oldValue, T newValue)
        {
            var result = new PropertyChangedEventArgs<T>(PropertyChangedEventKind.Update, oldValue, newValue);
            return result;
        }

        #endregion

        #region Constructor

        PropertyChangedEventArgs(PropertyChangedEventKind kind, T oldValue, T newValue)
        {
            Kind = kind;
            OldValue = oldValue;
            NewValue = newValue;
        }

        #endregion

    }
}

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

namespace Jackpot
{
    using EventKind = PlayerPrefsChangedEventKind;

    /// <summary>
    /// BindablePlayerPrefsのイベント送出時に渡されるEventArgs
    /// </summary>
    public class PlayerPrefsChangedEventArgs : EventArgs
    {
        #region Public Properties

        /// <summary>
        /// イベントの種類を示します。種類については<see cref="PlayerPrefsChangedEventKind"/>を参照してください
        /// </summary>
        /// <value>The kind.</value>
        public EventKind Kind { get; private set; }

        /// <summary>
        /// 変更のあったキーを示します
        /// </summary>
        /// <remarks>
        /// DeleteAllイベントでは、一意にキーを判定できない為<c>null</c>となります
        /// </remarks>
        /// <value>The key.</value>
        public string Key { get; private set; }

        /// <summary>
        /// 変更のあったpreferenceの値の型を示します
        /// </summary>
        /// <remarks>
        /// Delete, DeleteAllイベントで変更される値に対する型が推定できない場合は<c>void</c>型になります
        /// </remarks>
        /// <value>The type of the value.</value>
        public Type ValueType { get; private set; }

        /// <summary>
        /// 変更前の<c>int</c>型の値を示します
        /// </summary>
        /// <remarks>
        /// 新規に登録する場合は、型のデフォルト値である<c>0</c>が常に返ります
        /// </remarks>
        /// <value>The old int value.</value>
        public int OldIntValue { get; private set; }

        /// <summary>
        /// 変更後の<c>int</c>型の値を示します
        /// </summary>
        /// <remarks>
        /// Delete, DeleteAllイベントの場合、型のデフォルト値である<c>0</c>が常に返ります
        /// </remarks>
        /// <value>The new int value.</value>
        public int NewIntValue { get; private set; }

        /// <summary>
        /// 変更前の<c>float</c>型の値を示します
        /// </summary>
        /// <remarks>
        /// 新規に登録する場合は、型のデフォルト値である<c>0.0f</c>が常に返ります
        /// </remarks>
        /// <value>The old float value.</value>
        public float OldFloatValue { get; private set; }

        /// <summary>
        /// 変更後の<c>float</c>型の値を示します
        /// </summary>
        /// <remarks>
        /// Delete, DeleteAllイベントの場合、型のデフォルト値である<c>0.0f</c>が常に返ります
        /// </remarks>
        /// <value>The new float value.</value>
        public float NewFloatValue { get; private set; }

        /// <summary>
        /// 変更前の<c>string</c>型の値を示します
        /// </summary>
        /// <remarks>
        /// 新規に登録する場合は、型のデフォルト値である<c>""</c>（空文字列）が常に返ります
        /// </remarks>
        /// <value>The old string value.</value>
        public string OldStringValue { get; private set; }

        /// <summary>
        /// 変更後の<c>string</c>型の値を示します
        /// </summary>
        /// <remarks>
        /// Delete, DeleteAllイベントの場合、型のデフォルト値である<c>""</c>（空文字列）が常に返ります
        /// </remarks>
        /// <value>The new string value.</value>
        public string NewStringValue { get; private set; }

        #endregion

        #region Factory Method

        public static PlayerPrefsChangedEventArgs Update(string key, int oldValue, int newValue)
        {
            return new PlayerPrefsChangedEventArgs(EventKind.Update, key, oldValue, newValue);
        }

        public static PlayerPrefsChangedEventArgs Update(string key, float oldValue, float newValue)
        {
            return new PlayerPrefsChangedEventArgs(EventKind.Update, key, oldValue, newValue);
        }

        public static PlayerPrefsChangedEventArgs Update(string key, string oldValue, string newValue)
        {
            return new PlayerPrefsChangedEventArgs(EventKind.Update, key, oldValue, newValue);
        }

        public static PlayerPrefsChangedEventArgs Delete(string key)
        {
            return new PlayerPrefsChangedEventArgs(EventKind.Delete, key);
        }

        public static PlayerPrefsChangedEventArgs Delete(string key, int oldValue)
        {
            return new PlayerPrefsChangedEventArgs(EventKind.Delete, key, oldValue, 0);
        }

        public static PlayerPrefsChangedEventArgs Delete(string key, float oldValue)
        {
            return new PlayerPrefsChangedEventArgs(EventKind.Delete, key, oldValue, 0.0f);
        }

        public static PlayerPrefsChangedEventArgs Delete(string key, string oldValue)
        {
            return new PlayerPrefsChangedEventArgs(EventKind.Delete, key, oldValue, "");
        }

        public static PlayerPrefsChangedEventArgs DeleteAll()
        {
            return new PlayerPrefsChangedEventArgs(EventKind.DeletedAll);
        }

        #endregion

        #region Constructor

        PlayerPrefsChangedEventArgs(EventKind kind)
        {
            Kind = kind;
            ValueType = typeof(void);
        }

        PlayerPrefsChangedEventArgs(EventKind kind, string key)
        {
            Kind = kind;
            Key = key;
            ValueType = typeof(void);
        }

        PlayerPrefsChangedEventArgs(EventKind kind, string key, int oldValue, int newValue)
        {
            Kind = kind;
            Key = key;
            ValueType = typeof(int);
            OldIntValue = oldValue;
            NewIntValue = newValue;
        }

        PlayerPrefsChangedEventArgs(EventKind kind, string key, float oldValue, float newValue)
        {
            Kind = kind;
            Key = key;
            ValueType = typeof(float);
            OldFloatValue = oldValue;
            NewFloatValue = newValue;
        }

        PlayerPrefsChangedEventArgs(EventKind kind, string key, string oldValue, string newValue)
        {
            Kind = kind;
            Key = key;
            ValueType = typeof(string);
            OldStringValue = oldValue;
            NewStringValue = newValue;
        }

        #endregion
    }
}

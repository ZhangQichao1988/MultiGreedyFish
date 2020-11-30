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
using UnityEngine;
using System;

namespace Jackpot
{
    /// <summary>
    /// 要素の変更を監視できるPlayerPrefsクラスです
    /// </summary>
    /// <remarks>
    /// BindablePlayerPrefsから送出されるイベントの詳細は<see cref="Jackpot.PlayerPrefsChangedEventKind"/>を参考にしてください。
    /// BindablePlayerPrefsから送出されるイベントに付属するパラメータは<see cref="Jackpot.PlayerPrefsChangedEventArgs"/>を参考にしてください。
    /// </remarks>
    public sealed class BindablePlayerPrefs
    {
        #region Constants

        /// <summary>
        /// デフォルトの許容誤差を示します。
        /// </summary>
        public static readonly float defaultTolerance = 1e-5f;

        /// <summary>
        /// <c>int</c>型に対するBindablePlayerPrefsのデフォルトの変更ポリシーです。
        /// </summary>
        /// <returns><c>true</c>, if int value changes policy was defaulted, <c>false</c> otherwise.</returns>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        public static bool DefaultIntValueChangesPolicy(int oldValue, int newValue)
        {
            return oldValue != newValue;
        }

        /// <summary>
        /// <c>float</c>型に対するBindablePlayerPrefsのデフォルトの変更ポリシーです。
        /// </summary>
        /// <returns><c>true</c>, if float value changes policy was defaulted, <c>false</c> otherwise.</returns>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        public static bool DefaultFloatValueChangesPolicy(float oldValue, float newValue)
        {
            return !IsNearlyEquals(oldValue, newValue);
        }

        /// <summary>
        /// <c>string</c>型に対するBindablePlayerPrefsのデフォルトの変更ポリシーです。
        /// </summary>
        /// <returns><c>true</c>, if string value changes policy was defaulted, <c>false</c> otherwise.</returns>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        public static bool DefaultStringValueChangesPolicy(string oldValue, string newValue)
        {

            return oldValue != newValue;
        }

        #endregion

        #region Fields

        static EventDispatcher<PlayerPrefsChangedEventArgs> dispatcher = new EventDispatcher<PlayerPrefsChangedEventArgs>();

        static Func<int, int, bool> intValueChangesPolicy = DefaultIntValueChangesPolicy;

        static Func<float, float, bool> floatValueChangesPolicy = DefaultFloatValueChangesPolicy;

        static Func<string, string, bool> stringValueChangesPolicy = DefaultStringValueChangesPolicy;

        static float tolerance = defaultTolerance;

        #endregion

        #region Properties

        public static BindableRules Rule { get; set; }

        #endregion

        #region Binding API

        /// <summary>
        /// 変更通知を受け取るイベントハンドラを追加します。重複された場合は再登録されます。
        /// </summary>
        /// <remarks>
        /// 追加したイベントハンドラは強い参照で保持されます。
        /// 詳しくは<see cref="EventDispatcher{TEventArgs}.AddHandler"/>を参照してください
        /// </remarks>
        /// <param name="handler">イベントハンドラ</param>
        public static void Bind(EventHandler<PlayerPrefsChangedEventArgs> handler)
        {
            dispatcher.AddHandler(handler);
        }

        /// <summary>
        /// 参照オプションを指定して、変更通知を受け取るイベントハンドラを追加します。
        /// </summary>
        /// <remarks>
        /// 追加したイベントハンドラは弱い参照で保持されます。第一引数が<c>null</c>の場合は強い参照になります
        /// 詳しくは<see cref="EventDispatcher{TEventArgs}.AddHandler"/>を参照してください
        /// </remarks>
        /// <param name="target">ハンドリングの基準になるオブジェクト</param>
        /// <param name="handler">イベントハンドラ</param>
        public static void Bind(object target, EventHandler<PlayerPrefsChangedEventArgs> handler)
        {
            dispatcher.AddHandler(target, handler);
        }

        /// <summary>
        /// 変更通知を一回だけ受け取るイベントハンドラを追加します. 重複した場合は再登録されます
        /// </summary>
        /// <remarks>
        /// 追加したイベントハンドラは強い参照で保持されます。
        /// 詳しくは<see cref="EventDispatcher{TEventArgs}.AddHandler"/>を参照してください
        /// </remarks>
        /// <param name="handler">イベントハンドラ</param>
        public static void BindOnce(EventHandler<PlayerPrefsChangedEventArgs> handler)
        {
            dispatcher.AddHandler(handler, true);
        }

        /// <summary>
        /// 変更通知を一回だけ受け取るイベントハンドラを追加します. 重複した場合は再登録されます
        /// </summary>
        /// <remarks>
        /// 追加したイベントハンドラは弱い参照で保持されます。第一引数が<c>null</c>の場合は強い参照になります
        /// 詳しくは<see cref="EventDispatcher{TEventArgs}.AddHandler"/>を参照してください
        /// </remarks>
        /// <param name="target">ハンドリングの基準になるオブジェクト</param>
        /// <param name="handler">イベントハンドラ</param>
        public static void BindOnce(object target, EventHandler<PlayerPrefsChangedEventArgs> handler)
        {
            dispatcher.AddHandler(target, handler, true);
        }

        /// <summary>
        /// イベントハンドラが一つでも登録されているかどうかを示します。
        /// </summary>
        /// <returns><c>true</c> if is binding; otherwise, <c>false</c>.</returns>
        public static bool IsBinding()
        {
            return !dispatcher.HandlerIsEmpty;
        }

        /// <summary>
        /// 指定したイベントハンドラが一つでも登録されているかどうかを示します。
        /// </summary>
        /// <returns><c>true</c> if is binding the specified handler; otherwise, <c>false</c>.</returns>
        /// <param name="handler">Handler.</param>
        public static bool IsBinding(EventHandler<PlayerPrefsChangedEventArgs> handler)
        {
            return dispatcher.ContainsHandler(handler);
        }

        /// <summary>
        /// 変更通知を一度だけ受け取るイベントハンドラが一つでも登録されているかどうかを示します。
        /// </summary>
        /// <returns><c>true</c> if is binding once; otherwise, <c>false</c>.</returns>
        public static bool IsBindingOnce()
        {
            return !dispatcher.OnceHandlerIsEmpty;
        }

        /// <summary>
        /// 指定したイベントハンドラが、変更通知を一度だけ受け取るよう登録されているかどうかを示します。
        /// </summary>
        /// <returns><c>true</c> if is binding once the specified handler; otherwise, <c>false</c>.</returns>
        /// <param name="handler">Handler.</param>
        public static bool IsBindingOnce(EventHandler<PlayerPrefsChangedEventArgs> handler)
        {
            return dispatcher.ContainsHandler(handler, true);
        }

        /// <summary>
        /// 指定したイベントハンドラを登録解除します。
        /// </summary>
        /// <param name="handler">Handler.</param>
        public static void UnBind(EventHandler<PlayerPrefsChangedEventArgs> handler)
        {
            dispatcher.RemoveHandler(handler);
        }

        /// <summary>
        /// 全てのイベントハンドラを登録解除します。
        /// </summary>
        public static void UnBindAll()
        {
            dispatcher.RemoveAllHandlers();
        }

        /// <summary>
        /// 変更通知を知らせる条件の、<c>int</c>型に対する変更ポリシーを指定します。
        /// </summary>
        /// <remarks>
        /// <c>null</c>を指定した場合、同じ値を重複した登録に対しても通知されるようになります。
        /// </remarks>
        /// <param name="policy">Policy.</param>
        public static void SetIntValueChangesPolicy(Func<int, int, bool> policy)
        {
            intValueChangesPolicy = policy;
        }

        /// <summary>
        /// 変更通知を知らせる条件の、<c>int</c>型に対する変更ポリシーを指定します。
        /// </summary>
        /// <remarks>
        /// <c>null</c>を指定した場合、同じ値を重複した登録に対しても通知されるようになります。
        /// </remarks>
        /// <param name="policy">Policy.</param>
        public static void SetFloatValueChangesPolicy(Func<float, float, bool> policy)
        {
            floatValueChangesPolicy = policy;
        }

        /// <summary>
        /// 変更通知を知らせる条件の、<c>int</c>型に対する変更ポリシーを指定します。
        /// </summary>
        /// <remarks>
        /// <c>null</c>を指定した場合、同じ値を重複した登録に対しても通知されるようになります。
        /// </remarks>
        /// <param name="policy">Policy.</param>
        public static void SetStringValueChangesPolicy(Func<string, string, bool> policy)
        {
            stringValueChangesPolicy = policy;
        }

        /// <summary>
        /// <c>float</c>型に対する誤差許容値を指定します。指定された数の絶対値で比較されます。
        /// </summary>
        /// <param name="newTolerance">New tolerance.</param>
        public static void SetFloatValueTolerance(float newTolerance)
        {
            tolerance = Math.Abs(newTolerance);
        }

        #endregion

        #region PlayerPrefs API

        /// <summary>
        /// 指定したキーに対するエントリーが、PlayerPrefsに既に登録されているかどうかを示します。
        /// </summary>
        /// <returns><c>true</c> if has key the specified key; otherwise, <c>false</c>.</returns>
        /// <param name="key">Key.</param>
        public static bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        /// <summary>
        /// 指定したキーに登録されている値を取得します。
        /// </summary>
        /// <remarks>
        /// 登録されていなかった場合、デフォルト値の<c>0</c>が返ります。
        /// </remarks>
        /// <returns>The int.</returns>
        /// <param name="key">Key.</param>
        public static int GetInt(string key)
        {
            return PlayerPrefs.GetInt(key);
        }

        /// <summary>
        /// 指定したキーと値をPlayerPrefsに登録し、必要があればUpdateイベントを発行します。
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public static void SetInt(string key, int value)
        {
            ValidateCallStackChanging();
            ValidateKey(key);
            ValidateValueType(key, PlayerPrefsValueType.IntValue);

            var oldValue = GetInt(key);

            PlayerPrefs.SetInt(key, value);

            if (intValueChangesPolicy != null && !intValueChangesPolicy(oldValue, value))
            {
                return;
            }

            if (IsDispatchable())
            {
                dispatcher.Dispatch(PlayerPrefsChangedEventArgs.Update(key, oldValue, value));
            }
        }

        /// <summary>
        /// 指定したキーに登録されている値を取得します。
        /// </summary>
        /// <remarks>
        /// 登録されていなかった場合、デフォルト値の<c>0</c>が返ります。
        /// </remarks>
        /// <returns>The float.</returns>
        /// <param name="key">Key.</param>
        public static float GetFloat(string key)
        {
            return PlayerPrefs.GetFloat(key);
        }

        /// <summary>
        /// 指定したキーと値をPlayerPrefsに登録し、必要があればUpdateイベントを発行します。
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public static void SetFloat(string key, float value)
        {
            ValidateCallStackChanging();
            ValidateKey(key);
            ValidateValueType(key, PlayerPrefsValueType.FloatValue);

            var oldValue = GetFloat(key);

            PlayerPrefs.SetFloat(key, value);

            if (floatValueChangesPolicy != null && !floatValueChangesPolicy(oldValue, value))
            {
                return;
            }

            if (IsDispatchable())
            {
                dispatcher.Dispatch(PlayerPrefsChangedEventArgs.Update(key, oldValue, value));
            }
        }

        /// <summary>
        /// 指定したキーに登録されている値を取得します。
        /// </summary>
        /// <remarks>
        /// 登録されていなかった場合、デフォルト値の<c>""</c>（空文字列）が返ります。
        /// </remarks>
        /// <returns>The string.</returns>
        /// <param name="key">Key.</param>
        public static string GetString(string key)
        {
            return PlayerPrefs.GetString(key);
        }

        /// <summary>
        /// 指定したキーと値をPlayerPrefsに登録し、必要があればUpdateイベントを発行します。
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public static void SetString(string key, string value)
        {
            ValidateCallStackChanging();
            ValidateKey(key);
            ValidateValueType(key, PlayerPrefsValueType.StringValue);

            var oldValue = GetString(key);

            PlayerPrefs.SetString(key, value);

            if (stringValueChangesPolicy != null && !stringValueChangesPolicy(oldValue, value))
            {
                return;
            }

            if (IsDispatchable())
            {
                dispatcher.Dispatch(PlayerPrefsChangedEventArgs.Update(key, oldValue, value));
            }
        }

        /// <summary>
        /// 指定したキーの項目を削除し、必要があればDeleteイベントを発行します。
        /// </summary>
        /// <remarks>
        /// 登録されていなかったキーであった場合、何も起きず、通知もされません。
        /// </remarks>
        /// <param name="key">Key.</param>
        public static void DeleteKey(string key)
        {
            if (HasKey(key))
            {
                ValidateCallStackChanging();

                // HACK 消す前にEventArgs作ってOldValue確保しておく(Dirty)
                var eventArgs = TryCreateRemoveEvent(key);

                PlayerPrefs.DeleteKey(key);

                if (IsDispatchable())
                {
                    dispatcher.Dispatch(eventArgs);
                }
            }
        }

        /// <summary>
        /// 全てのPlayerPrefsの項目を削除し、DeleteAllイベントを発行します。
        /// </summary>
        /// <remarks>
        /// PlayerPrefsに一つでもキーが登録されていればDeleteAllイベントを発行します。
        /// DeleteAllイベントからは、「消されたキーの一覧」などを取得することは出来ません。
        /// </remarks>
        public static void DeleteAll()
        {
            ValidateCallStackChanging();

            PlayerPrefs.DeleteAll();

            if (IsDispatchable())
            {
                dispatcher.Dispatch(PlayerPrefsChangedEventArgs.DeleteAll());
            }
        }

        /// <summary>
        /// PlayerPrefsの状態を、各プラットフォームの設定ファイルに書き込みます。
        /// </summary>
        public static void Save()
        {
            PlayerPrefs.Save();
        }

        #endregion

        #region Private Method

        /// <summary>
        /// ２つの浮動小数点小数の比較に対し、「近似している（=ほぼ同じ）」ことを示します。
        /// </summary>
        /// <remarks>
        /// 許容誤差を変更する場合は、<see cref="BindablePlayerPrefs.SetFloatValueTolerance"/>を利用してください。
        /// </remarks>
        /// <returns><c>true</c> if is nearly equals the specified a b; otherwise, <c>false</c>.</returns>
        /// <param name="a">The alpha component.</param>
        /// <param name="b">The blue component.</param>
        static bool IsNearlyEquals(float a, float b)
        {
            return Math.Abs(Math.Abs(a) - Math.Abs(b)) < tolerance;
        }

        /// <summary>
        /// キーとして登録できる適切な値かどうかをチェックします。
        /// </summary>
        /// <param name="key">Key.</param>
        /// <exception cref="System.ArgumentNullException">キーが<c>null</c>の場合</exception>
        /// <exception cref="System.ArgumentException">キーが<c>""</c>（空文字列）の場合</exception> 
        static void ValidateKey(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key must not be empty");
            }
        }

        /// <summary>
        /// イベントハンドラの関数内部で、更にBindablePlayerPrefの項目を変更しようとしているかどうかをチェックします。
        /// </summary>
        /// <exception cref="System.InvalidOperationException">編集中の状態である場合</exception>
        static void ValidateCallStackChanging()
        {
            if (Rule == BindableRules.NotAllowCallStackChanging && dispatcher.IsDispatching)
            {
                throw new InvalidOperationException("A value cannot be changed while dispatching data.");
            }
        }

        /// <summary>
        /// 登録しようとしている項目を破壊的に変更しないかをチェックします。
        /// </summary>
        /// <remarks>
        /// 1) 既に「ある型の値」が登録されている
        /// 2) デフォルトでない値で登録されている
        /// 3) その項目に対して「異なる型の値」で上書きしようとしている
        /// 1), 2), 3) の条件を満たした場合に例外を出します。
        /// </remarks>
        /// <param name="key">Key.</param>
        /// <param name="type">Type.</param>
        static void ValidateValueType(string key, PlayerPrefsValueType type)
        {
            if (!HasKey(key))
            {
                // 新規の登録ならどの型でも良いとする
                return;
            }

            // HACK がんばってデフォルト値じゃないものを探す
            var i = GetInt(key);
            var f = GetFloat(key);
            var s = GetString(key);

            switch (type)
            {
                case PlayerPrefsValueType.IntValue:
                    if (s.Length > 0 || !IsNearlyEquals(0, f))
                    {
                        throw new InvalidOperationException("Already different type value set");
                    }
                    return;
                case PlayerPrefsValueType.FloatValue:
                    if (s.Length > 0 || i != 0)
                    {
                        throw new InvalidOperationException("Already different type value set");
                    }
                    return;
                case PlayerPrefsValueType.StringValue:
                    if (i != 0 || !IsNearlyEquals(0, f))
                    {
                        throw new InvalidOperationException("Already different type value set");
                    }
                    return;
                default: // VoidValue
                    return;
            }
        }

        /// <summary>
        /// 変更通知を発行する状態であるかどうかを示します。
        /// </summary>
        /// <returns><c>true</c> if is dispatchable; otherwise, <c>false</c>.</returns>
        static bool IsDispatchable()
        {
            if (dispatcher.IsDispatching)
            {
                if (Rule == BindableRules.NotAllowCallStackChanging || Rule == BindableRules.ThroughCallStackChanging)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 削除されたであろう値を推論してPlayerPrefsChangedEventArgsを生成します。
        /// </summary>
        /// <remarks>
        /// 仮に<c>int</c>, <c>float</c>, <c>string</c>のいずれかを登録していたとしても、その型のデフォルト値であった場合、
        /// 推論できないものとして<c>void</c>型のRemoveイベントと判断されます
        /// </remarks>
        /// <returns>The create remove event.</returns>
        /// <param name="key">Key.</param>
        static PlayerPrefsChangedEventArgs TryCreateRemoveEvent(string key)
        {
            // HACK がんばってデフォルト値じゃないものを探す
            var i = GetInt(key);
            var f = GetFloat(key);
            var s = GetString(key);

            if (s.Length > 0)
            {
                return PlayerPrefsChangedEventArgs.Delete(key, s);
            }
            else if (!IsNearlyEquals(0, f))
            {
                return PlayerPrefsChangedEventArgs.Delete(key, f);
            }
            else if (i != 0)
            {
                return PlayerPrefsChangedEventArgs.Delete(key, i);
            }
            else
            {
                // HACK 意図的にデフォルト値で登録されてた場合
                // さすがに型は推測できないので、しょうがないから void 扱いにする
                return PlayerPrefsChangedEventArgs.Delete(key);
            }
        }

        #endregion
    }
}

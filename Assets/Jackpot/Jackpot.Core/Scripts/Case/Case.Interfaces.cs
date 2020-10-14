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
    /// 1つのパラメータを用いて条件分岐を行う為の基礎クラスです
    /// </summary>
    public static partial class Case<T1> where T1 : IEquatable<T1>
    {
        /// <summary>
        /// 実装クラスは条件分岐を実施する機能を提供します
        /// </summary>
        public interface IApplyer
        {
            /// <summary>
            /// 事前に記述した条件分岐に従って処理を実施します
            /// </summary>
            /// <param name="candidate1">Candidate1.</param>
            void Apply(T1 candidate1);
        }

        /// <summary>
        /// 実装クラスは条件分岐を実施し、結果を返却する機能を提供します
        /// </summary>
        public interface IApplyer<TResult>
        {
            /// <summary>
            /// 事前に記述した条件分岐に従って処理を実施し、結果を返却します
            /// </summary>
            /// <param name="candidate1">Candidate1.</param>
            TResult Apply(T1 candidate1);
        }

        /// <summary>
        /// 実装クラスは条件分岐を記述、実施する機能を提供します
        /// </summary>
        public interface IMatcher : IApplyer
        {
            /// <summary>
            /// 指定のパラメータが渡された時のコールバックを指定します
            /// </summary>
            /// <param name="value">Value.</param>
            /// <param name="callback">Callback.</param>
            IMatcher When(T1 value, Action callback);

            /// <summary>
            /// 指定のパラメータが渡されたときに、その値を使用して実施するコールバックを指定します
            /// </summary>
            /// <param name="value">Value.</param>
            /// <param name="callback">Callback.</param>
            IMatcher When(T1 value, Action<T1> callback);

            /// <summary>
            /// パラメータが指定された条件を満たすときのコールバックを指定します。
            /// </summary>
            /// <param name="predicate">Predicate.</param>
            /// <param name="callback">Callback.</param>
            IMatcher When(Func<T1, bool> predicate, Action callback);

            /// <summary>
            /// パラメータが指定された条件を満たすときに、その値を使用して実施するコールバックを指定します。
            /// </summary>
            /// <param name="predicate">Predicate.</param>
            /// <param name="callback">Callback.</param>
            IMatcher When(Func<T1, bool> predicate, Action<T1> callback);

            /// <summary>
            /// 条件が満たされなかった時のコールバックを指定します。
            /// </summary>
            /// <param name="defaultCallback">Default callback.</param>
            IMatcher Default(Action defaultCallback);

            /// <summary>
            /// 条件に一致しなかった時に、その値を使用して実施するコールバックを指定します。
            /// </summary>
            /// <param name="defaultCallback">Default callback.</param>
            IMatcher Default(Action<T1> defaultCallback);
        }

        /// <summary>
        /// 実装クラスは条件分岐を記述、実施し、その結果を返却する機能を提供します
        /// </summary>
        public interface IResultMatcher<TResult> : IApplyer<TResult>
        {
            /// <summary>
            /// 指定のパラメータが渡された時に返却する値を示します
            /// </summary>
            /// <param name="value">Value.</param>
            /// <param name="result">Result.</param>
            IResultMatcher<TResult> When(T1 value, TResult result);

            /// <summary>
            /// 指定のパラメータが渡された時のコールバックを指定します
            /// </summary>
            /// <param name="value">Value.</param>
            /// <param name="callback">Callback.</param>
            IResultMatcher<TResult> When(T1 value, Func<TResult> callback);

            /// <summary>
            /// 指定のパラメータが渡されたときに、その値を使用して実施するコールバックを指定します
            /// </summary>
            /// <param name="value">Value.</param>
            /// <param name="callback">Callback.</param>
            IResultMatcher<TResult> When(T1 value, Func<T1, TResult> callback);

            /// <summary>
            /// パラメータが指定された条件を満たすときの結果を指定します
            /// </summary>
            /// <param name="value">Value.</param>
            /// <param name="result">Result.</param>
            IResultMatcher<TResult> When(Func<T1, bool> value, TResult result);

            /// <summary>
            /// 指定のパラメータが渡された時のコールバックを指定します
            /// </summary>
            /// <param name="value">Value.</param>
            /// <param name="callback">Callback.</param>
            IResultMatcher<TResult> When(Func<T1, bool> value, Func<TResult> callback);

            /// <summary>
            /// パラメータが指定された条件を満たすときに、その値を使用して実施するコールバックを指定します。
            /// </summary>
            /// <param name="predicate">Predicate.</param>
            /// <param name="callback">Callback.</param>
            IResultMatcher<TResult> When(Func<T1, bool> predicate, Func<T1, TResult> callback);

            /// <summary>
            /// 条件が満たされなかった時の結果を指定します。
            /// </summary>
            /// <param name="defaultResult">Default result.</param>
            IResultMatcher<TResult> Default(TResult defaultResult);

            /// <summary>
            /// 条件が満たされなかった時のコールバックを指定します。
            /// </summary>
            /// <param name="defaultCallback">Default callback.</param>
            IResultMatcher<TResult> Default(Func<TResult> defaultCallback);

            /// <summary>
            /// 条件に一致しなかった時に、その値を使用して実施するコールバックを指定します。
            /// </summary>
            /// <param name="defaultCallback">Default callback.</param>
            IResultMatcher<TResult> Default(Func<T1, TResult> defaultCallback);
        }
    }

    /// <summary>
    /// 2つのパラメータを用いて条件分岐を行う為の基礎クラスです
    /// </summary>
    public static partial class Case<T1, T2> where T1 : IEquatable<T1> where T2 : IEquatable<T2>
    {
        /// <summary>
        /// 実装クラスは条件分岐を実施する機能を提供します
        /// </summary>
        public interface IApplyer
        {
            /// <summary>
            /// 事前に記述した条件分岐に従って処理を実施します
            /// </summary>
            /// <param name="candidate1">Candidate1.</param>
            /// <param name="candidate2">Candidate2.</param>
            void Apply(T1 candidate1, T2 candidate2);
        }

        /// <summary>
        /// 実装クラスは条件分岐を実施し、結果を返却する機能を提供します
        /// </summary>
        public interface IApplyer<TResult>
        {
            /// <summary>
            /// 事前に記述した条件分岐に従って処理を実施し、結果を返却します
            /// </summary>
            /// <param name="candidate1">Candidate1.</param>
            /// <param name="candidate2">Candidate2.</param>
            TResult Apply(T1 candidate1, T2 candidate2);
        }

        /// <summary>
        /// 実装クラスは条件分岐を記述、実施する機能を提供します
        /// </summary>
        public interface IMatcher : IApplyer
        {
            /// <summary>
            /// 指定のパラメータが渡された時のコールバックを指定します
            /// </summary>
            /// <param name="value1">Value1.</param>
            /// <param name="value2">Value2.</param>
            /// <param name="callback">Callback.</param>
            IMatcher When(T1 value1, T2 value2, Action callback);

            /// <summary>
            /// 指定のパラメータが渡されたときに、その値を使用して実施するコールバックを指定します
            /// </summary>
            /// <param name="value1">Value1.</param>
            /// <param name="value2">Value2.</param>
            /// <param name="callback">Callback.</param>
            IMatcher When(T1 value1, T2 value2, Action<T1, T2> callback);

            /// <summary>
            /// パラメータが指定された条件を満たすときのコールバックを指定します。
            /// </summary>
            /// <param name="predicate">Predicate.</param>
            /// <param name="callback">Callback.</param>
            IMatcher When(Func<T1, T2, bool> predicate, Action callback);

            /// <summary>
            /// パラメータが指定された条件を満たすときに、その値を使用して実施するコールバックを指定します。
            /// </summary>
            /// <param name="predicate">Predicate.</param>
            /// <param name="callback">Callback.</param>
            IMatcher When(Func<T1, T2, bool> predicate, Action<T1, T2> callback);

            /// <summary>
            /// 条件が満たされなかった時のコールバックを指定します。
            /// </summary>
            /// <param name="defaultCallback">Default callback.</param>
            IMatcher Default(Action defaultCallback);

            /// <summary>
            /// 条件に一致しなかった時に、その値を使用して実施するコールバックを指定します。
            /// </summary>
            /// <param name="defaultCallback">Default callback.</param>
            IMatcher Default(Action<T1, T2> defaultCallback);
        }

        /// <summary>
        /// 実装クラスは条件分岐を記述、実施し、その結果を返却する機能を提供します
        /// </summary>
        public interface IResultMatcher<TResult> : IApplyer<TResult>
        {
            /// <summary>
            /// 指定のパラメータが渡された時に返却する値を示します
            /// </summary>
            /// <param name="value1">Value1.</param>
            /// <param name="value2">Value2.</param>
            /// <param name="result">Result.</param>
            IResultMatcher<TResult> When(T1 value1, T2 value2, TResult result);

            /// <summary>
            /// 指定のパラメータが渡された時のコールバックを指定します
            /// </summary>
            /// <param name="value1">Value1.</param>
            /// <param name="value2">Value2.</param>
            /// <param name="callback">Callback.</param>
            IResultMatcher<TResult> When(T1 value1, T2 value2, Func<TResult> callback);

            /// <summary>
            /// 指定のパラメータが渡されたときに、その値を使用して実施するコールバックを指定します
            /// </summary>
            /// <param name="value1">Value1.</param>
            /// <param name="value2">Value2.</param>
            /// <param name="callback">Callback.</param>
            IResultMatcher<TResult> When(T1 value1, T2 value2, Func<T1, T2, TResult> callback);

            /// <summary>
            /// パラメータが指定された条件を満たすときの結果を指定します
            /// </summary>
            /// <param name="predicate">Predicate.</param>
            /// <param name="result">Result.</param>
            IResultMatcher<TResult> When(Func<T1, T2, bool> predicate, TResult result);

            /// <summary>
            /// 指定のパラメータが渡された時のコールバックを指定します
            /// </summary>
            /// <param name="predicate">Predicate.</param>
            /// <param name="callback">Callback.</param>
            IResultMatcher<TResult> When(Func<T1, T2, bool> predicate, Func<TResult> callback);

            /// <summary>
            /// パラメータが指定された条件を満たすときに、その値を使用して実施するコールバックを指定します。
            /// </summary>
            /// <param name="predicate">Predicate.</param>
            /// <param name="callback">Callback.</param>
            IResultMatcher<TResult> When(Func<T1, T2, bool> predicate, Func<T1, T2, TResult> callback);

            /// <summary>
            /// 条件が満たされなかった時の結果を指定します。
            /// </summary>
            /// <param name="defaultResult">Default result.</param>
            IResultMatcher<TResult> Default(TResult defaultResult);

            /// <summary>
            /// 条件が満たされなかった時のコールバックを指定します。
            /// </summary>
            /// <param name="defaultCallback">Default callback.</param>
            IResultMatcher<TResult> Default(Func<TResult> defaultCallback);

            /// <summary>
            /// 条件に一致しなかった時に、その値を使用して実施するコールバックを指定します。
            /// </summary>
            /// <param name="defaultCallback">Default callback.</param>
            IResultMatcher<TResult> Default(Func<T1, T2, TResult> defaultCallback);
        }
    }

    /// <summary>
    /// 3つのパラメータを用いて条件分岐を行う為の基礎クラスです
    /// </summary>
    public static partial class Case<T1, T2, T3> where T1 : IEquatable<T1> where T2 : IEquatable<T2> where T3 : IEquatable<T3>
    {
        /// <summary>
        /// 実装クラスは条件分岐を実施する機能を提供します
        /// </summary>
        public interface IApplyer
        {
            /// <summary>
            /// 事前に記述した条件分岐に従って処理を実施します
            /// </summary>
            /// <param name="candidate1">Candidate1.</param>
            /// <param name="candidate2">Candidate2.</param>
            /// <param name="candidate3">Candidate3.</param>
            void Apply(T1 candidate1, T2 candidate2, T3 candidate3);
        }

        /// <summary>
        /// 実装クラスは条件分岐を実施し、結果を返却する機能を提供します
        /// </summary>
        public interface IApplyer<TResult>
        {
            /// <summary>
            /// 事前に記述した条件分岐に従って処理を実施し、結果を返却します
            /// </summary>
            /// <param name="candidate1">Candidate1.</param>
            /// <param name="candidate2">Candidate2.</param>
            /// <param name="candidate3">Candidate3.</param>
            TResult Apply(T1 candidate1, T2 candidate2, T3 candidate3);
        }

        /// <summary>
        /// 実装クラスは条件分岐を記述、実施する機能を提供します
        /// </summary>
        public interface IMatcher : IApplyer
        {
            /// <summary>
            /// 指定のパラメータが渡された時のコールバックを指定します
            /// </summary>
            /// <param name="value1">Value1.</param>
            /// <param name="value2">Value2.</param>
            /// <param name="value3">Value3.</param>
            /// <param name="callback">Callback.</param>
            IMatcher When(T1 value1, T2 value2, T3 value3, Action callback);

            /// <summary>
            /// 指定のパラメータが渡されたときに、その値を使用して実施するコールバックを指定します
            /// </summary>
            /// <param name="value1">Value1.</param>
            /// <param name="value2">Value2.</param>
            /// <param name="value3">Value3.</param>
            /// <param name="callback">Callback.</param>
            IMatcher When(T1 value1, T2 value2, T3 value3, Action<T1, T2, T3> callback);

            /// <summary>
            /// パラメータが指定された条件を満たすときのコールバックを指定します。
            /// </summary>
            /// <param name="predicate">Predicate.</param>
            /// <param name="callback">Callback.</param>
            IMatcher When(Func<T1, T2, T3, bool> predicate, Action callback);

            /// <summary>
            /// パラメータが指定された条件を満たすときに、その値を使用して実施するコールバックを指定します。
            /// </summary>
            /// <param name="predicate">Predicate.</param>
            /// <param name="callback">Callback.</param>
            IMatcher When(Func<T1, T2, T3, bool> predicate, Action<T1, T2, T3> callback);

            /// <summary>
            /// 条件が満たされなかった時のコールバックを指定します。
            /// </summary>
            /// <param name="defaultCallback">Default callback.</param>
            IMatcher Default(Action defaultCallback);

            /// <summary>
            /// 条件に一致しなかった時に、その値を使用して実施するコールバックを指定します。
            /// </summary>
            /// <param name="defaultCallback">Default callback.</param>
            IMatcher Default(Action<T1, T2, T3> defaultCallback);
        }

        /// <summary>
        /// 実装クラスは条件分岐を記述、実施し、その結果を返却する機能を提供します
        /// </summary>
        public interface IResultMatcher<TResult> : IApplyer<TResult>
        {
            /// <summary>
            /// 指定のパラメータが渡された時に返却する値を示します
            /// </summary>
            /// <param name="value1">Value1.</param>
            /// <param name="value2">Value2.</param>
            /// <param name="value3">Value3.</param>
            /// <param name="result">Result.</param>
            IResultMatcher<TResult> When(T1 value1, T2 value2, T3 value3, TResult result);

            /// <summary>
            /// 指定のパラメータが渡された時のコールバックを指定します
            /// </summary>
            /// <param name="value1">Value1.</param>
            /// <param name="value2">Value2.</param>
            /// <param name="value3">Value3.</param>
            /// <param name="callback">Callback.</param>
            IResultMatcher<TResult> When(T1 value1, T2 value2, T3 value3, Func<TResult> callback);

            /// <summary>
            /// 指定のパラメータが渡されたときに、その値を使用して実施するコールバックを指定します
            /// </summary>
            /// <param name="value">Value.</param>
            /// <param name="value2">Value2.</param>
            /// <param name="value3">Value3.</param>
            /// <param name="callback">Callback.</param>
            IResultMatcher<TResult> When(T1 value, T2 value2, T3 value3, Func<T1, T2, T3, TResult> callback);

            /// <summary>
            /// パラメータが指定された条件を満たすときの結果を指定します
            /// </summary>
            /// <param name="predicate">Predicate.</param>
            /// <param name="result">Result.</param>
            IResultMatcher<TResult> When(Func<T1, T2, T3, bool> predicate, TResult result);

            /// <summary>
            /// 指定のパラメータが渡された時のコールバックを指定します
            /// </summary>
            /// <param name="predicate">Predicate.</param>
            /// <param name="calllback">Calllback.</param>
            IResultMatcher<TResult> When(Func<T1, T2, T3, bool> predicate, Func<TResult> calllback);

            /// <summary>
            /// パラメータが指定された条件を満たすときに、その値を使用して実施するコールバックを指定します。
            /// </summary>
            /// <param name="predicate">Predicate.</param>
            /// <param name="callback">Callback.</param>
            IResultMatcher<TResult> When(Func<T1, T2, T3, bool> predicate, Func<T1, T2, T3, TResult> callback);

            /// <summary>
            /// 条件が満たされなかった時の結果を指定します。
            /// </summary>
            /// <param name="defaultResult">Default result.</param>
            IResultMatcher<TResult> Default(TResult defaultResult);

            /// <summary>
            /// 条件が満たされなかった時のコールバックを指定します。
            /// </summary>
            /// <param name="defaultCallback">Default callback.</param>
            IResultMatcher<TResult> Default(Func<TResult> defaultCallback);

            /// <summary>
            /// 条件に一致しなかった時に、その値を使用して実施するコールバックを指定します。
            /// </summary>
            /// <param name="defaultCallback">Default callback.</param>
            IResultMatcher<TResult> Default(Func<T1, T2, T3, TResult> defaultCallback);
        }

    }

    /// <summary>
    /// 4つのパラメータを用いて条件分岐を行う為の基礎クラスです
    /// </summary>
    public static partial class Case<T1, T2, T3, T4>
        where T1 : IEquatable<T1>
        where T2 : IEquatable<T2>
        where T3 : IEquatable<T3>
        where T4 : IEquatable<T4>
    {

        /// <summary>
        /// 実装クラスは条件分岐を実施する機能を提供します
        /// </summary>
        public interface IApplyer
        {
            /// <summary>
            /// 事前に記述した条件分岐に従って処理を実施します
            /// </summary>
            /// <param name="candidate1">Candidate1.</param>
            /// <param name="candidate2">Candidate2.</param>
            /// <param name="candidate3">Candidate3.</param>
            /// <param name="candidate4">Candidate4.</param>
            void Apply(T1 candidate1, T2 candidate2, T3 candidate3, T4 candidate4);
        }

        /// <summary>
        /// 実装クラスは条件分岐を実施し、結果を返却する機能を提供します
        /// </summary>
        public interface IApplyer<TResult>
        {
            /// <summary>
            /// 事前に記述した条件分岐に従って処理を実施し、結果を返却します
            /// </summary>
            /// <param name="candidate1">Candidate1.</param>
            /// <param name="candidate2">Candidate2.</param>
            /// <param name="candidate3">Candidate3.</param>
            /// <param name="candidate4">Candidate4.</param>
            TResult Apply(T1 candidate1, T2 candidate2, T3 candidate3, T4 candidate4);
        }

        /// <summary>
        /// 実装クラスは条件分岐を記述、実施する機能を提供します
        /// </summary>
        public interface IMatcher : IApplyer
        {
            /// <summary>
            /// 指定のパラメータが渡された時のコールバックを指定します
            /// </summary>
            /// <param name="value1">Value1.</param>
            /// <param name="value2">Value2.</param>
            /// <param name="value3">Value3.</param>
            /// <param name="value4">Value4.</param>
            /// <param name="callback">Callback.</param>
            IMatcher When(T1 value1, T2 value2, T3 value3, T4 value4, Action callback);

            /// <summary>
            /// 指定のパラメータが渡されたときに、その値を使用して実施するコールバックを指定します
            /// </summary>
            /// <param name="value1">Value1.</param>
            /// <param name="value2">Value2.</param>
            /// <param name="value3">Value3.</param>
            /// <param name="value4">Value4.</param>
            /// <param name="callback">Callback.</param>
            IMatcher When(T1 value1, T2 value2, T3 value3, T4 value4, Action<T1, T2, T3, T4> callback);

            /// <summary>
            /// パラメータが指定された条件を満たすときのコールバックを指定します。
            /// </summary>
            /// <param name="predicate">Predicate.</param>
            /// <param name="callback">Callback.</param>
            IMatcher When(Func<T1, T2, T3, T4, bool> predicate, Action callback);

            /// <summary>
            /// パラメータが指定された条件を満たすときに、その値を使用して実施するコールバックを指定します。
            /// </summary>
            /// <param name="predicate">Predicate.</param>
            /// <param name="callback">Callback.</param>
            IMatcher When(Func<T1, T2, T3, T4, bool> predicate, Action<T1, T2, T3, T4> callback);

            /// <summary>
            /// 条件が満たされなかった時のコールバックを指定します。
            /// </summary>
            /// <param name="defaultCallback">Default callback.</param>
            IMatcher Default(Action defaultCallback);

            /// <summary>
            /// 条件に一致しなかった時に、その値を使用して実施するコールバックを指定します。
            /// </summary>
            /// <param name="defaultCallback">Default callback.</param>
            IMatcher Default(Action<T1, T2, T3, T4> defaultCallback);
        }

        /// <summary>
        /// 実装クラスは条件分岐を記述、実施し、その結果を返却する機能を提供します
        /// </summary>
        public interface IResultMatcher<TResult> : IApplyer<TResult>
        {
            /// <summary>
            /// 指定のパラメータが渡された時に返却する値を示します
            /// </summary>
            /// <param name="value1">Value1.</param>
            /// <param name="value2">Value2.</param>
            /// <param name="value3">Value3.</param>
            /// <param name="value4">Value4.</param>
            /// <param name="result">Result.</param>
            IResultMatcher<TResult> When(T1 value1, T2 value2, T3 value3, T4 value4, TResult result);

            /// <summary>
            /// 指定のパラメータが渡された時のコールバックを指定します
            /// </summary>
            /// <param name="value1">Value1.</param>
            /// <param name="value2">Value2.</param>
            /// <param name="value3">Value3.</param>
            /// <param name="value4">Value4.</param>
            /// <param name="callback">Callback.</param>
            IResultMatcher<TResult> When(T1 value1, T2 value2, T3 value3, T4 value4, Func<TResult> callback);

            /// <summary>
            /// 指定のパラメータが渡されたときに、その値を使用して実施するコールバックを指定します
            /// </summary>
            /// <param name="value1">Value1.</param>
            /// <param name="value2">Value2.</param>
            /// <param name="value3">Value3.</param>
            /// <param name="value4">Value4.</param>
            /// <param name="callback">Callback.</param>
            IResultMatcher<TResult> When(
                T1 value1,
                T2 value2,
                T3 value3,
                T4 value4,
                Func<T1, T2, T3, T4, TResult> callback);

            /// <summary>
            /// パラメータが指定された条件を満たすときの結果を指定します
            /// </summary>
            /// <param name="predicate">Predicate.</param>
            /// <param name="result">Result.</param>
            IResultMatcher<TResult> When(Func<T1, T2, T3, T4, bool> predicate, TResult result);

            /// <summary>
            /// 指定のパラメータが渡された時のコールバックを指定します
            /// </summary>
            /// <param name="predicate">Predicate.</param>
            /// <param name="callback">Callback.</param>
            IResultMatcher<TResult> When(Func<T1, T2, T3, T4, bool> predicate, Func<TResult> callback);

            /// <summary>
            /// パラメータが指定された条件を満たすときに、その値を使用して実施するコールバックを指定します。
            /// </summary>
            /// <param name="predicate">Predicate.</param>
            /// <param name="callback">Callback.</param>
            IResultMatcher<TResult> When(Func<T1, T2, T3, T4, bool> predicate, Func<T1, T2, T3, T4, TResult> callback);

            /// <summary>
            /// 条件が満たされなかった時の結果を指定します。
            /// </summary>
            /// <param name="defaultResult">Default result.</param>
            IResultMatcher<TResult> Default(TResult defaultResult);

            /// <summary>
            /// 条件が満たされなかった時のコールバックを指定します。
            /// </summary>
            /// <param name="defaultCallback">Default callback.</param>
            IResultMatcher<TResult> Default(Func<TResult> defaultCallback);

            /// <summary>
            /// 条件に一致しなかった時に、その値を使用して実施するコールバックを指定します。
            /// </summary>
            /// <param name="defaultCallback">Default callback.</param>
            IResultMatcher<TResult> Default(Func<T1, T2, T3, T4, TResult> defaultCallback);
        }
    }
}

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
    /// 指定されたパラメータを用いて条件分岐を行うクラスのエントリポイントです
    /// </summary>
    public static class CaseAction
    {
#region Case`1.Action

        /// <summary>
        /// 1つのパラメータについて、指定のパラメータが渡された時のコールバックを指定します
        /// </summary>
        /// <param name="value1">Value1.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        public static Case<T1>.IMatcher When<T1>(T1 value1, Action callback) where T1 : IEquatable<T1>
        {
            return new Case<T1>.ActionMatcher().When(value1, callback);
        }

        /// <summary>
        /// 1つのパラメータについて、指定のパラメータが渡されたときに、その値を使用して実施するコールバックを指定します
        /// </summary>
        /// <param name="value1">Value1.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        public static Case<T1>.IMatcher When<T1>(T1 value1, Action<T1> callback) where T1 : IEquatable<T1>
        {
            return new Case<T1>.ActionMatcher().When(value1, callback);
        }

        /// <summary>
        /// 1つのパラメータについて、パラメータが指定された条件を満たすときのコールバックを指定します。
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        public static Case<T1>.IMatcher When<T1>(Func<T1, bool> predicate, Action callback) where T1 : IEquatable<T1>
        {
            return new Case<T1>.ActionMatcher().When(predicate, callback);
        }

        /// <summary>
        /// 1つのパラメータについて、パラメータが指定された条件を満たすときに、その値を使用して実施するコールバックを指定します。
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        public static Case<T1>.IMatcher When<T1>(Func<T1, bool> predicate, Action<T1> callback) where T1 : IEquatable<T1>
        {
            return new Case<T1>.ActionMatcher().When(predicate, callback);
        }

#endregion

#region Case`2.Action

        /// <summary>
        /// 2つのパラメータについて、指定のパラメータが渡された時のコールバックを指定します
        /// </summary>
        /// <param name="value1">Value1.</param>
        /// <param name="value2">Value2.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        public static Case<T1, T2>.IMatcher When<T1, T2>(T1 value1, T2 value2, Action callback) where T1 : IEquatable<T1> where T2 : IEquatable<T2>
        {
            return new Case<T1, T2>.ActionMatcher().When(value1, value2, callback);
        }

        /// <summary>
        /// 2つのパラメータについて、指定のパラメータが渡されたときに、その値を使用して実施するコールバックを指定します
        /// </summary>
        /// <param name="value1">Value1.</param>
        /// <param name="value2">Value2.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        public static Case<T1, T2>.IMatcher When<T1, T2>(T1 value1, T2 value2, Action<T1, T2> callback) where T1 : IEquatable<T1> where T2 : IEquatable<T2>
        {
            return new Case<T1, T2>.ActionMatcher().When(value1, value2, callback);
        }

        /// <summary>
        /// 2つのパラメータについて、パラメータが指定された条件を満たすときのコールバックを指定します。
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        public static Case<T1, T2>.IMatcher When<T1, T2>(Func<T1, T2, bool> predicate, Action callback) where T1 : IEquatable<T1> where T2 : IEquatable<T2>
        {
            return new Case<T1, T2>.ActionMatcher().When(predicate, callback);
        }

        /// <summary>
        /// 2つのパラメータについて、パラメータが指定された条件を満たすときに、その値を使用して実施するコールバックを指定します。
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        public static Case<T1, T2>.IMatcher When<T1, T2>(Func<T1, T2, bool> predicate, Action<T1, T2> callback) where T1 : IEquatable<T1> where T2 : IEquatable<T2>
        {
            return new Case<T1, T2>.ActionMatcher().When(predicate, callback);
        }

#endregion

#region Case`3.Action

        /// <summary>
        /// 3つのパラメータについて、指定のパラメータが渡された時のコールバックを指定します
        /// </summary>
        /// <param name="value1">Value1.</param>
        /// <param name="value2">Value2.</param>
        /// <param name="value3">Value3.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        public static Case<T1, T2, T3>.IMatcher When<T1, T2, T3>(T1 value1, T2 value2, T3 value3, Action callback)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
            where T3 : IEquatable<T3>
        {
            return new Case<T1, T2, T3>.ActionMatcher().When(value1, value2, value3, callback);
        }

        /// <summary>
        /// 3つのパラメータについて、指定のパラメータが渡されたときに、その値を使用して実施するコールバックを指定します
        /// </summary>
        /// <param name="value1">Value1.</param>
        /// <param name="value2">Value2.</param>
        /// <param name="value3">Value3.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        public static Case<T1, T2, T3>.IMatcher When<T1, T2, T3>(
            T1 value1,
            T2 value2,
            T3 value3,
            Action<T1, T2, T3> callback)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
            where T3 : IEquatable<T3>
        {
            return new Case<T1, T2, T3>.ActionMatcher().When(value1, value2, value3, callback);
        }

        /// <summary>
        /// 3つのパラメータについて、パラメータが指定された条件を満たすときのコールバックを指定します。
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        public static Case<T1, T2, T3>.IMatcher When<T1, T2, T3>(Func<T1, T2, T3, bool> predicate, Action callback)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
            where T3 : IEquatable<T3>
        {
            return new Case<T1, T2, T3>.ActionMatcher().When(predicate, callback);
        }

        /// <summary>
        /// 3つのパラメータについて、パラメータが指定された条件を満たすときに、その値を使用して実施するコールバックを指定します。
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        public static Case<T1, T2, T3>.IMatcher When<T1, T2, T3>(
            Func<T1, T2, T3, bool> predicate,
            Action<T1, T2, T3> callback)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
            where T3 : IEquatable<T3>
        {
            return new Case<T1, T2, T3>.ActionMatcher().When(predicate, callback);
        }

#endregion

#region Case`4.Action

        /// <summary>
        /// 4つのパラメータについて、指定のパラメータが渡された時のコールバックを指定します
        /// </summary>
        /// <param name="value1">Value1.</param>
        /// <param name="value2">Value2.</param>
        /// <param name="value3">Value3.</param>
        /// <param name="value4">Value4.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        /// <typeparam name="T4">The 4th type parameter.</typeparam>
        public static Case<T1, T2, T3, T4>.IMatcher When<T1, T2, T3, T4>(
            T1 value1,
            T2 value2,
            T3 value3,
            T4 value4,
            Action callback)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
            where T3 : IEquatable<T3>
            where T4 : IEquatable<T4>
        {
            return new Case<T1, T2, T3, T4>.ActionMatcher().When(value1, value2, value3, value4, callback);
        }

        /// <summary>
        /// 4つのパラメータについて、指定のパラメータが渡されたときに、その値を使用して実施するコールバックを指定します
        /// </summary>
        /// <param name="value1">Value1.</param>
        /// <param name="value2">Value2.</param>
        /// <param name="value3">Value3.</param>
        /// <param name="value4">Value4.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        /// <typeparam name="T4">The 4th type parameter.</typeparam>
        public static Case<T1, T2, T3, T4>.IMatcher When<T1, T2, T3, T4>(
            T1 value1,
            T2 value2,
            T3 value3,
            T4 value4,
            Action<T1, T2, T3, T4> callback)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
            where T3 : IEquatable<T3>
            where T4 : IEquatable<T4>
        {
            return new Case<T1, T2, T3, T4>.ActionMatcher().When(value1, value2, value3, value4, callback);
        }

        /// <summary>
        /// 4つのパラメータについて、パラメータが指定された条件を満たすときのコールバックを指定します。
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        /// <typeparam name="T4">The 4th type parameter.</typeparam>
        public static Case<T1, T2, T3, T4>.IMatcher When<T1, T2, T3, T4>(
            Func<T1, T2, T3, T4, bool> predicate,
            Action callback)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
            where T3 : IEquatable<T3>
            where T4 : IEquatable<T4>
        {
            return new Case<T1, T2, T3, T4>.ActionMatcher().When(predicate, callback);
        }

        /// <summary>
        /// 4つのパラメータについて、パラメータが指定された条件を満たすときに、その値を使用して実施するコールバックを指定します。
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        /// <typeparam name="T4">The 4th type parameter.</typeparam>
        public static Case<T1, T2, T3, T4>.IMatcher When<T1, T2, T3, T4>(
            Func<T1, T2, T3, T4, bool> predicate,
            Action<T1, T2, T3, T4> callback)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
            where T3 : IEquatable<T3>
            where T4 : IEquatable<T4>
        {
            return new Case<T1, T2, T3, T4>.ActionMatcher().When(predicate, callback);
        }

#endregion
    }

    /// <summary>
    /// 指定されたパラメータを用いて条件分岐を行い、結果を返却するクラスのエントリポイントです
    /// </summary>
    public static class CaseReturns<TResult>
    {
#region Case`1.Returns

        /// <summary>
        /// 1つのパラメータについて、指定のパラメータが渡された時に返却する値を示します
        /// </summary>
        /// <param name="value1">Value1.</param>
        /// <param name="result">Result.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        public static Case<T1>.IResultMatcher<TResult> When<T1>(T1 value1, TResult result) where T1 : IEquatable<T1>
        {
            return new Case<T1>.ResultMatcher<TResult>().When(value1, result);
        }

        /// <summary>
        /// 1つのパラメータについて、指定のパラメータが渡された時のコールバックを指定します
        /// </summary>
        /// <param name="value1">Value1.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        public static Case<T1>.IResultMatcher<TResult> When<T1>(T1 value1, Func<TResult> callback) where T1 : IEquatable<T1>
        {
            return new Case<T1>.ResultMatcher<TResult>().When(value1, callback);
        }

        /// <summary>
        /// 1つのパラメータについて、指定のパラメータが渡されたときに、その値を使用して実施するコールバックを指定します
        /// </summary>
        /// <param name="value1">Value1.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        public static Case<T1>.IResultMatcher<TResult> When<T1>(T1 value1, Func<T1, TResult> callback) where T1 : IEquatable<T1>
        {
            return new Case<T1>.ResultMatcher<TResult>().When(value1, callback);
        }

        /// <summary>
        /// 1つのパラメータについて、パラメータが指定された条件を満たすときの結果を指定します
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <param name="result">Result.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        public static Case<T1>.IResultMatcher<TResult> When<T1>(Func<T1, bool> predicate, TResult result) where T1 : IEquatable<T1>
        {
            return new Case<T1>.ResultMatcher<TResult>().When(predicate, result);
        }

        /// <summary>
        /// 1つのパラメータについて、指定のパラメータが渡された時のコールバックを指定します
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        public static Case<T1>.IResultMatcher<TResult> When<T1>(Func<T1, bool> predicate, Func<TResult> callback) where T1 : IEquatable<T1>
        {
            return new Case<T1>.ResultMatcher<TResult>().When(predicate, callback);
        }

        /// <summary>
        /// 1つのパラメータについて、パラメータが指定された条件を満たすときに、その値を使用して実施するコールバックを指定します。
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        public static Case<T1>.IResultMatcher<TResult> When<T1>(Func<T1, bool> predicate, Func<T1, TResult> callback) where T1 : IEquatable<T1>
        {
            return new Case<T1>.ResultMatcher<TResult>().When(predicate, callback);
        }

#endregion

#region Case`2.Returns

        /// <summary>
        /// 2つのパラメータについて、指定のパラメータが渡された時に返却する値を示します
        /// </summary>
        /// <param name="value1">Value1.</param>
        /// <param name="value2">Value2.</param>
        /// <param name="result">Result.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        public static Case<T1, T2>.IResultMatcher<TResult> When<T1, T2>(T1 value1, T2 value2, TResult result)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
        {
            return new Case<T1, T2>.ResultMatcher<TResult>().When(value1, value2, result);
        }

        /// <summary>
        /// 2つのパラメータについて、指定のパラメータが渡された時のコールバックを指定します
        /// </summary>
        /// <param name="value1">Value1.</param>
        /// <param name="value2">Value2.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        public static Case<T1, T2>.IResultMatcher<TResult> When<T1, T2>(T1 value1, T2 value2, Func<TResult> callback)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
        {
            return new Case<T1, T2>.ResultMatcher<TResult>().When(value1, value2, callback);
        }

        /// <summary>
        /// 2つのパラメータについて、指定のパラメータが渡されたときに、その値を使用して実施するコールバックを指定します
        /// </summary>
        /// <param name="value1">Value1.</param>
        /// <param name="value2">Value2.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        public static Case<T1, T2>.IResultMatcher<TResult> When<T1, T2>(
            T1 value1,
            T2 value2,
            Func<T1, T2, TResult> callback)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
        {
            return new Case<T1, T2>.ResultMatcher<TResult>().When(value1, value2, callback);
        }

        /// <summary>
        /// 2つのパラメータについて、パラメータが指定された条件を満たすときの結果を指定します
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <param name="result">Result.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        public static Case<T1, T2>.IResultMatcher<TResult> When<T1, T2>(Func<T1, T2, bool> predicate, TResult result)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
        {
            return new Case<T1, T2>.ResultMatcher<TResult>().When(predicate, result);
        }

        /// <summary>
        /// 2つのパラメータについて、指定のパラメータが渡された時のコールバックを指定します
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        public static Case<T1, T2>.IResultMatcher<TResult> When<T1, T2>(
            Func<T1, T2, bool> predicate,
            Func<TResult> callback)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
        {
            return new Case<T1, T2>.ResultMatcher<TResult>().When(predicate, callback);
        }

        /// <summary>
        /// 2つのパラメータについて、パラメータが指定された条件を満たすときに、その値を使用して実施するコールバックを指定します。
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        public static Case<T1, T2>.IResultMatcher<TResult> When<T1, T2>(
            Func<T1, T2, bool> predicate,
            Func<T1, T2, TResult> callback)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
        {
            return new Case<T1, T2>.ResultMatcher<TResult>().When(predicate, callback);
        }

#endregion

#region Case`3.Returns

        /// <summary>
        /// 3つのパラメータについて、指定のパラメータが渡された時に返却する値を示します
        /// </summary>
        /// <param name="value1">Value1.</param>
        /// <param name="value2">Value2.</param>
        /// <param name="value3">Value3.</param>
        /// <param name="result">Result.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        public static Case<T1, T2, T3>.IResultMatcher<TResult> When<T1, T2, T3>(
            T1 value1,
            T2 value2,
            T3 value3,
            TResult result)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
            where T3 : IEquatable<T3>
        {
            return new Case<T1, T2, T3>.ResultMatcher<TResult>().When(value1, value2, value3, result);
        }

        /// <summary>
        /// 3つのパラメータについて、指定のパラメータが渡された時のコールバックを指定します
        /// </summary>
        /// <param name="value1">Value1.</param>
        /// <param name="value2">Value2.</param>
        /// <param name="value3">Value3.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        public static Case<T1, T2, T3>.IResultMatcher<TResult> When<T1, T2, T3>(
            T1 value1,
            T2 value2,
            T3 value3,
            Func<TResult> callback)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
            where T3 : IEquatable<T3>
        {
            return new Case<T1, T2, T3>.ResultMatcher<TResult>().When(value1, value2, value3, callback);
        }

        /// <summary>
        /// 3つのパラメータについて、指定のパラメータが渡されたときに、その値を使用して実施するコールバックを指定します
        /// </summary>
        /// <param name="value1">Value1.</param>
        /// <param name="value2">Value2.</param>
        /// <param name="value3">Value3.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        public static Case<T1, T2, T3>.IResultMatcher<TResult> When<T1, T2, T3>(
            T1 value1,
            T2 value2,
            T3 value3,
            Func<T1, T2, T3, TResult> callback)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
            where T3 : IEquatable<T3>
        {
            return new Case<T1, T2, T3>.ResultMatcher<TResult>().When(value1, value2, value3, callback);
        }

        /// <summary>
        /// 3つのパラメータについて、パラメータが指定された条件を満たすときの結果を指定します
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <param name="result">Result.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        public static Case<T1, T2, T3>.IResultMatcher<TResult> When<T1, T2, T3>(
            Func<T1, T2, T3, bool> predicate,
            TResult result)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
            where T3 : IEquatable<T3>
        {
            return new Case<T1, T2, T3>.ResultMatcher<TResult>().When(predicate, result);
        }

        /// <summary>
        /// 3つのパラメータについて、指定のパラメータが渡された時のコールバックを指定します
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        public static Case<T1, T2, T3>.IResultMatcher<TResult> When<T1, T2, T3>(
            Func<T1, T2, T3, bool> predicate,
            Func<TResult> callback)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
            where T3 : IEquatable<T3>
        {
            return new Case<T1, T2, T3>.ResultMatcher<TResult>().When(predicate, callback);
        }

        /// <summary>
        /// 3つのパラメータについて、パラメータが指定された条件を満たすときに、その値を使用して実施するコールバックを指定します。
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        public static Case<T1, T2, T3>.IResultMatcher<TResult> When<T1, T2, T3>(
            Func<T1, T2, T3, bool> predicate,
            Func<T1, T2, T3, TResult> callback)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
            where T3 : IEquatable<T3>
        {
            return new Case<T1, T2, T3>.ResultMatcher<TResult>().When(predicate, callback);
        }

#endregion

#region Case`4.Returns

        /// <summary>
        /// 4つのパラメータについて、指定のパラメータが渡された時に返却する値を示します
        /// </summary>
        /// <param name="value1">Value1.</param>
        /// <param name="value2">Value2.</param>
        /// <param name="value3">Value3.</param>
        /// <param name="value4">Value4.</param>
        /// <param name="result">Result.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        /// <typeparam name="T4">The 4th type parameter.</typeparam>
        public static Case<T1, T2, T3, T4>.IResultMatcher<TResult> When<T1, T2, T3, T4>(
            T1 value1,
            T2 value2,
            T3 value3,
            T4 value4,
            TResult result)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
            where T3 : IEquatable<T3>
            where T4 : IEquatable<T4>
        {
            return new Case<T1, T2, T3, T4>.ResultMatcher<TResult>().When(value1, value2, value3, value4, result);
        }

        /// <summary>
        /// 4つのパラメータについて、指定のパラメータが渡された時のコールバックを指定します
        /// </summary>
        /// <param name="value1">Value1.</param>
        /// <param name="value2">Value2.</param>
        /// <param name="value3">Value3.</param>
        /// <param name="value4">Value4.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        /// <typeparam name="T4">The 4th type parameter.</typeparam>
        public static Case<T1, T2, T3, T4>.IResultMatcher<TResult> When<T1, T2, T3, T4>(
            T1 value1,
            T2 value2,
            T3 value3,
            T4 value4,
            Func<TResult> callback)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
            where T3 : IEquatable<T3>
            where T4 : IEquatable<T4>
        {
            return new Case<T1, T2, T3, T4>.ResultMatcher<TResult>().When(value1, value2, value3, value4, callback);
        }

        /// <summary>
        /// 4つのパラメータについて、指定のパラメータが渡されたときに、その値を使用して実施するコールバックを指定します
        /// </summary>
        /// <param name="value1">Value1.</param>
        /// <param name="value2">Value2.</param>
        /// <param name="value3">Value3.</param>
        /// <param name="value4">Value4.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        /// <typeparam name="T4">The 4th type parameter.</typeparam>
        public static Case<T1, T2, T3, T4>.IResultMatcher<TResult> When<T1, T2, T3, T4>(
            T1 value1,
            T2 value2,
            T3 value3,
            T4 value4,
            Func<T1, T2, T3, T4, TResult> callback)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
            where T3 : IEquatable<T3>
            where T4 : IEquatable<T4>
        {
            return new Case<T1, T2, T3, T4>.ResultMatcher<TResult>().When(value1, value2, value3, value4, callback);
        }

        /// <summary>
        /// 4つのパラメータについて、パラメータが指定された条件を満たすときの結果を指定します
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <param name="result">Result.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        /// <typeparam name="T4">The 4th type parameter.</typeparam>
        public static Case<T1, T2, T3, T4>.IResultMatcher<TResult> When<T1, T2, T3, T4>(
            Func<T1, T2, T3, T4, bool> predicate,
            TResult result)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
            where T3 : IEquatable<T3>
            where T4 : IEquatable<T4>
        {
            return new Case<T1, T2, T3, T4>.ResultMatcher<TResult>().When(predicate, result);
        }

        /// <summary>
        /// 4つのパラメータについて、指定のパラメータが渡された時のコールバックを指定します
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        /// <typeparam name="T4">The 4th type parameter.</typeparam>
        public static Case<T1, T2, T3, T4>.IResultMatcher<TResult> When<T1, T2, T3, T4>(
            Func<T1, T2, T3, T4, bool> predicate,
            Func<TResult> callback)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
            where T3 : IEquatable<T3>
            where T4 : IEquatable<T4>
        {
            return new Case<T1, T2, T3, T4>.ResultMatcher<TResult>().When(predicate, callback);
        }

        /// <summary>
        /// 4つのパラメータについて、パラメータが指定された条件を満たすときに、その値を使用して実施するコールバックを指定します。
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        /// <typeparam name="T4">The 4th type parameter.</typeparam>
        public static Case<T1, T2, T3, T4>.IResultMatcher<TResult> When<T1, T2, T3, T4>(
            Func<T1, T2, T3, T4, bool> predicate,
            Func<T1, T2, T3, T4, TResult> callback)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
            where T3 : IEquatable<T3>
            where T4 : IEquatable<T4>
        {
            return new Case<T1, T2, T3, T4>.ResultMatcher<TResult>().When(predicate, callback);
        }

#endregion

    }

    internal static class Case
    {
        internal class PairBase<TPredicate, TCallback>
        {
            public TPredicate Predicate { get; private set; }

            public TCallback Callback { get; private set; }

            public PairBase(TPredicate predicate, TCallback callback)
            {
                Predicate = predicate;
                Callback = callback;
            }
        }

        internal static void ValidateWhenArguments(object predicate, object callback)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }
        }

        static Case()
        {
        }
    }

}


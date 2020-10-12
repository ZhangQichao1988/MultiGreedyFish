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
#pragma warning disable 0436
using System;

namespace Jackpot.Extensions
{
    /// <summary>
    /// Tupleの拡張メソッド群です
    /// </summary>
    /// <remarks>
    /// Tupleのプロパティの参照にはtuple.Item1みたいな形をとる必要があって冗長なので、その作業を楽にします
    /// <code>
    /// using UnityEngine;
    /// using Jackpot.Extensions;
    /// 
    /// var tuple = Tuple.Create<string, string, string>("foo", "bar", "baz");
    /// Debug.Log(tuple.Item1); // => "foo";
    /// Debug.Log(tuple.Item2); // => "bar";
    /// Debug.Log(tuple.Item3); // => "baz";
    /// 
    /// // 名前をふりつつ捌く例
    /// var result = string.Empty;
    /// Tuple.Create<string, string, string>("foo", "bar", "baz")
    /// .Unpack((foo, bar, baz) =>
    /// {
    ///     result = foo + bar + baz;
    /// });
    /// Debug.Log(result) => "foobarbaz";
    ///
    /// // 値を返却するようにすればその結果がUnpackで返却される
    /// var totalLength = Tuple.Create<string, string, string>("foo", "bar", "baz")
    /// .Unpack((foo, bar, baz) => foo.Length + bar.Length + baz.Length);
    /// Debug.Log(totalLength); // => 9
    /// </code>
    /// </remarks>
    public static class TupleExtensions
    {
        /// <summary>
        /// Unpack the specified self and action.
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="action">Action.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        public static void Unpack<T1>(this Tuple<T1> self, Action<T1> action)
        {
            action(self.Item1);
        }

        /// <summary>
        /// Unpack the specified self and func.
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="func">Func.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="TResult">The 2nd type parameter.</typeparam>
        public static TResult Unpack<T1, TResult>(this Tuple<T1> self, Func<T1, TResult> func)
        {
            return func(self.Item1);
        }

        /// <summary>
        /// Unpack the specified self and action.
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="action">Action.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        public static void Unpack<T1, T2>(this Tuple<T1, T2> self, Action<T1, T2> action)
        {
            action(self.Item1, self.Item2);
        }

        /// <summary>
        /// Unpack the specified self and func.
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="func">Func.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="TResult">The 3rd type parameter.</typeparam>
        public static TResult Unpack<T1, T2, TResult>(this Tuple<T1, T2> self, Func<T1, T2, TResult> func)
        {
            return func(self.Item1, self.Item2);
        }

        /// <summary>
        /// Unpack the specified self and action.
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="action">Action.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        public static void Unpack<T1, T2, T3>(this Tuple<T1, T2, T3> self, Action<T1, T2, T3> action)
        {
            action(self.Item1, self.Item2, self.Item3);
        }

        /// <summary>
        /// Unpack the specified self and func.
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="func">Func.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        /// <typeparam name="TResult">The 4th type parameter.</typeparam>
        public static TResult Unpack<T1, T2, T3, TResult>(this Tuple<T1, T2, T3> self, Func<T1, T2, T3, TResult> func)
        {
            return func(self.Item1, self.Item2, self.Item3);
        }

        /// <summary>
        /// Unpack the specified self and action.
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="action">Action.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        /// <typeparam name="T4">The 4th type parameter.</typeparam>
        public static void Unpack<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> self, Action<T1, T2, T3, T4> action)
        {
            action(self.Item1, self.Item2, self.Item3, self.Item4);
        }

        /// <summary>
        /// Unpack the specified self and func.
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="func">Func.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        /// <typeparam name="T4">The 4th type parameter.</typeparam>
        /// <typeparam name="TResult">The 5th type parameter.</typeparam>
        public static TResult Unpack<T1, T2, T3, T4, TResult>(this Tuple<T1, T2, T3, T4> self, Func<T1, T2, T3, T4, TResult> func)
        {
            return func(self.Item1, self.Item2, self.Item3, self.Item4);
        }

        /// <summary>
        /// Unpack the specified self and action.
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="action">Action.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        /// <typeparam name="T4">The 4th type parameter.</typeparam>
        /// <typeparam name="T5">The 5th type parameter.</typeparam>
        public static void Unpack<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> self, Action<T1, T2, T3, T4, T5> action)
        {
            action(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5);
        }

        /// <summary>
        /// Unpack the specified self and func.
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="func">Func.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        /// <typeparam name="T4">The 4th type parameter.</typeparam>
        /// <typeparam name="T5">The 5th type parameter.</typeparam>
        /// <typeparam name="TResult">The 6th type parameter.</typeparam>
        public static TResult Unpack<T1, T2, T3, T4, T5, TResult>(this Tuple<T1, T2, T3, T4, T5> self, Func<T1, T2, T3, T4, T5, TResult> func)
        {
            return func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5);
        }

        /// <summary>
        /// Unpack the specified self and action.
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="action">Action.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        /// <typeparam name="T4">The 4th type parameter.</typeparam>
        /// <typeparam name="T5">The 5th type parameter.</typeparam>
        /// <typeparam name="T6">The 6th type parameter.</typeparam>
        public static void Unpack<T1, T2, T3, T4, T5, T6>(this Tuple<T1, T2, T3, T4, T5, T6> self, Action<T1, T2, T3, T4, T5, T6> action)
        {
            action(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6);
        }

        /// <summary>
        /// Unpack the specified self and func.
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="func">Func.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        /// <typeparam name="T4">The 4th type parameter.</typeparam>
        /// <typeparam name="T5">The 5th type parameter.</typeparam>
        /// <typeparam name="T6">The 6th type parameter.</typeparam>
        /// <typeparam name="TResult">The 7th type parameter.</typeparam>
        public static TResult Unpack<T1, T2, T3, T4, T5, T6, TResult>(this Tuple<T1, T2, T3, T4, T5, T6> self, Func<T1, T2, T3, T4, T5, T6, TResult> func)
        {
            return func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6);
        }

        /// <summary>
        /// Unpack the specified self and action.
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="action">Action.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        /// <typeparam name="T4">The 4th type parameter.</typeparam>
        /// <typeparam name="T5">The 5th type parameter.</typeparam>
        /// <typeparam name="T6">The 6th type parameter.</typeparam>
        /// <typeparam name="T7">The 7th type parameter.</typeparam>
        public static void Unpack<T1, T2, T3, T4, T5, T6, T7>(this Tuple<T1, T2, T3, T4, T5, T6, T7> self, Action<T1, T2, T3, T4, T5, T6, T7> action)
        {
            action(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, self.Item7);
        }

        /// <summary>
        /// Unpack the specified self and func.
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="func">Func.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        /// <typeparam name="T4">The 4th type parameter.</typeparam>
        /// <typeparam name="T5">The 5th type parameter.</typeparam>
        /// <typeparam name="T6">The 6th type parameter.</typeparam>
        /// <typeparam name="T7">The 7th type parameter.</typeparam>
        /// <typeparam name="TResult">The 8th type parameter.</typeparam>
        public static TResult Unpack<T1, T2, T3, T4, T5, T6, T7, TResult>(this Tuple<T1, T2, T3, T4, T5, T6, T7> self, Func<T1, T2, T3, T4, T5, T6, T7, TResult> func)
        {
            return func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, self.Item7);
        }

        /// <summary>
        /// Unpack the specified self and action.
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="action">Action.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        /// <typeparam name="T4">The 4th type parameter.</typeparam>
        /// <typeparam name="T5">The 5th type parameter.</typeparam>
        /// <typeparam name="T6">The 6th type parameter.</typeparam>
        /// <typeparam name="T7">The 7th type parameter.</typeparam>
        /// <typeparam name="TRest">The 8th type parameter.</typeparam>
        public static void Unpack<T1, T2, T3, T4, T5, T6, T7, TRest>(this Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> self, Action<T1, T2, T3, T4, T5, T6, T7, TRest> action)
        {
            action(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, self.Item7, self.Rest);
        }

        /// <summary>
        /// Unpack the specified self and func.
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="func">Func.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        /// <typeparam name="T4">The 4th type parameter.</typeparam>
        /// <typeparam name="T5">The 5th type parameter.</typeparam>
        /// <typeparam name="T6">The 6th type parameter.</typeparam>
        /// <typeparam name="T7">The 7th type parameter.</typeparam>
        /// <typeparam name="TRest">The 8th type parameter.</typeparam>
        /// <typeparam name="TResult">The 9th type parameter.</typeparam>
        public static TResult Unpack<T1, T2, T3, T4, T5, T6, T7, TRest, TResult>(this Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> self, Func<T1, T2, T3, T4, T5, T6, T7, TRest, TResult> func)
        {
            return func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, self.Item7, self.Rest);
        }

    }
}

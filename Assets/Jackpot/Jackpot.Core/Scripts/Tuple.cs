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
// Unityがmono3とか.NET4.0ベース以上になったら、このファイルは削除

using System;
using System.Collections;
using System.Text;

namespace Jackpot
{
    /// <summary>
    /// Tuple .
    /// </summary>
    public static class Tuple
    {
        #region Factory

        /// <summary>
        /// Create the specified item1.
        /// </summary>
        /// <param name="item1">Item1.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        public static Tuple<T1> Create<T1>(T1 item1)
        {
            return new Tuple<T1>(item1);
        }

        /// <summary>
        /// Create the specified item1 and item2.
        /// </summary>
        /// <param name="item1">Item1.</param>
        /// <param name="item2">Item2.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
        {
            return new Tuple<T1, T2>(item1, item2);
        }

        /// <summary>
        /// Create the specified item1, item2 and item3.
        /// </summary>
        /// <param name="item1">Item1.</param>
        /// <param name="item2">Item2.</param>
        /// <param name="item3">Item3.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        public static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3)
        {
            return new Tuple<T1, T2, T3>(item1, item2, item3);
        }

        /// <summary>
        /// Create the specified item1, item2, item3 and item4.
        /// </summary>
        /// <param name="item1">Item1.</param>
        /// <param name="item2">Item2.</param>
        /// <param name="item3">Item3.</param>
        /// <param name="item4">Item4.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        /// <typeparam name="T4">The 4th type parameter.</typeparam>
        public static Tuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            return new Tuple<T1, T2, T3, T4>(item1, item2, item3, item4);
        }

        /// <summary>
        /// Create the specified item1, item2, item3, item4 and item5.
        /// </summary>
        /// <param name="item1">Item1.</param>
        /// <param name="item2">Item2.</param>
        /// <param name="item3">Item3.</param>
        /// <param name="item4">Item4.</param>
        /// <param name="item5">Item5.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        /// <typeparam name="T4">The 4th type parameter.</typeparam>
        /// <typeparam name="T5">The 5th type parameter.</typeparam>
        public static Tuple<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
        {
            return new Tuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5);
        }

        /// <summary>
        /// Create the specified item1, item2, item3, item4, item5 and item6.
        /// </summary>
        /// <param name="item1">Item1.</param>
        /// <param name="item2">Item2.</param>
        /// <param name="item3">Item3.</param>
        /// <param name="item4">Item4.</param>
        /// <param name="item5">Item5.</param>
        /// <param name="item6">Item6.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        /// <typeparam name="T4">The 4th type parameter.</typeparam>
        /// <typeparam name="T5">The 5th type parameter.</typeparam>
        /// <typeparam name="T6">The 6th type parameter.</typeparam>
        public static Tuple<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
        {
            return new Tuple<T1, T2, T3, T4, T5, T6>(item1, item2, item3, item4, item5, item6);
        }

        /// <summary>
        /// Create the specified item1, item2, item3, item4, item5, item6 and item7.
        /// </summary>
        /// <param name="item1">Item1.</param>
        /// <param name="item2">Item2.</param>
        /// <param name="item3">Item3.</param>
        /// <param name="item4">Item4.</param>
        /// <param name="item5">Item5.</param>
        /// <param name="item6">Item6.</param>
        /// <param name="item7">Item7.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        /// <typeparam name="T4">The 4th type parameter.</typeparam>
        /// <typeparam name="T5">The 5th type parameter.</typeparam>
        /// <typeparam name="T6">The 6th type parameter.</typeparam>
        /// <typeparam name="T7">The 7th type parameter.</typeparam>
        public static Tuple<T1, T2, T3, T4, T5, T6, T7> Create<T1, T2, T3, T4, T5, T6, T7>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
        {
            return new Tuple<T1, T2, T3, T4, T5, T6, T7>(item1, item2, item3, item4, item5, item6, item7);
        }

        /// <summary>
        /// Create the specified item1, item2, item3, item4, item5, item6, item7 and rest.
        /// </summary>
        /// <param name="item1">Item1.</param>
        /// <param name="item2">Item2.</param>
        /// <param name="item3">Item3.</param>
        /// <param name="item4">Item4.</param>
        /// <param name="item5">Item5.</param>
        /// <param name="item6">Item6.</param>
        /// <param name="item7">Item7.</param>
        /// <param name="rest">Rest.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        /// <typeparam name="T4">The 4th type parameter.</typeparam>
        /// <typeparam name="T5">The 5th type parameter.</typeparam>
        /// <typeparam name="T6">The 6th type parameter.</typeparam>
        /// <typeparam name="T7">The 7th type parameter.</typeparam>
        /// <typeparam name="TRest">The 8th type parameter must be Tuple.</typeparam>
        public static Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> Create<T1, T2, T3, T4, T5, T6, T7, TRest>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, TRest rest)
        {
            return new Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>(item1, item2, item3, item4, item5, item6, item7, rest);
        }

        #endregion

        #region Unitilities

        internal static int CombineHashCodes(int h1, int h2)
        {
            return (((h1 << 5) + h1) ^ h2);
        }

        internal static int CombineHashCodes(int h1, int h2, int h3)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2), h3);
        }

        internal static int CombineHashCodes(int h1, int h2, int h3, int h4)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2), CombineHashCodes(h3, h4));
        }

        internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), h5);
        }

        internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), CombineHashCodes(h5, h6));
        }

        internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6, int h7)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), CombineHashCodes(h5, h6, h7));
        }

        internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6, int h7, int h8)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), CombineHashCodes(h5, h6, h7, h8));
        }

        #endregion
    }

}

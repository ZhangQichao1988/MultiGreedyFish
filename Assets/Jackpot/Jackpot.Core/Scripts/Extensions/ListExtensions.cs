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

namespace Jackpot.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// Determines if is empty the specified self.
        /// </summary>
        /// <returns><c>true</c> if is empty the specified self; otherwise, <c>false</c>.</returns>
        /// <param name="self">Self.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static bool IsEmpty<T>(this List<T> self)
        {
            return self.Count == 0;
        }

        /// <summary>
        /// Counts by matcher.
        /// </summary>
        /// <returns>The by.</returns>
        /// <param name="self">Self.</param>
        /// <param name="match">Match.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static int CountBy<T>(this List<T> self, Predicate<T> match)
        {
            var result = 0;
            for (var i = 0; i < self.Count; i++)
            {
                var item = self[i];
                if (match(item))
                {
                    result++;
                }
            }
            return result;
        }

        /// <summary>
        /// Finds the index of the all.
        /// </summary>
        /// <returns>The all index.</returns>
        /// <param name="self">Self.</param>
        /// <param name="match">Match.</param>
        public static List<int> FindAllIndex<T>(this List<T> self, Predicate<T> match)
        {
            var results = new List<int>();
            for (var i = 0; i < self.Count; i++)
            {
                var item = self[i];
                if (match(item))
                {
                    results.Add(i);
                }
            }
            return results;
        }

        /// <summary>
        /// Finds all index and item.
        /// </summary>
        /// <returns>The all index and item.</returns>
        /// <param name="self">Self.</param>
        /// <param name="match">Match.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static Tuple<List<int>, List<T>> FindAllIndexAndItem<T>(this List<T> self, Predicate<T> match)
        {
            var indexes = new List<int>();
            var items = new List<T>();
            for (var i = 0; i < self.Count; i++)
            {
                var item = self[i];
                if (match(item))
                {
                    indexes.Add(i);
                    items.Add(item);
                }
            }
            return Tuple.Create<List<int>, List<T>>(indexes, items);
        }

        /// <summary>
        /// Adds the unique.
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="item">Item.</param>
        /// <param name="moreItems">More items.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void AddUnique<T>(this List<T> self, T item, params T[] moreItems)
        {
            Action<T> addUniqueCore = (i) =>
            {
                if (self.Contains(i))
                {
                    return;
                }
                self.Add(i);
            };
            addUniqueCore(item);
            if (moreItems != null && moreItems.Length > 0)
            {
                for (var i = 0; i < moreItems.Length; i++)
                {
                    addUniqueCore(moreItems[i]);
                }
            }
        }

        /// <summary>
        /// Contains the specified self and predicate.
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="predicate">Predicate.</param>
        public static bool Contains<T>(this List<T> self, Predicate<T> predicate)
        {
            for (var i = 0; i < self.Count; i++)
            {
                if (predicate(self[i]))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Filter the specified self and predicate.
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="predicate">Predicate.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static List<T> Filter<T>(this List<T> self, Predicate<T> predicate)
        {
            var results = new List<T>();
            for (var i = 0; i < self.Count; i++)
            {
                var item = self[i];
                if (predicate(item))
                {
                    results.Add(item);
                }
            }
            return results;
        }

        /// <summary>
        /// Reject the specified self and predicate.
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="predicate">Predicate.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static List<T> Reject<T>(this List<T> self, Predicate<T> predicate)
        {
            var results = new List<T>();
            for (var i = 0; i < self.Count; i++)
            {
                var item = self[i];
                if (!predicate(item))
                {
                    results.Add(item);
                }
            }
            return results;
        }

        /// <summary>
        /// Rejects the null.
        /// </summary>
        /// <returns>The null.</returns>
        /// <param name="self">Self.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static List<T> RejectNull<T>(this List<T> self)
        {
            return self.Reject(item => item == null);
        }

        /// <summary>
        /// Map the specified self and callback.
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        /// <typeparam name="U">The 2nd type parameter.</typeparam>
        public static List<U> Map<T, U>(this List<T> self, Func<T, U> callback)
        {
            var results = new List<U>();
            for (var i = 0; i < self.Count; i++)
            {
                var item = self[i];
                results.Add(callback(item));
            }
            return results;
        }

        /// <summary>
        /// Map the specified self and callback.
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        /// <typeparam name="U">The 2nd type parameter.</typeparam>
        public static List<U> Map<T, U>(this List<T> self, Func<int, T, U> callback)
        {
            var results = new List<U>();
            for (var i = 0; i < self.Count; i++)
            {
                var item = self[i];
                results.Add(callback(i, item));
            }
            return results;
        }

        /// <summary>
        /// Flats the map.
        /// </summary>
        /// <returns>The map.</returns>
        /// <param name="">.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        /// <typeparam name="U">The 2nd type parameter.</typeparam>
        public static List<U> FlatMap<T, U>(this List<T> self, Func<T, IEnumerable<U>> callback)
        {
            var results = new List<U>();
            for (var i = 0; i < self.Count; i++)
            {
                var item = self[i];
                results.AddRange(callback(item));
            }
            return results;
        }

        /// <summary>
        /// Flats the map.
        /// </summary>
        /// <returns>The map.</returns>
        /// <param name="self">Self.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        /// <typeparam name="U">The 2nd type parameter.</typeparam>
        public static List<U> FlatMap<T, U>(this List<T> self, Func<int, T, IEnumerable<U>> callback)
        {
            var results = new List<U>();
            for (var i = 0; i < self.Count; i++)
            {
                var item = self[i];
                results.AddRange(callback(i, item));
            }
            return results;
        }
    }
}

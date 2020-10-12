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
    static partial class Case<T1>
    {
        /// <summary>
        /// <see cref="Jackpot.Case{T1}.IResultMatcher{TResult}"/>の実装クラスです
        /// </summary>
        public class ResultMatcher<TResult> : IResultMatcher<TResult>
        {
            class Pair : Case.PairBase<Func<T1, bool>, Func<T1, TResult>>
            {
                public Pair(Func<T1, bool> predicate, Func<T1, TResult> callback) : base(predicate, callback)
                {
                }
            }

            readonly List<Pair> pairs;
            readonly Func<T1, TResult> defaultCallback;

            public IResultMatcher<TResult> When(T1 value, TResult result)
            {
                return When(a => object.Equals(a, value), () => result);
            }

            public IResultMatcher<TResult> When(T1 value, Func<TResult> callback)
            {
                return When(a => object.Equals(a, value), a => callback());
            }

            public IResultMatcher<TResult> When(T1 value, Func<T1, TResult> callback)
            {
                return When(a => object.Equals(a, value), callback);
            }

            public IResultMatcher<TResult> When(Func<T1, bool> predicate, TResult result)
            {
                return When(predicate, a => result);
            }

            public IResultMatcher<TResult> When(Func<T1, bool> predicate, Func<TResult> callback)
            {
                return When(predicate, a => callback());
            }

            public IResultMatcher<TResult> When(Func<T1, bool> predicate, Func<T1, TResult> callback)
            {
                Case.ValidateWhenArguments(predicate, callback);
                var copied = new List<Pair>(pairs);
                copied.Add(new Pair(predicate, callback));
                return new ResultMatcher<TResult>(copied, defaultCallback);
            }

            public IResultMatcher<TResult> Default(TResult defaultValue)
            {
                return Default(a => defaultValue);
            }

            public IResultMatcher<TResult> Default(Func<TResult> defaultCallback)
            {
                return Default(a => defaultCallback());
            }

            public IResultMatcher<TResult> Default(Func<T1, TResult> defaultCallback)
            {
                return new ResultMatcher<TResult>(pairs, defaultCallback);
            }

            public TResult Apply(T1 candidate1)
            {
                foreach (var pair in pairs)
                {
                    if (pair.Predicate(candidate1))
                    {
                        return pair.Callback(candidate1);
                    }
                }
                if (defaultCallback != null)
                {
                    return defaultCallback(candidate1);
                }
                return default(TResult);
            }

            internal ResultMatcher() : this(new List<Pair>(), null)
            {
            }

            ResultMatcher(List<Pair> pairs, Func<T1, TResult> defaultCallback)
            {
                this.pairs = pairs;
                this.defaultCallback = defaultCallback;
            }
        }
    }

    static partial class Case<T1, T2>
    {
        /// <summary>
        /// <see cref="Jackpot.Case{T1, T2}.IResultMatcher{TResult}"/>の実装クラスです
        /// </summary>
        public class ResultMatcher<TResult> : IResultMatcher<TResult>
        {
            class Pair : Case.PairBase<Func<T1, T2, bool>, Func<T1, T2, TResult>>
            {
                public Pair(Func<T1, T2, bool> predicate, Func<T1, T2, TResult> callback) : base(predicate, callback)
                {
                }
            }

            readonly List<Pair> pairs;
            readonly Func<T1, T2, TResult> defaultCallback;

            public IResultMatcher<TResult> When(T1 value1, T2 value2, TResult result)
            {
                return When((a, b) => object.Equals(a, value1) && object.Equals(b, value2), (a, b) => result);
            }

            public IResultMatcher<TResult> When(T1 value1, T2 value2, Func<TResult> callback)
            {
                return When((a, b) => object.Equals(a, value1) && object.Equals(b, value2), (a, b) => callback());
            }

            public IResultMatcher<TResult> When(T1 value1, T2 value2, Func<T1, T2, TResult> callback)
            {
                return When((a, b) => object.Equals(a, value1) && object.Equals(b, value2), callback);
            }

            public IResultMatcher<TResult> When(Func<T1, T2, bool> predicate, TResult result)
            {
                return When(predicate, (a, b) => result);
            }

            public IResultMatcher<TResult> When(Func<T1, T2, bool> predicate, Func<TResult> callback)
            {
                return When(predicate, (a, b) => callback());
            }

            public IResultMatcher<TResult> When(Func<T1, T2, bool> predicate, Func<T1, T2, TResult> callback)
            {
                Case.ValidateWhenArguments(predicate, callback);
                var copied = new List<Pair>(pairs);
                copied.Add(new Pair(predicate, callback));
                return new ResultMatcher<TResult>(copied, defaultCallback);
            }

            public IResultMatcher<TResult> Default(TResult defaultResult)
            {
                return Default((a, b) => defaultResult);
            }

            public IResultMatcher<TResult> Default(Func<TResult> defaultCallback)
            {
                return Default((a, b) => defaultCallback());
            }

            public IResultMatcher<TResult> Default(Func<T1, T2, TResult> defaultCallback)
            {
                return new ResultMatcher<TResult>(pairs, defaultCallback);
            }

            public TResult Apply(T1 candidate1, T2 candidate2)
            {
                foreach (var pair in pairs)
                {
                    if (pair.Predicate(candidate1, candidate2))
                    {
                        return pair.Callback(candidate1, candidate2);
                    }
                }
                if (defaultCallback != null)
                {
                    return defaultCallback(candidate1, candidate2);
                }
                return default(TResult);
            }

            internal ResultMatcher() : this(new List<Pair>(), null)
            {
            }

            ResultMatcher(List<Pair> pairs, Func<T1, T2, TResult> defaultCallback)
            {
                this.pairs = pairs;
                this.defaultCallback = defaultCallback;
            }
        }
    }

    static partial class Case<T1, T2, T3>
    {
        /// <summary>
        /// <see cref="Jackpot.Case{T1, T2, T3}.IResultMatcher{TResult}"/>の実装クラスです
        /// </summary>
        public class ResultMatcher<TResult> : IResultMatcher<TResult>
        {
            class Pair : Case.PairBase<Func<T1, T2, T3, bool>, Func<T1, T2, T3, TResult>>
            {
                public Pair(Func<T1, T2, T3, bool> predicate, Func<T1, T2, T3, TResult> callback) : base(
                        predicate,
                        callback)
                {
                }
            }

            readonly List<Pair> pairs;
            readonly Func<T1, T2, T3, TResult> defaultCallback;

            public IResultMatcher<TResult> When(T1 value1, T2 value2, T3 value3, TResult result)
            {
                return When(
                    (a, b, c) => object.Equals(a, value1) && object.Equals(b, value2) && object.Equals(c, value3),
                    (a, b, c) => result
                );
            }

            public IResultMatcher<TResult> When(T1 value1, T2 value2, T3 value3, Func<TResult> callback)
            {
                return When(
                    (a, b, c) => object.Equals(a, value1) && object.Equals(b, value2) && object.Equals(c, value3),
                    (a, b, c) => callback()
                );
            }

            public IResultMatcher<TResult> When(T1 value1, T2 value2, T3 value3, Func<T1, T2, T3, TResult> callback)
            {
                return When(
                    (a, b, c) => object.Equals(a, value1) && object.Equals(b, value2) && object.Equals(c, value3),
                    callback
                );
            }

            public IResultMatcher<TResult> When(Func<T1, T2, T3, bool> predicate, TResult result)
            {
                return When(predicate, (a, b, c) => result);
            }

            public IResultMatcher<TResult> When(Func<T1, T2, T3, bool> predicate, Func<TResult> callback)
            {
                return When(predicate, (a, b, c) => callback());
            }

            public IResultMatcher<TResult> When(Func<T1, T2, T3, bool> predicate, Func<T1, T2, T3, TResult> callback)
            {
                Case.ValidateWhenArguments(predicate, callback);
                var copied = new List<Pair>(pairs);
                copied.Add(new Pair(predicate, callback));
                return new ResultMatcher<TResult>(copied, defaultCallback);
            }

            public IResultMatcher<TResult> Default(TResult defaultResult)
            {
                return Default((a, b, c) => defaultResult);
            }

            public IResultMatcher<TResult> Default(Func<TResult> defaultCallback)
            {
                return Default((a, b, c) => defaultCallback());
            }

            public IResultMatcher<TResult> Default(Func<T1, T2, T3, TResult> defaultCallback)
            {
                return new ResultMatcher<TResult>(pairs, defaultCallback);
            }

            public TResult Apply(T1 candidate1, T2 candidate2, T3 candidate3)
            {
                foreach (var pair in pairs)
                {
                    if (pair.Predicate(candidate1, candidate2, candidate3))
                    {
                        return pair.Callback(candidate1, candidate2, candidate3);
                    }
                }
                if (defaultCallback != null)
                {
                    return defaultCallback(candidate1, candidate2, candidate3);
                }
                return default(TResult);
            }

            internal ResultMatcher() : this(new List<Pair>(), null)
            {
            }

            ResultMatcher(List<Pair> pairs, Func<T1, T2, T3, TResult> defaultCallback)
            {
                this.pairs = pairs;
                this.defaultCallback = defaultCallback;
            }
        }
    }

    static partial class Case<T1, T2, T3, T4>
    {
        /// <summary>
        /// <see cref="Jackpot.Case{T1, T2, T3, T4}.IResultMatcher{TResult}"/>の実装クラスです
        /// </summary>
        public class ResultMatcher<TResult> : IResultMatcher<TResult>
        {
            class Pair : Case.PairBase<Func<T1, T2, T3, T4, bool>, Func<T1, T2, T3, T4, TResult>>
            {
                public Pair(Func<T1, T2, T3, T4, bool> predicate, Func<T1, T2, T3, T4, TResult> callback) : base(
                        predicate,
                        callback)
                {
                }
            }

            readonly List<Pair> pairs;
            readonly Func<T1, T2, T3, T4, TResult> defaultCallback;

            public IResultMatcher<TResult> When(T1 value1, T2 value2, T3 value3, T4 value4, TResult result)
            {
                return When(
                    (a, b, c, d) => object.Equals(a, value1)
                    && object.Equals(b, value2)
                    && object.Equals(c, value3)
                    && object.Equals(d, value4),
                    (a, b, c, d) => result
                );
            }

            public IResultMatcher<TResult> When(T1 value1, T2 value2, T3 value3, T4 value4, Func<TResult> callback)
            {
                return When(
                    (a, b, c, d) => object.Equals(a, value1)
                    && object.Equals(b, value2)
                    && object.Equals(c, value3)
                    && object.Equals(d, value4),
                    (a, b, c, d) => callback()
                );
            }

            public IResultMatcher<TResult> When(
                T1 value1,
                T2 value2,
                T3 value3,
                T4 value4,
                Func<T1, T2, T3, T4, TResult> callback)
            {
                return When(
                    (a, b, c, d) => object.Equals(a, value1)
                    && object.Equals(b, value2)
                    && object.Equals(c, value3)
                    && object.Equals(d, value4),
                    callback
                );
            }

            public IResultMatcher<TResult> When(Func<T1, T2, T3, T4, bool> predicate, TResult result)
            {
                return When(predicate, (a, b, c, d) => result);
            }

            public IResultMatcher<TResult> When(Func<T1, T2, T3, T4, bool> predicate, Func<TResult> callback)
            {
                return When(predicate, (a, b, c, d) => callback());
            }

            public IResultMatcher<TResult> When(
                Func<T1, T2, T3, T4, bool> predicate,
                Func<T1, T2, T3, T4, TResult> callback)
            {
                Case.ValidateWhenArguments(predicate, callback);
                var copied = new List<Pair>(pairs);
                copied.Add(new Pair(predicate, callback));
                return new ResultMatcher<TResult>(copied, defaultCallback);
            }

            public IResultMatcher<TResult> Default(TResult defaultResult)
            {
                return Default((a, b, c, d) => defaultResult);
            }

            public IResultMatcher<TResult> Default(Func<TResult> defaultCallback)
            {
                return Default((a, b, c, d) => defaultCallback());
            }

            public IResultMatcher<TResult> Default(Func<T1, T2, T3, T4, TResult> defaultCallback)
            {
                return new ResultMatcher<TResult>(pairs, defaultCallback);
            }

            public TResult Apply(T1 candidate1, T2 candidate2, T3 candidate3, T4 candidate4)
            {
                foreach (var pair in pairs)
                {
                    if (pair.Predicate(candidate1, candidate2, candidate3, candidate4))
                    {
                        return pair.Callback(candidate1, candidate2, candidate3, candidate4);
                    }
                }
                if (defaultCallback != null)
                {
                    return defaultCallback(candidate1, candidate2, candidate3, candidate4);
                }
                return default(TResult);
            }

            internal ResultMatcher() : this(new List<Pair>(), null)
            {
            }

            ResultMatcher(List<Pair> pairs, Func<T1, T2, T3, T4, TResult> defaultCallback)
            {
                this.pairs = pairs;
                this.defaultCallback = defaultCallback;
            }
        }
    }
}

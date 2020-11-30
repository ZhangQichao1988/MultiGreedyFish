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
        /// <see cref="Jackpot.Case{T1}.IMatcher"/>の実装クラスです
        /// </summary>
        public class ActionMatcher : IMatcher
        {
            class Pair : Case.PairBase<Func<T1, bool>, Action<T1>>
            {
                public Pair(Func<T1, bool> predicate, Action<T1> callback) : base(predicate, callback)
                {
                }
            }

            readonly List<Pair> pairs;
            readonly Action<T1> defaultCallback;

            public IMatcher When(T1 value, Action callback)
            {
                return When(a => object.Equals(a, value), a => callback());
            }

            public IMatcher When(T1 value, Action<T1> callback)
            {
                return When(a => object.Equals(a, value), callback);
            }

            public IMatcher When(Func<T1, bool> predicate, Action callback)
            {
                return When(predicate, a => callback());
            }

            public IMatcher When(Func<T1, bool> predicate, Action<T1> callback)
            {
                Case.ValidateWhenArguments(predicate, callback);
                var copied = new List<Pair>(pairs);
                copied.Add(new Pair(predicate, callback));
                return new ActionMatcher(copied, defaultCallback);
            }

            public IMatcher Default(Action defaultCallback)
            {
                return Default(a => defaultCallback());
            }

            public IMatcher Default(Action<T1> defaultCallback)
            {
                return new ActionMatcher(pairs, defaultCallback);
            }

            public void Apply(T1 candidate1)
            {
                foreach (var pair in pairs)
                {
                    if (pair.Predicate(candidate1))
                    {
                        pair.Callback(candidate1);
                        return;
                    }
                }
                if (defaultCallback != null)
                {
                    defaultCallback(candidate1);
                }
            }

            internal ActionMatcher() : this(new List<Pair>(), null)
            {
            }

            ActionMatcher(List<Pair> pairs, Action<T1> defaultCallback)
            {
                this.pairs = pairs;
                this.defaultCallback = defaultCallback;
            }
        }

    }

    static partial class Case<T1, T2>
    {
        /// <summary>
        /// <see cref="Jackpot.Case{T1, T2}.IMatcher"/>の実装クラスです
        /// </summary>
        public class ActionMatcher : IMatcher
        {
            class Pair : Case.PairBase<Func<T1, T2, bool>, Action<T1, T2>>
            {
                public Pair(Func<T1, T2, bool> predicate, Action<T1, T2> callback) : base(predicate, callback)
                {
                }
            }

            readonly List<Pair> pairs;
            readonly Action<T1, T2> defaultCallback;

            public IMatcher When(T1 value1, T2 value2, Action callback)
            {
                return When((a, b) => object.Equals(a, value1) && object.Equals(b, value2), (a, b) => callback());
            }

            public IMatcher When(T1 value1, T2 value2, Action<T1, T2> callback)
            {
                return When((a, b) => object.Equals(a, value1) && object.Equals(b, value2), callback);
            }

            public IMatcher When(Func<T1, T2, bool> predicate, Action callback)
            {
                return When(predicate, (a, b) => callback());
            }

            public IMatcher When(Func<T1, T2, bool> predicate, Action<T1, T2> callback)
            {
                Case.ValidateWhenArguments(predicate, callback);
                var copied = new List<Pair>(pairs);
                copied.Add(new Pair(predicate, callback));
                return new ActionMatcher(copied, defaultCallback);
            }

            public IMatcher Default(Action defaultCallback)
            {
                return Default((a, b) => defaultCallback());
            }

            public IMatcher Default(Action<T1, T2> defaultCallback)
            {
                return new ActionMatcher(pairs, defaultCallback);
            }

            public void Apply(T1 candidate1, T2 candidate2)
            {
                foreach (var pair in pairs)
                {
                    if (pair.Predicate(candidate1, candidate2))
                    {
                        pair.Callback(candidate1, candidate2);
                        return;
                    }
                }
                if (defaultCallback != null)
                {
                    defaultCallback(candidate1, candidate2);
                }
            }

            internal ActionMatcher() : this(new List<Pair>(), null)
            {
            }

            ActionMatcher(List<Pair> pairs, Action<T1, T2> defaultCallback)
            {
                this.pairs = pairs;
                this.defaultCallback = defaultCallback;
            }
        }
    }

    static partial class Case<T1, T2, T3>
    {
        /// <summary>
        /// <see cref="Jackpot.Case{T1, T2, T3}.IMatcher"/>の実装クラスです
        /// </summary>
        public class ActionMatcher : IMatcher
        {
            class Pair : Case.PairBase<Func<T1, T2, T3, bool>, Action<T1, T2, T3>>
            {
                public Pair(Func<T1, T2, T3, bool> predicate, Action<T1, T2, T3> callback) : base(predicate, callback)
                {
                }
            }

            readonly List<Pair> pairs;
            readonly Action<T1, T2, T3> defaultCallback;

            public IMatcher When(T1 value1, T2 value2, T3 value3, Action callback)
            {
                return When(
                    (a, b, c) => object.Equals(a, value1) && object.Equals(b, value2) && object.Equals(c, value3),
                    (a, b, c) => callback()
                );
            }

            public IMatcher When(T1 value1, T2 value2, T3 value3, Action<T1, T2, T3> callback)
            {
                return When(
                    (a, b, c) => object.Equals(a, value1) && object.Equals(b, value2) && object.Equals(c, value3),
                    callback
                );
            }

            public IMatcher When(Func<T1, T2, T3, bool> predicate, Action callback)
            {
                return When(predicate, (a, b, c) => callback());
            }

            public IMatcher When(Func<T1, T2, T3, bool> predicate, Action<T1, T2, T3> callback)
            {
                Case.ValidateWhenArguments(predicate, callback);
                var copied = new List<Pair>(pairs);
                copied.Add(new Pair(predicate, callback));
                return new ActionMatcher(copied, defaultCallback);
            }

            public IMatcher Default(Action defaultCallback)
            {
                return Default((a, b, c) => defaultCallback());
            }

            public IMatcher Default(Action<T1, T2, T3> defaultCallback)
            {
                return new ActionMatcher(pairs, defaultCallback);
            }

            public void Apply(T1 candidate1, T2 candidate2, T3 candidate3)
            {
                foreach (var pair in pairs)
                {
                    if (pair.Predicate(candidate1, candidate2, candidate3))
                    {
                        pair.Callback(candidate1, candidate2, candidate3);
                        return;
                    }
                }
                if (defaultCallback != null)
                {
                    defaultCallback(candidate1, candidate2, candidate3);
                }
            }

            internal ActionMatcher() : this(new List<Pair>(), null)
            {
            }

            ActionMatcher(List<Pair> pairs, Action<T1, T2, T3> defaultCallback)
            {
                this.pairs = pairs;
                this.defaultCallback = defaultCallback;
            }
        }

    }

    static partial class Case<T1, T2, T3, T4>
    {
        /// <summary>
        /// <see cref="Jackpot.Case{T1, T2, T3, T4}.IMatcher"/>の実装クラスです
        /// </summary>
        public class ActionMatcher : IMatcher
        {
            class Pair : Case.PairBase<Func<T1, T2, T3, T4, bool>, Action<T1, T2, T3, T4>>
            {
                public Pair(Func<T1, T2, T3, T4, bool> predicate, Action<T1, T2, T3, T4> callback) : base(
                        predicate,
                        callback)
                {
                }
            }

            readonly List<Pair> pairs;
            readonly Action<T1, T2, T3, T4> defaultCallback;

            public IMatcher When(T1 value1, T2 value2, T3 value3, T4 value4, Action callback)
            {
                return When(
                    (a, b, c, d) => object.Equals(a, value1)
                    && object.Equals(b, value2)
                    && object.Equals(c, value3)
                    && object.Equals(d, value4),
                    (a, b, c, d) => callback()
                );
            }

            public IMatcher When(T1 value1, T2 value2, T3 value3, T4 value4, Action<T1, T2, T3, T4> callback)
            {
                return When(
                    (a, b, c, d) => object.Equals(a, value1)
                    && object.Equals(b, value2)
                    && object.Equals(c, value3)
                    && object.Equals(d, value4),
                    callback
                );
            }

            public IMatcher When(Func<T1, T2, T3, T4, bool> predicate, Action callback)
            {
                return When(predicate, (a, b, c, d) => callback());
            }

            public IMatcher When(Func<T1, T2, T3, T4, bool> predicate, Action<T1, T2, T3, T4> callback)
            {
                Case.ValidateWhenArguments(predicate, callback);
                var copied = new List<Pair>(pairs);
                copied.Add(new Pair(predicate, callback));
                return new ActionMatcher(copied, defaultCallback);
            }

            public IMatcher Default(Action defaultCallback)
            {
                return Default((a, b, c, d) => defaultCallback());
            }

            public IMatcher Default(Action<T1, T2, T3, T4> defaultCallback)
            {
                return new ActionMatcher(pairs, defaultCallback);
            }

            public void Apply(T1 candidate1, T2 candidate2, T3 candidate3, T4 candidate4)
            {
                foreach (var pair in pairs)
                {
                    if (pair.Predicate(candidate1, candidate2, candidate3, candidate4))
                    {
                        pair.Callback(candidate1, candidate2, candidate3, candidate4);
                        return;
                    }
                }
                if (defaultCallback != null)
                {
                    defaultCallback(candidate1, candidate2, candidate3, candidate4);
                }
            }

            internal ActionMatcher() : this(new List<Pair>(), null)
            {
            }

            ActionMatcher(List<Pair> pairs, Action<T1, T2, T3, T4> defaultCallback)
            {
                this.pairs = pairs;
                this.defaultCallback = defaultCallback;
            }
        }
    }
}

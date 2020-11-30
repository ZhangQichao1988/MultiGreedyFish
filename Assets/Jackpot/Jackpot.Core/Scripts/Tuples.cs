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

#if !UNITY_2017_1_OR_NEWER || (UNITY_2017_1_OR_NEWER && (NET_2_0 || NET_2_0_SUBSET))

using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Collections.Generic
{
    interface IStructuralEquatable
    {
        bool Equals(Object that, IEqualityComparer comparer);

        int GetHashCode(IEqualityComparer comparer);
    }

    interface IStructuralComparable
    {
        int CompareTo(object that, IComparer comparer);
    }
}

namespace System
{
    interface ITuple
    {
        int Size { get; }

        string ToString();
    }
}

namespace Jackpot
{
    [Serializable]
    public class Tuple<T1> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple
    {
        /// <summary>
        /// Gets the item1.
        /// </summary>
        /// <value>The item1.</value>
        public T1 Item1 { get; private set; }

        int ITuple.Size { get { return 1; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.Tuple`1"/> class.
        /// </summary>
        /// <param name="item1">Item1.</param>
        public Tuple(T1 item1)
        {
            Item1 = item1;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Jackpot.Tuple`1"/>.
        /// </summary>
        /// <param name="that">The <see cref="System.Object"/> to compare with the current <see cref="Jackpot.Tuple`1"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="Jackpot.Tuple`1"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object that)
        {
            return ((IStructuralEquatable) this).Equals(that, EqualityComparer<object>.Default);
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="Jackpot.Tuple`1"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return ((IStructuralEquatable) this).GetHashCode(EqualityComparer<object>.Default);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="Jackpot.Tuple`1"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="Jackpot.Tuple`1"/>.</returns>
        public override string ToString()
        {
            return "(" + ((ITuple) this).ToString() + ")";
        }

        bool IStructuralEquatable.Equals(object that, IEqualityComparer comparer)
        {
            if (that == null)
            {
                return false;
            }
            var thatTuple = that as Tuple<T1>;
            if (thatTuple == null)
            {
                return false;
            }
            return comparer.Equals(Item1, thatTuple.Item1);
        }

        int IComparable.CompareTo(object that)
        {
            return ((IStructuralComparable) this).CompareTo(that, Comparer<object>.Default);
        }

        int IStructuralComparable.CompareTo(object that, IComparer comparer)
        {
            if (that == null)
            {
                return 1;
            }
            var thatTuple = that as Tuple<T1>;

            if (thatTuple == null)
            {
                return 1;
            }
            return comparer.Compare(Item1, thatTuple.Item1);
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            return comparer.GetHashCode(Item1);
        }

        string ITuple.ToString()
        {
            return string.Format("{0}", Item1);
        }
    }

    [Serializable]
    /// <summary>
    /// Tuple.
    /// </summary>
    public class Tuple<T1, T2> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple
    {
        /// <summary>
        /// Gets the item1.
        /// </summary>
        /// <value>The item1.</value>
        public T1 Item1 { get; private set; }

        /// <summary>
        /// Gets the item2.
        /// </summary>
        /// <value>The item2.</value>
        public T2 Item2 { get; private set; }

        int ITuple.Size { get { return 2; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.Tuple`2"/> class.
        /// </summary>
        /// <param name="item1">Item1.</param>
        /// <param name="item2">Item2.</param>
        public Tuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Jackpot.Tuple`2"/>.
        /// </summary>
        /// <param name="that">The <see cref="System.Object"/> to compare with the current <see cref="Jackpot.Tuple`2"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="Jackpot.Tuple`2"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object that)
        {
            return ((IStructuralEquatable) this).Equals(that, EqualityComparer<object>.Default);
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="Jackpot.Tuple`2"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return ((IStructuralEquatable) this).GetHashCode(EqualityComparer<object>.Default);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="Jackpot.Tuple`2"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="Jackpot.Tuple`2"/>.</returns>
        public override string ToString()
        {
            return "(" + ((ITuple) this).ToString() + ")";
        }

        bool IStructuralEquatable.Equals(object that, IEqualityComparer comparer)
        {
            if (that == null)
            {
                return false;
            }
            var thatTuple = that as Tuple<T1, T2>;

            if (thatTuple == null)
            {
                return false;
            }
            return comparer.Equals(Item1, thatTuple.Item1)
            && comparer.Equals(Item2, thatTuple.Item2);
        }

        int IComparable.CompareTo(object that)
        {
            return ((IStructuralComparable) this).CompareTo(that, Comparer<object>.Default);
        }

        int IStructuralComparable.CompareTo(object that, IComparer comparer)
        {
            if (that == null)
            {
                return 1;
            }
            var thatTuple = that as Tuple<T1, T2>;
            if (thatTuple == null)
            {
                return 1;
            }
            var c = 0;
            c = comparer.Compare(Item1, thatTuple.Item1);
            if (c != 0)
            {
                return c;
            }
            return comparer.Compare(Item2, thatTuple.Item2);
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            return Tuple.CombineHashCodes(
                comparer.GetHashCode(Item1),
                comparer.GetHashCode(Item2)
            );
        }

        string ITuple.ToString()
        {
            return string.Format("{0}, {1}", Item1, Item2);
        }
    }

    [Serializable]
    public class Tuple<T1, T2, T3> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple
    {
        /// <summary>
        /// Gets the item1.
        /// </summary>
        /// <value>The item1.</value>
        public T1 Item1 { get; private set; }

        /// <summary>
        /// Gets the item2.
        /// </summary>
        /// <value>The item2.</value>
        public T2 Item2 { get; private set; }

        /// <summary>
        /// Gets the item3.
        /// </summary>
        /// <value>The item3.</value>
        public T3 Item3 { get; private set; }

        int ITuple.Size { get { return 3; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.Tuple`3"/> class.
        /// </summary>
        /// <param name="item1">Item1.</param>
        /// <param name="item2">Item2.</param>
        /// <param name="item3">Item3.</param>
        public Tuple(T1 item1, T2 item2, T3 item3)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Jackpot.Tuple`3"/>.
        /// </summary>
        /// <param name="that">The <see cref="System.Object"/> to compare with the current <see cref="Jackpot.Tuple`3"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="Jackpot.Tuple`3"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object that)
        {
            return ((IStructuralEquatable) this).Equals(that, EqualityComparer<object>.Default);
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="Jackpot.Tuple`3"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return ((IStructuralEquatable) this).GetHashCode(EqualityComparer<object>.Default);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="Jackpot.Tuple`3"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="Jackpot.Tuple`3"/>.</returns>
        public override string ToString()
        {
            return "(" + ((ITuple) this).ToString() + ")";
        }

        bool IStructuralEquatable.Equals(object that, IEqualityComparer comparer)
        {
            if (that == null)
            {
                return false;
            }
            var thatTuple = that as Tuple<T1, T2, T3>;
            if (thatTuple == null)
            {
                return false;
            }
            return comparer.Equals(Item1, thatTuple.Item1)
            && comparer.Equals(Item2, thatTuple.Item2)
            && comparer.Equals(Item3, thatTuple.Item3);
        }

        int IComparable.CompareTo(object that)
        {
            return ((IStructuralComparable) this).CompareTo(that, Comparer<object>.Default);
        }

        int IStructuralComparable.CompareTo(object that, IComparer comparer)
        {
            if (that == null)
            {
                return 1;
            }
            var thatTuple = that as Tuple<T1, T2, T3>;
            if (thatTuple == null)
            {
                return 1;
            }
            var c = 0;
            c = comparer.Compare(Item1, thatTuple.Item1);
            if (c != 0)
            {
                return c;
            }
            c = comparer.Compare(Item2, thatTuple.Item2);
            if (c != 0)
            {
                return c;
            }
            return comparer.Compare(Item3, thatTuple.Item3);
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            return Tuple.CombineHashCodes(
                comparer.GetHashCode(Item1),
                comparer.GetHashCode(Item2),
                comparer.GetHashCode(Item3)
            );
        }

        string ITuple.ToString()
        {
            return string.Format("{0}, {1}, {2}", Item1, Item2, Item3);
        }
    }

    [Serializable]
    public class Tuple<T1, T2, T3, T4> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple
    {
        /// <summary>
        /// Gets the item1.
        /// </summary>
        /// <value>The item1.</value>
        public T1 Item1 { get; private set; }

        /// <summary>
        /// Gets the item2.
        /// </summary>
        /// <value>The item2.</value>
        public T2 Item2 { get; private set; }

        /// <summary>
        /// Gets the item3.
        /// </summary>
        /// <value>The item3.</value>
        public T3 Item3 { get; private set; }

        /// <summary>
        /// Gets the item4.
        /// </summary>
        /// <value>The item4.</value>
        public T4 Item4 { get; private set; }

        int ITuple.Size { get { return 4; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.Tuple`4"/> class.
        /// </summary>
        /// <param name="item1">Item1.</param>
        /// <param name="item2">Item2.</param>
        /// <param name="item3">Item3.</param>
        /// <param name="item4">Item4.</param>
        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Jackpot.Tuple`4"/>.
        /// </summary>
        /// <param name="that">The <see cref="System.Object"/> to compare with the current <see cref="Jackpot.Tuple`4"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="Jackpot.Tuple`4"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object that)
        {
            return ((IStructuralEquatable) this).Equals(that, EqualityComparer<object>.Default);
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="Jackpot.Tuple`4"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return ((IStructuralEquatable) this).GetHashCode(EqualityComparer<object>.Default);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="Jackpot.Tuple`4"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="Jackpot.Tuple`4"/>.</returns>
        public override string ToString()
        {
            return "(" + ((ITuple) this).ToString() + ")";
        }

        bool IStructuralEquatable.Equals(object that, IEqualityComparer comparer)
        {
            if (that == null)
            {
                return false;
            }
            var thatTuple = that as Tuple<T1, T2, T3, T4>;
            if (thatTuple == null)
            {
                return false;
            }
            return comparer.Equals(Item1, thatTuple.Item1)
            && comparer.Equals(Item2, thatTuple.Item2)
            && comparer.Equals(Item3, thatTuple.Item3)
            && comparer.Equals(Item4, thatTuple.Item4);
        }

        int IComparable.CompareTo(object that)
        {
            return ((IStructuralComparable) this).CompareTo(that, Comparer<object>.Default);
        }

        int IStructuralComparable.CompareTo(object that, IComparer comparer)
        {
            if (that == null)
            {
                return 1;
            }
            var thatTuple = that as Tuple<T1, T2, T3, T4>;
            if (thatTuple == null)
            {
                return 1;
            }
            var c = 0;
            c = comparer.Compare(Item1, thatTuple.Item1);
            if (c != 0)
            {
                return c;
            }
            c = comparer.Compare(Item2, thatTuple.Item2);
            if (c != 0)
            {
                return c;
            }
            c = comparer.Compare(Item3, thatTuple.Item3);
            if (c != 0)
            {
                return c;
            }
            return comparer.Compare(Item4, thatTuple.Item4);
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            return Tuple.CombineHashCodes(
                comparer.GetHashCode(Item1),
                comparer.GetHashCode(Item2),
                comparer.GetHashCode(Item3),
                comparer.GetHashCode(Item4)
            );
        }

        string ITuple.ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}", Item1, Item2, Item3, Item4);
        }
    }

    [Serializable]
    public class Tuple<T1, T2, T3, T4, T5> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple
    {
        /// <summary>
        /// Gets the item1.
        /// </summary>
        /// <value>The item1.</value>
        public T1 Item1 { get; private set; }

        /// <summary>
        /// Gets the item2.
        /// </summary>
        /// <value>The item2.</value>
        public T2 Item2 { get; private set; }

        /// <summary>
        /// Gets the item3.
        /// </summary>
        /// <value>The item3.</value>
        public T3 Item3 { get; private set; }

        /// <summary>
        /// Gets the item4.
        /// </summary>
        /// <value>The item4.</value>
        public T4 Item4 { get; private set; }

        /// <summary>
        /// Gets the item5.
        /// </summary>
        /// <value>The item5.</value>
        public T5 Item5 { get; private set; }

        int ITuple.Size { get { return 5; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.Tuple`5"/> class.
        /// </summary>
        /// <param name="item1">Item1.</param>
        /// <param name="item2">Item2.</param>
        /// <param name="item3">Item3.</param>
        /// <param name="item4">Item4.</param>
        /// <param name="item5">Item5.</param>
        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Jackpot.Tuple`5"/>.
        /// </summary>
        /// <param name="that">The <see cref="System.Object"/> to compare with the current <see cref="Jackpot.Tuple`5"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="Jackpot.Tuple`5"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object that)
        {
            return ((IStructuralEquatable) this).Equals(that, EqualityComparer<object>.Default);
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="Jackpot.Tuple`5"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return ((IStructuralEquatable) this).GetHashCode(EqualityComparer<object>.Default);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="Jackpot.Tuple`5"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="Jackpot.Tuple`5"/>.</returns>
        public override string ToString()
        {
            return "(" + ((ITuple) this).ToString() + ")";
        }

        bool IStructuralEquatable.Equals(object that, IEqualityComparer comparer)
        {
            if (that == null)
            {
                return false;
            }
            var thatTuple = that as Tuple<T1, T2, T3, T4, T5>;
            if (thatTuple == null)
            {
                return false;
            }
            return comparer.Equals(Item1, thatTuple.Item1)
            && comparer.Equals(Item2, thatTuple.Item2)
            && comparer.Equals(Item3, thatTuple.Item3)
            && comparer.Equals(Item4, thatTuple.Item4)
            && comparer.Equals(Item5, thatTuple.Item5);
        }

        int IComparable.CompareTo(object that)
        {
            return ((IStructuralComparable) this).CompareTo(that, Comparer<object>.Default);
        }

        int IStructuralComparable.CompareTo(object that, IComparer comparer)
        {
            if (that == null)
            {
                return 1;
            }
            var thatTuple = that as Tuple<T1, T2, T3, T4, T5>;
            if (thatTuple == null)
            {
                return 1;
            }
            var c = 0;
            c = comparer.Compare(Item1, thatTuple.Item1);
            if (c != 0)
            {
                return c;
            }
            c = comparer.Compare(Item2, thatTuple.Item2);
            if (c != 0)
            {
                return c;
            }
            c = comparer.Compare(Item3, thatTuple.Item3);
            if (c != 0)
            {
                return c;
            }
            c = comparer.Compare(Item4, thatTuple.Item4);
            if (c != 0)
            {
                return c;
            }
            return comparer.Compare(Item5, thatTuple.Item5);
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            return Tuple.CombineHashCodes(
                comparer.GetHashCode(Item1),
                comparer.GetHashCode(Item2),
                comparer.GetHashCode(Item3),
                comparer.GetHashCode(Item4),
                comparer.GetHashCode(Item5)
            );
        }

        string ITuple.ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}, {4}", Item1, Item2, Item3, Item4, Item5);
        }
    }

    [Serializable]
    public class Tuple<T1, T2, T3, T4, T5, T6> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple
    {
        /// <summary>
        /// Gets the item1.
        /// </summary>
        /// <value>The item1.</value>
        public T1 Item1 { get; private set; }

        /// <summary>
        /// Gets the item2.
        /// </summary>
        /// <value>The item2.</value>
        public T2 Item2 { get; private set; }

        /// <summary>
        /// Gets the item3.
        /// </summary>
        /// <value>The item3.</value>
        public T3 Item3 { get; private set; }

        /// <summary>
        /// Gets the item4.
        /// </summary>
        /// <value>The item4.</value>
        public T4 Item4 { get; private set; }

        /// <summary>
        /// Gets the item5.
        /// </summary>
        /// <value>The item5.</value>
        public T5 Item5 { get; private set; }

        /// <summary>
        /// Gets the item6.
        /// </summary>
        /// <value>The item6.</value>
        public T6 Item6 { get; private set; }

        int ITuple.Size { get { return 6; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.Tuple`6"/> class.
        /// </summary>
        /// <param name="item1">Item1.</param>
        /// <param name="item2">Item2.</param>
        /// <param name="item3">Item3.</param>
        /// <param name="item4">Item4.</param>
        /// <param name="item5">Item5.</param>
        /// <param name="item6">Item6.</param>
        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
            Item6 = item6;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Jackpot.Tuple`6"/>.
        /// </summary>
        /// <param name="that">The <see cref="System.Object"/> to compare with the current <see cref="Jackpot.Tuple`6"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="Jackpot.Tuple`6"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object that)
        {
            return ((IStructuralEquatable) this).Equals(that, EqualityComparer<object>.Default);
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="Jackpot.Tuple`6"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return ((IStructuralEquatable) this).GetHashCode(EqualityComparer<object>.Default);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="Jackpot.Tuple`6"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="Jackpot.Tuple`6"/>.</returns>
        public override string ToString()
        {
            return "(" + ((ITuple) this).ToString() + ")";
        }

        bool IStructuralEquatable.Equals(object that, IEqualityComparer comparer)
        {
            if (that == null)
            {
                return false;
            }
            var thatTuple = that as Tuple<T1, T2, T3, T4, T5, T6>;
            if (thatTuple == null)
            {
                return false;
            }
            return comparer.Equals(Item1, thatTuple.Item1)
            && comparer.Equals(Item2, thatTuple.Item2)
            && comparer.Equals(Item3, thatTuple.Item3)
            && comparer.Equals(Item4, thatTuple.Item4)
            && comparer.Equals(Item5, thatTuple.Item5)
            && comparer.Equals(Item6, thatTuple.Item6);
        }

        int IComparable.CompareTo(object that)
        {
            return ((IStructuralComparable) this).CompareTo(that, Comparer<object>.Default);
        }

        int IStructuralComparable.CompareTo(object that, IComparer comparer)
        {
            if (that == null)
            {
                return 1;
            }
            var thatTuple = that as Tuple<T1, T2, T3, T4, T5, T6>;
            if (thatTuple == null)
            {
                return 1;
            }
            var c = 0;
            c = comparer.Compare(Item1, thatTuple.Item1);
            if (c != 0)
            {
                return c;
            }
            c = comparer.Compare(Item2, thatTuple.Item2);
            if (c != 0)
            {
                return c;
            }
            c = comparer.Compare(Item3, thatTuple.Item3);
            if (c != 0)
            {
                return c;
            }
            c = comparer.Compare(Item4, thatTuple.Item4);
            if (c != 0)
            {
                return c;
            }
            c = comparer.Compare(Item5, thatTuple.Item5);
            if (c != 0)
            {
                return c;
            }
            return comparer.Compare(Item6, thatTuple.Item6);
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            return Tuple.CombineHashCodes(
                comparer.GetHashCode(Item1),
                comparer.GetHashCode(Item2),
                comparer.GetHashCode(Item3),
                comparer.GetHashCode(Item4),
                comparer.GetHashCode(Item5),
                comparer.GetHashCode(Item6)
            );
        }

        string ITuple.ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}, {4}, {5}", Item1, Item2, Item3, Item4, Item5, Item6);
        }
    }

    [Serializable]
    public class Tuple<T1, T2, T3, T4, T5, T6, T7> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple
    {
        /// <summary>
        /// Gets the item1.
        /// </summary>
        /// <value>The item1.</value>
        public T1 Item1 { get; private set; }

        /// <summary>
        /// Gets the item2.
        /// </summary>
        /// <value>The item2.</value>
        public T2 Item2 { get; private set; }

        /// <summary>
        /// Gets the item3.
        /// </summary>
        /// <value>The item3.</value>
        public T3 Item3 { get; private set; }

        /// <summary>
        /// Gets the item4.
        /// </summary>
        /// <value>The item4.</value>
        public T4 Item4 { get; private set; }

        /// <summary>
        /// Gets the item5.
        /// </summary>
        /// <value>The item5.</value>
        public T5 Item5 { get; private set; }

        /// <summary>
        /// Gets the item6.
        /// </summary>
        /// <value>The item6.</value>
        public T6 Item6 { get; private set; }

        /// <summary>
        /// Gets the item7.
        /// </summary>
        /// <value>The item7.</value>
        public T7 Item7 { get; private set; }

        int ITuple.Size { get { return 7; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.Tuple`7"/> class.
        /// </summary>
        /// <param name="item1">Item1.</param>
        /// <param name="item2">Item2.</param>
        /// <param name="item3">Item3.</param>
        /// <param name="item4">Item4.</param>
        /// <param name="item5">Item5.</param>
        /// <param name="item6">Item6.</param>
        /// <param name="item7">Item7.</param>
        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
            Item6 = item6;
            Item7 = item7;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Jackpot.Tuple`7"/>.
        /// </summary>
        /// <param name="that">The <see cref="System.Object"/> to compare with the current <see cref="Jackpot.Tuple`7"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="Jackpot.Tuple`7"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object that)
        {
            return ((IStructuralEquatable) this).Equals(that, EqualityComparer<object>.Default);
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="Jackpot.Tuple`7"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return ((IStructuralEquatable) this).GetHashCode(EqualityComparer<object>.Default);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="Jackpot.Tuple`7"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="Jackpot.Tuple`7"/>.</returns>
        public override string ToString()
        {
            return "(" + ((ITuple) this).ToString() + ")";
        }

        bool IStructuralEquatable.Equals(object that, IEqualityComparer comparer)
        {
            if (that == null)
            {
                return false;
            }
            var thatTuple = that as Tuple<T1, T2, T3, T4, T5, T6, T7>;
            if (thatTuple == null)
            {
                return false;
            }
            return comparer.Equals(Item1, thatTuple.Item1)
            && comparer.Equals(Item2, thatTuple.Item2)
            && comparer.Equals(Item3, thatTuple.Item3)
            && comparer.Equals(Item4, thatTuple.Item4)
            && comparer.Equals(Item5, thatTuple.Item5)
            && comparer.Equals(Item6, thatTuple.Item6)
            && comparer.Equals(Item7, thatTuple.Item7);
        }

        int IComparable.CompareTo(object that)
        {
            return ((IStructuralComparable) this).CompareTo(that, Comparer<object>.Default);
        }

        int IStructuralComparable.CompareTo(object that, IComparer comparer)
        {
            if (that == null)
            {
                return 1;
            }
            var thatTuple = that as Tuple<T1, T2, T3, T4, T5, T6, T7>;
            if (thatTuple == null)
            {
                return 1;
            }
            var c = 0;
            c = comparer.Compare(Item1, thatTuple.Item1);
            if (c != 0)
            {
                return c;
            }
            c = comparer.Compare(Item2, thatTuple.Item2);
            if (c != 0)
            {
                return c;
            }
            c = comparer.Compare(Item3, thatTuple.Item3);
            if (c != 0)
            {
                return c;
            }
            c = comparer.Compare(Item4, thatTuple.Item4);
            if (c != 0)
            {
                return c;
            }
            c = comparer.Compare(Item5, thatTuple.Item5);
            if (c != 0)
            {
                return c;
            }
            c = comparer.Compare(Item6, thatTuple.Item6);
            if (c != 0)
            {
                return c;
            }
            return comparer.Compare(Item7, thatTuple.Item7);
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            return Tuple.CombineHashCodes(
                comparer.GetHashCode(Item1),
                comparer.GetHashCode(Item2),
                comparer.GetHashCode(Item3),
                comparer.GetHashCode(Item4),
                comparer.GetHashCode(Item5),
                comparer.GetHashCode(Item6),
                comparer.GetHashCode(Item7)
            );
        }

        string ITuple.ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}", Item1, Item2, Item3, Item4, Item5, Item6, Item7);
        }
    }

    [Serializable]
    public class Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple
    {
        /// <summary>
        /// Gets the item1.
        /// </summary>
        /// <value>The item1.</value>
        public T1 Item1 { get; private set; }

        /// <summary>
        /// Gets the item2.
        /// </summary>
        /// <value>The item2.</value>
        public T2 Item2 { get; private set; }

        /// <summary>
        /// Gets the item3.
        /// </summary>
        /// <value>The item3.</value>
        public T3 Item3 { get; private set; }

        /// <summary>
        /// Gets the item4.
        /// </summary>
        /// <value>The item4.</value>
        public T4 Item4 { get; private set; }

        /// <summary>
        /// Gets the item5.
        /// </summary>
        /// <value>The item5.</value>
        public T5 Item5 { get; private set; }

        /// <summary>
        /// Gets the item6.
        /// </summary>
        /// <value>The item6.</value>
        public T6 Item6 { get; private set; }

        /// <summary>
        /// Gets the item7.
        /// </summary>
        /// <value>The item7.</value>
        public T7 Item7 { get; private set; }

        /// <summary>
        /// Gets the rest.
        /// </summary>
        /// <value>The rest.</value>
        public TRest Rest { get; private set; }

        int ITuple.Size { get { return 7 + ((ITuple) Rest).Size; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.Tuple`8"/> class.
        /// </summary>
        /// <param name="item1">Item1.</param>
        /// <param name="item2">Item2.</param>
        /// <param name="item3">Item3.</param>
        /// <param name="item4">Item4.</param>
        /// <param name="item5">Item5.</param>
        /// <param name="item6">Item6.</param>
        /// <param name="item7">Item7.</param>
        /// <param name="rest">Rest.</param>
        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, TRest rest)
        {
            if (!(rest is ITuple))
            {
                throw new ArgumentException("Tuple`8 last argument not Tuple");
            }
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
            Item6 = item6;
            Item7 = item7;
            Rest = rest;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Jackpot.Tuple`8"/>.
        /// </summary>
        /// <param name="that">The <see cref="System.Object"/> to compare with the current <see cref="Jackpot.Tuple`8"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="Jackpot.Tuple`8"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object that)
        {
            return ((IStructuralEquatable) this).Equals(that, EqualityComparer<object>.Default);
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="Jackpot.Tuple`8"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return ((IStructuralEquatable) this).GetHashCode(EqualityComparer<object>.Default);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="Jackpot.Tuple`8"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="Jackpot.Tuple`8"/>.</returns>
        public override string ToString()
        {
            return "(" + ((ITuple) this).ToString() + ")";
        }

        bool IStructuralEquatable.Equals(object that, IEqualityComparer comparer)
        {
            if (that == null)
            {
                return false;
            }
            var thatTuple = that as Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>;
            if (thatTuple == null)
            {
                return false;
            }
            return comparer.Equals(Item1, thatTuple.Item1)
            && comparer.Equals(Item2, thatTuple.Item2)
            && comparer.Equals(Item3, thatTuple.Item3)
            && comparer.Equals(Item4, thatTuple.Item4)
            && comparer.Equals(Item5, thatTuple.Item5)
            && comparer.Equals(Item6, thatTuple.Item6)
            && comparer.Equals(Item7, thatTuple.Item7)
            && comparer.Equals(Rest, thatTuple.Rest);
        }

        int IComparable.CompareTo(object that)
        {
            return ((IStructuralComparable) this).CompareTo(that, Comparer<object>.Default);
        }

        int IStructuralComparable.CompareTo(object that, IComparer comparer)
        {
            if (that == null)
            {
                return 1;
            }
            var thatTuple = that as Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>;
            if (thatTuple == null)
            {
                return 1;
            }
            var c = 0;
            c = comparer.Compare(Item1, thatTuple.Item1);
            if (c != 0)
            {
                return c;
            }
            c = comparer.Compare(Item2, thatTuple.Item2);
            if (c != 0)
            {
                return c;
            }
            c = comparer.Compare(Item3, thatTuple.Item3);
            if (c != 0)
            {
                return c;
            }
            c = comparer.Compare(Item4, thatTuple.Item4);
            if (c != 0)
            {
                return c;
            }
            c = comparer.Compare(Item5, thatTuple.Item5);
            if (c != 0)
            {
                return c;
            }
            c = comparer.Compare(Item6, thatTuple.Item6);
            if (c != 0)
            {
                return c;
            }
            c = comparer.Compare(Item7, thatTuple.Item7);
            if (c != 0)
            {
                return c;
            }
            return comparer.Compare(Rest, thatTuple.Rest);
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            return Tuple.CombineHashCodes(
                comparer.GetHashCode(Item1),
                comparer.GetHashCode(Item2),
                comparer.GetHashCode(Item3),
                comparer.GetHashCode(Item4),
                comparer.GetHashCode(Item5),
                comparer.GetHashCode(Item6),
                comparer.GetHashCode(Item7),
                ((IStructuralEquatable) Rest).GetHashCode(comparer)
            );
        }

        string ITuple.ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", Item1, Item2, Item3, Item4, Item5, Item6, Item7, ((ITuple) Rest).ToString());
        }
    }
}
#endif

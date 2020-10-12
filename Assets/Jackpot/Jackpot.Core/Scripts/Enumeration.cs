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
using System.Reflection;
using Jackpot.Extensions;

namespace Jackpot
{
    /// <summary>
    /// enumをより構造的に取り扱いたい場合に使用する抽象クラスです
    /// </summary>
    /// <remarks>
    /// C#のenumはとてもシンプルで良いですが、
    /// シンプルが故に、列挙値に紐づくパラメータやロジックは新たに用意せねばなりません。
    /// 加えて、full-aotフレンドリーではない側面があります。
    /// これを解消するのがEnumerationクラスです。
    /// Enumerationクラスを継承し、列挙値を定義することで、enumと同等の機能を持つオブジェクトを作成できます。
    /// Javaのenumほど優秀ではないですが、Javaに近いEnumになります。
    /// (残念ながら論理演算は現在サポートしていません。PullRequest募集中です。)
    /// <code>
    /// using Jackpot;
    /// public class MyEnum : Enumeration
    /// {
    ///     // public static readonlyに列挙値を宣言しましょう(readonlyはなくても動きますが、常識的に考えて)
    ///     // valueとnameはユニークな値を使用する必要があります
    ///     // isDefaultをtrue指定すると、パース失敗時のデフォルト値として利用されます(1つだけ指定しましょう)
    ///     // 指定しない場合はパース失敗時はnullが返却されます
    ///     public static readonly MyEnum Foo = new MyEnum(0, "Foo", true);
    ///     public static readonly MyEnum Bar = new MyEnum(1, "Bar");
    ///     public static readonly MyEnum Baz = new MyEnum(2, "Baz");
    ///
    ///     MyEnum(int value, string name) : this(value, name, false)
    ///     {
    ///     }
    ///
    ///     MyEnum(int value, string name, bool isDefault) : base(value, name, isDefault)
    ///     {
    ///     }
    /// }
    /// 
    /// var foo = Enumeration.FromValue<MyEnum>(0); // MyEnum.Foo
    /// var bar = Enumeration.FromValue<MyEnum>(1); // MyEnum.Bar
    /// var baz = Enumeration.FromValue<MyEnum>(2); // MyEnum.Baz
    ///
    /// // MyEnum.Foo宣言時、isDefaultを指定したので、パース失敗時はMyEnum.Fooが返却されます
    /// // 要するにNullObjectパターンな形にできます
    /// var defaultEnum = Enumeration.FromValue<MyEnum>(3); // MyEnum.Foo
    /// UnityEngine.Debug.Log(defaultEnum.IsDefault); // => True
    ///
    /// // 厳密にやるのが好みな方はEnumeration.*Strictly<TEnum>()を使用すると、パース失敗時は例外を投げます
    /// var except = Enumeration.FromValueStrictly<MyEnum>(4); // throws System.ArgumentsException
    ///
    /// // valueと同様にnameでもMyEnumを取得できます
    /// var bar2 = Enumeration.FromName<MyEnum>("Bar");             // MyEnum.Bar
    /// var defaultEnum2 = Enumeration.FromName<MyEnum>("Obobobo"); // MyEnum.Foo
    /// var except2 = Enumeration.FromNameStrictly<MyEnum>("Uhyo-");// throws System.ArgumentException
    /// 一応intにキャストできます
    /// var value = (int) MyEnum.Bar; // value = 1;
    /// </code>
    /// </remarks>
    public abstract class Enumeration : IComparable
    {
        #region Constants

        protected const string EnumerationsCacheKey = "Jackpot.Enumeration.ValuesCache";
        protected const string DefaultValueCacheKey = "Jackpot.Enumeration.DefaultValueCache";
        const string NameComposite = "(Composite)";

        #endregion

        #region Properties

        /// <summary>
        /// 列挙値を示します
        /// </summary>
        /// <value>The value.</value>
        public int Value { get { return value; } }

        /// <summary>
        /// 表示名を示します
        /// </summary>
        /// <value>The name.</value>
        public string Name { get { return name; } }

        /// <summary>
        /// このオブジェクトがDefaultのEnumerationか否かを示します
        /// </summary>
        /// <remarks>
        /// Enumerationの定義において、複数IsDefault = trueとされると困る(する意味ないと思う)のでしないでください
        /// </remarks>
        /// <value><c>true</c> if this instance is default; otherwise, <c>false</c>.</value>
        public bool IsDefault { get { return isDefault; } }

        #endregion

        #region Fields

        readonly int value;
        readonly string name;
        readonly bool isDefault;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.Enumeration"/> class.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="name">Name.</param>
        protected Enumeration(int value, string name) : this(value, name, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.Enumeration"/> class.
        /// </summary>
        /// <remarks>
        /// Enumerationの定義において、複数isDefault = trueとされると困る(する意味ないと思う)のでしないでください
        /// </remarks>
        /// <param name="value">Value.</param>
        /// <param name="name">Name.</param>
        /// <param name="isDefault">If set to <c>true</c> is default.</param>
        protected Enumeration(int value, string name, bool isDefault)
        {
            this.value = value;
            this.name = name;
            this.isDefault = isDefault;
        }

        #endregion

        #region Collection

        /// <summary>
        /// 指定のEnumerationを使用して新しいListオブジェクトを作成します
        /// </summary>
        /// <param name="enumeration">Enumeration.</param>
        /// <param name="moreEnumerations">More enumerations.</param>
        /// <typeparam name="TEnum">The 1st type parameter.</typeparam>
        public static List<TEnum> Of<TEnum>(TEnum enumeration, params TEnum[] moreEnumerations) where TEnum : Enumeration
        {
            var results = new List<TEnum>();
            results.AddUnique(enumeration, moreEnumerations);
            return results;
        }

        /// <summary>
        /// 指定の型で定義されている全てのEnumerationをListにして返却します
        /// </summary>
        /// <typeparam name="TEnum">The 1st type parameter.</typeparam>
        public static List<TEnum> All<TEnum>() where TEnum : Enumeration
        {
            var cache = Cache(EnumerationsCacheKey);
            var type = typeof(TEnum);
            var cacheKey = type.FullName;
            if (!cache.Contains(cacheKey))
            {
                var enumerations = new List<TEnum>();
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
                foreach (var field in fields)
                {
                    var enumeration = field.GetValue(null) as TEnum;
                    if (enumeration == null)
                    {
                        continue;
                    }
                    enumerations.Add(enumeration);
                }
                cache.Set<List<TEnum>>(cacheKey, enumerations);
            }
            return cache.Get<List<TEnum>>(cacheKey);
        }

        #endregion

        #region Parse Value

        /// <summary>
        /// 列挙値に一意なEnumerationを返却します
        /// </summary>
        /// <remarks>
        /// 列挙値に一意なEnumerationが存在しない場合、IsDefaultがtrueとなっているEnumerationを返却します
        /// IsDefaultがtrueのものがない場合、nullを返却します
        /// </remarks>
        /// <returns>The value.</returns>
        /// <param name="value">Value.</param>
        /// <typeparam name="TEnum">The 1st type parameter.</typeparam>
        public static TEnum FromValue<TEnum>(int value) where TEnum : Enumeration
        {
            return FromValue<TEnum>(value, Default<TEnum>());
        }

        /// <summary>
        /// 列挙値に一意なEnumerationを返却します。存在しない場合に代わりに返却するEnumerationを指定できます
        /// </summary>
        /// <returns>The value.</returns>
        /// <param name="value">Value.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <typeparam name="TEnum">The 1st type parameter.</typeparam>
        public static TEnum FromValue<TEnum>(int value, TEnum defaultValue) where TEnum : Enumeration
        {
            return Parse<TEnum>(enumeration => value == enumeration.Value, defaultValue);
        }

        /// <summary>
        /// 列挙値に一意なEnumerationを返却し、存在しない場合は例外をスローします
        /// </summary>
        /// <returns>The value strictly.</returns>
        /// <param name="value">Value.</param>
        /// <typeparam name="TEnum">The 1st type parameter.</typeparam>
        /// <exception cref="System.ArgumentException">列挙値に一意なEnumerationが存在しない場合にスローされます</exception>
        public static TEnum FromValueStrictly<TEnum>(int value) where TEnum : Enumeration
        {
            // SingleOrDefault
            var result = default(TEnum);
            foreach (var enumeration in All<TEnum>())
            {
                if (value == enumeration.Value)
                {
                    result = enumeration;
                    break;
                }
            }
            if (result == default(TEnum))
            {
                throw new ArgumentException(string.Format("Failed to parse {0}. The value was {1}", typeof(TEnum).FullName, value));
            }
            return result;
        }

        #endregion

        #region Parse Generic Value

        /// <summary>
        /// 列挙値に一意なEnumerationを返却します
        /// </summary>
        /// <remarks>
        /// 列挙値に一意なEnumerationが存在しない場合、IsDefaultがtrueとなっているEnumerationを返却します
        /// IsDefaultがtrueのものがない場合、nullを返却します
        /// </remarks>
        /// <returns>The value.</returns>
        /// <param name="value">Value.</param>
        /// <typeparam name="TEnum">The 1st type parameter.</typeparam>
        /// <typeparam name="TValue">The 2nd type parameter.</typeparam>
        public static TEnum FromValue<TEnum, TValue>(TValue value) where TEnum : Enumeration<TValue> where TValue : IComparable
        {
            return FromValue<TEnum, TValue>(value, Default<TEnum>());
        }

        /// <summary>
        /// 列挙値に一意なEnumerationを返却します。存在しない場合に代わりに返却するEnumerationを指定できます
        /// </summary>
        /// <returns>The value.</returns>
        /// <param name="value">Value.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <typeparam name="TEnum">The 1st type parameter.</typeparam>
        /// <typeparam name="TValue">The 2nd type parameter.</typeparam>
        public static TEnum FromValue<TEnum, TValue>(TValue value, TEnum defaultValue) where TEnum : Enumeration<TValue> where TValue : IComparable
        {
            return FromValue<TEnum, TValue>(value, defaultValue, (a, b) => a != null && a.Equals(b));
        }

        /// <summary>
        /// 列挙値の一致判定する処理を指定しつつ、一意なEnumerationを返却します
        /// </summary>
        /// <returns>The value.</returns>
        /// <param name="value">Value.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <param name="equalityComparer">Equality comparer.</param>
        /// <typeparam name="TEnum">The 1st type parameter.</typeparam>
        /// <typeparam name="TValue">The 2nd type parameter.</typeparam>
        public static TEnum FromValue<TEnum, TValue>(TValue value, TEnum defaultValue, Func<TValue, TValue, bool> equalityComparer) where TEnum : Enumeration<TValue> where TValue : IComparable
        {
            return Parse<TEnum>(enumeration => equalityComparer(value, enumeration.Value), defaultValue);
        }

        /// <summary>
        /// 列挙値に一意なEnumerationを返却し、存在しない場合は例外がスローされます
        /// </summary>
        /// <returns>The value strictly.</returns>
        /// <param name="value">Value.</param>
        /// <typeparam name="TEnum">The 1st type parameter.</typeparam>
        /// <typeparam name="TValue">The 2nd type parameter.</typeparam>
        /// <exception cref="System.ArgumentException">列挙値に一意なEnumerationが存在しない場合にスローされます</exception>
        public static TEnum FromValueStrictly<TEnum, TValue>(TValue value) where TEnum : Enumeration<TValue> where TValue : IComparable
        {
            return FromValueStrictly<TEnum, TValue>(value, (a, b) => a != null && a.Equals(b));
        }

        /// <summary>
        /// 列挙値の一致判定処理を指定しつつ、一意なEnumerationを返却し、存在しない場合は例外をスローします
        /// </summary>
        /// <returns>The value strictly.</returns>
        /// <param name="value">Value.</param>
        /// <param name="equalityComparer">Equality comparer.</param>
        /// <typeparam name="TEnum">The 1st type parameter.</typeparam>
        /// <typeparam name="TValue">The 2nd type parameter.</typeparam>
        /// <exception cref="System.ArgumentException">列挙値に一意なEnumerationが存在しない場合にスローされます</exception>
        public static TEnum FromValueStrictly<TEnum, TValue>(TValue value, Func<TValue, TValue, bool> equalityComparer) where TEnum : Enumeration<TValue> where TValue : IComparable
        {
            // SingleOrDefault
            var result = default(TEnum);
            foreach (var enumeration in All<TEnum>())
            {
                if (equalityComparer(value, enumeration.Value))
                {
                    result = enumeration;
                    break;
                }
            }
            if (result == default(TEnum))
            {
                throw new ArgumentException(string.Format("Failed to parse {0}. The value was {1}", typeof(TEnum).FullName, value));
            }
            return result;
        }

        #endregion

        #region Parse Name

        /// <summary>
        /// 表示名に一意なEnumerationを返却します
        /// </summary>
        /// <remarks>
        /// 表示名に一意なEnumerationが存在しない場合、IsDefaultがtrueとなっているEnumerationを返却します
        /// IsDefaultがtrueのものがない場合、nullを返却します
        /// </remarks>
        /// <returns>The name.</returns>
        /// <param name="name">Name.</param>
        /// <typeparam name="TEnum">The 1st type parameter.</typeparam>
        public static TEnum FromName<TEnum>(string name) where TEnum : Enumeration
        {
            return FromName<TEnum>(name, Default<TEnum>());
        }

        /// <summary>
        /// 表示名に一意なEnumerationを返却します。存在しない場合に代わりに返却するEnumerationを指定できます
        /// </summary>
        /// <returns>The name.</returns>
        /// <param name="name">Name.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <typeparam name="TEnum">The 1st type parameter.</typeparam>
        public static TEnum FromName<TEnum>(string name, TEnum defaultValue) where TEnum : Enumeration
        {
            return Parse<TEnum>(enumeration => name == enumeration.Name, defaultValue);
        }

        /// <summary>
        /// 表示名に一意なEnumerationを返却し、存在しない場合は例外をスローします
        /// </summary>
        /// <returns>The name strictly.</returns>
        /// <param name="name">Name.</param>
        /// <typeparam name="TEnum">The 1st type parameter.</typeparam>
        /// <exception cref="System.ArgumentException">表示名に一意なEnumerationが存在しない場合にスローされます</exception>
        public static TEnum FromNameStrictly<TEnum>(string name) where TEnum : Enumeration
        {
            // SingleOrDefault
            var result = default(TEnum);
            foreach (var enumeration in All<TEnum>())
            {
                if (name == enumeration.Name)
                {
                    result = enumeration;
                    break;
                }
            }
            if (result == default(TEnum))
            {
                throw new ArgumentException(string.Format("Failed to parse {0}. The name was {1}", typeof(TEnum).FullName, name));
            }
            return result;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Enumerationを実装するクラスで任意のParseメソッドを実装する場合に使用します
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <typeparam name="TEnum">The 1st type parameter.</typeparam>
        protected static TEnum Parse<TEnum>(Predicate<TEnum> predicate, TEnum defaultValue) where TEnum : Enumeration
        {
            var result = defaultValue;
            foreach (var enumeration in All<TEnum>())
            {
                if (predicate(enumeration))
                {
                    result = enumeration;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Enumerationを実装するクラスでデフォルトのEnumerationを取得したい場合に使用します
        /// </summary>
        /// <typeparam name="TEnum">The 1st type parameter.</typeparam>
        protected static TEnum Default<TEnum>() where TEnum : Enumeration
        {
            var cache = Cache(DefaultValueCacheKey);
            var type = typeof(TEnum);
            var cacheKey = type.FullName;
            if (!cache.Contains(cacheKey))
            {
                var defaultValue = default(TEnum);
                var enumerations = All<TEnum>();
                foreach (var enumeration in enumerations)
                {
                    if (enumeration.IsDefault)
                    {
                        defaultValue = enumeration;
                        break;
                    }
                }
                cache.Set<TEnum>(cacheKey, defaultValue);
            }
            return cache.Get<TEnum>(cacheKey);
        }

        /// <summary>
        /// Enumerationを実装するクラスでAll()やDefault()のキャッシュ結果を直接参照したい場合に使用します
        /// </summary>
        /// <param name="cacheKey">Cache key.</param>
        protected static IMemory Cache(string cacheKey)
        {
            if (!ApplicationCache.Instance.Contains(cacheKey))
            {
                var memory = new Memory();
                ApplicationCache.Instance.Set<Memory>(cacheKey, memory);
            }
            return ApplicationCache.Instance.Get<Memory>(cacheKey);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 指定のEnumerationに一致するか否かを示します
        /// </summary>
        /// <remarks>
        /// <code>
        /// if (fooEnum.Value == FooEnum.Foo.Value || fooEnum.Value == FooEnum.Bar.Value || fooEnum.Value == FooEnum.Baz.Value)
        /// {
        ///     // do something
        /// }
        /// </code>
        /// 上記コードを以下の様に表現できます
        /// <code>
        /// if (fooEnum.IsAnyOf(FooEnum.Foo, FooEnum.Bar, FooEnum.Baz))
        /// {
        ///     // do something
        /// }
        /// </code>
        /// </remarks>
        /// <returns><c>true</c> if this instance is any of the specified enumeration moreEnumerations; otherwise, <c>false</c>.</returns>
        /// <param name="enumeration">Enumeration.</param>
        /// <param name="moreEnumerations">More enumerations.</param>
        public bool IsAnyOf(Enumeration enumeration, params Enumeration[] moreEnumerations)
        {
            if (enumeration != null)
            {
                if (Equals(enumeration))
                {
                    return true;
                }
            }
            if (moreEnumerations != null && moreEnumerations.Length > 0)
            {
                for (var i = 0; i < moreEnumerations.Length; i++)
                {
                    var moreEnumeration = moreEnumerations[i];
                    if (moreEnumeration == null)
                    {
                        continue;
                    }
                    if (Equals(moreEnumeration))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Jackpot.Enumeration"/>.
        /// </summary>
        /// <param name="that">The <see cref="System.Object"/> to compare with the current <see cref="Jackpot.Enumeration"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="Jackpot.Enumeration"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object that)
        {
            if (ReferenceEquals(this, that))
            {
                return true;
            }
            if (that == null)
            {
                return false;
            }
            var thatEnum = that as Enumeration;
            if (thatEnum == null)
            {
                return false;
            }
            return Value.Equals(thatEnum.Value)
            && GetType().Equals(that.GetType());
        }

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <returns>The to.</returns>
        /// <param name="that">That.</param>
        public virtual int CompareTo(object that)
        {
            if (ReferenceEquals(this, that))
            {
                return 0;
            }
            if (that == null)
            {
                return 1;
            }
            var thatEnum = that as Enumeration;
            if (thatEnum == null)
            {
                return 1;
            }
            return Value.CompareTo(thatEnum.Value);
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="Jackpot.Enumeration"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// 表示名を出力します
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="Jackpot.Enumeration"/>.</returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion

        #region Implicit Operator

        /// <param name="enumeration">Enumeration.</param>
        public static implicit operator int(Enumeration enumeration)
        {
            return enumeration.Value;
        }

        #endregion
    }

    /// <summary>
    /// Genericに列挙値を指定できるEnumerationクラスです
    /// </summary>
    /// <remarks>
    /// 列挙値の型としてint以外を使いたい場合に使用します
    /// </remarks>
    public abstract class Enumeration<TValue> : Enumeration where TValue : IComparable
    {
        #region Properties

        public new TValue Value { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.Enumeration`1"/> class.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="name">Name.</param>
        protected Enumeration(TValue value, string name) : this(value, name, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.Enumeration`1"/> class.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="name">Name.</param>
        /// <param name="isDefault">If set to <c>true</c> is default.</param>
        protected Enumeration(TValue value, string name, bool isDefault) : base(value.GetHashCode(), name, isDefault)
        {
            Value = value;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Jackpot.Enumeration`1"/>.
        /// </summary>
        /// <param name="that">The <see cref="System.Object"/> to compare with the current <see cref="Jackpot.Enumeration`1"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="Jackpot.Enumeration`1"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object that)
        {
            if (ReferenceEquals(this, that))
            {
                return true;
            }
            if (that == null)
            {
                return false;
            }
            var thatEnum = that as Enumeration<TValue>;
            if (thatEnum == null)
            {
                return false;
            }
            return Value.Equals(thatEnum.Value)
            && GetType().Equals(that.GetType());
        }

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <returns>The to.</returns>
        /// <param name="that">That.</param>
        public override int CompareTo(object that)
        {
            if (ReferenceEquals(this, that))
            {
                return 0;
            }
            if (that == null)
            {
                return 1;
            }
            var thatEnum = that as Enumeration<TValue>;
            if (thatEnum == null)
            {
                return 1;
            }
            return Value.CompareTo(thatEnum.Value);
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="Jackpot.Enumeration`1"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return Value == null ? base.GetHashCode() : Value.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="Jackpot.Enumeration`1"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="Jackpot.Enumeration`1"/>.</returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion

    }
}

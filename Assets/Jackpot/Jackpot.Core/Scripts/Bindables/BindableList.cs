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
using System.Collections;
using System.Collections.Generic;
using Jackpot.Extensions;

namespace Jackpot
{
    [Serializable]
    /// <summary>
    /// 要素の変更を監視できるListクラスです
    /// </summary>
    /// <remarks>
    /// BindableListから送出されるイベントの詳細は<see cref="Jackpot.ListChangedEventKind"/>を参考にしてください。
    /// BindableListから送出されるイベントに付属するパラメータは<see cref="Jackpot.ListChangedEventArgs{T}"/>を参考にしてください。
    /// </remarks>
    public class BindableList<T> : IList<T>, IList, ICollection<T>, ICollection, IEnumerable<T>, IEnumerable, ICloneable
    {
        protected enum EditType
        {
            None,
            Update,
            Remove,
            Edit
        }

        #region Constants

        /// <summary>
        /// パフォーマンスの都合上、BindableListの管理する要素のデフォルト変更ポリシーはありません
        /// </summary>
        static readonly Func<T, T, bool> DefaultChangesPolicy = null;

        #endregion

        #region Public Properties

        /// <summary>
        /// 要素数を示します
        /// </summary>
        /// <value>The count.</value>
        public int Count { get { return items.Count; } }

        /// <summary>
        /// 指定のインデックスの要素を取得したり設定したりします
        /// </summary>
        /// <remarks>setterによって値が変更された場合、ListChangedEventKind.Updateな変更通知が送出されます</remarks>
        /// <param name="index">Index.</param>
        public T this[int index]
        {
            get { return items[index]; }
            set
            {
                if (index < 0 || index >= items.Count)
                {
                    throw new ArgumentOutOfRangeException();
                }
                SetItem(index, value);
            }
        }

        /// <summary>
        /// 通知する際のルールを示します<see cref="Jackpot.BindableRules"/>
        /// </summary>
        /// <value>The rule.</value>
        public BindableRules Rule { get; private set; }

        /// <summary>
        /// 通知する際に変更前の値を含めるかを示します
        /// </summary>
        /// <remarks>
        /// 変更前の値は常に必要な情報ではなく、その生成コストの問題から、デフォルトではfalseとなっています。
        /// 通知する際に変更前の値を含めたい場合、コンストラクタ引数から指定します
        /// </remarks>
        /// <value><c>true</c> if this instance is old value enabled; otherwise, <c>false</c>.</value>
        public bool OldValueEnabled { get; private set; }

        /// <summary>
        /// BindableListを編集中か否かを示します
        /// </summary>
        /// <value><c>true</c> if editting; otherwise, <c>false</c>.</value>
        public bool Editting { get { return editType != EditType.None; } }

        #endregion

        #region IList Properties

        object IList.this[int index]
        {
            get { return items[index]; }
            set
            {
                ValidateNotEditting();
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                this[index] = (T) value;
            }
        }

        #endregion

        #region ICollection Properties

        bool ICollection.IsSynchronized { get { return false; } }

        object ICollection.SyncRoot { get { return this; } }

        #endregion

        #region ICollection<T> Properties

        bool ICollection<T>.IsReadOnly { get { return ((ICollection<T>) items).IsReadOnly; } }

        #endregion

        #region IList Properties

        bool IList.IsFixedSize { get { return ((IList) items).IsFixedSize; } }

        bool IList.IsReadOnly { get { return ((IList) items).IsReadOnly; } }

        #endregion

        #region Fields

        List<T> items;
        EventDispatcher<ListChangedEventArgs<T>> dispatcher;
        Func<T, T, bool> changesPolicy;
        EditType editType;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.BindableList`1"/> class.
        /// </summary>
        public BindableList() : this(new List<T>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.BindableList`1"/> class.
        /// </summary>
        /// <param name="rule">Rule.</param>
        public BindableList(BindableRules rule) : this(rule, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.BindableList`1"/> class.
        /// </summary>
        /// <param name="oldValueEnabled">If set to <c>true</c> old value enabled.</param>
        public BindableList(bool oldValueEnabled) : this(BindableRules.NotAllowCallStackChanging, oldValueEnabled)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.BindableList`1"/> class.
        /// </summary>
        /// <param name="rule">Rule.</param>
        /// <param name="oldValueEnabled">If set to <c>true</c> old value enabled.</param>
        public BindableList(BindableRules rule, bool oldValueEnabled) : this(null, rule, oldValueEnabled)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.BindableList`1"/> class.
        /// </summary>
        /// <param name="collection">Collection.</param>
        public BindableList(IEnumerable<T> collection) : this(collection, BindableRules.NotAllowCallStackChanging)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.BindableList`1"/> class.
        /// </summary>
        /// <param name="collection">Collection.</param>
        /// <param name="rule">Rule.</param>
        public BindableList(IEnumerable<T> collection, BindableRules rule) : this(collection, rule, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.BindableList`1"/> class.
        /// </summary>
        /// <param name="collection">Collection.</param>
        /// <param name="oldValueEnabled">If set to <c>true</c> old value enabled.</param>
        public BindableList(IEnumerable<T> collection, bool oldValueEnabled) : this(collection, BindableRules.NotAllowCallStackChanging, oldValueEnabled)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.BindableList`1"/> class.
        /// </summary>
        /// <param name="collection">Collection.</param>
        /// <param name="rule">Rule.</param>
        /// <param name="oldValueEnabled">If set to <c>true</c> old value enabled.</param>
        public BindableList(IEnumerable<T> collection, BindableRules rule, bool oldValueEnabled)
        {
            if (typeof(T) == typeof(ICollection))
            {
                throw new NotSupportedException();
            }
            items = collection == null ? new List<T>() : new List<T>(collection);
            Rule = rule;
            OldValueEnabled = oldValueEnabled;
            dispatcher = new EventDispatcher<ListChangedEventArgs<T>>();
            changesPolicy = DefaultChangesPolicy;
            editType = EditType.None;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.BindableList`1"/> class.
        /// </summary>
        /// <param name="that">That.</param>
        public BindableList(BindableList<T> that) : this(that, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.BindableList`1"/> class.
        /// </summary>
        /// <param name="that">That.</param>
        /// <param name="withTakeOverHandler">If set to <c>true</c> with take over handler.</param>
        public BindableList(BindableList<T> that, bool withTakeOverHandler) : this(that, withTakeOverHandler, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.BindableList`1"/> class.
        /// </summary>
        /// <param name="that">That.</param>
        /// <param name="withTakeOverHandler">If set to <c>true</c> with take over handler.</param>
        /// <param name="withCopyItem">If set to <c>true</c> with copy item.</param>
        public BindableList(BindableList<T> that, bool withTakeOverHandler, bool withCopyItem)
        {
            if (withCopyItem && that.Count > 0 && that[0] is ICloneable)
            {
                items = new List<T>();
                foreach (var thatItem in that.items)
                {
                    items.Add((T) ((ICloneable) thatItem).Clone());
                }
            }
            if (items == null)
            {
                items = new List<T>(that.items);
            }
            this.Rule = that.Rule;
            this.OldValueEnabled = that.OldValueEnabled;
            dispatcher = withTakeOverHandler
                ? new EventDispatcher<ListChangedEventArgs<T>>(that.dispatcher)
                : new EventDispatcher<ListChangedEventArgs<T>>();
            changesPolicy = that.changesPolicy;
            editType = EditType.None;
        }

        #endregion

        #region Public Binding Methods

        /// <summary>
        /// 変更通知を受け取るイベントハンドラを追加します. 重複した場合は再登録されます
        /// </summary>
        /// <remarks>
        /// 追加したイベントハンドラは強い参照で保持されます。
        /// 詳しくは<see cref="EventDispatcher{TEventArgs}.AddHandler"/>を参照してください
        /// </remarks>
        /// <param name="handler">イベントハンドラ</param>
        public void Bind(EventHandler<ListChangedEventArgs<T>> handler)
        {
            dispatcher.AddHandler(handler);
        }

        /// <summary>
        /// 変更通知を受け取るイベントハンドラを追加します. 重複した場合は再登録されます
        /// </summary>
        /// <remarks>
        /// 追加したイベントハンドラは弱い参照で保持されます。第一引数が<c>null</c>の場合は強い参照になります
        /// 詳しくは<see cref="EventDispatcher{TEventArgs}.AddHandler"/>を参照してください
        /// </remarks>
        /// <param name="target">ハンドリングの基準になるオブジェクト</param>
        /// <param name="handler">イベントハンドラ</param>
        public void Bind(object target, EventHandler<ListChangedEventArgs<T>> handler)
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
        public void BindOnce(EventHandler<ListChangedEventArgs<T>> handler)
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
        public void BindOnce(object target, EventHandler<ListChangedEventArgs<T>> handler)
        {
            dispatcher.AddHandler(target, handler, true);
        }

        /// <summary>
        /// 変更通知のイベントハンドラが1つでも登録されているかを示します
        /// </summary>
        /// <returns><c>true</c> if this instance is binding; otherwise, <c>false</c>.</returns>
        public bool IsBinding()
        {
            return !dispatcher.HandlerIsEmpty;
        }

        /// <summary>
        /// 指定のイベントハンドラが登録されているかを示します
        /// </summary>
        /// <returns><c>true</c> if this instance is binding the specified binder; otherwise, <c>false</c>.</returns>
        /// <param name="binder">Binder.</param>
        public bool IsBinding(EventHandler<ListChangedEventArgs<T>> binder)
        {
            return dispatcher.ContainsHandler(binder);
        }

        /// <summary>
        /// 1度だけ変更通知を受け取るイベントハンドラが登録されているかを示します
        /// </summary>
        /// <returns><c>true</c> if this instance is binding once; otherwise, <c>false</c>.</returns>
        public bool IsBindingOnce()
        {
            return !dispatcher.OnceHandlerIsEmpty;
        }

        /// <summary>
        /// 指定のイベントハンドラが一度だけ変更通知を受け取るように設定されているかを示します
        /// </summary>
        /// <returns><c>true</c> if this instance is binding once; otherwise, <c>false</c>.</returns>
        public bool IsBindingOnce(EventHandler<ListChangedEventArgs<T>> binder)
        {
            return dispatcher.ContainsHandler(binder, true);
        }

        /// <summary>
        /// 指定のイベントハンドラを登録解除します
        /// </summary>
        /// <param name="binder">Binder.</param>
        public void Unbind(EventHandler<ListChangedEventArgs<T>> binder)
        {
            dispatcher.RemoveHandler(binder);
        }

        /// <summary>
        /// 全てのイベントハンドラを登録解除します
        /// </summary>
        public void UnBindAll()
        {
            dispatcher.RemoveAllHandlers();
        }

        /// <summary>
        /// どのようなデータの変更があった場合に通知するかを設定します
        /// </summary>
        /// <remarks>
        /// パフォーマンスの都合上、BindableListの管理する要素のデフォルト変更ポリシーはありません。
        /// nullを指定した場合、変更ポリシーはないと判断され、通知可能な状態の時は必ずデータ変更通知が飛びます。
        /// 加えて、要素の型が参照型であるBindableListの変更ポリシーを設定する場合、
        /// 要素の更新時はその値が参照型である事を考慮し、参照の違う新しいObjectを設定、ないし返却する必要があります。
        /// ただし、要素の型がICloneableを実装している場合、CloneしたObjectがコールバックに渡されるので、
        /// ICloneableが実装されていれば、その手間を省略できます。(index accessorではCloneされません)。
        /// </remarks>
        /// <param name="changesPolicy">Changes policy.</param>
        public void SetChangesPolicy(Func<T, T, bool> changesPolicy)
        {
            this.changesPolicy = changesPolicy;
        }

        #endregion

        #region Public List Methods

        /// <summary>
        /// Enumeratorを返却します。
        /// </summary>
        /// <remarks>
        /// Enumeratorを使用して要素を変更しても変更通知がされないので、
        /// 代わりにUpdateAll()を使用してください。
        /// ハック的にデータの変更通知を飛ばさない為にこのメソッドを使用されるのは心外です。
        /// </remarks>
        /// <returns>The enumerator.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        /// <summary>
        /// 要素をArrayにコピーします
        /// </summary>
        /// <param name="array">Array.</param>
        /// <param name="arrayIndex">Array index.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 要素の型の配列として返却します
        /// </summary>
        /// <returns>The array.</returns>
        public T[] ToArray()
        {
            return items.ToArray();
        }

        /// <summary>
        /// 要素数がない(すなわち空である)事を示します
        /// </summary>
        /// <returns><c>true</c> if this instance is empty; otherwise, <c>false</c>.</returns>
        public bool IsEmpty()
        {
            return items.Count <= 0;
        }

        /// <summary>
        /// 指定の条件を満たす要素の数を返却します
        /// </summary>
        /// <returns>The by.</returns>
        /// <param name="predicate">Predicate.</param>
        public int CountBy(Predicate<T> predicate)
        {
            return items.CountBy(predicate);
        }

        /// <Docs>The object to locate in the current collection.</Docs>
        /// <para>Determines whether the current collection contains a specific value.</para>
        /// <summary>
        /// 指定の要素が存在するかを示します
        /// </summary>
        /// <param name="item">Item.</param>
        public bool Contains(T item)
        {
            return items.Contains(item);
        }

        /// <Docs>The object to locate in the current collection.</Docs>
        /// <para>Determines whether the current collection contains a specific value.</para>
        /// <summary>
        /// 指定の条件を満たす要素が存在するかを示します
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        public bool Contains(Predicate<T> predicate)
        {
            return items.Contains(predicate);
        }

        /// <Docs>To be added.</Docs>
        /// <para>Determines the index of a specific item in the current instance.</para>
        /// <summary>
        /// 指定の要素が存在するインデックスを示します
        /// </summary>
        /// <returns>The of.</returns>
        /// <param name="item">Item.</param>
        public int IndexOf(T item)
        {
            return items.IndexOf(item);
        }

        /// <summary>
        /// 指定の要素が存在する最後のインデックスを示します
        /// </summary>
        /// <returns>The index of.</returns>
        /// <param name="item">Item.</param>
        public int LastIndexOf(T item)
        {
            return items.LastIndexOf(item);
        }

        /// <summary>
        /// 指定の条件に一致する先頭の要素を返却します
        /// </summary>
        /// <param name="match">Match.</param>
        public T Find(Predicate<T> match)
        {
            return Find(match, default(T));
        }

        /// <summary>
        /// 指定の条件に一致する先頭の要素を返却します。存在しない場合、defaultValueを返却します
        /// </summary>
        /// <param name="match">Match.</param>
        /// <param name="defaultValue">Default value.</param>
        public T Find(Predicate<T> match, T defaultValue)
        {
            var result = items.Find(match);
            if (result == null)
            {
                return defaultValue;
            }
            return result;
        }

        /// <summary>
        /// 指定の条件に一致する末尾の要素を返却します
        /// </summary>
        /// <returns>The last.</returns>
        /// <param name="match">Match.</param>
        public T FindLast(Predicate<T> match)
        {
            return FindLast(match);
        }

        /// <summary>
        /// 指定の条件に一致する末尾の要素を返却します。存在しない場合、defaultValueを返却します
        /// </summary>
        /// <returns>The last.</returns>
        /// <param name="match">Match.</param>
        /// <param name="defaultValue">Default value.</param>
        public T FindLast(Predicate<T> match, T defaultValue)
        {
            var result = items.FindLast(match);
            if (result == null)
            {
                return defaultValue;
            }
            return result;
        }

        /// <summary>
        /// 指定の条件に一致する全ての要素のインデックスと要素を返却します
        /// </summary>
        /// <remarks>
        /// using UnityEngine;
        /// using Jackpot;
        /// using Jackpot.Extensions;
        /// 
        /// using Jackpot;
        /// class User
        /// {
        ///     public int Id { get; set; }
        ///     public string Name { get; set; }
        /// }
        /// 
        /// var bindable = new BindableList<User>()
        /// {
        ///     new User(1, "foo"),
        ///     new User(2, "bar"),
        ///     new User(3, "baz")
        /// };
        /// Tuple.UnpackはJackpot.Extensionsに含まれています
        /// bindable.FindAllIndexAndItems(user => user.Id % 2 != 0).Unpack((indexes, users) =>
        /// {
        ///     indexes.ForEach(index => Debug.Log(index));     // => 0, 2
        ///     users.ForEach(user => Debug.Log(user.Name));    // => "foo", "bar"
        /// };
        /// </remarks>
        /// <returns>The all index and item.</returns>
        /// <param name="match">Match.</param>
        public Tuple<List<int>, List<T>> FindAllIndexAndItem(Predicate<T> match)
        {
            return items.FindAllIndexAndItem(match);
        }

        /// <summary>
        /// BindableListが取り扱っているリストを返却します
        /// </summary>
        /// <returns>The list.</returns>
        public List<T> AsList()
        {
            return new List<T>(items);
        }

        /// <summary>
        /// 指定の条件に一致する全ての要素をListで返却します
        /// </summary>
        /// <remarks>
        /// LinqのWhereに相当します
        /// <code>
        /// using Jackpot;
        /// class User
        /// {
        ///     public int Id { get; set; }
        ///     public string Name { get; set; }
        /// }
        /// 
        /// var bindable = new BindableList<User>()
        /// {
        ///     new User(1, "foo"),
        ///     new User(2, "bar"),
        ///     new User(3, "baz")
        /// };
        /// var filtered = bindable.Filter(user => user.Id % 2 != 0); // User.Idが奇数なList
        /// </code>
        /// </remarks>
        /// <param name="match">Match.</param>
        public List<T> Filter(Predicate<T> match)
        {
            return items.Filter(match);
        }

        /// <summary>
        /// 指定の条件に一致しない全ての要素をListで返却します
        /// </summary>
        /// <remarks>
        /// Filter、及びLinqのWhereの否定形です
        /// <code>
        /// using Jackpot;
        /// class User
        /// {
        ///     public int Id { get; set; }
        ///     public string Name { get; set; }
        /// }
        /// 
        /// var bindable = new BindableList<User>()
        /// {
        ///     new User(1, "foo"),
        ///     null,
        ///     new User(2, "bar"),
        ///     null,
        ///     new User(3, "baz")
        /// };
        /// var rejected = bindable.Reject(user => user == null); // nullな要素を除外したList
        /// </code>
        /// </remarks>
        /// <param name="match">Match.</param>
        public List<T> Reject(Predicate<T> match)
        {
            return items.Reject(match);
        }

        /// <summary>
        /// 全ての要素をcallbackで走査し、callbackの返却値から新しいListを返却します
        /// </summary>
        /// <remarks>
        /// LinqのSelectに相当します。
        /// <code>
        /// using Jackpot;
        /// class User
        /// {
        ///     public int Id { get; set; }
        ///     public string Name { get; set; }
        /// }
        /// 
        /// var bindable = new BindableList<User>()
        /// {
        ///     new User(1, "foo"),
        ///     new User(2, "bar"),
        ///     new User(3, "baz")
        /// };
        /// var userIds = bindable.Map(user => user.Id);        // IdのList
        /// var userNames = bindable.Map(user => user.Name);    // NameのList
        /// </code>
        /// </remarks>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="U">The 1st type parameter.</typeparam>
        public List<U> Map<U>(Func<T, U> callback)
        {
            return items.Map(callback);
        }

        /// <summary>
        /// 全ての要素をcallbackで走査し、callbackの返却値から新しいListを返却します
        /// </summary>
        /// <remarks>
        /// LinqのSelectManyに相当します
        /// <code>
        /// using Jackpot;
        /// class User
        /// {
        ///     public int Id { get; set; }
        ///     public string Name { get; set; }
        /// }
        /// 
        /// var bindable = new BindableList<User>()
        /// {
        ///     new User(1, "foo"),
        ///     new User(2, "bar"),
        ///     new User(3, "baz")
        /// };
        /// // 以下の場合
        /// // list.Count => 6
        /// // list[0] => "1",
        /// // list[1] => "foo",
        /// // list[2] => "2",
        /// // list[3] => "3",
        /// // :
        /// // というようなListが返却される
        /// var list = bindable.FlatMap(user => new List<string>() { user.Id.ToString(), user.Name });
        /// </code>
        /// </remarks>
        /// <returns>The map.</returns>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="U">The 1st type parameter.</typeparam>
        public List<U> FlatMap<U>(Func<T, IEnumerable<U>> callback)
        {
            return items.FlatMap(callback);
        }

        /// <Docs>The item to add to the current collection.</Docs>
        /// <para>Adds an item to the current collection.</para>
        /// <remarks>To be added.</remarks>
        /// <exception cref="System.NotSupportedException">The current collection is read-only.</exception>
        /// <summary>
        /// 要素を追加し、ListChangedEventKind.Addな変更通知を送出します
        /// </summary>
        /// <param name="item">Item.</param>
        /// <exception cref="System.InvalidOperationException">編集中は実行できません</exception>
        public void Add(T item)
        {
            ValidateCallStackChanging();
            ValidateNotEditting();

            var oldValue = ResolveOldValueIfNeeded();
            var index = items.Count;
            items.Add(item);

            if (IsDispatchable())
            {
                var eventArgs = ListChangedEventArgs<T>.Add(index, oldValue);
                dispatcher.Dispatch(this, eventArgs);
            }
        }

        /// <summary>
        /// 複数の要素を追加し、ListChangedEventKind.Addな変更通知を送出します
        /// </summary>
        /// <param name="item">Item.</param>
        /// <param name="moreItems">More items.</param>
        /// <exception cref="System.InvalidOperationException">編集中は実行できません</exception>
        public void Add(T item, params T[] moreItems)
        {
            if (moreItems != null && moreItems.Length > 0)
            {
                var items = new List<T> { item };
                items.AddRange(moreItems);
                AddRange(items);
            }
            else
            {
                Add(item);
            }
        }

        /// <summary>
        /// 複数の要素を追加し、ListChangedEventKind.Addな変更通知を送出します
        /// </summary>
        /// <param name="items">Items.</param>
        /// <exception cref="System.InvalidOperationException">編集中は実行できません</exception>
        public void AddRange(IEnumerable<T> items)
        {
            ValidateCallStackChanging();
            ValidateNotEditting();
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            var oldValue = ResolveOldValueIfNeeded();
            var startIndex = this.items.Count;
            this.items.AddRange(items);

            var indexes = new List<int>();
            for (var i = startIndex; i < this.items.Count; i++)
            {
                indexes.Add(i);
            }

            if (indexes.Count > 0 && IsDispatchable())
            {
                var eventArgs = ListChangedEventArgs<T>.Add(indexes, oldValue);
                dispatcher.Dispatch(this, eventArgs);
            }
        }

        /// <summary>
        /// 複数の要素を重複を取り除いた上で追加し、ListChangedEventKind.Addな変更通知を送出します。
        /// </summary>
        /// <remarks>
        /// 重複を取り除いた上で、要素が1つも追加されなかった場合、変更通知は行われません。
        /// </remarks>
        /// <param name="item">Item.</param>
        /// <param name="moreItems">More items.</param>
        /// <exception cref="System.InvalidOperationException">編集中は実行できません</exception>
        public void AddUnique(T item, params T[] moreItems)
        {
            var items = new List<T> { item };
            if (moreItems != null && moreItems.Length > 0)
            {
                items.AddRange(moreItems);
            }
            AddUnique(items);
        }

        /// <summary>
        /// 複数の要素を重複を取り除いた上で追加し、ListChangedEventKind.Addな変更通知を送出します
        /// </summary>
        /// <remarks>
        /// 重複を取り除いた上で、要素が1つも追加されなかった場合、変更通知は行われません。
        /// </remarks>
        /// <param name="items">Items.</param>
        /// <exception cref="System.InvalidOperationException">編集中は実行できません</exception>
        public void AddUnique(IEnumerable<T> items)
        {
            ValidateCallStackChanging();
            ValidateNotEditting();
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            var oldValue = ResolveOldValueIfNeeded();
            var indexes = new List<int>();

            foreach (var item in items)
            {
                if (!Contains(item))
                {
                    indexes.Add(this.items.Count);
                    this.items.Add(item);
                }
            }

            if (indexes.Count > 0 && IsDispatchable())
            {
                var eventArgs = ListChangedEventArgs<T>.Add(indexes, oldValue);
                dispatcher.Dispatch(this, eventArgs);
            }
        }

        /// <summary>
        /// 指定のインデックスに要素を追加し、ListChangedEventKind.Addな変更通知を送出します
        /// </summary>
        /// <param name="index">Index.</param>
        /// <param name="item">Item.</param>
        /// <exception cref="System.InvalidOperationException">編集中は実行できません</exception>
        public void Insert(int index, T item)
        {
            ValidateCallStackChanging();
            ValidateNotEditting();

            var oldValue = ResolveOldValueIfNeeded();
            items.Insert(index, item);

            if (IsDispatchable())
            {
                var eventArgs = ListChangedEventArgs<T>.Add(index, oldValue);
                dispatcher.Dispatch(this, eventArgs);
            }
        }

        /// <summary>
        /// 指定のインデックスに複数の要素を追加し、ListChangedEventKind.Addな変更通知を送出します
        /// </summary>
        /// <param name="index">Index.</param>
        /// <param name="items">Items.</param>
        /// <exception cref="System.InvalidOperationException">編集中は実行できません</exception>
        public void InsertRange(int index, IEnumerable<T> items)
        {
            ValidateCallStackChanging();
            ValidateNotEditting();
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            var oldValue = ResolveOldValueIfNeeded();
            var oldCount = this.items.Count;
            this.items.InsertRange(index, items);

            var endCount = index + this.items.Count - oldCount;
            var indexes = new List<int>();
            for (var i = index; i < endCount; i++)
            {
                indexes.Add(i);
            }

            if (indexes.Count > 0 && IsDispatchable())
            {
                var eventArgs = ListChangedEventArgs<T>.Add(indexes, oldValue);
                dispatcher.Dispatch(this, eventArgs);
            }
        }

        /// <summary>
        /// 指定のインデックスに要素を重複を除きつつ追加し、ListChangedEventKind.Addな変更通知を送出します
        /// </summary>
        /// <remarks>
        /// 重複を取り除いた上で、要素が1つも追加されなかった場合、変更通知は行われません。
        /// </remarks>
        /// <param name="index">Index.</param>
        /// <param name="item">Item.</param>
        /// <param name="moreItems">More items.</param>
        /// <exception cref="System.InvalidOperationException">編集中は実行できません</exception>
        public void InsertUnique(int index, T item, params T[] moreItems)
        {
            var items = new List<T> { item };
            if (moreItems != null && moreItems.Length > 0)
            {
                items.AddRange(moreItems);
            }
            InsertUnique(index, items);
        }

        /// <summary>
        /// 指定のインデックスに要素を重複を除きつつ追加し、ListChangedEventKind.Addな変更通知を送出します
        /// </summary>
        /// <remarks>
        /// 重複を取り除いた上で、要素が1つも追加されなかった場合、変更通知は行われません。
        /// </remarks>
        /// <param name="index">Index.</param>
        /// <param name="items">Items.</param>
        /// <exception cref="System.InvalidOperationException">編集中は実行できません</exception>
        public void InsertUnique(int index, IEnumerable<T> items)
        {
            ValidateCallStackChanging();
            ValidateNotEditting();
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            var oldValue = ResolveOldValueIfNeeded();
            var indexes = new List<int>();

            foreach (var item in items)
            {
                if (!Contains(item))
                {
                    indexes.Add(index);
                    this.items.Insert(index, item);
                    index++;
                }
            }

            if (indexes.Count > 0 && IsDispatchable())
            {
                var eventArgs = ListChangedEventArgs<T>.Add(indexes, oldValue);
                dispatcher.Dispatch(this, eventArgs);
            }
        }

        /// <summary>
        /// SwapによるListの更新を行い、ListChangedEventKind.Updateな変更通知を送出します
        /// </summary>
        /// <param name="aIndex">A index.</param>
        /// <param name="bIndex">B index.</param>
        public void Swap(int aIndex, int bIndex)
        {
            ValidateCallStackChanging();
            ValidateNotEditting();

            var oldValue = ResolveOldValueIfNeeded();
            var aItem = items[aIndex];
            items[aIndex] = items[bIndex];
            items[bIndex] = aItem;

            if (IsDispatchable())
            {
                var indexes = new List<int>{ aIndex, bIndex };
                var eventArgs = ListChangedEventArgs<T>.Update(indexes, oldValue);
                dispatcher.Dispatch(this, eventArgs);
            }
        }

        /// <summary>
        /// SwapによるListの更新を行い、ListChangedEventKind.Updateな変更通知を送出します
        /// </summary>
        /// <param name="aFindFirst">A find first.</param>
        /// <param name="bFindFirst">B find first.</param>
        public void Swap(Predicate<T> aFindFirst, Predicate<T> bFindFirst)
        {
            if (aFindFirst == null)
            {
                throw new ArgumentNullException("aFindFirst");
            }
            if (bFindFirst == null)
            {
                throw new ArgumentNullException("bFindFirst");
            }

            var aIndex = items.FindIndex(aFindFirst);
            if (aIndex < 0)
            {
                return;
            }
            var bIndex = items.FindIndex(bFindFirst);
            if (bIndex < 0)
            {
                return;
            }
            Swap(aIndex, bIndex);
        }

        /// <summary>
        /// 指定のインデックスの要素を更新し、ListChangedEventKind.Updateな変更通知を送出します。
        /// </summary>
        /// <remarks>
        /// インデクサ経由での要素の設定(list[index] = hoge;)と同じです。
        /// </remarks>
        /// <param name="index">Index.</param>
        /// <param name="item">Item.</param>
        /// <exception cref="System.InvalidOperationException">編集中は実行できません</exception>
        public void Update(int index, T item)
        {
            SetItem(index, item);
        }

        /// <summary>
        /// 指定されたインデックスの要素をコールバックを経由して更新し、ListChangedEventKind.Updateな変更通知を送出します。
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="index">Index.</param>
        /// <param name="updater">Updater.</param>
        /// <exception cref="System.InvalidOperationException">編集中は実行できません</exception>
        public void Update(int index, Action<T> updater)
        {
            Update(index, item =>
            {
                updater(item);
                return item;
            });
        }

        /// <summary>
        /// 指定されたインデックスの要素をコールバックを経由して更新し、ListChangedEventKind.Updateな変更通知を送出します。
        /// </summary>
        /// <param name="index">Index.</param>
        /// <param name="updater">Updater.</param>
        /// <exception cref="System.InvalidOperationException">編集中は実行できません</exception>
        public void Update(int index, Func<T, T> updater)
        {
            ValidateCallStackChanging();
            ValidateNotEditting();
            if (updater == null)
            {
                throw new ArgumentNullException("updater");
            }
            if (index < 0)
            {
                return;
            }

            var oldValue = ResolveOldValueIfNeeded();
            var oldItem = items[index];
            var forUpdate = ResolveOldItem(index);
            var updated = EditSection(EditType.Update, () =>
            {
                var newItem = updater(forUpdate);
                items[index] = newItem;
                return AreDifferByChangesPolicy(oldItem, newItem);
            });

            if (updated && IsDispatchable())
            {
                var eventArgs = ListChangedEventArgs<T>.Update(index, oldValue);
                dispatcher.Dispatch(this, eventArgs);
            }
        }

        /// <summary>
        /// 指定の条件を満たす要素をコールバックを経由して更新し、ListChangedEventKind.Updateな変更通知を送出します。
        /// </summary>
        /// <remarks>
        /// 参照型の場合は基本、このメソッドを利用すれば良いです。
        /// <code>
        /// using Jackpot;
        /// class User
        /// {
        ///     public int Id { get; private set; };
        ///     public string Name { get; set; }
        ///
        ///     public User(int id, string name)
        ///     {
        ///         Id = id;
        ///         Name = name;
        ///     }
        /// }
        /// var users = new BindableList<User>()
        /// {
        ///     new User(1, "taro"),
        ///     new User(2, "jiro"),
        ///     new User(3, "saburo"),
        /// };
        /// users.Update(1, user => user.Name = "DarkFlameMaster"); // 通知され、jiroは中二病に目覚める。
        /// </code>
        /// 
        /// プリミティブ値の場合は(値渡しになるので)
        /// 変更通知がされていても、実装次第ではBindableListに変更後の値が反映されません。
        /// その場合、代わりにUpdate(Predicate`1, Func`2)を使用(つまりupdater内でreturn item;する)ようにしてください。
        /// <code>
        /// usin Jackpot;
        /// var names = new BindableList<string>()
        /// {
        ///     "taro",
        ///     "jiro",
        ///     "saburo"
        /// };
        /// names.Update(1, name => name = "DarkFlameMaster");                  // 通知され、jiroは中二病に目覚める。
        /// names.Update(1, name => "DarkFlameMaster");                         // 通知され、jiroは中二病以下略
        /// names.Update(1, name => { var i = 0; name = "DarkFlameMaster"; });  // 通知されず、jiroは中二病に目覚められない。
        /// names.Update(1, name => { var i = 0; return "DarkFlameMaster"; });  // 通知され、以下略
        /// </code>
        /// </remarks>
        /// <param name="match">Match.</param>
        /// <param name="updater">Updater.</param>
        /// 指定の条件を満たす要素をコールバックを経由して更新し、ListChangedEventKind.Updateな変更通知を送出します。
        /// <exception cref="System.InvalidOperationException">編集中は実行できません</exception>
        public void Update(Predicate<T> match, Action<T> updater)
        {
            if (match == null)
            {
                throw new ArgumentException("match");
            }
            Update(match, item =>
            {
                updater(item);
                return item;
            });
        }

        /// <summary>
        /// 指定の条件を満たす要素をコールバックを経由して更新し、ListChangedEventKind.Updateな変更通知を送出します。
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="match">Match.</param>
        /// <param name="updater">Updater.</param>
        /// <exception cref="System.InvalidOperationException">編集中は実行できません</exception>
        public void Update(Predicate<T> match, Func<T, T> updater)
        {
            ValidateCallStackChanging();
            ValidateNotEditting();
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }
            if (updater == null)
            {
                throw new ArgumentNullException("updater");
            }

            var oldValue = ResolveOldValueIfNeeded();
            var index = -1;
            var updated = EditSection(EditType.Update, () =>
            {
                index = items.FindIndex(match);
                if (index < 0)
                {
                    return false;
                }

                var oldItem = items[index];
                var forUpdate = ResolveOldItem(index);
                var newItem = updater(forUpdate);
                items[index] = newItem;
                return AreDifferByChangesPolicy(oldItem, newItem);
            });

            if (updated && IsDispatchable())
            {
                var eventArgs = ListChangedEventArgs<T>.Update(index, oldValue);
                dispatcher.Dispatch(this, eventArgs);
            }
        }

        /// <summary>
        /// 全ての要素をコールバックを経由して更新し、ListChangedEventKind.Updateな変更通知を行います。
        /// </summary>
        /// <param name="updater">Updater.</param>
        /// <exception cref="System.InvalidOperationException">編集中は実行できません</exception>
        public void UpdateAll(Action<T> updater)
        {
            UpdateAll(item =>
            {
                updater(item);
                return item;
            });
        }

        /// <summary>
        /// 全ての要素をコールバックを経由して更新し、ListChangedEventKind.Updateな変更通知を行います。
        /// </summary>
        /// <param name="updater">Updater.</param>
        /// 指定の条件を満たす全ての要素をコールバックを経由して更新し、ListChangedEventKind.Updateな変更通知を送出します。
        /// <exception cref="System.InvalidOperationException">編集中は実行できません</exception>
        public void UpdateAll(Func<T, T> updater)
        {
            UpdateAll(item => true, updater);
        }

        /// <summary>
        /// 指定の条件を満たす全ての要素をコールバックを経由して更新し、ListChangedEventKind.Updateな変更通知を送出します。
        /// </summary>
        /// <param name="match">Match.</param>
        /// <param name="updater">Updater.</param>
        /// <exception cref="System.InvalidOperationException">編集中は実行できません</exception>
        public void UpdateAll(Predicate<T> match, Func<T, T> updater)
        {
            ValidateCallStackChanging();
            ValidateNotEditting();
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }
            if (updater == null)
            {
                throw new ArgumentNullException("updater");
            }

            var oldValue = ResolveOldValueIfNeeded();
            var updatedIndexes = new List<int>();

            var updated = EditSection(EditType.Update, () =>
            {
                var indexes = items.FindAllIndex(match);
                if (indexes == null || indexes.Count <= 0)
                {
                    return false;
                }
                indexes.ForEach(index =>
                {
                    var oldItem = items[index];
                    var forUpdate = ResolveOldItem(index);
                    var newItem = updater(forUpdate);
                    items[index] = newItem;
                    if (AreDifferByChangesPolicy(oldItem, newItem))
                    {
                        updatedIndexes.Add(index);
                    }
                });
                return updatedIndexes.Count > 0;
            });

            if (updated && IsDispatchable())
            {
                var eventArgs = ListChangedEventArgs<T>.Update(updatedIndexes, oldValue);
                dispatcher.Dispatch(this, eventArgs);
            }
        }

        /// <summary>
        /// 新しいCollectionオブジェクトをBindableListに設定し、ListChangedEventKind.Editな変更通知を送出します
        /// </summary>
        /// <param name="newValue">New value.</param>
        /// <exception cref="System.ArgumentNullException">nullは設定できません</exception>
        /// <exception cref="System.InvalidOperationException">編集中は実行できません</exception>
        public void Edit(IEnumerable<T> newValue)
        {
            if (newValue == null)
            {
                throw new ArgumentNullException("newValue");
            }
            Edit(list => newValue);
        }

        /// <summary>
        /// リストを直接編集し、ListChangedEventKind.Editな変更通知を送出します
        /// </summary>
        /// <remarks>
        /// おおよそUpdateAllと同じですが、こちらはBindableListが保持するリストを直接編集できます。
        /// Updateでは都合の悪い、より複雑な操作を行う場合(Sort、Reverse等)に使用します。
        /// </remarks>
        /// <param name="editor">Editor.</param>
        /// <exception cref="System.InvalidOperationException">編集中は実行できません</exception>
        public void Edit(Action<List<T>> editor)
        {
            Edit(list =>
            {
                editor(list);
                return list;
            });
        }

        /// <summary>
        /// リストを直接編集し、ListChangedEventKind.Editな変更通知を送出します
        /// </summary>
        /// <remarks>
        /// おおよそUpdateAllと同じですが、こちらはリストを直接編集できます。
        /// editor内でCollectionオブジェクトを返却することで、より複雑な操作を行った結果をBindableListに反映する事ができます。
        /// </remarks>
        /// <param name="editor">Editor.</param>
        /// <exception cref="System.InvalidOperationException">編集中は実行できません</exception>
        public void Edit(Func<List<T>, IEnumerable<T>> editor)
        {
            ValidateCallStackChanging();
            ValidateNotEditting();
            if (editor == null)
            {
                throw new ArgumentNullException("editor");
            }

            var oldValue = ResolveOldValueIfNeeded();
            var forEdit = ResolveOldValue();
            var editted = EditSection(EditType.Edit, () =>
            {
                var newValue = editor(forEdit);
                if (newValue == null)
                {
                    throw new InvalidOperationException("return value of editor");
                }
                items = new List<T>(newValue);
                return true;
            });

            if (editted && IsDispatchable())
            {
                var eventArgs = ListChangedEventArgs<T>.Edit(oldValue);
                dispatcher.Dispatch(this, eventArgs);
            }
        }

        /// <summary>
        /// BidnableRulesを検証しつつ、問題なければ強制的にListChangedEventKind.ForceUpdateな変更通知を送出します
        /// </summary>
        public void ForceUpdate()
        {
            ValidateCallStackChanging();
            ValidateNotEditting();

            if (IsDispatchable())
            {
                var oldValue = ResolveOldValueIfNeeded();
                var eventArgs = ListChangedEventArgs<T>.ForceUpdate(oldValue);
                dispatcher.Dispatch(this, eventArgs);
            }
        }

        /// <Docs>The item to remove from the current collection.</Docs>
        /// <para>Removes the first occurrence of an item from the current collection.</para>
        /// <summary>
        /// 指定の要素を削除し、ListChangedEventKind.Removeな変更通知を送出します
        /// </summary>
        /// <param name="item">Item.</param>
        /// <exception cref="System.InvalidOperationException">編集中は実行できません</exception>
        public bool Remove(T item)
        {
            ValidateCallStackChanging();
            ValidateNotEditting();

            var index = items.IndexOf(item);
            if (index < 0)
            {
                return false;
            }
            var oldValue = ResolveOldValueIfNeeded();
            items.Remove(item);

            if (IsDispatchable())
            {
                var eventArgs = ListChangedEventArgs<T>.Remove(index, oldValue);
                dispatcher.Dispatch(this, eventArgs);
            }
            return true;
        }

        /// <summary>
        /// 指定のインデックスの要素を削除し、ListChangedEventKind.Removeな変更通知を送出します
        /// </summary>
        /// <param name="index">Index.</param>
        /// <exception cref="System.InvalidOperationException">編集中は実行できません</exception>
        public void RemoveAt(int index)
        {
            ValidateCallStackChanging();
            ValidateNotEditting();

            if (index < 0)
            {
                return;
            }
            var oldValue = ResolveOldValueIfNeeded();
            items.RemoveAt(index);

            if (IsDispatchable())
            {
                var eventArgs = ListChangedEventArgs<T>.Remove(index, oldValue);
                dispatcher.Dispatch(this, eventArgs);
            }
        }

        /// <summary>
        /// 指定の条件を満たす全ての要素を削除し、ListChangedEventKind.Removeな変更通知を送出します
        /// </summary>
        /// <param name="match">Match.</param>
        /// <exception cref="System.InvalidOperationException">編集中は実行できません</exception>
        public void RemoveAll(Predicate<T> match)
        {
            ValidateCallStackChanging();
            ValidateNotEditting();
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }

            var oldValue = ResolveOldValueIfNeeded();
            var indexes = default(List<int>);
            var removed = EditSection(EditType.Remove, () =>
            {
                var result = false;
                items.FindAllIndexAndItem(match).Unpack((matchedIndexes, matchedItems) =>
                {
                    if (matchedIndexes.Count < 0)
                    {
                        return;
                    }
                    indexes = matchedIndexes;
                    matchedItems.ForEach(item => items.Remove(item));
                    result = true;
                });
                return result;
            });

            if (removed && IsDispatchable())
            {
                var eventArgs = ListChangedEventArgs<T>.Remove(indexes, oldValue);
                dispatcher.Dispatch(this, eventArgs);
            }
        }

        /// <summary>
        /// 全ての要素を削除し、ListChangedEventKind.Removeな変更通知を送出します
        /// </summary>
        /// <exception cref="System.InvalidOperationException">編集中は実行できません</exception>
        public void Clear()
        {
            ValidateCallStackChanging();
            ValidateNotEditting();

            var oldValue = ResolveOldValueIfNeeded();
            var indexes = new List<int>();
            for (var i = 0; i < items.Count; i++)
            {
                indexes.Add(i);
            }
            items.Clear();

            if (IsDispatchable())
            {
                var eventArgs = ListChangedEventArgs<T>.Remove(indexes, oldValue);
                dispatcher.Dispatch(this, eventArgs);
            }
        }

        #endregion

        #region Public ICloneable Methods

        /// <summary>
        /// このインスタンスのコピーを生成します。
        /// </summary>
        /// <remarks>
        /// イベントハンドラの設定を引き継きたくない場合はClone(false)とします。
        /// BindableListが取り扱うオブジェクトがICloneableを実装している場合、
        /// そのオブジェクトをCloneしたい場合はClone({true|false}, true);とします。
        /// </remarks>
        public virtual object Clone()
        {
            ValidateNotEditting();
            return Clone(true);
        }

        /// <summary>
        /// イベントハンドラの設定を引き継ぐか指定しつつ、このインスタンスのコピーを生成します。
        /// </summary>
        /// <param name="withTakeOverHandler">If set to <c>true</c> with take over handler.</param>
        /// 各要素を参照渡しでなくコピーするか指定しつつ、このインスタンスのコピーを生成します。
        public virtual object Clone(bool withTakeOverHandler)
        {
            ValidateNotEditting();
            return Clone(withTakeOverHandler, true);
        }

        /// <summary>
        /// 各要素を参照渡しでなくコピーするか指定しつつ、このインスタンスのコピーを生成します。
        /// </summary>
        /// <param name="withTakeOverHandler">If set to <c>true</c> with take over handler.</param>
        /// <param name="withCopyItem">If set to <c>true</c> with copy item.</param>
        public virtual object Clone(bool withTakeOverHandler, bool withCopyItem)
        {
            ValidateNotEditting();
            return new BindableList<T>(this, withTakeOverHandler, withCopyItem);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Determines if is compatible object the specified value.
        /// </summary>
        /// <returns><c>true</c> if is compatible object the specified value; otherwise, <c>false</c>.</returns>
        /// <param name="value">Value.</param>
        protected static bool IsCompatibleObject(object value)
        {
            return ((value is T) || (value == null && default(T) == null));
        }

        /// <summary>
        /// Validates the null and nulls are illegal.
        /// </summary>
        /// <param name="value">Value.</param>
        protected static void ValidateNullAndNullsAreIllegal(object value)
        {
            if (value == null && default(T) != null)
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Validates the not editting.
        /// </summary>
        protected virtual void ValidateNotEditting()
        {
            if (Editting)
            {
                throw new InvalidOperationException(string.Format("Still Editting. type: {0}", editType.ToString()));
            }
        }

        /// <summary>
        /// Validates the call stack changing.
        /// </summary>
        protected virtual void ValidateCallStackChanging()
        {
            if (Rule == BindableRules.NotAllowCallStackChanging && dispatcher.IsDispatching)
            {
                throw new InvalidOperationException("A value cannot be changed while dispatching data.");
            }
        }

        /// <summary>
        /// Determines whether this instance is dispatchable.
        /// </summary>
        /// <returns><c>true</c> if this instance is dispatchable; otherwise, <c>false</c>.</returns>
        protected bool IsDispatchable()
        {
            if (Editting)
            {
                return false;
            }
            if (dispatcher.IsDispatching)
            {
                if (Rule == BindableRules.NotAllowCallStackChanging
                    || Rule == BindableRules.ThroughCallStackChanging)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 変更ポリシーに従って、二値が異なる値（変更されたと判別できる）かどうかを示します
        /// </summary>
        /// <remarks>
        /// 変更ポリシーが指定されていない状態では、二値の比較に関わらず<c>true</c>となります
        /// </remarks>
        /// <returns>二値がchangesPolicyに基いて異なる値であれば<c>true</c>、同じと見なされれば<c>false</c>を返します</returns>
        /// <param name="before">Before value.</param>
        /// <param name="after">After value.</param>
        protected bool AreDifferByChangesPolicy(T before, T after)
        {
            return changesPolicy == null || changesPolicy(before, after);
        }

        /// <summary>
        /// Sets the item and dispach ListChangedEvent(Kind = Update).
        /// </summary>
        /// <param name="index">Index.</param>
        /// <param name="item">Item.</param>
        protected virtual void SetItem(int index, T item)
        {
            ValidateCallStackChanging();
            ValidateNotEditting();

            var oldValue = ResolveOldValueIfNeeded();
            var oldItem = ResolveOldItem(index);
            items[index] = item;

            if (AreDifferByChangesPolicy(oldItem, item) && IsDispatchable())
            {
                var eventArgs = ListChangedEventArgs<T>.Update(index, oldValue);
                dispatcher.Dispatch(this, eventArgs);
            }
        }

        /// <summary>
        /// 変更前のリストを必要あれば返却します
        /// </summary>
        /// <returns>The old value if needed.</returns>
        protected virtual List<T> ResolveOldValueIfNeeded()
        {
            return IsDispatchable() && OldValueEnabled ? ResolveOldValue() : null;
        }

        /// <summary>
        /// 変更前のリストを返却します
        /// </summary>
        /// <remarks>
        /// 直接itemsを参照しても良いですが、このメソッドを経由すると、
        /// 要素がIClonableを実装している場合Cloneを実施するので、
        /// 変更前の要素を正しく保持する事ができます。
        /// </remarks>
        /// <returns>The old value.</returns>
        protected virtual List<T> ResolveOldValue()
        {
            var results = default(List<T>);
            if (items.Count > 0 && items[0] is ICloneable)
            {
                results = new List<T>();
                items.ForEach(item => results.Add((T) ((ICloneable) item).Clone()));
            }
            else
            {
                results = new List<T>(items);
            }
            return results;
        }

        /// <summary>
        /// 変更前の要素を返却します
        /// </summary>
        /// <returns>The old item.</returns>
        /// <remarks>
        /// 直接itemsを参照しても良いですが、このメソッドを経由すると、
        /// 要素がIClonableを実装している場合Cloneを実施するので、
        /// 変更前の要素を正しく保持する事ができます。
        /// </remarks>
        /// <param name="index">Index.</param>
        protected virtual T ResolveOldItem(int index)
        {
            var item = items[index];
            var cloneableItem = item as ICloneable;
            return cloneableItem == null ? item : (T) cloneableItem.Clone();
        }

        /// <summary>
        /// 編集中である事を保証しながら、指定のセクションを実施します
        /// </summary>
        /// <remarks>
        /// UpdateやEdit等の編集用のメソッドは、このメソッドを使用して実装する事で
        /// 編集中にさらにBindableListの編集を行うようなロジックをブロッキングする事ができます。
        /// 利用者に編集処理を委譲ようなメソッドを実施する処理は
        /// 必ずこのメソッドを経由してください。
        /// </remarks>
        /// <returns><c>true</c>, if section was edited, <c>false</c> otherwise.</returns>
        /// <param name="type">Type.</param>
        /// <param name="section">Section.</param>
        protected bool EditSection(EditType type, Func<bool> section)
        {
            if (type == EditType.None)
            {
                throw new ArgumentException("unexpected parameter", "type");
            }
            editType = type;
            var editted = false;
            try
            {
                editted = section();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                editType = EditType.None;
            }
            return editted;
        }

        #endregion

        #region IEnumerable implementation

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) items).GetEnumerator();
        }

        #endregion

        #region ICollection implementation

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException();
            }
            if (array.Rank != 1)
            {
                throw new ArgumentException();
            }
            if (array.GetLowerBound(0) != 0)
            {
                throw new ArgumentException();
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (array.Length - index < Count)
            {
                throw new ArgumentException();
            }

            var tArray = array as T[];
            if (tArray == null)
            {
                var targetType = array.GetType().GetElementType();
                var sourceType = typeof(T);
                if (!(targetType.IsAssignableFrom(sourceType) || sourceType.IsAssignableFrom(targetType)))
                {
                    throw new ArgumentException();
                }

                var objects = array as object[];
                if (objects == null)
                {
                    throw new ArgumentException();
                }

                int count = items.Count;
                for (var i = 0; i < count; i++)
                {
                    objects[index++] = items[i];
                }

                return;
            }
            items.CopyTo(tArray, index);
        }

        #endregion

        #region IList implementation

        int IList.Add(object value)
        {
            ValidateNullAndNullsAreIllegal(value);
            Add((T) value);
            return this.Count - 1;
        }

        bool IList.Contains(object value)
        {
            if (!IsCompatibleObject(value))
            {
                return false;
            }
            return Contains((T) value);
        }

        int IList.IndexOf(object value)
        {
            if (!IsCompatibleObject(value))
            {
                return -1;
            }
            return IndexOf((T) value);
        }

        void IList.Insert(int index, object value)
        {
            ValidateNullAndNullsAreIllegal(value);
            Insert(index, (T) value);
        }

        void IList.Remove(object value)
        {
            if (!IsCompatibleObject(value))
            {
                return;
            }
            Remove((T) value);
        }

        #endregion

    }
}

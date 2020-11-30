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

namespace Jackpot
{
    using EventKind = DictionaryChangedEventKind;

    [Serializable]
    /// <summary>
    /// 要素の変更を監視できるDictionaryクラスです
    /// </summary>
    /// <remarks>
    /// BindableDictionaryから送出される変更通知の詳細は<see cref="Jackpot.DictionaryChangedEventKind"/>を参考にしてください。
    /// BindableDictionaryから送出される変更通知に付属するパラメータは<see cref="Jackpot.DictionaryChangedEventArgs{TKey,TValue}"/>を参考にしてください。
    /// </remarks>
    public class BindableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary,
        ICollection<KeyValuePair<TKey, TValue>>, ICollection,
        IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
    {
        protected enum EditType
        {
            None,
            Update,
            Remove,
            Edit
        }

        /// <summary>
        /// パフォーマンスの都合上、BindableDictionaryのデフォルトの変更ポリシーはありません
        /// </summary>
        static readonly Func<TValue, TValue, bool> DefaultChangesPolicy = null;

        #region Static Method

        /// <summary>
        /// キーがnullでないかを判別し、<c>null</c>であれば不適切として<see cref="ArgumentNullException"/>をスローします。
        /// </summary>
        /// <param name="key">Key.</param>
        static void ValidateNotNullKey(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
        }

        /// <summary>
        /// BindableDictionaryのキーとして適切かを判別します。
        /// </summary>
        /// <param name="key">Key.</param>
        static void ValidateObjectEnableAsKey(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (!(key is TKey))
            {
                throw new ArgumentException("not of type: " + typeof(TKey).ToString(), "key");
            }
        }

        /// <summary>
        /// BindableDictionaryの値として適切かを判別します。
        /// </summary>
        /// <param name="value">Value.</param>
        static void ValidateObjectEnableAsValue(object value)
        {
            if (!(value is TValue))
            {
                throw new ArgumentException("not of type: " + typeof(TValue).ToString(), "value");
            }
        }

        #endregion

        #region Private Fields

        Dictionary<TKey, TValue> entries;

        EventDispatcher<DictionaryChangedEventArgs<TKey, TValue>> dispatcher;

        Func<TValue, TValue, bool> changesPolicy;

        EditType editType;

        #endregion

        #region Public Properties

        /// <summary>
        /// 格納されている要素数を示します
        /// </summary>
        /// <value>The count.</value>
        public int Count { get { return entries.Count; } }

        /// <summary>
        /// 指定したキーに格納された要素を取得したり設定したりします
        /// </summary>
        /// <remarks>
        /// setter での変更は、既にキーが存在する場合は Update, 存在していない場合 は Add を通知します
        /// </remarks>
        /// <exception cref="KeyNotFoundException">getter で存在していないキーを指定された場合</exception>
        /// <param name="key">Key.</param>
        public TValue this[TKey key]
        {
            get
            {
                ValidateContainsKey(key);
                return entries[key];
            }
            set
            {
                SetEntry(key, value);
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

        /// <summary>
        /// 格納されているキーを Collection として返します
        /// </summary>
        /// <value>The keys.</value>
        public Dictionary<TKey, TValue>.KeyCollection Keys { get { return entries.Keys; } }

        /// <summary>
        /// 格納されている値を Collection として返します
        /// </summary>
        /// <value>The values.</value>
        public Dictionary<TKey, TValue>.ValueCollection Values { get { return entries.Values; } }

        #endregion

        #region Constructor

        public BindableDictionary() : this(new Dictionary<TKey, TValue>())
        {
        }

        public BindableDictionary(IDictionary<TKey, TValue> dictionary) : this(dictionary, false)
        {
        }

        public BindableDictionary(BindableRules rule) : this(null, rule)
        {
        }

        public BindableDictionary(bool oldValueEnabled) : this(null, oldValueEnabled)
        {
        }

        public BindableDictionary(IDictionary<TKey, TValue> dictionary, BindableRules rule) : this(dictionary, rule, false)
        {
        }

        public BindableDictionary(IDictionary<TKey, TValue> dictionary, bool oldValueEnabled) : this(dictionary, BindableRules.NotAllowCallStackChanging, oldValueEnabled)
        {
        }

        public BindableDictionary(IDictionary<TKey, TValue> dictionary, BindableRules rule, bool oldValueEnabled)
        {
            entries = dictionary == null ? new Dictionary<TKey, TValue>() : new Dictionary<TKey,TValue>(dictionary);
            Rule = rule;
            OldValueEnabled = oldValueEnabled;
            dispatcher = new EventDispatcher<DictionaryChangedEventArgs<TKey, TValue>>();
            changesPolicy = DefaultChangesPolicy;
            editType = EditType.None;
        }

        public BindableDictionary(BindableDictionary<TKey, TValue> that) : this(that, true)
        {
        }

        public BindableDictionary(BindableDictionary<TKey, TValue> that, bool withTakeOverHandler) : this(that, withTakeOverHandler, false)
        {
        }

        public BindableDictionary(BindableDictionary<TKey, TValue> that, bool withTakeOverHandler, bool withCopyItem)
        {
            entries = withCopyItem ? that.ResolveCurrentEntries() : new Dictionary<TKey, TValue>(that.entries);
            Rule = that.Rule;
            OldValueEnabled = that.OldValueEnabled;
            dispatcher = withTakeOverHandler
                ? new EventDispatcher<DictionaryChangedEventArgs<TKey, TValue>>(that.dispatcher)
                : new EventDispatcher<DictionaryChangedEventArgs<TKey, TValue>>();
            changesPolicy = that.changesPolicy;
            editType = EditType.None;
        }

        #endregion

        #region Binding Method

        /// <summary>
        /// 変更通知を受け取るイベントハンドラを追加します. 重複した場合は再登録されます
        /// </summary>
        /// <remarks>
        /// 追加したイベントハンドラは強い参照で保持されます。
        /// 詳しくは<see cref="EventDispatcher{TEventArgs}.AddHandler"/>を参照してください
        /// </remarks>
        /// <param name="handler">Handler.</param>
        public void Bind(EventHandler<DictionaryChangedEventArgs<TKey, TValue>> handler)
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
        /// <param name="handler">Handler.</param>
        public void Bind(object target, EventHandler<DictionaryChangedEventArgs<TKey, TValue>> handler)
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
        /// <param name="handler">Handler.</param>
        public void BindOnce(EventHandler<DictionaryChangedEventArgs<TKey, TValue>> handler)
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
        /// <param name="handler">Handler.</param>
        public void BindOnce(object target, EventHandler<DictionaryChangedEventArgs<TKey, TValue>> handler)
        {
            dispatcher.AddHandler(target, handler, true);
        }

        /// <summary>
        /// 変更通知を受け取るイベントハンドラが、ひとつでも登録されているかを示します。
        /// </summary>
        /// <returns><c>true</c> if this instance is binding; otherwise, <c>false</c>.</returns>
        public bool IsBinding()
        {
            return !(dispatcher.HandlerIsEmpty);
        }

        /// <summary>
        /// 指定したイベントハンドラが登録されているかを示します。
        /// </summary>
        /// <returns><c>true</c> if this instance is binding the specified handler; otherwise, <c>false</c>.</returns>
        /// <param name="handler">Handler.</param>
        public bool IsBinding(EventHandler<DictionaryChangedEventArgs<TKey, TValue>> handler)
        {
            return dispatcher.ContainsHandler(handler);
        }

        /// <summary>
        /// 変更通知を一回だけ受け取るよう設定されたイベントハンドラが、ひとつでも登録されているかを示します。
        /// </summary>
        /// <returns>ひとつでも登録されていれば<c>true</c>、無ければ<c>false</c>になります</returns>
        public bool IsBindingOnce()
        {
            return !(dispatcher.OnceHandlerIsEmpty);
        }

        /// <summary>
        /// 指定したイベントハンドラが、一度のみ変更通知を受け取るよう登録されているかを示します。
        /// </summary>
        /// <returns><c>true</c> if this instance is binding once the specified handler; otherwise, <c>false</c>.</returns>
        /// <param name="handler">Handler.</param>
        public bool IsBindingOnce(EventHandler<DictionaryChangedEventArgs<TKey, TValue>> handler)
        {
            return dispatcher.ContainsHandler(handler, true);
        }

        /// <summary>
        /// 登録されている全てのイベントハンドラを登録解除します。
        /// </summary>
        public void UnBindAll()
        {
            dispatcher.RemoveAllHandlers();
        }

        /// <summary>
        /// 指定したイベントハンドラを登録解除します
        /// </summary>
        /// <param name="handler">Handler.</param>
        public void UnBind(EventHandler<DictionaryChangedEventArgs<TKey, TValue>> handler)
        {
            dispatcher.RemoveHandler(handler);
        }

        /// <summary>
        /// どのようなデータの変更があった場合に通知するかを設定します
        /// </summary>
        /// <remarks>
        /// パフォーマンスの都合上、BindableDictionaryの管理する要素のデフォルト変更ポリシーはありません。
        /// nullを指定した場合、変更ポリシーはないと判断され、通知可能な状態の時は必ず変更通知が飛びます。
        /// 加えて、要素の型が参照型であるBindableDictionaryの変更ポリシーを設定する場合、
        /// 要素の更新時はその値が参照型である事を考慮し、参照の違う新しいObjectを設定、ないし返却する必要があります。
        /// ただし、要素の型がICloneableを実装している場合、CloneしたObjectがコールバックに渡されるので、
        /// ICloneableが実装されていれば、その手間を省略できます。(index accessorではCloneされません)。
        /// </remarks>
        /// <param name="policy">Changes policy.</param>
        public void SetChangesPolicy(Func<TValue, TValue, bool> policy)
        {
            changesPolicy = policy;
        }

        #endregion

        #region Public Method

        /// <summary>
        /// 要素が空かどうかを示します
        /// </summary>
        /// <returns><c>true</c> if this instance is empty; otherwise, <c>false</c>.</returns>
        public bool IsEmpty()
        {
            return entries.Count < 1;
        }

        /// <Docs>The key to locate in the current instance.</Docs>
        /// <para>Determines whether the current instance contains an entry with the specified key.</para>
        /// <summary>
        /// 指定したキーがエントリーとして登録されているかを示します。
        /// </summary>
        /// <returns><c>true</c>, if key was containsed, <c>false</c> otherwise.</returns>
        /// <param name="key">Key.</param>
        public bool ContainsKey(TKey key)
        {
            ValidateNotNullKey(key);
            return entries.ContainsKey(key);
        }

        /// <summary>
        /// 指定した値が、登録されたエントリーに含まれているかを示します。
        /// </summary>
        /// <returns><c>true</c>, if value was containsed, <c>false</c> otherwise.</returns>
        /// <param name="value">Value.</param>
        public bool ContainsValue(TValue value)
        {
            return entries.ContainsValue(value);
        }

        /// <summary>
        /// 呼び出した時点での登録された全てのエントリーをキャッシュし、コールバックで走査します。
        /// </summary>
        /// <param name="iterator">Iterator.</param>
        public void ForEach(Action<TKey, TValue> iterator)
        {
            if (iterator == null)
            {
                throw new ArgumentNullException("iterator");
            }
            foreach (var item in ResolveCurrentEntries())
            {
                iterator(item.Key, item.Value);
            }
        }

        /// <summary>
        /// コールバックが返す条件に一致するエントリーのみの、サブセットとしてのDictionaryを返します。
        /// </summary>
        /// <param name="match">Match.</param>
        public Dictionary<TKey, TValue> Filter(Func<TKey, TValue, bool> match)
        {
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }
            var ret = new Dictionary<TKey, TValue>();
            ForEach((key, value) =>
            {
                if (match(key, value))
                {
                    ret.Add(key, value);
                }
            });
            return ret;
        }

        /// <summary>
        /// コールバックが返す条件に一致したキーのリストを取得します。
        /// </summary>
        /// <returns>The keys.</returns>
        /// <param name="match">Match.</param>
        public List<TKey> MatchKeys(Func<TKey, TValue, bool> match)
        {
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }
            return new List<TKey>(Filter(match).Keys);
        }

        /// <summary>
        /// 新しくエントリーを追加し、変更通知としてAddイベントを発行します。
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <exception cref="ArgumentException">重複するキーを指定していた場合</exception>
        /// <exception cref="InvalidOperationException">編集中は実行できません</exception>
        public void Add(TKey key, TValue value)
        {
            ValidateNotNullKey(key);
            ValidateCallStackChanging();
            ValidateNotEditting();

            var oldValue = ResolveCurrentEntriesIfNeeded();

            entries.Add(key, value);

            if (IsDispatchable())
            {
                var eventArgs = DictionaryChangedEventArgs<TKey, TValue>.Add(key, oldValue);
                dispatcher.Dispatch(this, eventArgs);
            }
        }

        /// <summary>
        /// 指定したDictionaryの要素をまとめて追加し、変更通知としてAddイベントを発行します。
        /// </summary>
        /// <remarks>
        /// 渡されるDictionaryはキーが重複していないことを前提としています。
        /// もし、「キーが重複する可能性がある」用途には、<see cref="Jackpot.BindableDictionary{TKey,TValue}.Edit"/>を使用してください
        /// </remarks>
        /// <param name="dictionary">Dictionary.</param>
        /// <exception cref="ArgumentException">重複するキーが内在していた場合</exception>
        /// <exception cref="InvalidOperationException">編集中は実行できません</exception>
        public void Add(IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            ValidateCallStackChanging();
            ValidateNotEditting();

            var oldValue = ResolveCurrentEntriesIfNeeded();
            var changes = new List<TKey>(dictionary.Keys);

            foreach (var item in dictionary)
            {
                entries.Add(item.Key, item.Value);
            }

            if (changes.Count > 0 && IsDispatchable())
            {
                var eventArgs = DictionaryChangedEventArgs<TKey, TValue>.Add(changes, oldValue);
                dispatcher.Dispatch(this, eventArgs);
            }
        }

        /// <summary>
        /// 指定したキーのエントリーを、指定した値で置き換え、変更通知としてUpdateイベントを発行します。
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <exception cref="KeyNotFoundException">指定したキーが登録されていなかった場合</exception>
        /// <exception cref="InvalidOperationException">編集中は実行できません</exception>
        public void Update(TKey key, TValue value)
        {
            ValidateCallStackChanging();
            ValidateNotEditting();

            var before = ResolveCurrentValue(key);
            var oldValue = ResolveCurrentEntriesIfNeeded();

            entries[key] = value;

            if (AreDifferByChangesPolicy(before, value) && IsDispatchable())
            {
                var eventArgs = DictionaryChangedEventArgs<TKey, TValue>.Update(key, oldValue);
                dispatcher.Dispatch(this, eventArgs);
            }
        }

        /// <summary>
        /// 指定したキーのエントリーを、コールバックを介して更新し、変更通知としてUpdateイベントを発行します。
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="updater">Updater.</param>
        /// <exception cref="InvalidOperationException">編集中は実行できません</exception>
        public void Update(TKey key, Action<TValue> updater)
        {
            Update(key, value =>
            {
                updater(value);
                return value;
            });
        }

        /// <summary>
        /// 指定したキーのエントリーを、コールバックの結果から更新し、変更通知としてUpdateイベントを発行します。
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="updater">Updater.</param>
        /// <exception cref="InvalidOperationException">編集中は実行できません</exception>
        public void Update(TKey key, Func<TValue, TValue> updater)
        {
            ValidateCallStackChanging();
            ValidateNotEditting();

            var forUpdate = ResolveCurrentValue(key);
            var oldValue = ResolveCurrentEntriesIfNeeded();
            var before = entries[key];

            var updated = EditSection(EditType.Update, () =>
            {
                var after = updater(forUpdate);
                entries[key] = after;

                return AreDifferByChangesPolicy(before, after);
            });

            if (updated && IsDispatchable())
            {
                var eventArgs = DictionaryChangedEventArgs<TKey, TValue>.Update(key, oldValue);
                dispatcher.Dispatch(this, eventArgs);
            }
        }

        /// <summary>
        /// コールバックが返す条件に一致するエントリーのみ、コールバックを介して更新し、変更通知としてUpdateイベントを発行します。
        /// </summary>
        /// <param name="match">Match.</param>
        /// <param name="updater">Updater.</param>
        /// <exception cref="InvalidOperationException">編集中は実行できません</exception>
        public void UpdateAll(Func<TKey, TValue, bool> match, Func<TValue, TValue> updater)
        {
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }
            if (updater == null)
            {
                throw new ArgumentNullException("updater");
            }
            ValidateCallStackChanging();
            ValidateNotEditting();

            var oldValues = ResolveCurrentEntriesIfNeeded();
            var changes = default(List<TKey>);
            var updated = EditSection(EditType.Update, () =>
            {
                changes = MatchKeys((key, value) =>
                {
                    if (!match(key, value))
                    {
                        return false;
                    }

                    var after = updater(value);
                    entries[key] = after;

                    return AreDifferByChangesPolicy(value, after);
                });

                return changes.Count > 0;
            });

            if (updated && IsDispatchable())
            {
                var eventArgs = DictionaryChangedEventArgs<TKey, TValue>.Update(changes, oldValues);
                dispatcher.Dispatch(this, eventArgs);
            }
        }

        /// <summary>
        /// 指定したDictionaryに全て置き換え、変更通知としてEditイベントを発行します。
        /// </summary>
        /// <remarks>
        /// 新規で登録するキー、削除するキー、値を更新するキーが複合している場合、一括で変更する際に利用できます。
        /// ただし、重複チェックもせず、変更通知としてAdd, Remove, Update, いずれも発行されません。
        /// </remarks>
        /// <param name="alternate">Alternate.</param>
        /// <exception cref="InvalidOperationException">編集中は実行できません</exception>
        public void Edit(IDictionary<TKey, TValue> alternate)
        {
            if (alternate == null)
            {
                throw new ArgumentNullException("alternate");
            }
            Edit(dictionary => alternate);
        }

        /// <summary>
        /// コールバックで全ての要素を変更し、変更通知としてEditイベントを発行します。
        /// </summary>
        /// <remarks>
        /// コールバック内で、更に深くAdd, Remove, Update, Editを呼び出すことはできません。
        /// </remarks>
        /// <param name="editor">Editor.</param>
        /// <exception cref="InvalidOperationException">編集中は実行できません</exception>
        public void Edit(Action<IDictionary<TKey, TValue>> editor)
        {
            if (editor == null)
            {
                throw new ArgumentNullException("editor");
            }
            Edit(dictionary =>
            {
                editor(dictionary);
                return dictionary;
            });
        }

        /// <summary>
        /// コールバックの結果をエントリーを上書きし、変更通知としてEditイベントを発行します。
        /// </summary>
        /// <remarks>
        /// コールバック内で、更に深くAdd, Remove, Update, Editを呼び出すことはできません。
        /// </remarks>
        /// <param name="editor">Editor.</param>
        /// <exception cref="InvalidOperationException">編集中は実行できません</exception>
        public void Edit(Func<IDictionary<TKey, TValue>, IDictionary<TKey, TValue>> editor)
        {
            if (editor == null)
            {
                throw new ArgumentNullException("editor");
            }
            ValidateCallStackChanging();

            var oldValue = ResolveCurrentEntriesIfNeeded();
            var forEdit = ResolveCurrentEntries();

            var editted = EditSection(EditType.Edit, () =>
            {
                var after = editor(forEdit);
                if (after == null)
                {
                    throw new InvalidOperationException("return value of editor");
                }
                entries = new Dictionary<TKey, TValue>(after);
                return true;
            });

            if (editted && IsDispatchable())
            {
                var eventArgs = DictionaryChangedEventArgs<TKey, TValue>.Edit(oldValue);
                dispatcher.Dispatch(this, eventArgs);
            }
        }

        /// <summary>
        /// BidnableRulesを検証しつつ、問題なければ強制的に変更通知としてForceUpdateイベントを発行します。
        /// </summary>
        /// <exception cref="InvalidOperationException">編集中は実行できません</exception>
        public void ForceUpdate()
        {
            ValidateCallStackChanging();
            ValidateNotEditting();

            if (IsDispatchable())
            {
                var values = ResolveCurrentEntriesIfNeeded();
                var eventArgs = DictionaryChangedEventArgs<TKey, TValue>.ForceUpdate(values);
                dispatcher.Dispatch(this, eventArgs);
            }
        }

        /// <summary>
        /// 指定されたキーに一致するエントリーを削除し、変更通知としてRemoveイベントを発行します。
        /// 削除されたエントリーがあるかどうかを<c>bool</c>で返します。
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="more">More.</param>
        /// <returns>削除されたエントリーがあれば<c>true</c>, 無ければ<c>false</c></returns>
        /// <exception cref="InvalidOperationException">編集中は実行できません</exception>
        public bool Remove(TKey key, params TKey[] more)
        {
            ValidateNotNullKey(key);
            ValidateCallStackChanging();
            ValidateNotEditting();

            var oldValue = ResolveCurrentEntriesIfNeeded();
            var changes = new List<TKey>();
            var keys = new List<TKey> { key };

            if (more != null && more.Length > 0)
            {
                keys.AddRange(more);
            }

            foreach (var k in keys)
            {
                if (entries.Remove(k))
                {
                    changes.Add(k);
                }
            }

            var removed = changes.Count > 0;

            if (removed && IsDispatchable())
            {
                var eventArgs = DictionaryChangedEventArgs<TKey, TValue>.Remove(changes, oldValue);
                dispatcher.Dispatch(this, eventArgs);
            }

            return removed;
        }

        /// <summary>
        /// コールバックが返す条件に一致するエントリーを削除し、変更通知としてRemoveイベントを発行します。
        /// 削除されたエントリーがあるかどうかを<c>bool</c>で返します。
        /// </summary>
        /// <param name="match">Match.</param>
        /// <returns>削除されたエントリーがあれば<c>true</c>, 無ければ<c>false</c></returns>
        /// <exception cref="InvalidOperationException">編集中は実行できません</exception>
        public bool Remove(Func<TKey, TValue, bool> match)
        {
            ValidateCallStackChanging();
            ValidateNotEditting();

            var oldValue = ResolveCurrentEntriesIfNeeded();
            var changes = MatchKeys((key, value) => match(key, value) && entries.Remove(key));
            var removed = changes.Count > 0;

            if (removed && IsDispatchable())
            {
                var eventArgs = DictionaryChangedEventArgs<TKey, TValue>.Remove(changes, oldValue);
                dispatcher.Dispatch(this, eventArgs);
            }

            return removed;
        }

        /// <summary>
        /// 登録されているすべてのエントリーを削除し、変更通知としてRemoveイベントを発行します。
        /// </summary>
        /// <exception cref="InvalidOperationException">編集中は実行できません</exception>
        public void Clear()
        {
            ValidateCallStackChanging();
            ValidateNotEditting();

            var oldValue = ResolveCurrentEntriesIfNeeded();
            var changes = new List<TKey>(Keys);

            entries.Clear();

            if (changes.Count > 0 && IsDispatchable())
            {
                var eventArgs = DictionaryChangedEventArgs<TKey, TValue>.Remove(changes, oldValue);
                dispatcher.Dispatch(this, eventArgs);
            }
        }

        #endregion

        #region Protected Method

        /// <summary>
        /// Update など、「既に存在するキーを指定」する前提で、指定いたキーが実際に登録されているかをチェックします
        /// </summary>
        /// <exception cref="ArgumentNullException">指定したキーがnullだった場合</exception>
        /// <exception cref="KeyNotFoundException">指定したキーが登録されていなかった場合</exception>
        /// <param name="key">Key.</param>
        protected virtual void ValidateContainsKey(TKey key)
        {
            ValidateNotNullKey(key);
            if (!entries.ContainsKey(key))
            {
                throw new KeyNotFoundException();
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
        /// イベントを送出できる状態にあるか否かを示します
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
        protected bool AreDifferByChangesPolicy(TValue before, TValue after)
        {
            return changesPolicy == null || changesPolicy(before, after);
        }

        /// <summary>
        /// 呼び出された時点での全てのエントリーの複製を必要に応じて返します。
        /// <see cref="OldValueEnabled"/>が<c>false</c>であった場合<c>null</c>を返します。
        /// </summary>
        /// <returns>The current entries if needed.</returns>
        protected virtual Dictionary<TKey, TValue> ResolveCurrentEntriesIfNeeded()
        {
            return IsDispatchable() && OldValueEnabled ? ResolveCurrentEntries() : null;
        }

        /// <summary>
        /// 呼び出された時点での全てのエントリーを複製します。値が<see cref="ICloneable"/>を実装されていた場合はクローンを格納します。
        /// 更新する処理の前に一旦キャッシュとして前の状態を保持する用途で利用できます。
        /// </summary>
        /// <returns>The current entries.</returns>
        protected virtual Dictionary<TKey, TValue> ResolveCurrentEntries()
        {
            // HACK 非バインド型パラメーターとDictionary.ValueCollectionの仕様上、一回foreachループまわさないとわからない。空でもfalse返す
            var isClonableValue = false;
            if (entries.Count > 0)
            {
                foreach (var item in entries)
                {
                    isClonableValue = item.Value is ICloneable;
                    break;
                }
            }

            if (isClonableValue)
            {
                var ret = new Dictionary<TKey, TValue>();
                foreach (var item in entries)
                {
                    var clonedValue = (TValue) ((ICloneable) item.Value).Clone();
                    ret.Add(item.Key, clonedValue);
                }
                return ret;
            }
            // クローンを複製する必要が無ければそのまま新しいインスタンスを作って返す
            return new Dictionary<TKey, TValue>(entries);
        }

        /// <summary>
        /// 呼び出された時点でのエントリーに登録された値を複製。値が<see cref="ICloneable"/>を実装されていた場合はクローンを返します。
        /// </summary>
        /// <returns>The current value.</returns>
        /// <param name="key">Key.</param>
        protected virtual TValue ResolveCurrentValue(TKey key)
        {
            ValidateContainsKey(key);
            var value = this[key];
            var clone = value as ICloneable;
            return clone == null ? value : (TValue) clone.Clone();
        }

        protected virtual void SetEntry(TKey key, TValue value)
        {
            if (ContainsKey(key))
            {
                Update(key, value);
            }
            else
            {
                Add(key, value);
            }
        }

        protected virtual bool EditSection(EditType type, Func<bool> section)
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

        #region Implement Interfaces Properties

        int ICollection.Count { get { return Count; } }

        bool ICollection.IsSynchronized { get { return false; } }

        object ICollection.SyncRoot { get { return this; } }

        bool IDictionary.IsFixedSize { get { return false; } }

        bool IDictionary.IsReadOnly { get { return false; } }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly { get { return false; } }

        object IDictionary.this[object key]
        {
            get
            {
                ValidateObjectEnableAsKey(key);
                return this[(TKey) key];
            }
            set
            {
                ValidateObjectEnableAsKey(key);
                ValidateObjectEnableAsValue(value);
                this[(TKey) key] = (TValue) value;
            }
        }

        ICollection IDictionary.Keys { get { return entries.Keys; } }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys { get { return entries.Keys; } }

        ICollection IDictionary.Values { get { return entries.Values; } }

        ICollection<TValue> IDictionary<TKey, TValue>.Values { get { return entries.Values; } }

        #endregion

        #region Implement Interfaces Method

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        void IDictionary.Add(object key, object value)
        {
            ValidateObjectEnableAsKey(key);
            ValidateObjectEnableAsValue(value);
            Add((TKey) key, (TValue) value);
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            Add(key, value);
        }

        void IDictionary.Clear()
        {
            Clear();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            Clear();
        }

        bool IDictionary.Contains(object key)
        {
            ValidateObjectEnableAsKey(key);
            return ContainsKey((TKey) key);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>) entries).Contains(item);
        }

        bool IDictionary<TKey, TValue>.ContainsKey(TKey key)
        {
            return ContainsKey(key);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection) entries).CopyTo(array, index);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>) entries).CopyTo(array, index);
        }

        public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
        {
            return entries.GetEnumerator();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary) entries).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return entries.GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return entries.GetEnumerator();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        void IDictionary.Remove(object key)
        {
            ValidateObjectEnableAsKey(key);
            Remove((TKey) key);
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            return Remove(key);
        }

        bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
        {
            return entries.TryGetValue(key, out value);
        }

        #endregion

    }
}


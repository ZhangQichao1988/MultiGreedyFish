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
#pragma warning disable 0109
using System;
using System.Collections.Generic;

namespace Jackpot
{
    /// <summary>
    /// データの変更を監視できるクラスです
    /// </summary>
    /// <remarks>
    /// BindableObjectを利用する事で、各層ロジックの抽象化を図る事ができます
    /// Jackpot.Examplesにもサンプル実装があるので、参考に
    /// 以下は例です(このサンプルだけではあまりメリットが見えないかもですが…)
    /// <code>
    /// using UnityEngine;
    /// using Jackpot;
    ///
    /// public interface IViewModel
    /// {
    ///     BindableObject<string> State { get; }
    /// }
    ///
    /// public class ViewModel : IViewModel
    /// {
    ///     public BindableObject<string> State { get; private set; }
    ///     public ViewModel(string initialState = "none")
    ///     {
    ///         // コンストラクタには初期値を設定します
    ///         State = new BindableObject<string>(initialState);
    ///     }
    /// }
    ///
    /// public class StartCommand
    /// {
    ///     IViewModel viewModel;
    ///     public StartCommand(IViewModel viewModel)
    ///     {
    ///         this.viewModel = viewModel;
    ///     }
    ///
    ///     public void Exec()
    ///     {
    ///         // BindableObjectのValueプロパティに値を設定することで、変更があれば通知が飛びます
    ///         viewModel.State.Value = "start";
    ///     }
    /// }
    ///
    /// public class ViewController : MonoBehaviour
    /// {
    ///     IViewModel viewModel;
    ///     StartCommand command;
    ///
    ///     public void Initialize(IViewModel viewModel)
    ///     {
    ///         this.viewModel = viewModel ?? new ViewModel();
    ///         this.viewModel.State.Bind(OnStateChanged);
    ///         command = new StartCommand(this.viewModel);
    ///     }
    ///
    ///     void Update()
    ///     {
    ///         if (Input.GetButtonUp("Fire1")
    ///         {
    ///             command.Exec();
    ///         }
    ///     }
    ///
    ///     void OnDestroy()
    ///     {
    ///         if (viewModel == null)
    ///         {
    ///             return;
    ///         }
    ///         viewModel.State.Unbind(OnStateChanged);
    ///     }
    ///
    ///     void OnStateChanged(object sender, PropertyChangedEventArgs<string> e)
    ///     {
    ///         Debug.Log(string.Format("state changed: {0} => {1}", e.OldValue, e.NewValue));
    ///     }
    /// }
    ///
    /// </code>
    /// 値の変更ルールを変更したり(<see cref="Jackpot.BindableObject.SetChangesPolicy()"/> )
    /// 一度だけ変更を検知する事もできます(<see cref="Jackpot.BindableObject.BindOnce()"/>)
    /// </remarks>
    public class BindableObject<T>
    {
        #region constants

        static readonly Func<T, T, bool> DefaultChangesPolicy = (oldValue, newValue) =>
        {
            if (oldValue == null && newValue == null)
            {
                return false;
            }
            if ((oldValue == null && newValue != null) || (oldValue != null && newValue == null))
            {
                return true;
            }
            return !oldValue.Equals(newValue);
        };

        #endregion

        #region properties

        /// <summary>
        /// 監視対象の値を示します。このプロパティにセットする事で
        /// 値に変更があった場合は通知処理が実行されます
        /// </summary>
        /// <value>The value.</value>
        public T Value
        {
            get
            {
                return value;
            }
            set
            {
                ValidateCallStackChanging();
                T oldValue = this.value;
                this.value = value;

                if (changesPolicy != null && !changesPolicy(oldValue, value))
                {
                    return;
                }
                if (!IsDispatchable())
                {
                    return;
                }
                dispatcher.Dispatch(this, PropertyChangedEventArgs<T>.Update(oldValue, value));
            }
        }

        /// <summary>
        /// 通知する際のルールを示します<see cref="Jackpot.BindableRules"/>
        /// </summary>
        /// <value>The rule.</value>
        public BindableRules Rule { get; set; }

        T value;
        EventDispatcher<PropertyChangedEventArgs<T>> dispatcher;
        Func<T, T, bool> changesPolicy;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.BindableObject`1"/> class.
        /// </summary>
        public BindableObject() : this(default(T))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.BindableObject`1"/> class.
        /// </summary>
        /// <param name="value">Value.</param>
        public BindableObject(T value) : this(value, BindableRules.NotAllowCallStackChanging)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.BindableObject`1"/> class.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="rule">Rule.</param>
        public BindableObject(T value, BindableRules rule)
        {
            this.value = value;
            Rule = rule;
            dispatcher = new EventDispatcher<PropertyChangedEventArgs<T>>();
            changesPolicy = DefaultChangesPolicy;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.BindableObject`1"/> class.
        /// </summary>
        /// <param name="that">That.</param>
        public BindableObject(BindableObject<T> that) : this(that, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.BindableObject`1"/> class.
        /// </summary>
        /// <param name="that">That.</param>
        /// <param name="withCopyBinding">If set to <c>true</c> with copy binding.</param>
        public BindableObject(BindableObject<T> that, bool withCopyBinding) : this(that, withCopyBinding, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.BindableObject`1"/> class.
        /// </summary>
        /// <param name="that">That.</param>
        /// <param name="withCopyBinding">If set to <c>true</c> with copy binding.</param>
        /// <param name="withCopyValue">If set to <c>true</c> with copy value.</param>
        public BindableObject(BindableObject<T> that, bool withCopyBinding, bool withCopyValue)
        {
            if (withCopyValue && that.value is ICloneable)
            {
                this.value = (T) (that.value as ICloneable).Clone();
            }
            else
            {
                this.value = that.value;
            }
            this.Rule = that.Rule;
            dispatcher = withCopyBinding
                ? new EventDispatcher<PropertyChangedEventArgs<T>>(that.dispatcher)
                : new EventDispatcher<PropertyChangedEventArgs<T>>();
            changesPolicy = that.changesPolicy;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 変更通知を受け取るイベントハンドラを追加します. 重複した場合は再登録されます
        /// </summary>
        /// <remarks>
        /// 追加したイベントハンドラは強い参照で保持されます。
        /// 詳しくは<see cref="EventDispatcher{TEventArgs}.AddHandler"/>を参照してください
        /// </remarks>
        /// <param name="binder">イベントハンドラ</param>
        public void Bind(EventHandler<PropertyChangedEventArgs<T>> binder)
        {
            dispatcher.AddHandler(binder);
        }

        /// <summary>
        /// 変更通知を受け取るイベントハンドラを追加します. 重複した場合は再登録されます
        /// </summary>
        /// <remarks>
        /// 追加したイベントハンドラは弱い参照で保持されます。第一引数が<c>null</c>の場合は強い参照になります
        /// 詳しくは<see cref="EventDispatcher{TEventArgs}.AddHandler"/>を参照してください
        /// </remarks>
        /// <param name="target">ハンドリングの基準になるオブジェクト</param>
        /// <param name="binder">イベントハンドラ</param>
        public void Bind(object target, EventHandler<PropertyChangedEventArgs<T>> binder)
        {
            dispatcher.AddHandler(target, binder);
        }

        /// <summary>
        /// 変更通知を一回だけ受け取るイベントハンドラを追加します. 重複した場合は再登録されます
        /// </summary>
        /// <remarks>
        /// 追加したイベントハンドラは強い参照で保持されます。
        /// 詳しくは<see cref="EventDispatcher{TEventArgs}.AddHandler"/>を参照してください
        /// </remarks>
        /// <param name="binder">イベントハンドラ</param>
        public void BindOnce(EventHandler<PropertyChangedEventArgs<T>> binder)
        {
            dispatcher.AddHandler(binder, true);
        }

        /// <summary>
        /// 変更通知を一回だけ受け取るイベントハンドラを追加します. 重複した場合は再登録されます
        /// </summary>
        /// <remarks>
        /// 追加したイベントハンドラは弱い参照で保持されます。第一引数が<c>null</c>の場合は強い参照になります
        /// 詳しくは<see cref="EventDispatcher{TEventArgs}.AddHandler"/>を参照してください
        /// </remarks>
        /// <param name="target">ハンドリングの基準になるオブジェクト</param>
        /// <param name="binder">イベントハンドラ</param>
        public void BindOnce(object target, EventHandler<PropertyChangedEventArgs<T>> binder)
        {
            dispatcher.AddHandler(target, binder, true);
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
        public bool IsBinding(EventHandler<PropertyChangedEventArgs<T>> binder)
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
        public bool IsBindingOnce(EventHandler<PropertyChangedEventArgs<T>> binder)
        {
            return dispatcher.ContainsHandler(binder, true);
        }

        /// <summary>
        /// 指定のイベントハンドラを登録解除します
        /// </summary>
        /// <param name="binder">Binder.</param>
        public void Unbind(EventHandler<PropertyChangedEventArgs<T>> binder)
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
        /// nullを指定した場合、変更ポリシーはないと判断され、通知可能な状態の時は必ずデータ変更通知が飛びます
        /// </remarks>
        /// <param name="changesPolicy">Changes policy.</param>
        public void SetChangesPolicy(Func<T, T, bool> changesPolicy)
        {
            this.changesPolicy = changesPolicy;
        }

        /// <summary>
        /// BindableRuleを検証しつつ、問題なければ強制的にプロパティ変更イベントを送出します。
        /// </summary>
        /// <remarks>
        /// 通常のデータの変更通知時と同様に、RuleのValidationが行われます。
        /// BindableRules.NotAllowCallStackChangingが指定されている場合、イベント送出中にこのメソッドが実行されたら例外がスローされます。
        /// BindableRules.ThroughCallStackChangingが指定されている場合、イベント送出中にこのメソッドが実行されたら、イベントを送出しません。
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">BindableRules.NotAllowCallStackChangingが指定されている場合、イベント送出中にこのメソッドが実行されたら例外がスローされます。</exception>
        public void ForceUpdate()
        {
            ValidateCallStackChanging();
            if (!IsDispatchable())
            {
                return;
            }
            dispatcher.Dispatch(this, PropertyChangedEventArgs<T>.ForceUpdate(Value));
        }

        /// <summary>
        /// このインスタンスのコピーを生成します。
        /// </summary>
        /// <remarks>
        /// イベントハンドラの設定を引き継きたくない場合はJackpot.BindableObject.Clone(false)とします。
        /// BindableObjectが取り扱うオブジェクトがICloneableを実装している場合、
        /// そのオブジェクトをCloneしたい場合はJackpot.BindableObject.Clone({true|false}, true);とします。
        /// </remarks>
        public virtual object Clone()
        {
            return Clone(true);
        }

        /// <summary>
        /// このインスタンスのコピーを生成します。
        /// </summary>
        /// <param name="withCopyBinding">If set to <c>true</c> with copy binding.</param>
        public virtual object Clone(bool withCopyBinding)
        {
            return Clone(withCopyBinding, false);
        }

        /// <summary>
        /// このインスタンスのコピーを生成します。
        /// </summary>
        /// <param name="withCopyBinding">If set to <c>true</c> with copy binding.</param>
        /// <param name="withCopyValue">If set to <c>true</c> with copy value.</param>
        public virtual object Clone(bool withCopyBinding, bool withCopyValue)
        {
            return new BindableObject<T>(this, withCopyBinding, withCopyValue);
        }

        /// <summary>
        /// BindableObjectが管理している値を文字列出力します
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="Jackpot.BindableObject`1"/>.</returns>
        public override string ToString()
        {
            if (value == null)
            {
                return string.Empty;
            }
            return value.ToString();
        }

        [Obsolete("Deprecated", true)]
        public new bool Equals(object objA, object objB)
        {
            throw new InvalidOperationException("deprecated");
        }

        [Obsolete("Deprecated", true)]
        public new bool Equals(T x, T y)
        {
            throw new InvalidOperationException("deprecated");
        }

        [Obsolete("Deprecated", true)]
        public new int GetHashCode(T obj)
        {
            throw new InvalidOperationException("deprecated");
        }

        #endregion

        #region Protected methods

        protected virtual void ValidateCallStackChanging()
        {
            if (Rule == BindableRules.NotAllowCallStackChanging && dispatcher.IsDispatching)
            {
                throw new InvalidOperationException("A value cannot be changed while dispatching data.");
            }
        }

        protected bool IsDispatchable()
        {
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

        #endregion
    }
}


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
using UnityEngine;
using Jackpot.Extensions;

namespace Jackpot
{
    /// <summary>
    /// EventのHandler管理とEvent送出機能を提供するクラスです
    /// </summary>
    /// <remarks>
    /// GameObjectおよびそれにattachされるMonoBehaviourはライフサイクルが不安定です。
    /// C#のeventはこの不安定なライフサイクルを(当然ながら)考慮していないので
    /// event送出時にGameObjectを参照を失っていて例外を吐いたり
    /// MonoBehaviour上に存在するHandlerが参照できずぬるぽを吐いたりします。
    /// 本来、正しく実装するのであれば、OnDestroy()やデストラクタでRemoveHandlerする必要があります。その実装は大変です。
    /// しかし、EventDispatcherは弱参照(WeakReference)で実装されており
    /// かつハンドラの存在有無やGameObjectMissingをハンドリングするので
    /// よっぽどのHandlerの実装がつらい事になっていなければその実装は必要ありません。
    /// もちろんオブジェクトに所属しない匿名delegateについても、強参照でHandlerを保持するオプションがあるので安心です。
    /// EventDispatcherを使用する事で、その不安定なライフサイクルに対する実装負担を解消する事ができます。
    /// </remarks>
    public class EventDispatcher<TEventArgs> where TEventArgs : EventArgs
    {
        #region Constants

        static readonly ILogger logger = Logger.Get<EventDispatcher<TEventArgs>>();

        #endregion

        #region Fields

        /// <summary>
        /// 登録されているHandlerの数を示します
        /// </summary>
        /// <value>The handlers count.</value>
        public int HandlersCount { get { return references.Count; } }

        /// <summary>
        /// Eventを送出中か否かを示します
        /// </summary>
        /// <value><c>true</c> if this instance is dispatching; otherwise, <c>false</c>.</value>
        public bool IsDispatching { get { return dispatchingCount > 0; } }

        /// <summary>
        /// Handlerが登録されているか否かを示します
        /// </summary>
        /// <value><c>true</c> if handler is empty; otherwise, <c>false</c>.</value>
        public bool HandlerIsEmpty { get { return HandlersCount <= 0; } }

        /// <summary>
        /// 一度だけハンドリングするHandlerが登録されているか否かを示します
        /// </summary>
        /// <value><c>true</c> if once handler is empty; otherwise, <c>false</c>.</value>
        public bool OnceHandlerIsEmpty { get { return references.CountBy(reference => reference.IsHandleOnce) <= 0; } }

        /// <summary>
        /// List<IEventHandler<TEventArgs>>な形にしたいけどfull-aotでcompile error
        /// </summary>
        readonly List<EventHandlerReference<TEventArgs>> references;

        int dispatchingCount;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.EventDispatcher`1"/> class.
        /// </summary>
        public EventDispatcher() : this(new List<EventHandlerReference<TEventArgs>>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.EventDispatcher`1"/> class.
        /// </summary>
        /// <param name="that">That.</param>
        public EventDispatcher(EventDispatcher<TEventArgs> that) : this(that.references)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.EventDispatcher`1"/> class.
        /// </summary>
        /// <param name="references">References.</param>
        EventDispatcher(List<EventHandlerReference<TEventArgs>> references)
        {
            this.references = references;
            dispatchingCount = 0;
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="Jackpot.EventDispatcher`1"/> is reclaimed by garbage collection.
        /// </summary>
        ~EventDispatcher()
        {
            RemoveAllHandlers();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// イベントハンドラを強い参照で登録します。重複した場合は再登録されます
        /// </summary>
        /// <remarks>
        /// イベントハンドラの内部で、外側のスコープのMonoBehaviourの状態を変えるようなコールバックを指定する際は注意してください
        /// 対象のオブジェクト自体が<see cref="UnityEngine.GameObject.Destroy"/>によって参照できない状況があります
        /// そのような場合は、第一引数にオブジェクトを渡すオーバーロードを利用してください
        /// </remarks>
        /// <param name="handler">Handler.</param>
        /// <param name="isHandleOnce">一度だけハンドリングしたい場合は<c>true</c>を設定します</param>
        public void AddHandler(EventHandler<TEventArgs> handler, bool isHandleOnce = false)
        {
            AddHandler(null, handler, isHandleOnce);
        }

        /// <summary>
        /// イベントハンドラを弱い参照で登録します。重複した場合は再登録されます
        /// </summary>
        /// <remarks>
        /// MonoBehaviourのメソッドを登録する際はこちらを指定します.
        /// 第一引数に渡されたオブジェクトがGCやDestroyで参照を失った場合、このイベントハンドラはDispatchされず、自動的に取り除かれます
        /// 第一引数にオブジェクト（gameObjectなど）を、第二引数にラムダ式のdelegateやインスタンスメソッドを渡します
        /// </remarks>
        /// <param name="target">ハンドリングの基準になるオブジェクト</param>
        /// <param name="handler">ラムダ式のdelegateまたはインスタンスメソッド</param>
        /// <param name="isHandleOnce">一度だけハンドリングしたい場合は<c>true</c>を設定します</param>
        public void AddHandler(object target, EventHandler<TEventArgs> handler, bool isHandleOnce = false)
        {
            RemoveHandler(handler);
            if (target == null)
            {
                references.Add(new HardEventHandlerReference<TEventArgs>(handler, isHandleOnce));
            }
            else
            {
                references.Add(new WeakEventHandlerReference<TEventArgs>(target, handler, isHandleOnce));
            }
        }

        /// <summary>
        /// 指定したHandlerが登録されているかを示します
        /// </summary>
        /// <returns><c>true</c>, if handler was containsed, <c>false</c> otherwise.</returns>
        /// <param name="handler">Handler.</param>
        /// <param name="isHandleOnce">一度だけハンドリングするように設定されているかを検証したい場合<c>true</c> を指定します</param>
        public bool ContainsHandler(EventHandler<TEventArgs> handler, bool isHandleOnce = false)
        {
            if (handler == null)
            {
                return false;
            }
            var result = false;
            CallbackWithRemoveNotAlivedReferences(reference =>
            {
                if (reference.Handler != handler)
                {
                    return false;
                }
                if (isHandleOnce && reference.IsHandleOnce != isHandleOnce)
                {
                    return false;
                }
                result = true;
                return true;
            });
            return result;
        }

        /// <summary>
        /// 指定のHandlerの登録を解除します
        /// </summary>
        /// <param name="handler">Handler.</param>
        public void RemoveHandler(EventHandler<TEventArgs> handler)
        {
            CallbackWithRemoveNotAlivedReferences(reference =>
            {
                if (reference.Handler != handler)
                {
                    return false;
                }
                references.Remove(reference);
                return true;
            });
        }

        /// <summary>
        /// 全てのHandlerの登録を解除します
        /// </summary>
        public void RemoveAllHandlers()
        {
            if (references == null)
            {
                return;
            }
            references.Clear();
        }

        /// <summary>
        /// 不要なWeakReferenceを削除します
        /// </summary>
        public void RemoveNotAlivedReferences()
        {
            CallbackWithRemoveNotAlivedReferences();
        }

        /// <summary>
        /// eventを送出します
        /// </summary>
        /// <param name="eventArgs">Event arguments.</param>
        public void Dispatch(TEventArgs eventArgs)
        {
            Dispatch(this, eventArgs);
        }

        /// <summary>
        /// senderを指定してeventを送出します
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="eventArgs">Event arguments.</param>
        public void Dispatch(object sender, TEventArgs eventArgs)
        {
            var forDispatch = new List<EventHandler<TEventArgs>>();
            CallbackWithRemoveNotAlivedReferences(reference =>
            {
                forDispatch.Add(reference.Handler);
                if (reference.IsHandleOnce)
                {
                    references.Remove(reference);
                }
                return false;
            });
            // int.MaxValueまでいくと破綻するけど、さすがにそんな実装する人いないでしょう
            dispatchingCount++;
            var caughtException = false;
            try
            {
                forDispatch.ForEach(handler =>
                {
                    try
                    {
                        handler(sender, eventArgs);
                    }
                    catch (MissingReferenceException e)
                    {
                        // GameObjectが失われていた時にこの例外が投げられる
                        // 一応使い手が検知できるようにログだけ吐いておく
                        logger.Debug(e);
                    }
                });
            }
            catch (Exception e)
            {
                caughtException = true;
                throw e;
            }
            finally
            {
                dispatchingCount = caughtException ? 0 : dispatchingCount - 1;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 不要なWeakReferenceを削除しながらCallbackを実施します
        /// </summary>
        /// <param name="callback">callback内でtrueを返却するとループを停止します</param>
        void CallbackWithRemoveNotAlivedReferences(Predicate<EventHandlerReference<TEventArgs>> callback = null)
        {
            var cache = new List<EventHandlerReference<TEventArgs>>(references);

            foreach (var reference in cache)
            {
                if (!reference.IsAlive)
                {
                    references.Remove(reference);
                    continue;
                }
                if (callback == null)
                {
                    continue;
                }
                if (callback(reference))
                {
                    break;
                }
            }
        }

        #endregion
    }
}

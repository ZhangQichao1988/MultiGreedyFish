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
using System.Threading;
using UnityEngine;

namespace Jackpot
{
    /// <summary>
    /// 指定された処理をメインスレッドで実行するクラスです。
    /// </summary>
    /// <remarks>
    /// <p>使用するためには、メインスレッド上で<see cref="Jackpot.MainThreadDispatcher.Initialize"/>を実施する必要があります</p>
    /// <p>また、このクラスを用いて実行された処理においてハンドルされていない例外が投げられた場合、キャッチされExceptionCallbackが実施されます</p>
    /// </remarks>
    public class MainThreadDispatcher : MonoBehaviour
    {
#region Constants

        static readonly Action<Exception> DefaultExceptionCallback = e => Logger.Get<MainThreadDispatcher>().Error(e);

#endregion

#region Static Properties

        /// <summary>
        /// 実行中に、ハンドルされていないExceptionが投げられた場合、キャッチされ、このメソッドが実行されます
        /// </summary>
        /// <value>The exception callback.</value>
        public static Action<Exception> ExceptionCallback
        {
            private get
            {
                Initialize();
                return exceptionCallback;
            }
            set
            {
                Initialize();
                if (value == null)
                {
                    value = DefaultExceptionCallback;
                }
                exceptionCallback = value;
            }
        }

        public static bool IsMainThread
        {
            get
            {
                Initialize();
                return Thread.CurrentThread.ManagedThreadId == mainThreadId;
            }
        }

        static MainThreadDispatcher Instance
        {
            get
            {
                Initialize();
                return instance;
            }
        }

#endregion

#region Static Fields

        static readonly object lifecycleLock = new object();
        static Action<Exception> exceptionCallback = DefaultExceptionCallback;
        static MainThreadDispatcher instance;
        static bool initialized;
        static bool quitting;
        static int mainThreadId;

#endregion

#region Instance Fields

        ActionQueue queue = new ActionQueue();

#endregion

#region Unity Methods

        void Update()
        {
            if (queue == null)
            {
                return;
            }
            queue.ProcessAll(ExceptionCallback);
        }

        void OnDestroy()
        {
            lock (lifecycleLock)
            {
                instance = null;
                mainThreadId = 0;
                initialized = false;
            }
        }

        void OnApplicationQuit()
        {
            quitting = true;
        }

#endregion

#region Public Methods

        /// <summary>
        /// MainThreadDispatcherを初期化します。メインスレッド上で実施してください
        /// </summary>
        public static void Initialize()
        {
            if (quitting)
            {
                return;
            }
            lock (lifecycleLock)
            {
                if (initialized)
                {
                    return;
                }
                GameObject gameObject = null;
                try
                {
                    gameObject = new GameObject("MainThreadDispatcher");
                    DontDestroyOnLoad(gameObject);
                    instance = gameObject.AddComponent<MainThreadDispatcher>();
                }
                catch
                {
                    if (gameObject != null)
                    {
                        Destroy(gameObject);
                    }
                    instance = null;
                    var e = new Exception("Require a MainThreadDispatcher component create on main thread.");
                    Logger.Get<MainThreadDispatcher>().Fatal(e);
                    throw e;
                }
                mainThreadId = Thread.CurrentThread.ManagedThreadId;
                initialized = true;
            }
        }

        /// <summary>
        /// 指定されたActionはメインスレッド上で実行されます
        /// </summary>
        /// <param name="action">Action.</param>
        public static void Post(Action action)
        {
            var stackTrace = Environment.StackTrace;
            if (quitting)
            {
                return;
            }
            Instance.queue.Enqueue(() =>
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    throw new MainThreadDispatcherException(stackTrace, e);
                }
            });
        }

        /// <summary>
        /// 指定されたEnumeratorはメインスレッドでStartCoroutineを用いて実行されます
        /// </summary>
        /// <param name="enumerator">Enumerator.</param>
        public static void Post(IEnumerator enumerator)
        {
            if (quitting)
            {
                return;
            }
            Instance.queue.Enqueue(() => Instance.StartCoroutine(enumerator));
        }

        /// <summary>
        /// 指定されたアクションは、現在のスレッドがメインスレッドであれば即時実行され、そうでない場合はPostされます
        /// </summary>
        /// <param name="action">Action.</param>
        public static void Send(Action action)
        {
            if (quitting)
            {
                return;
            }
            if (action == null)
            {
                return;
            }
            if (IsMainThread)
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    ExceptionCallback(e);
                }
                return;
            }
            Post(action);
        }

        /// <summary>
        /// 指定されたenumeratorは、現在のスレッドがメインスレッドであれば即時StartCoroutineされ、そうでない場合はPostされます
        /// </summary>
        /// <param name="enumerator">Enumerator.</param>
        public static Coroutine Send(IEnumerator enumerator)
        {
            if (quitting)
            {
                return null;
            }
            if (enumerator == null)
            {
                return null;
            }
            if (IsMainThread)
            {
                return Instance.StartCoroutine(enumerator);
            }
            Post(enumerator);
            return null;
        }

        /// <summary>
        /// メインスレッド外から、指定のアクションがメインスレッド上で実行完了するまで待機したい場合に使用します
        /// </summary>
        /// <remarks>
        /// メインスレッド上からも実行はできますが、そもそもその場合このメソッドを使用する必要はありません
        /// </remarks>
        /// <param name="action">Action.</param>
        public static void SendAndWait(Action action)
        {
            if (quitting)
            {
                return;
            }
            if (action == null)
            {
                return;
            }
            if (IsMainThread)
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    ExceptionCallback(e);
                }
                return;
            }

            using (var completeEvent = new ManualResetEvent(false))
            {
                Post(() =>
                {
                    try
                    {
                        action();
                    }
                    finally
                    {
                        completeEvent.Set();
                    }
                });
                completeEvent.WaitOne();
            }
        }

#endregion
    }
}

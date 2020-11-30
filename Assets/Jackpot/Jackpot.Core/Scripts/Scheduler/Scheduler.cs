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
using UnityEngine;
using System.Collections.Generic;

namespace Jackpot
{
    /// <summary>
    /// 任意のタスクの実行を時間や描画フレーム数を指定して遅延実行できるスケジューラです
    /// </summary>
    /// <remarks>
    /// スケジューラがどのような機能を持っているかの詳細は<see cref="IScheduler"/>を参照してください
    /// 
    /// Schedulerインスタンスはシーンの切替時に自動的に破棄されません
    /// シーン切替時に、稼働しているスケジューラを一括で停止、破棄したい場合に<see cref="Scheduler.Clear"/>を呼び出してください
    /// 
    /// Delay系（一度きりのコールバックを指定した時間やフレーム数で遅延）
    /// <code>
    /// using UnityEngine;
    /// using System;
    /// using Jackpot;
    /// 
    /// class HitEffect : MonoBehaviour
    /// {
    ///     public GameObject prefab;
    /// 
    ///     GameObject effect;
    /// 
    ///     void Start()
    ///     {
    ///         effect = Instantiate(prefab, transform.position, transform.rotation) as GameObject;
    /// 
    ///         // 1度きりのコルーチンを使う代わりにScheduler.DelaySecondsでこのように書くことができます
    ///         Scheduler.DelaySeconds(3.0f, () =>
    ///         {
    ///             Destroy(effect);
    ///         });
    ///     }
    /// }
    /// </code>
    /// 
    /// Interval系（指定した時間の間隔で繰り返し実行）
    /// <code>
    /// using UnityEngine;
    /// using System;
    /// using Jackpot;
    /// 
    /// class Deamon : MonoBehaviour
    /// {
    ///     public float interval;
    ///     IScheduler work;
    /// 
    ///     void Start()
    ///     {
    ///         work = Scheduler.Schedule(interval, worker =>
    ///         {
    ///             // ...
    ///         });
    ///     }
    ///     
    ///     void Update()
    ///     {
    ///         if (Input.touchCount > 0)
    ///         {
    ///             switch (Input.GetTouch(0).phase)
    ///             {
    ///                 case TouchPhase.Began:
    ///                     // 一時停止できます
    ///                     work.Pause();
    ///                     break;
    ///                 case TouchPhase.Ended:
    ///                     // 途中から再開させることができます
    ///                     work.Resume();
    ///                     break;
    ///                 default:
    ///                     break;
    ///             }
    ///         }
    ///     }
    /// 
    ///     void OnDisable()
    ///     {
    ///         if (work != null)
    ///         {
    ///             // 強制的に終了させることもできます
    ///             work.Finish();
    ///         }
    ///     }
    /// }
    /// </code>
    /// </remarks>
    public class Scheduler : MonoBehaviour
    {
#region Field

        List<IScheduler> workers = new List<IScheduler>();
        List<IScheduler> addWorkers = new List<IScheduler>();

#endregion

#region Singleton

        static volatile Scheduler instance;
        static object syncRoot = new System.Object();

        static Scheduler Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new GameObject("Scheduler").AddComponent<Scheduler>();
                            DontDestroyOnLoad(instance.gameObject);
                        }
                    }
                }
                return instance;
            }
        }

#endregion

        /// <summary>
        /// 登録しているタスクを全て消去し、Schedulerシングルトンを破棄します
        /// </summary>
        /// <remarks>
        /// Schedulerインスタンスはシーンの切替時に自動的に破棄されません。
        /// シーン切替時に、稼働しているスケジューラを一括で停止、破棄したい場合にClearを呼び出してください
        /// </remarks>
        public static void Clear()
        {
            if (instance != null)
            {
                instance.workers.Clear();
                instance.addWorkers.Clear();
                Destroy(instance.gameObject);
                instance = null;
            }
        }

#region Unity Method

        void Update()
        {
            var deltaTime = Time.deltaTime;
            var unscaledDeltaTime = Time.unscaledDeltaTime;

            workers.RemoveAll(worker => (worker as SchedulerBase).Update(deltaTime, unscaledDeltaTime));
        }

        void LateUpdate()
        {
            // Schedulerの追加はLateUpdateで行う
            workers.AddRange(addWorkers);
            addWorkers.Clear();
        }

#endregion

#region DelayMilliSeconds

        /// <summary>
        /// 指定したミリ秒数の経過後に遅延実行します
        /// </summary>
        /// <returns><see cref="TimeoutScheduler"/>インスタンス</returns>
        /// <param name="milliSeconds">ミリ秒単位の遅延時間</param>
        /// <param name="action">コールバック</param>
        public static IScheduler DelayMilliSeconds(int milliSeconds, Action action)
        {
            var s = new TimeoutScheduler(milliSeconds, action);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定したミリ秒数の経過後に遅延実行します
        /// </summary>
        /// <returns><see cref="TimeoutScheduler"/>インスタンス</returns>
        /// <param name="milliSeconds">ミリ秒単位の遅延時間</param>
        /// <param name="action">コールバック</param>
        /// <param name="isFollowTimeScale"><see cref="UnityEngine.Time.timeScale"/>に合わせて実行するか</param>
        public static IScheduler DelayMilliSeconds(int milliSeconds, Action action, bool isFollowTimeScale)
        {
            var s = new TimeoutScheduler(milliSeconds, action, isFollowTimeScale);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定したミリ秒数の経過後に遅延実行します
        /// </summary>
        /// <returns><see cref="TimeoutScheduler"/>インスタンス</returns>
        /// <param name="milliSeconds">ミリ秒単位の遅延時間</param>
        /// <param name="action"><see cref="IScheduler"/>を引数に取るコールバック</param>
        public static IScheduler DelayMilliSeconds(int milliSeconds, Action<IScheduler> action)
        {
            var s = new TimeoutScheduler(milliSeconds, action);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定したミリ秒数の経過後に遅延実行します
        /// </summary>
        /// <returns><see cref="TimeoutScheduler"/>インスタンス</returns>
        /// <param name="milliSeconds">ミリ秒単位の遅延時間</param>
        /// <param name="action"><see cref="IScheduler"/>を引数に取るコールバック</param>
        /// <param name="isFollowTimeScale"><see cref="UnityEngine.Time.timeScale"/>に合わせて実行するか</param>
        public static IScheduler DelayMilliSeconds(int milliSeconds, Action<IScheduler> action, bool isFollowTimeScale)
        {
            var s = new TimeoutScheduler(milliSeconds, action, isFollowTimeScale);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定したミリ秒数の経過後に遅延実行します
        /// </summary>
        /// <returns><see cref="TimeoutScheduler"/>インスタンス</returns>
        /// <param name="name"><see cref="UnityEngine.GameObject"/>の名前</param>
        /// <param name="milliSeconds">ミリ秒単位の遅延時間</param>
        /// <param name="action">実行するコールバック</param>
        public static IScheduler DelayMilliSeconds(string name, int milliSeconds, Action action)
        {
            var s = new TimeoutScheduler(name, milliSeconds, action);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定したミリ秒数の経過後に遅延実行します
        /// </summary>
        /// <returns><see cref="TimeoutScheduler"/>インスタンス</returns>
        /// <param name="name"><see cref="UnityEngine.GameObject"/>の名前</param>
        /// <param name="milliSeconds">ミリ秒単位の遅延時間</param>
        /// <param name="action">実行するコールバック</param>
        /// <param name="isFollowTimeScale"><see cref="UnityEngine.Time.timeScale"/>に合わせて実行するか</param>
        public static IScheduler DelayMilliSeconds(
            string name,
            int milliSeconds,
            Action action,
            bool isFollowTimeScale)
        {
            var s = new TimeoutScheduler(name, milliSeconds, action, isFollowTimeScale);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定したミリ秒数の経過後に遅延実行します
        /// </summary>
        /// <returns><see cref="TimeoutScheduler"/>インスタンス</returns>
        /// <param name="name"><see cref="UnityEngine.GameObject"/>の名前</param>
        /// <param name="milliSeconds">ミリ秒単位の遅延時間</param>
        /// <param name="action"><see cref="IScheduler"/>を引数に取るコールバック</param>
        public static IScheduler DelayMilliSeconds(string name, int milliSeconds, Action<IScheduler> action)
        {
            var s = new TimeoutScheduler(name, milliSeconds, action);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定したミリ秒数の経過後に遅延実行します
        /// </summary>
        /// <returns><see cref="TimeoutScheduler"/>インスタンス</returns>
        /// <param name="name"><see cref="UnityEngine.GameObject"/>の名前</param>
        /// <param name="milliSeconds">ミリ秒単位の遅延時間</param>
        /// <param name="action"><see cref="IScheduler"/>を引数に取るコールバック</param>
        /// <param name="isFollowTimeScale"><see cref="UnityEngine.Time.timeScale"/>に合わせて実行するか</param>
        public static IScheduler DelayMilliSeconds(
            string name,
            int milliSeconds,
            Action<IScheduler> action,
            bool isFollowTimeScale)
        {
            var s = new TimeoutScheduler(name, milliSeconds, action, isFollowTimeScale);
            Instance.addWorkers.Add(s);
            return s;
        }

#endregion

#region DelaySeconds

        /// <summary>
        /// 指定した秒数の経過後に遅延実行します。一度きりのコルーチンを利用するのと同じ使い方ができます
        /// </summary>
        /// <returns><see cref="TimeoutScheduler"/>インスタンス</returns>
        /// <param name="seconds">秒単位の遅延時間</param>
        /// <param name="action">コールバック</param>
        public static IScheduler DelaySeconds(float seconds, Action action)
        {
            var s = new TimeoutScheduler(seconds, action);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定した秒数の経過後に遅延実行します。一度きりのコルーチンを利用するのと同じ使い方ができます
        /// </summary>
        /// <returns><see cref="TimeoutScheduler"/>インスタンス</returns>
        /// <param name="seconds">秒単位の遅延時間</param>
        /// <param name="action">コールバック</param>
        /// <param name="isFollowTimeScale"><see cref="UnityEngine.Time.timeScale"/>に合わせて実行するか</param>
        public static IScheduler DelaySeconds(float seconds, Action action, bool isFollowTimeScale)
        {
            var s = new TimeoutScheduler(seconds, action, isFollowTimeScale);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定した秒数の経過後に遅延実行します。一度きりのコルーチンを利用するのと同じ使い方ができます
        /// </summary>
        /// <returns><see cref="TimeoutScheduler"/>インスタンス</returns>
        /// <param name="seconds">秒単位の遅延時間</param>
        /// <param name="action"><see cref="IScheduler"/>を引数に取るコールバック</param>
        public static IScheduler DelaySeconds(float seconds, Action<IScheduler> action)
        {
            var s = new TimeoutScheduler(seconds, action);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定した秒数の経過後に遅延実行します。一度きりのコルーチンを利用するのと同じ使い方ができます
        /// </summary>
        /// <returns><see cref="TimeoutScheduler"/>インスタンス</returns>
        /// <param name="seconds">秒単位の遅延時間</param>
        /// <param name="action"><see cref="IScheduler"/>を引数に取るコールバック</param>
        /// <param name="isFollowTimeScale"><see cref="UnityEngine.Time.timeScale"/>に合わせて実行するか</param>
        public static IScheduler DelaySeconds(float seconds, Action<IScheduler> action, bool isFollowTimeScale)
        {
            var s = new TimeoutScheduler(seconds, action, isFollowTimeScale);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定した秒数の経過後に遅延実行します。一度きりのコルーチンを利用するのと同じ使い方ができます
        /// </summary>
        /// <returns><see cref="TimeoutScheduler"/>インスタンス</returns>
        /// <param name="name"><see cref="UnityEngine.GameObject"/>の名前</param>
        /// <param name="seconds">秒単位の遅延時間</param>
        /// <param name="action">コールバック</param>
        public static IScheduler DelaySeconds(string name, float seconds, Action action)
        {
            var s = new TimeoutScheduler(name, seconds, action);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定した秒数の経過後に遅延実行します。一度きりのコルーチンを利用するのと同じ使い方ができます
        /// </summary>
        /// <returns><see cref="TimeoutScheduler"/>インスタンス</returns>
        /// <param name="name"><see cref="UnityEngine.GameObject"/>の名前</param>
        /// <param name="seconds">秒単位の遅延時間</param>
        /// <param name="action">コールバック</param>
        /// <param name="isFollowTimeScale"><see cref="UnityEngine.Time.timeScale"/>に合わせて実行するか</param>
        public static IScheduler DelaySeconds(string name, float seconds, Action action, bool isFollowTimeScale)
        {
            var s = new TimeoutScheduler(name, seconds, action, isFollowTimeScale);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定した秒数の経過後に遅延実行します。一度きりのコルーチンを利用するのと同じ使い方ができます
        /// </summary>
        /// <returns><see cref="TimeoutScheduler"/>インスタンス</returns>
        /// <param name="name"><see cref="UnityEngine.GameObject"/>の名前</param>
        /// <param name="seconds">秒単位の遅延時間</param>
        /// <param name="action"><see cref="IScheduler"/>を引数に取るコールバック</param>
        public static IScheduler DelaySeconds(string name, float seconds, Action<IScheduler> action)
        {
            var s = new TimeoutScheduler(name, seconds, action);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定した秒数の経過後に遅延実行します。一度きりのコルーチンを利用するのと同じ使い方ができます
        /// </summary>
        /// <returns><see cref="TimeoutScheduler"/>インスタンス</returns>
        /// <param name="name"><see cref="UnityEngine.GameObject"/>の名前</param>
        /// <param name="seconds">秒単位の遅延時間</param>
        /// <param name="action"><see cref="IScheduler"/>を引数に取るコールバック</param>
        /// <param name="isFollowTimeScale"><see cref="UnityEngine.Time.timeScale"/>に合わせて実行するか</param>
        public static IScheduler DelaySeconds(
            string name,
            float seconds,
            Action<IScheduler> action,
            bool isFollowTimeScale)
        {
            var s = new TimeoutScheduler(name, seconds, action, isFollowTimeScale);
            Instance.addWorkers.Add(s);
            return s;
        }

#endregion

#region DelayFrame

        /// <summary>
        /// １フレーム描画後に遅延実行します
        /// </summary>
        /// <returns><see cref="DelayFrameScheduler"/>インスタンス</returns>
        /// <param name="action">コールバック</param>
        public static IScheduler DelayFrame(Action action)
        {
            var s = new DelayFrameScheduler(action);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// １フレーム描画後に遅延実行します
        /// </summary>
        /// <returns><see cref="DelayFrameScheduler"/>インスタンス</returns>
        /// <param name="action"><see cref="IScheduler"/>を引数に取るコールバック</param>
        public static IScheduler DelayFrame(Action<IScheduler> action)
        {
            var s = new DelayFrameScheduler(action);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定したフレーム数の描画後に遅延実行します
        /// </summary>
        /// <returns><see cref="DelayFrameScheduler"/>インスタンス</returns>
        /// <param name="frameCount">遅延させるフレーム描画数</param>
        /// <param name="action">コールバック</param>
        public static IScheduler DelayFrame(int frameCount, Action action)
        {
            var s = new DelayFrameScheduler(frameCount, action);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定したフレーム数の描画後に遅延実行します
        /// </summary>
        /// <returns><see cref="DelayFrameScheduler"/>インスタンス</returns>
        /// <param name="frameCount">遅延させるフレーム描画数</param>
        /// <param name="action"><see cref="IScheduler"/>を引数に取るコールバック</param>
        public static IScheduler DelayFrame(int frameCount, Action<IScheduler> action)
        {
            var s = new DelayFrameScheduler(frameCount, action);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// １フレーム描画後に遅延実行します
        /// </summary>
        /// <returns><see cref="DelayFrameScheduler"/>インスタンス</returns>
        /// <param name="name"><see cref="UnityEngine.GameObject"/>の名前</param>
        /// <param name="action">コールバック</param>
        public static IScheduler DelayFrame(string name, Action action)
        {
            var s = new DelayFrameScheduler(name, action);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// １フレーム描画後に遅延実行します
        /// </summary>
        /// <returns><see cref="DelayFrameScheduler"/>インスタンス</returns>
        /// <param name="name"><see cref="UnityEngine.GameObject"/>の名前</param>
        /// <param name="action"><see cref="IScheduler"/>を引数に取るコールバック</param>
        public static IScheduler DelayFrame(string name, Action<IScheduler> action)
        {
            var s = new DelayFrameScheduler(name, action);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定したフレーム数の描画後に遅延実行します
        /// </summary>
        /// <returns><see cref="DelayFrameScheduler"/>インスタンス</returns>
        /// <param name="name"><see cref="UnityEngine.GameObject"/>の名前</param>
        /// <param name="frameCount">遅延させるフレーム描画数</param>
        /// <param name="action">コールバック/param>
        public static IScheduler DelayFrame(string name, int frameCount, Action action)
        {
            var s = new DelayFrameScheduler(name, frameCount, action);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定したフレーム数の描画後に遅延実行します
        /// </summary>
        /// <returns><see cref="DelayFrameScheduler"/>インスタンス</returns>
        /// <param name="name"><see cref="UnityEngine.GameObject"/>の名前</param>
        /// <param name="frameCount">遅延させるフレーム描画数</param>
        /// <param name="action"><see cref="IScheduler"/>を引数に取るコールバック/param>
        public static IScheduler DelayFrame(string name, int frameCount, Action<IScheduler> action)
        {
            var s = new DelayFrameScheduler(name, frameCount, action);
            Instance.addWorkers.Add(s);
            return s;
        }

#endregion

#region Schedule

        /// <summary>
        /// 指定した間隔で繰り返し実行します
        /// </summary>
        /// <remarks>
        /// 一時停止させる場合は<see cref="IScheduler.Pause"/>を、終了させる場合には<see cref="IScheduler.Finish"/>を利用してください
        /// </remarks>
        /// <param name="interval">秒単位の時間間隔</param>
        /// <param name="action">コールバック</param>
        public static IScheduler Schedule(float interval, Action action)
        {
            var s = new IntervalScheduler(interval, action);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定した間隔で繰り返し実行します
        /// </summary>
        /// <remarks>
        /// 一時停止させる場合は<see cref="IScheduler.Pause"/>を、終了させる場合には<see cref="IScheduler.Finish"/>を利用してください
        /// </remarks>
        /// <param name="interval">秒単位の時間間隔</param>
        /// <param name="action">コールバック</param>
        /// <param name="isFollowTimeScale"><see cref="UnityEngine.Time.timeScale"/>に合わせて実行するか</param>
        public static IScheduler Schedule(float interval, Action action, bool isFollowTimeScale)
        {
            var s = new IntervalScheduler(interval, action, isFollowTimeScale);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定した間隔で繰り返し実行します
        /// </summary>
        /// <remarks>
        /// 一時停止させる場合は<see cref="IScheduler.Pause"/>を、終了させる場合には<see cref="IScheduler.Finish"/>を利用してください
        /// </remarks>
        /// <param name="interval">秒単位の時間間隔</param>
        /// <param name="action"><see cref="IScheduler"/>を引数に取るコールバック</param>
        public static IScheduler Schedule(float interval, Action<IScheduler> action)
        {
            var s = new IntervalScheduler(interval, action);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定した間隔で繰り返し実行します
        /// </summary>
        /// <remarks>
        /// 一時停止させる場合は<see cref="IScheduler.Pause"/>を、終了させる場合には<see cref="IScheduler.Finish"/>を利用してください
        /// </remarks>
        /// <param name="interval">秒単位の時間間隔</param>
        /// <param name="action"><see cref="IScheduler"/>を引数に取るコールバック</param>
        /// <param name="isFollowTimeScale"><see cref="UnityEngine.Time.timeScale"/>に合わせて実行するか</param>
        public static IScheduler Schedule(float interval, Action<IScheduler> action, bool isFollowTimeScale)
        {
            var s = new IntervalScheduler(interval, action, isFollowTimeScale);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定した間隔で繰り返し実行します
        /// </summary>
        /// <remarks>
        /// 一時停止させる場合は<see cref="IScheduler.Pause"/>を、終了させる場合には<see cref="IScheduler.Finish"/>を利用してください
        /// </remarks>
        /// <param name="name"><see cref="UnityEngine.GameObject"/>の名前</param>
        /// <param name="interval">秒単位の時間間隔</param>
        /// <param name="action">コールバック</param>
        public static IScheduler Schedule(string name, float interval, Action action)
        {
            var s = new IntervalScheduler(name, interval, action);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定した間隔で繰り返し実行します
        /// </summary>
        /// <remarks>
        /// 一時停止させる場合は<see cref="IScheduler.Pause"/>を、終了させる場合には<see cref="IScheduler.Finish"/>を利用してください
        /// </remarks>
        /// <param name="name"><see cref="UnityEngine.GameObject"/>の名前</param>
        /// <param name="interval">秒単位の時間間隔</param>
        /// <param name="action">コールバック</param>
        /// <param name="isFollowTimeScale"><see cref="UnityEngine.Time.timeScale"/>に合わせて実行するか</param>
        public static IScheduler Schedule(string name, float interval, Action action, bool isFollowTimeScale)
        {
            var s = new IntervalScheduler(name, interval, action, isFollowTimeScale);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定した間隔で繰り返し実行します
        /// </summary>
        /// <remarks>
        /// 一時停止させる場合は<see cref="IScheduler.Pause"/>を、終了させる場合には<see cref="IScheduler.Finish"/>を利用してください
        /// </remarks>
        /// <param name="name"><see cref="UnityEngine.GameObject"/>の名前</param>
        /// <param name="interval">秒単位の時間間隔</param>
        /// <param name="action"><see cref="IScheduler"/>を引数に取るコールバック</param>
        public static IScheduler Schedule(string name, float interval, Action<IScheduler> action)
        {
            var s = new IntervalScheduler(name, interval, action);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定した間隔で繰り返し実行します
        /// </summary>
        /// <remarks>
        /// 一時停止させる場合は<see cref="IScheduler.Pause"/>を、終了させる場合には<see cref="IScheduler.Finish"/>を利用してください
        /// </remarks>
        /// <param name="name"><see cref="UnityEngine.GameObject"/>の名前</param>
        /// <param name="interval">秒単位の時間間隔</param>
        /// <param name="action"><see cref="IScheduler"/>を引数に取るコールバック</param>
        /// <param name="isFollowTimeScale"><see cref="UnityEngine.Time.timeScale"/>に合わせて実行するか</param>
        public static IScheduler Schedule(
            string name,
            float interval,
            Action<IScheduler> action,
            bool isFollowTimeScale)
        {
            var s = new IntervalScheduler(name, interval, action, isFollowTimeScale);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定した間隔で繰り返し実行します
        /// </summary>
        /// <remarks>
        /// 一時停止させる場合は<see cref="IScheduler.Pause"/>を、終了させる場合には<see cref="IScheduler.Finish"/>を利用してください
        /// </remarks>
        /// <param name="interval"><see cref="TimeSpan"/>で指定する繰り返し実行する時間間隔</param>
        /// <param name="action">コールバック</param>
        public static IScheduler Schedule(TimeSpan interval, Action action)
        {
            var s = new IntervalScheduler(interval, action);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定した間隔で繰り返し実行します
        /// </summary>
        /// <remarks>
        /// 一時停止させる場合は<see cref="IScheduler.Pause"/>を、終了させる場合には<see cref="IScheduler.Finish"/>を利用してください
        /// </remarks>
        /// <param name="interval"><see cref="TimeSpan"/>で指定する繰り返し実行する時間間隔</param>
        /// <param name="action">コールバック</param>
        /// <param name="isFollowTimeScale"><see cref="UnityEngine.Time.timeScale"/>に合わせて実行するか</param>
        public static IScheduler Schedule(TimeSpan interval, Action action, bool isFollowTimeScale)
        {
            var s = new IntervalScheduler(interval, action, isFollowTimeScale);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定した間隔で繰り返し実行します
        /// </summary>
        /// <remarks>
        /// 一時停止させる場合は<see cref="IScheduler.Pause"/>を、終了させる場合には<see cref="IScheduler.Finish"/>を利用してください
        /// </remarks>
        /// <param name="interval"><see cref="TimeSpan"/>で指定する繰り返し実行する時間間隔</param>
        /// <param name="action"><see cref="IScheduler"/>を引数に取るコールバック</param>
        public static IScheduler Schedule(TimeSpan interval, Action<IScheduler> action)
        {
            var s = new IntervalScheduler(interval, action);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定した間隔で繰り返し実行します
        /// </summary>
        /// <remarks>
        /// 一時停止させる場合は<see cref="IScheduler.Pause"/>を、終了させる場合には<see cref="IScheduler.Finish"/>を利用してください
        /// </remarks>
        /// <param name="interval"><see cref="TimeSpan"/>で指定する繰り返し実行する時間間隔</param>
        /// <param name="action"><see cref="IScheduler"/>を引数に取るコールバック</param>
        /// <param name="isFollowTimeScale"><see cref="UnityEngine.Time.timeScale"/>に合わせて実行するか</param>
        public static IScheduler Schedule(TimeSpan interval, Action<IScheduler> action, bool isFollowTimeScale)
        {
            var s = new IntervalScheduler(interval, action, isFollowTimeScale);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定した間隔で繰り返し実行します
        /// </summary>
        /// <remarks>
        /// 一時停止させる場合は<see cref="IScheduler.Pause"/>を、終了させる場合には<see cref="IScheduler.Finish"/>を利用してください
        /// </remarks>
        /// <param name="name"><see cref="UnityEngine.GameObject"/>の名前</param>
        /// <param name="interval"><see cref="TimeSpan"/>で指定する繰り返し実行する時間間隔</param>
        /// <param name="action">Action.</param>
        public static IScheduler Schedule(string name, TimeSpan interval, Action action)
        {
            var s = new IntervalScheduler(name, interval, action);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定した間隔で繰り返し実行します
        /// </summary>
        /// <remarks>
        /// 一時停止させる場合は<see cref="IScheduler.Pause"/>を、終了させる場合には<see cref="IScheduler.Finish"/>を利用してください
        /// </remarks>
        /// <param name="name"><see cref="UnityEngine.GameObject"/>の名前</param>
        /// <param name="interval"><see cref="TimeSpan"/>で指定する繰り返し実行する時間間隔</param>
        /// <param name="action">Action.</param>
        /// <param name="isFollowTimeScale"><see cref="UnityEngine.Time.timeScale"/>に合わせて実行するか</param>
        public static IScheduler Schedule(string name, TimeSpan interval, Action action, bool isFollowTimeScale)
        {
            var s = new IntervalScheduler(name, interval, action, isFollowTimeScale);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定した間隔で繰り返し実行します
        /// </summary>
        /// <remarks>
        /// 一時停止させる場合は<see cref="IScheduler.Pause"/>を、終了させる場合には<see cref="IScheduler.Finish"/>を利用してください
        /// </remarks>
        /// <param name="name"><see cref="UnityEngine.GameObject"/>の名前</param>
        /// <param name="interval"><see cref="TimeSpan"/>で指定する繰り返し実行する時間間隔</param>
        /// <param name="action"><see cref="IScheduler"/>を引数に取るコールバック</param>
        public static IScheduler Schedule(string name, TimeSpan interval, Action<IScheduler> action)
        {
            var s = new IntervalScheduler(name, interval, action);
            Instance.addWorkers.Add(s);
            return s;
        }

        /// <summary>
        /// 指定した間隔で繰り返し実行します
        /// </summary>
        /// <remarks>
        /// 一時停止させる場合は<see cref="IScheduler.Pause"/>を、終了させる場合には<see cref="IScheduler.Finish"/>を利用してください
        /// </remarks>
        /// <param name="name"><see cref="UnityEngine.GameObject"/>の名前</param>
        /// <param name="interval"><see cref="TimeSpan"/>で指定する繰り返し実行する時間間隔</param>
        /// <param name="action"><see cref="IScheduler"/>を引数に取るコールバック</param>
        /// <param name="isFollowTimeScale"><see cref="UnityEngine.Time.timeScale"/>に合わせて実行するか</param>
        public static IScheduler Schedule(
            string name,
            TimeSpan interval,
            Action<IScheduler> action,
            bool isFollowTimeScale)
        {
            var s = new IntervalScheduler(name, interval, action, isFollowTimeScale);
            Instance.addWorkers.Add(s);
            return s;
        }

#endregion
    }
}

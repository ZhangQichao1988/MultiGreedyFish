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

namespace Jackpot
{
    /// <summary>
    /// 時間の経過を元に、コールバックを実行する<see cref="IScheduler"/>の実装クラスです
    /// </summary>
    public class TimeoutScheduler : SchedulerBase
    {
        /// <summary>
        /// シーンのヒエラルキーに表示される、デフォルトのGameObjectの名前です
        /// </summary>
        public static readonly string DefaultName = "Scheduler Delay by Time";

        public static readonly bool DefaultIsFollowTimeScale = false;

        /// <summary>
        /// 実行を遅延する秒単位の指定時間を示します
        /// </summary>
        public float Delay { get; private set; }

        /// <summary>
        /// 稼働中の待ち時間の合計を秒単位で示します
        /// </summary>
        /// <value>The total delta time.</value>
        public float TotalDeltaTime { get; private set; }

        #region Constructor

        public TimeoutScheduler(float seconds, Action action) : this(
                DefaultName,
                seconds,
                action,
                DefaultIsFollowTimeScale)
        {
        }

        public TimeoutScheduler(float seconds, Action action, bool isFollowTimeScale) : this(
                DefaultName,
                seconds,
                action,
                isFollowTimeScale)
        {
        }

        public TimeoutScheduler(float seconds, Action<IScheduler> action) : this(
                DefaultName,
                seconds,
                action,
                DefaultIsFollowTimeScale)
        {
        }

        public TimeoutScheduler(float seconds, Action<IScheduler> action, bool isFollowTimeScale) : this(
                DefaultName,
                seconds,
                action,
                isFollowTimeScale)
        {
        }

        public TimeoutScheduler(string name, float seconds, Action action) : this(
                name,
                seconds,
                that => action(),
                DefaultIsFollowTimeScale)
        {
        }

        public TimeoutScheduler(string name, float seconds, Action action, bool isFollowTimeScale) : this(
                name,
                seconds,
                that => action(),
                isFollowTimeScale)
        {
        }

        public TimeoutScheduler(string name, float seconds, Action<IScheduler> action) : this(
                name,
                seconds,
                action,
                DefaultIsFollowTimeScale)
        {
        }

        public TimeoutScheduler(string name, float seconds, Action<IScheduler> action, bool isFollowTimeScale)
        {
            State = SchedulerState.Working;
            Delay = seconds;
            Name = name;
            this.action = action;
            IsFollowTimeScale = isFollowTimeScale;
        }

        public TimeoutScheduler(int milliSeconds, Action action) : this(
                DefaultName,
                milliSeconds,
                action,
                DefaultIsFollowTimeScale)
        {
        }

        public TimeoutScheduler(int milliSeconds, Action action, bool isFollowTimeScale) : this(
                DefaultName,
                milliSeconds,
                action,
                isFollowTimeScale)
        {
        }

        public TimeoutScheduler(int milliSeconds, Action<IScheduler> action) : this(
                DefaultName,
                milliSeconds,
                action,
                DefaultIsFollowTimeScale)
        {
        }

        public TimeoutScheduler(int milliSeconds, Action<IScheduler> action, bool isFollowTimeScale) : this(
                DefaultName,
                milliSeconds,
                action,
                isFollowTimeScale)
        {
        }

        public TimeoutScheduler(string name, int milliSeconds, Action action) : this(
                name,
                milliSeconds,
                that => action(),
                DefaultIsFollowTimeScale)
        {
        }

        public TimeoutScheduler(string name, int milliSeconds, Action action, bool isFollowTimeScale) : this(
                name,
                milliSeconds,
                that => action(),
                isFollowTimeScale)
        {
        }

        public TimeoutScheduler(string name, int milliSeconds, Action<IScheduler> action) : this(
                name,
                milliSeconds,
                action,
                DefaultIsFollowTimeScale)
        {
        }

        public TimeoutScheduler(string name, int milliSeconds, Action<IScheduler> action, bool isFollowTimeScale) : this(
                name,
                milliSeconds / 1000f,
                action,
                isFollowTimeScale)
        {
        }

        #endregion

        #region override SchedulerBase

        protected override void Reset()
        {
            TotalDeltaTime = 0;
        }

        protected override void Work(float delta)
        {
            TotalDeltaTime += delta;
            if (TotalDeltaTime >= Delay)
            {
                try
                {
                    action(this);
                }
                catch (Exception e)
                {
                    ExceptionCallback(e);
                }
                Finish();
            }
        }

        #endregion
    }
}

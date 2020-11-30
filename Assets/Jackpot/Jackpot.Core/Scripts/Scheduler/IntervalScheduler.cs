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
using Jackpot;

namespace Jackpot
{
    /// <summary>
    /// <see cref="TimeSpan"/>をもとに、繰り返しコールバックを実行する<see cref="IScheduler"/>実装クラスです
    /// </summary>
    /// <remarks>
    /// <see cref="IScheduler.Pause"/>、または<see cref="IScheduler.Finish"/>を呼ぶまで繰り返し実行されます
    /// </remarks>
    public class IntervalScheduler : SchedulerBase
    {
        /// <summary>
        /// シーンのヒエラルキーに表示される、デフォルトのGameObjectの名前です
        /// </summary>
        public static readonly string DefaultName = "Scheduler Schedule";

        public static readonly bool DefaultIsFollowTimeScale = false;

        /// <summary>
        /// 繰り返し実行する時間間隔を示します
        /// </summary>
        public TimeSpan Interval { get; private set; }

        /// <summary>
        /// 時間間隔の秒単位の表現を示します
        /// </summary>
        /// <value>The seconds.</value>
        public float Seconds
        {
            get
            {
                return (float) Interval.TotalSeconds;
            }
            private set
            {
                Interval = TimeSpan.FromSeconds(value);
            }
        }

        /// <summary>
        /// 稼働中の待ち時間の累積を秒単位で示します
        /// </summary>
        /// <value>The stack time.</value>
        public float StackTime { get; private set; }

        #region Constructor

        public IntervalScheduler(float seconds, Action action) : this(
                DefaultName,
                seconds,
                action,
                DefaultIsFollowTimeScale)
        {
        }

        public IntervalScheduler(float seconds, Action action, bool isFollowTimeScale) : this(
                DefaultName,
                seconds,
                action,
                isFollowTimeScale)
        {
        }

        public IntervalScheduler(float seconds, Action<IScheduler> action) : this(
                DefaultName,
                seconds,
                action,
                DefaultIsFollowTimeScale)
        {
        }

        public IntervalScheduler(float seconds, Action<IScheduler> action, bool isFollowTimeScale) : this(
                DefaultName,
                seconds,
                action,
                isFollowTimeScale)
        {
        }

        public IntervalScheduler(string name, float seconds, Action action) : this(
                name,
                seconds,
                that => action(),
                DefaultIsFollowTimeScale)
        {
        }

        public IntervalScheduler(string name, float seconds, Action action, bool isFollowTimeScale) : this(
                name,
                seconds,
                that => action(),
                isFollowTimeScale)
        {
        }

        public IntervalScheduler(string name, float seconds, Action<IScheduler> action) : this(
                name,
                seconds,
                action,
                DefaultIsFollowTimeScale)
        {
        }


        public IntervalScheduler(string name, float seconds, Action<IScheduler> action, bool isFollowTimeScale)
        {
            State = SchedulerState.Working;
            Name = name;
            Seconds = seconds;
            this.action = action;
            IsFollowTimeScale = isFollowTimeScale;
        }

        public IntervalScheduler(TimeSpan interval, Action action) : this(
                DefaultName,
                interval,
                action,
                DefaultIsFollowTimeScale)
        {
        }

        public IntervalScheduler(TimeSpan interval, Action action, bool isFollowTimeScale) : this(
                DefaultName,
                interval,
                action,
                isFollowTimeScale)
        {
        }

        public IntervalScheduler(TimeSpan interval, Action<IScheduler> action) : this(
                DefaultName,
                interval,
                action,
                DefaultIsFollowTimeScale)
        {
        }

        public IntervalScheduler(TimeSpan interval, Action<IScheduler> action, bool isFollowTimeScale) : this(
                DefaultName,
                interval,
                action,
                isFollowTimeScale)
        {
        }

        public IntervalScheduler(string name, TimeSpan interval, Action action) : this(
                name,
                interval,
                that => action(),
                DefaultIsFollowTimeScale)
        {
        }

        public IntervalScheduler(string name, TimeSpan interval, Action action, bool isFollowTimeScale) : this(
                name,
                interval,
                that => action(),
                isFollowTimeScale)
        {
        }

        public IntervalScheduler(string name, TimeSpan interval, Action<IScheduler> action) : this(
                name,
                interval,
                action,
                DefaultIsFollowTimeScale)
        {
        }

        public IntervalScheduler(string name, TimeSpan interval, Action<IScheduler> action, bool isFollowTimeScale)
        {
            State = SchedulerState.Working;
            Name = name;
            Interval = interval;
            this.action = action;
            IsFollowTimeScale = isFollowTimeScale;
        }

        #endregion

        #region override SchedulerBase

        protected override void Reset()
        {
            StackTime = 0;
        }

        protected override void Work(float delta)
        {
            StackTime += delta;
            if (StackTime >= Seconds)
            {
                try
                {
                    action(this);
                }
                catch (Exception e)
                {
                    ExceptionCallback(e);
                }
                StackTime -= Seconds;
            }
        }

        #endregion
    }
}

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
    /// フレーム描画数をもとに、遅延してコールバックを実行する<see cref="IScheduler"/>実装クラスです
    /// </summary>
    public class DelayFrameScheduler : SchedulerBase
    {
        /// <summary>
        /// シーンのヒエラルキーに表示される、デフォルトのGameObjectの名前です
        /// </summary>
        public static readonly string DefaultName = "Scheduler Delay by FrameCount";

        /// <summary>
        /// 実行を遅延するフレーム描画数を示します
        /// </summary>
        public int Delay { get; private set; }

        /// <summary>
        /// 稼働中に待機したフレーム描画数を示します
        /// </summary>
        /// <value>The frame count.</value>
        public int FrameCount { get; private set; }

        #region Constructor

        public DelayFrameScheduler(Action action) : this(1, action)
        {
        }

        public DelayFrameScheduler(Action<IScheduler> action) : this(1, action)
        {
        }

        public DelayFrameScheduler(int frameCount, Action action) : this(DefaultName, frameCount, action)
        {
        }

        public DelayFrameScheduler(int frameCount, Action<IScheduler> action) : this(DefaultName, frameCount, action)
        {
        }

        public DelayFrameScheduler(string name, Action action) : this(name, 1, action)
        {
        }

        public DelayFrameScheduler(string name, Action<IScheduler> action) : this(name, 1, action)
        {
        }

        public DelayFrameScheduler(string name, int frameCount, Action action) : this(name, frameCount, that => action())
        {
        }

        public DelayFrameScheduler(string name, int frameCount, Action<IScheduler> action)
        {
            State = SchedulerState.Working;
            Delay = frameCount;
            Name = name;
            this.action = action;
        }

        #endregion

        #region override SchedulerBase

        protected override void Reset()
        {
            FrameCount = 0;
        }

        protected override void Work(float delta)
        {
            FrameCount++;
            if (FrameCount >= Delay)
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

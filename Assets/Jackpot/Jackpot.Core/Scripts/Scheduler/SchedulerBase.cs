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

namespace Jackpot
{
    /// <summary>
    /// ISchedulerを実装している基本のクラスです
    /// </summary>
    public class SchedulerBase : IScheduler
    {
        static readonly Action<Exception> DefaultExceptionCallback = e => Logger.Get<SchedulerBase>().Error(e);

        #region Properties

        /// <summary>
        /// Schedulerの現在の状態を示します
        /// </summary>
        /// <value>The state.</value>
        public SchedulerState State { get; protected set; }

        /// <summary>
        /// SchedulerがUnityの<see cref="UnityEngine.Time.timeScale"/>に合わせて実行するかを示します
        /// </summary>
        /// <remarks>
        /// <c>true</c>の場合、<see cref="UnityEngine.Time.timeScale"/>の値を<c>0</c>とするとSchedulerも動かなくなります。
        /// timeScaleに影響しない動作をさせたい場合は<c>false</c>に指定してください
        /// </remarks>
        /// <value><c>true</c> if this instance is follow time scale; otherwise, <c>false</c>.</value>
        public bool IsFollowTimeScale { get; protected set; }

        /// <summary>
        /// スケジューラのタスク名を示します
        /// </summary>
        /// <remarks>ファクトリメソッドなどで、明示的に名前をつけずに生成されたIScheduler実装クラスのインスタンスは、クラスが持つデフォルト名になります</remarks>
        /// <value>The name.</value>
        public string Name { get; protected set; }


        /// <summary>
        /// Scheduler実行中に、Exceptionが投げられた場合に実行されるコールバックを示します
        /// </summary>
        /// <value>The exception callback.</value>
        public Action<Exception> ExceptionCallback
        {
            protected get
            {
                return exceptionCallback;
            }
            set
            {
                if (value == null)
                {
                    value = DefaultExceptionCallback;
                }
                exceptionCallback = value;
            }
        }

        #endregion

        #region Fields

        protected Action<IScheduler> action;

        Action<Exception> exceptionCallback = DefaultExceptionCallback;
        long timeTicks;


        #endregion

        public SchedulerBase()
        {
            timeTicks = DateTime.Now.Ticks;
        }

        /// <summary>
        /// <see cref="UnityEngine.Time"/>からフレーム更新差分の秒数を受け取り、スケジューラの状態を更新します
        /// </summary>
        /// <remarks>
        /// スケジューラを実働させる都合上インターフェースとして宣言していますが、ライブラリ利用者はこれを呼び出さないようにしてください
        /// </remarks>
        /// <param name="delta">UnityのtimeScaleに合ったフレーム更新差分秒数</param>
        /// <param name="unscaled">UnityのtimeScaleに合わせないフレーム更新差分秒数</param>
        /// <returns>スケジューラの状態が完了状態であれば<c>true</c>、継続中であれば<c>false</c></returns>
        public bool Update(float delta, float unscaled)
        {
            if (timeTicks > 0)
            {
                var addDelta = (float) (DateTime.Now.Ticks - timeTicks) / TimeSpan.TicksPerSecond;
                delta += addDelta;
                unscaled += addDelta;
                timeTicks = 0;
            }

            if (State == SchedulerState.Working)
            {
                Work(IsFollowTimeScale ? delta : unscaled);
            }
            return State == SchedulerState.Done;
        }

        #region IScheduler implementation

        public void Pause()
        {
            if (State != SchedulerState.Done)
            {
                State = SchedulerState.Waiting;
            }
        }

        public void Resume()
        {
            if (State != SchedulerState.Done)
            {
                State = SchedulerState.Working;
            }
        }

        public void Restart()
        {
            if (State != SchedulerState.Done)
            {
                Reset();
                State = SchedulerState.Working;
            }
        }

        public void Finish()
        {
            action = null;
            State = SchedulerState.Done;
        }

        public void FinishWithAction()
        {
            if (action != null && State != SchedulerState.Done)
            {
                try
                {
                    action(this);
                }
                catch (Exception e)
                {
                    ExceptionCallback(e);
                }
            }
            Finish();
        }

        #endregion

        #region Protected Method

        /// <summary>
        /// <see cref="Restart"/>を呼ばれた時の処理を示します
        /// </summary>
        protected virtual void Reset()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="State"/>が<see cref="SchedulerState.Working"/>であるときに、<see cref="Update"/>で行われる処理を示します
        /// </summary>
        /// <param name="delta">前フレーム描画からの経過時間として秒単位で受け取ります</param>
        protected virtual void Work(float delta)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

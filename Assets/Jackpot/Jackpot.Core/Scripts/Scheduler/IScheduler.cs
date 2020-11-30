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
    public interface IScheduler
    {
        /// <summary>
        /// Schedulerの現在の状態を示します
        /// </summary>
        /// <value>The state.</value>
        SchedulerState State { get; }

        /// <summary>
        /// スケジューラのタスク名を示します
        /// </summary>
        /// <remarks>
        /// ファクトリメソッドなどで、明示的に名前をつけずに生成されたIScheduler実装クラスのインスタンスは、クラスが持つデフォルト名になります
        /// </remarks>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// SchedulerがUnityの<see cref="UnityEngine.Time.timeScale"/>に合わせて実行するかを示します
        /// </summary>
        /// <remarks>
        /// <c>true</c>の場合、<see cref="UnityEngine.Time.timeScale"/>の値を<c>0</c>とするとSchedulerも動かなくなります。
        /// timeScaleに影響しない動作をさせたい場合は<c>false</c>に指定してください
        /// </remarks>
        /// <value><c>true</c> if this instance is follow time scale; otherwise, <c>false</c>.</value>
        bool IsFollowTimeScale { get; }

        /// <summary>
        /// Scheduler実行中に、Exceptionが投げられた場合に実行されるコールバックを示します
        /// </summary>
        /// <remarks>
        /// 特にコールバックが設定されていない場合はログ出力だけを行います。
        /// </remarks>
        /// <value>The exception callback.</value>
        Action<Exception> ExceptionCallback { set; }

        /// <summary>
        /// Schedulerの実行を停止させます
        /// </summary>
        /// <remarks>
        /// 復帰するには<see cref="Resume"/>を呼びます
        /// </remarks>
        void Pause();

        /// <summary>
        /// Schedulerの実行を復帰させます
        /// </summary>
        /// <remarks>
        /// コルーチンとして再開するので、<see cref="Pause"/>で停止させた状態から再開します
        /// </remarks>
        void Resume();

        /// <summary>
        /// Schedulerの実行を始めから再実行させます
        /// </summary>
        /// <remarks>
        /// 実行中であるか、待機状態かを問わず最初の状態から再開させます
        /// </remarks>
        void Restart();

        /// <summary>
        /// 実行の完了を問わず、Schedulerを停止させます
        /// </summary>
        /// <remarks>
        /// 一度Finishを呼び出すと再開することはできません
        /// </remarks>
        void Finish();

        /// <summary>
        /// 未実行のアクションがあった場合に実行し、その後Schedulerを停止させます
        /// </summary>
        /// <remarks>
        /// 待機状態かを問わずアクションを実行します
        /// </remarks>
        void FinishWithAction();
    }
}

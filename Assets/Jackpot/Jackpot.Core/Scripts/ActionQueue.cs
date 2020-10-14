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

namespace Jackpot
{
    /// <summary>
    /// スレッドセーフなActionの実行キューです
    /// </summary>
    public class ActionQueue
    {
        static readonly Action<Exception> DefaultExceptionCallback = e => Logger.Get<ActionQueue>().Error(e);
        private readonly List<Action>[] buffers;

        private int frontBufferIndex;

        private List<Action> frontBuffer
        {
            get { return buffers[frontBufferIndex]; }
        }

        private object gate;

        private bool isDequeing;

        public bool IsProcessing
        {
            get
            {
                return isDequeing;
            }
        }

        public int Count
        {
            get
            {
                var result = 0;
                lock (gate)
                {
                    result = frontBuffer.Count;
                }
                return result;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.ActionQueue"/> class.
        /// </summary>
        public ActionQueue()
        {
            buffers = new[]
            {
                new List<Action>(),
                new List<Action>()
            };
            frontBufferIndex = 0;
            gate = new object();
            isDequeing = false;
        }

        /// <summary>
        /// 指定のアクションをキューに追加します
        /// </summary>
        /// <param name="action">Action.</param>
        public void Enqueue(Action action)
        {
            lock (gate)
            {
                frontBuffer.Add(action);
            }
        }

        /// <summary>
        /// <see cref="buffers"/>をスワップする
        /// </summary>
        /// <remarks><see cref="gate"/>をロックしない</remarks>
        /// <returns>スワップ後のバックバッファ</returns>
        private List<Action> SwapBuffers()
        {
            var backBufferIndex = frontBufferIndex;
            frontBufferIndex = (frontBufferIndex == 1) ? 0 : 1;

            return buffers[backBufferIndex];
        }

        /// <summary>
        /// ActionQueueに積まれたActionを1つ実行します
        /// </summary>
        /// <param name="exceptionCallback">Exception callback.</param>
        public void Process(Action<Exception> exceptionCallback)
        {
            Action action = null;
            lock (gate)
            {
                // 実行中は例外を投げる
                if (isDequeing)
                {
                    ExecuteExeptionCallback(exceptionCallback, new InvalidOperationException("Queue already executing"));
                    return;
                }
                if (frontBuffer.Count <= 0)
                {
                    return;
                }
                isDequeing = true;
                action = frontBuffer[0];
                frontBuffer.RemoveAt(0);
            }

            try
            {
                action();
            }
            catch (Exception e)
            {
                ExecuteExeptionCallback(exceptionCallback, e);
            }

            lock (gate)
            {
                isDequeing = false;
            }
        }

        /// <summary>
        /// キューに積まれた全てのActionを実行します
        /// </summary>
        /// <param name="exceptionCallback">Exception callback.</param>
        public void ProcessAll(Action<Exception> exceptionCallback)
        {
            List<Action> actions;
            lock (gate)
            {
                // 実行中は例外を投げる
                if (isDequeing)
                {
                    ExecuteExeptionCallback(exceptionCallback, new InvalidOperationException("Queue already executing"));
                    return;
                }
                isDequeing = true;

                // 実行すべきアクションを取得
                actions = SwapBuffers();
            }

            for (int i = 0; i < actions.Count; i++)
            {
                var action = actions[i];
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    ExecuteExeptionCallback(exceptionCallback, ex);
                }
            }
            actions.Clear();

            lock (gate)
            {
                isDequeing = false;
            }
        }

        /// <summary>
        /// 指定したミリ秒で処理できる限り、キューに積まれたActionを実行します
        /// </summary>
        /// <param name="milliSeconds">Milli seconds.</param>
        /// <param name="exceptionCallback">Exception callback.</param>
        public void ProcessWhile(uint milliSeconds, Action<Exception> exceptionCallback = null)
        {
            var limit = DateTime.Now.AddMilliseconds(milliSeconds);
            while (Count > 0 && limit > DateTime.Now)
            {
                Process(exceptionCallback);
            }
        }

        private void ExecuteExeptionCallback(Action<Exception> exceptionCallback, Exception e)
        {
            if (exceptionCallback == null)
            {
                DefaultExceptionCallback(e);
            }
            else
            {
                exceptionCallback(e);
            }
        }
    }
}

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
    /// Context queue.
    /// </summary>
    public class ContextQueue<T> where T : Context
    {
        static ILogger logger = Logger.Get("Jackpot.ContextQueue");
        IContextExecutor<T> executor;
        Queue<T> queue;

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.ContextQueue`1"/> class.
        /// </summary>
        /// <param name="executor">Executor.</param>
        public ContextQueue(IContextExecutor<T> executor)
        {
            this.executor = executor;
            queue = new Queue<T>();
        }

        /// <summary>
        /// Enqueue the specified context.
        /// </summary>
        /// <param name="context">Context.</param>
        public void Enqueue(T context)
        {
            queue.Enqueue(context);
        }

        /// <summary>
        /// Processes the queue.
        /// </summary>
        public void ProcessQueue()
        {
            if (queue.Count <= 0)
            {
                return;
            }
            var next = queue.Peek();
            if (!executor.CanExecute(next))
            {
                return;
            }
            queue.Dequeue();
            logger.Debug("Execute context: {0}", next.Id);
            executor.Execute(next);
        }
    }
}

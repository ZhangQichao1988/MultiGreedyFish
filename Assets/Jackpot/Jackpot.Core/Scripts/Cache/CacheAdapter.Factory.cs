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
    partial class CacheAdapter
    {
        /// <summary>
        /// The default.
        /// </summary>
        public static readonly ICacheAdapter Default = new CacheAdapter(DefaultCacheAdapterId, DefaultExpireSeconds);

        const string CacheAdapterPoolKey = "Jackpot.CacheAdapterPool";
        const string DefaultCacheAdapterId = "Jackpot.CacheAdapter.Default";
        const int DefaultExpireSeconds = 30;
        static readonly object LifecycleLock = new object();

        public static ICacheAdapter Create(string id)
        {
            lock (LifecycleLock)
            {
                if (id == DefaultCacheAdapterId)
                {
                    return Default;
                }
                var pool = GetCacheAdapterPool();
                if (!pool.ContainsKey(id) || pool[id] == null)
                {
                    pool.Add(id, new CacheAdapter(id, DefaultExpireSeconds));
                }
                return pool[id];
            }
        }

        public static void Delete(string id)
        {
            lock (LifecycleLock)
            {
                if (id == DefaultCacheAdapterId)
                {
                    Default.Clear();
                    return;
                }
                var pool = GetCacheAdapterPool();
                if (pool.ContainsKey(id))
                {
                    var cache = pool[id];
                    if (cache != null)
                    {
                        cache.Clear();
                    }
                    pool.Remove(id);
                }
            }
        }

        static Dictionary<string, ICacheAdapter> GetCacheAdapterPool()
        {
            if (!ApplicationCache.Instance.Contains(CacheAdapterPoolKey))
            {
                ApplicationCache.Instance.Set<Dictionary<string, ICacheAdapter>>(
                    CacheAdapterPoolKey,
                    new Dictionary<string, ICacheAdapter>()
                );
            }
            return ApplicationCache.Instance.Get<Dictionary<string, ICacheAdapter>>(CacheAdapterPoolKey);
        }
    }
}

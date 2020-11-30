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
using System.Threading;

namespace Jackpot
{
    /// <summary>
    /// 期限付きキャッシュ機構を持つクラスです
    /// </summary>
    /// <remarks>
    /// このクラスはスレッドセーフです
    /// </remarks>
    public partial class CacheAdapter : ICacheAdapter
    {

#region Properties

        public string Id { get; private set; }

        public int ExpireSeconds
        {
            get
            {
                expireSlim.EnterReadLock();
                int result;
                try
                {
                    result = expireSeconds;
                }
                finally
                {
                    expireSlim.ExitReadLock();
                }
                return result;
            }
            set
            {
                expireSlim.EnterWriteLock();
                try
                {
                    if (value < -1)
                    {
                        throw new ArgumentException(string.Format(
                            "Expected argument was greater than -1(-1 means Timeout.Infinite), but was {0}",
                            value
                        ));
                    }
                    expireSeconds = value;
                }
                finally
                {
                    expireSlim.ExitWriteLock();
                }
            }
        }

#endregion

#region Fields

        readonly ReaderWriterLockSlim expireSlim;
        readonly Dictionary<string, Timer> expireTimers;
        readonly Memory memory;
        int expireSeconds;

#endregion

#region Constructor

        CacheAdapter(string id, int expireSeconds)
        {
            expireSlim = new ReaderWriterLockSlim();
            expireTimers = new Dictionary<string, Timer>();
            memory = new Memory();
            Id = id;
            ExpireSeconds = expireSeconds;
        }

#endregion

#region Public Methods

        public TValue Get<TValue>(string key, Func<TValue> fallback)
        {
            return Get<TValue>(key, DefaultExpireSeconds, fallback);
        }

        public TValue Get<TValue>(string key, int expireSeconds, Func<TValue> fallback)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
            if (fallback == null)
            {
                throw new ArgumentNullException("fallback");
            }

            TValue result;
            expireSlim.EnterUpgradeableReadLock();
            try
            {
                if (memory.TryGet<TValue>(key, out result))
                {
                    return result;
                }
                result = fallback();
                Set(key, expireSeconds, result);
            }
            finally
            {
                expireSlim.ExitUpgradeableReadLock();
            }
            return result;
        }

        public bool TryGet<TValue>(string key, out TValue value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            bool result;
            expireSlim.EnterReadLock();
            try
            {
                result = memory.TryGet<TValue>(key, out value);
            }
            finally
            {
                expireSlim.ExitReadLock();
            }
            return result;
        }

        public CacheAdapterAsync<TValue> GetAsync<TValue>(string key, AsyncFallbackDelegate<TValue> fallback)
        {
            return GetAsync<TValue>(key, ExpireSeconds, fallback);
        }

        public CacheAdapterAsync<TValue> GetAsync<TValue>(
            string key,
            int expireSeconds,
            AsyncFallbackDelegate<TValue> fallback)
        {
            return new CacheAdapterAsync<TValue>(this, key, expireSeconds, fallback);
        }

        public void Set(string key, object value)
        {
            Set(key, DefaultExpireSeconds, value);
        }

        public void Set(string key, int expireSeconds, object value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
            if (expireSeconds < -1)
            {
                throw new ArgumentException(string.Format(
                    "Expected argument expireSeconds was greater than -1(-1 means Timeout.Infinite), but was {0}",
                    expireSeconds
                ));
            }

            if (expireSeconds == 0)
            {
                return;
            }

            expireSlim.EnterWriteLock();
            try
            {
                memory.Set(key, value);
                if (expireSeconds == Timeout.Infinite)
                {
                    return;
                }
                RemoveExpireTimerInternal(key);
                var newTimer = new Timer(state =>
                {
                    expireSlim.EnterWriteLock();
                    try
                    {
                        memory.Remove(key);
                        RemoveExpireTimerInternal(key);
                    }
                    finally
                    {
                        expireSlim.ExitWriteLock();
                    }
                });
                expireTimers.Add(key, newTimer);
                newTimer.Change(expireSeconds * 1000, Timeout.Infinite);

            }
            finally
            {
                expireSlim.ExitWriteLock();
            }
        }

        public bool ContainsKey(string key)
        {
            bool result;
            expireSlim.EnterReadLock();
            try
            {
                result = memory.ContainsKey(key);
            }
            finally
            {
                expireSlim.ExitReadLock();
            }
            return result;
        }

        public void Remove(string key)
        {
            expireSlim.EnterWriteLock();
            try
            {
                memory.Remove(key);
                RemoveExpireTimerInternal(key);
            }
            finally
            {
                expireSlim.ExitWriteLock();
            }
        }

        public void Clear()
        {
            expireSlim.EnterWriteLock();
            try
            {
                memory.RemoveAll();
                if (expireTimers != null && expireTimers.Count > 0)
                {
                    foreach (var pair in expireTimers)
                    {
                        pair.Value.Dispose();
                    }
                    expireTimers.Clear();
                }
            }
            finally
            {
                expireSlim.ExitWriteLock();
            }
        }

#endregion

#region Private Methods

        void RemoveExpireTimerInternal(string key)
        {
            // NOTE: このメソッド自体はスレッドセーフではないので
            //       適切にクリティカルセクションをこのメソッドの外で定義して
            //       呼び出すこと
            if (!expireTimers.ContainsKey(key))
            {
                return;
            }
            var timer = expireTimers[key];
            timer.Dispose();
            expireTimers.Remove(key);
        }

#endregion

        static CacheAdapter()
        {
            ICacheAdapter cache = new CacheAdapter("fullaot", Timeout.Infinite);

            cache.Get("b", () => default(bool));
            bool b;
            cache.TryGet("b", out b);
            cache.GetAsync<bool>("b", (z, _) => z(default(bool)));

            cache.Get("i", () => default(int));
            int i;
            cache.TryGet("i", out i);
            cache.GetAsync<int>("i", (z, _) => z(default(int)));

            cache.Get("l", () => default(long));
            long l;
            cache.TryGet("l", out l);
            cache.GetAsync<long>("l", (z, _) => z(default(long)));

            cache.Get("s", () => default(short));
            short s;
            cache.TryGet("s", out s);
            cache.GetAsync<short>("s", (z, _) => z(default(short)));

            cache.Get("bs", () => default(byte));
            byte bs;
            cache.TryGet("bs", out bs);
            cache.GetAsync<byte>("bs", (z, _) => z(default(byte)));

            cache.Get("f", () => default(float));
            float f;
            cache.TryGet("f", out f);
            cache.GetAsync<float>("f", (z, _) => z(default(float)));

            cache.Get("d", () => default(decimal));
            decimal d;
            cache.TryGet("d", out d);
            cache.GetAsync<decimal>("d", (z, _) => z(default(decimal)));

            cache.Get("u", () => default(uint));
            uint u;
            cache.TryGet("u", out u);
            cache.GetAsync<uint>("u", (z, _) => z(default(uint)));

            cache.Get("c", () => default(char));
            char c;
            cache.TryGet("c", out c);
            cache.GetAsync<char>("c", (z, _) => z(default(char)));

            cache.Clear();
        }
    }
}

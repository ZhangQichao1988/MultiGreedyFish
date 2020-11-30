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
    /// スレッドセーフなIMemoryの実装クラスです
    /// </summary>
    public class ConcurrentMemory : IMemory
    {
#region Fields

        Dictionary<string, object> dict;
        ReaderWriterLockSlim slim;

#endregion

#region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.ThreadSafeMemory"/> class.
        /// </summary>
        public ConcurrentMemory() : this(new Dictionary<string, object>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.ThreadSafeMemory"/> class.
        /// </summary>
        /// <param name="dict">Dict.</param>
        public ConcurrentMemory(Dictionary<string,object> dict)
        {
            this.dict = dict;
            slim = new ReaderWriterLockSlim();
        }

#endregion

#region Public Methods

        public TValue Get<TValue>(string key)
        {
            return Get<TValue>(key, default(TValue));
        }

        public TValue Get<TValue>(string key, TValue defaultValue)
        {
            var result = defaultValue;
            slim.EnterReadLock();
            try
            {
                if (dict.ContainsKey(key) && dict[key] != null)
                {
                    result = (TValue) dict[key];
                }
            }
            finally
            {
                slim.ExitReadLock();
            }
            return result;
        }

        public bool TryGet<TValue>(string key, out TValue value)
        {
            var succeeded = false;
            slim.EnterReadLock();
            try
            {
                object obj;
                succeeded = dict.TryGetValue(key, out obj);
                try
                {
                    value = succeeded ? (TValue) obj : default(TValue);
                }
                catch
                {
                    value = default(TValue);
                    succeeded = false;
                }
            }
            finally
            {
                slim.ExitReadLock();
            }
            return succeeded;
        }

        public bool Contains(string key)
        {
            var result = false;
            slim.EnterReadLock();
            try
            {
                if (dict.ContainsKey(key) && dict[key] != null)
                {
                    result = true;
                }
            }
            finally
            {
                slim.ExitReadLock();
            }
            return result;
        }

        public bool ContainsKey(string key)
        {
            var result = false;
            slim.EnterReadLock();
            try
            {
                if (dict.ContainsKey(key))
                {
                    result = true;
                }
            }
            finally
            {
                slim.ExitReadLock();
            }
            return result;
        }

        public void Set<TValue>(string key, TValue value)
        {
            slim.EnterWriteLock();
            try
            {
                if (dict.ContainsKey(key))
                {
                    dict.Remove(key);
                }
                dict.Add(key, value);
            }
            finally
            {
                slim.ExitWriteLock();
            }
        }

        public void Remove(string key)
        {
            slim.EnterWriteLock();
            try
            {
                if (dict.ContainsKey(key))
                {
                    dict.Remove(key);
                }
            }
            finally
            {
                slim.ExitWriteLock();
            }
        }

        public void RemoveAll()
        {
            slim.EnterWriteLock();
            try
            {
                dict.Clear();
            }
            finally
            {
                slim.ExitWriteLock();
            }
        }

#endregion

        static ConcurrentMemory()
        {
            var mem = new ConcurrentMemory();
            var key = "fullaot";

            mem.Set(key, default(bool));
            mem.Get<bool>(key);
            bool boolValue;
            mem.TryGet(key, out boolValue);

            mem.Set(key, default(int));
            mem.Get<int>(key);
            int intValue;
            mem.TryGet(key, out intValue);

            mem.Set(key, default(long));
            mem.Get<long>(key);
            long longValue;
            mem.TryGet(key, out longValue);

            mem.Set(key, default(short));
            mem.Get<short>(key);
            short shortValue;
            mem.TryGet(key, out shortValue);

            mem.Set(key, default(byte));
            mem.Get<byte>(key);
            byte byteValue;
            mem.TryGet(key, out byteValue);

            mem.Set(key, default(float));
            mem.Get<float>(key);
            float floatValue;
            mem.TryGet(key, out floatValue);

            mem.Set(key, default(decimal));
            mem.Get<decimal>(key);
            decimal decimalValue;
            mem.TryGet(key, out decimalValue);

            mem.Set(key, default(uint));
            mem.Get<uint>(key);
            uint uintValue;
            mem.TryGet(key, out uintValue);

            mem.Set(key, default(char));
            mem.Get<char>(key);
            char charValue;
            mem.TryGet(key, out charValue);

            mem.Set(key, default(double));
            mem.Get<double>(key);
            double doubleValue;
            mem.TryGet(key, out doubleValue);

            mem.Set(key, default(DateTime));
            mem.Get<DateTime>(key);
            DateTime datetimeValue;
            mem.TryGet(key, out datetimeValue);
        }
    }
}

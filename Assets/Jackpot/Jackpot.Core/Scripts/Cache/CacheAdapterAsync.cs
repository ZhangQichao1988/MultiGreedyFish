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
    /// 非同期に<see cref="Jackpot.ICacheAdapter" />を取り扱うクラスです
    /// </summary>
    /// <remarks>
    /// このクラスのインスタンスの生成には<see cref="Jackpot.CacheAdapter.GetAsync"/>を使用することを推奨します
    /// </remarks>
    public class CacheAdapterAsync<TValue> : Context
    {

#region Fields

        readonly ICacheAdapter cache;
        readonly string key;
        readonly int expireSeconds;
        readonly AsyncFallbackDelegate<TValue> fallback;
        readonly Action<TValue> successCallback;
        readonly Action<object> failureCallback;

#endregion

#region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.CacheAdapterAsync`1"/> class.
        /// </summary>
        /// <param name="cache">Cache.</param>
        /// <param name="key">Key.</param>
        /// <param name="expireSeconds">Expire seconds.</param>
        /// <param name="fallback">Fallback.</param>
        public CacheAdapterAsync(
            ICacheAdapter cache,
            string key,
            int expireSeconds,
            AsyncFallbackDelegate<TValue> fallback)
            : this(cache, key, expireSeconds, fallback, null, null)
        {
        }

        CacheAdapterAsync(
            ICacheAdapter cache,
            string key,
            int expireSeconds,
            AsyncFallbackDelegate<TValue> fallback,
            Action<TValue> successCallback,
            Action<object> failureCallback)
        {
            if (cache == null)
            {
                throw new ArgumentNullException("cache");
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
            if (fallback == null)
            {
                throw new ArgumentNullException("fallback");
            }
            this.cache = cache;
            this.key = key;
            this.expireSeconds = expireSeconds;
            this.fallback = fallback;
            this.successCallback = successCallback;
            this.failureCallback = failureCallback;
        }

        CacheAdapterAsync(
            int id,
            ICacheAdapter cache,
            string key,
            int expireSeconds,
            AsyncFallbackDelegate<TValue> fallback,
            Action<TValue> successCallback,
            Action<object> failureCallback) : base(id)
        {
            if (cache == null)
            {
                throw new ArgumentNullException("cache");
            }
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
            if (fallback == null)
            {
                throw new ArgumentNullException("fallback");
            }
            this.cache = cache;
            this.key = key;
            this.expireSeconds = expireSeconds;
            this.fallback = fallback;
            this.successCallback = successCallback;
            this.failureCallback = failureCallback;
        }

#endregion

#region Public Methods

        /// <summary>
        /// 値の取得に成功した際のコールバックを指定します
        /// </summary>
        /// <param name="successCallback">Success callback.</param>
        public CacheAdapterAsync<TValue> OnSuccess(Action<TValue> successCallback)
        {
            return new CacheAdapterAsync<TValue>(
                Id,
                cache,
                key,
                expireSeconds,
                fallback,
                successCallback,
                failureCallback
            );
        }

        /// <summary>
        /// 値の取得に失敗した際のコールバックを指定します
        /// </summary>
        /// <param name="failureCallback">failureCallbackの引数には、fallback時に発生したException、あるいはAsyncFallbackDelegateのonFailureの引数に指定した値が渡されます</param>
        public CacheAdapterAsync<TValue> OnFailure(Action<object> failureCallback)
        {
            return new CacheAdapterAsync<TValue>(
                Id,
                cache,
                key,
                expireSeconds,
                fallback,
                successCallback,
                failureCallback
            );
        }

        /// <summary>
        /// Execute this context.
        /// </summary>
        public override void Execute()
        {
            TValue cachedResult;
            if (cache.TryGet(key, out cachedResult))
            {
                InvokeSuccessCallback(cachedResult);
                return;
            }

            InvokeFallback();
        }

#endregion

#region Private Methods

        void InvokeFallback()
        {
            try
            {
                fallback(
                    cacheValue =>
                    {
                        cache.Set(key, expireSeconds, cacheValue);
                        InvokeSuccessCallback(cacheValue);
                    },
                    error => InvokeFailureCallback(error)
                );
            }
            catch (Exception e)
            {
                InvokeFailureCallback(e);
            }
        }

        void InvokeSuccessCallback(TValue result)
        {
            if (successCallback != null)
            {
                try
                {
                    successCallback(result);
                }
                catch
                {
                    cache.Remove(key);
                    throw;
                }
            }
        }

        void InvokeFailureCallback(object error)
        {
            try
            {
                if (failureCallback != null)
                {
                    failureCallback(error);
                }
            }
            finally
            {
                cache.Remove(key);
            }
        }

#endregion
    }
}

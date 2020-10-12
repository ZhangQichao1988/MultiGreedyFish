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
    /// 実装クラスは期限付きキャッシュ機能を提供します
    /// </summary>
    public interface ICacheAdapter
    {
        /// <summary>
        /// CacheAdapterに一意なIDです
        /// </summary>
        /// <value>The identifier.</value>
        string Id { get; }

        /// <summary>
        /// キャッシュを破棄する標準の時間(秒)を示します。
        /// </summary>
        /// <remarks>
        /// ICacheAdapter.Getで、expireSeconds引数を指定しなかった場合に適用されます。
        /// </remarks>
        /// <value>The expire seconds.</value>
        int ExpireSeconds { get; set; }

        /// <summary>
        /// DefaultExpireSecondsを使用してCacheAdapter.Get()を実行します。
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="fallback">Fallback.</param>
        /// <typeparam name="TValue">The 1st type parameter.</typeparam>
        TValue Get<TValue>(string key, Func<TValue> fallback);

        /// <summary>
        /// 指定したkeyでキャッシュしている値を取得し、キャッシュが存在しなければfallbackした結果を返却しつつキャッシュします。
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="expireSeconds">Expire seconds.</param>
        /// <param name="fallback">Fallback.</param>
        /// <typeparam name="TValue">The 1st type parameter.</typeparam>
        TValue Get<TValue>(string key, int expireSeconds, Func<TValue> fallback);

        /// <summary>
        /// 指定したkeyでキャッシュしている値を引数valueに出力し、キャッシュから取得できた場合はtrueを、そうでない場合はfalseを返却します。
        /// </summary>
        /// <returns><c>true</c>, if get was tryed, <c>false</c> otherwise.</returns>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <typeparam name="TValue">The 1st type parameter.</typeparam>
        bool TryGet<TValue>(string key, out TValue value);

        /// <summary>
        /// 非同期にICacheAdapter.Get()を実施します。標準のキャッシュ破棄時間が適用されます。
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="key">Key.</param>
        /// <param name="fallback">Fallback.</param>
        /// <typeparam name="TValue">The 1st type parameter.</typeparam>
        CacheAdapterAsync<TValue> GetAsync<TValue>(string key, AsyncFallbackDelegate<TValue> fallback);

        /// <summary>
        /// 非同期にICacheAdapter.Get()を実施します。
        /// </summary>
        /// <remarks>
        /// <code>
        /// var cache = CacheAdapter.Default;
        /// cache.GetAsync<string>("cache-hogehoge", 30, (success, failure) => success("hogehoge"))
        ///     .OnSuccess(result => UnityEngine.Debug.Log(result))
        ///     .OnFailure(error => UnityEngine.Debug.Log(error))
        ///     .Execute();
        /// </code>
        /// </remarks>
        /// <returns>The async.</returns>
        /// <param name="key">Key.</param>
        /// <param name="expireSeconds">Expire seconds.</param>
        /// <param name="fallback">Fallback.</param>
        /// <typeparam name="TValue">The 1st type parameter.</typeparam>
        CacheAdapterAsync<TValue> GetAsync<TValue>(
            string key,
            int expireSeconds,
            AsyncFallbackDelegate<TValue> fallback);

        /// <summary>
        /// 指定したkeyで値をキャッシュします
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        void Set(string key, object value);

        /// <summary>
        /// 指定したkeyとexpireSecondsを指定して値をキャッシュします。
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="expireSeconds">Expire seconds.</param>
        /// <param name="value">Value.</param>
        void Set(string key, int expireSeconds, object value);

        /// <summary>
        /// 指定したkeyで値がキャッシュされているかを示します。
        /// </summary>
        /// <param name="key">Key.</param>
        bool ContainsKey(string key);

        /// <summary>
        /// 指定したkeyでキャッシュされている値を破棄します。
        /// </summary>
        /// <param name="key">Key.</param>
        void Remove(string key);

        /// <summary>
        /// すべてのキャッシュをクリアします。
        /// </summary>
        void Clear();
    }
}


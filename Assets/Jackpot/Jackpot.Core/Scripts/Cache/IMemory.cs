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
namespace Jackpot
{
    /// <summary>
    /// 実装クラスは、様々なデータ型を型安全に取り扱うDictionaryな機能お提供します
    /// </summary>
    public interface IMemory
    {
        /// <summary>
        /// 指定したkeyに紐づくObjectを指定の型引数で返却します
        /// </summary>
        /// <param name="key">Key.</param>
        /// <typeparam name="TValue">The 1st type parameter.</typeparam>
        TValue Get<TValue>(string key);

        /// <summary>
        /// 指定したkeyに紐づくObjectを指定の型引数で返却します。keyが存在しない、またはnullの場合、代わりにdefaultValueを返却します
        /// </summary>
        /// <param name="key">Key.</param>
        /// <typeparam name="TValue">The 1st type parameter.</typeparam>
        TValue Get<TValue>(string key, TValue defaultValue);

        /// <summary>
        /// 指定したkeyに紐づくObjectを引数valueに出力し、keyが存在し取得できた場合はtrueを、そうでない場合はfalseを返却します
        /// </summary>
        /// <returns><c>true</c>, if get was tryed, <c>false</c> otherwise.</returns>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <typeparam name="TValue">The 1st type parameter.</typeparam>
        bool TryGet<TValue>(string key, out TValue value);

        /// <summary>
        /// 指定したkeyにObjectが紐付けられており、そのObjectはnullでなければtrueを、そうでない場合はfalseを返却します
        /// </summary>
        /// <remarks>
        /// Objectがnullであるかも検証します。
        /// 指定したkeyにnullをセットした場合、このメソッドではfalseが返却される事に注意してください。
        /// </remarks>
        /// <param name="key">Key.</param>
        bool Contains(string key);

        /// <summary>
        /// 指定したkeyにObjectが紐付けられているかを示します。Objectがnullか否かは検証しません
        /// </summary>
        /// <returns><c>true</c>, if key was containsed, <c>false</c> otherwise.</returns>
        /// <param name="key">Key.</param>
        bool ContainsKey(string key);

        /// <summary>
        /// 指定したkeyに指定した型引数のObjectを設定します
        /// 型が違ってもkeyは共有である事に注意してください
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <typeparam name="TValue">The 1st type parameter.</typeparam>
        void Set<TValue>(string key, TValue value);

        /// <summary>
        /// 指定したkeyに紐づくObjectを削除します
        /// </summary>
        /// <param name="key">Key.</param>
        void Remove(string key);

        /// <summary>
        /// 設定されているすべてのkeyとkeyに紐づくObjectを削除します
        /// </summary>
        void RemoveAll();
    }
}


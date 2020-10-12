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
    /// 実装クラスはContextを実行する機能を持ちます
    /// </summary>
    public interface IContextExecutor<T> where T : Context
    {
        /// <summary>
        /// 指定したcontextがExecutor上で実行可能か否かを示します
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns><c>true</c> if this instance is running the specified context; otherwise, <c>false</c>.</returns>
        /// <param name="context">Context.</param>
        bool CanExecute(T context);

        /// <summary>
        /// 指定のContextを実施します。事前にCanExecuteで実行可能かを調べてからこのメソッドを実行する必要があります
        /// </summary>
        /// <param name="context">Context.</param>
        void Execute(T context);
    }
}

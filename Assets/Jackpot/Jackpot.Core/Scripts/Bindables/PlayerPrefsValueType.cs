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
    /// PlayerPrefsへ登録できる設定項目の型の種類を示す列挙型です。
    /// </summary>
    public enum PlayerPrefsValueType
    {
        /// <summary>
        /// DeleteKey, DeleteAllにおいて、元の値の型が特定できないことを示します
        /// </summary>
        Undefined,

        /// <summary>
        /// <c>int</c>型の設定項目であることを示します
        /// </summary>
        IntValue,

        /// <summary>
        /// <c>float</c>型の設定項目であることを示します
        /// </summary>
        FloatValue,

        /// <summary>
        /// <c>string</c>型の設定項目であることを示します
        /// </summary>
        StringValue
    }
}

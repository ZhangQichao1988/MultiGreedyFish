﻿/**
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

namespace Jackpot.Billing
{
    /// <summary>
    /// IBilling.QueryProducts()を実行するための商品IDを取得する処理をカスタマイズする際に指定するdelegateです
    /// </summary>
    public delegate void QueryRequestDelegate(Action<List<string>> startQuery, Action<QueryError> failQuery);

    /// <summary>
    /// Query items request delegate.
    /// </summary>
    public delegate void QueryItemsRequestDelegate(Action<List<string>, List<string>> startQuery, Action<QueryError> failQuery);
}
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
using System.Text;

namespace Jackpot.Billing
{
    public class QueryError
    {
        /// <summary>
        /// QueryRequestDelegateが失敗した事を示します
        /// </summary>
        /// <value><c>true</c> if this instance is failed on query request; otherwise, <c>false</c>.</value>
        public bool IsFailedOnQueryRequest { get; private set; }

        /// <summary>
        /// アプリサーバーがメンテナンスである事を示します
        /// </summary>
        /// <value><c>true</c> if this instance is Maintenance app server; otherwise, <c>false</c>.</value>
        public bool IsMaintenanceAppServer { get; private set; }

        /// <summary>
        /// 終了していない購入処理がある事を示します
        /// </summary>
        /// <value><c>true</c> if this instance is still purchasing; otherwise, <c>false</c>.</value>
        public bool IsStillPurchasing { get; private set; }

        /// <summary>
        /// プラットフォームの商品一覧取得処理が失敗した事を示します
        /// </summary>
        /// <value><c>true</c> if this instance is failed on platform request; otherwise, <c>false</c>.</value>
        public bool IsFailedOnPlatformRequest { get; private set; }

        /// <summary>
        /// 実装上の問題である事を示します
        /// </summary>
        /// <value><c>true</c> if this instance is invalid operation; otherwise, <c>false</c>.</value>
        public bool IsInvalidOperation { get; private set; }

        /// <summary>
        /// エラーメッセージを示します
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; private set; }

        /// <summary>
        /// エラーとなったProductIdの配列
        /// </summary>
        public List<string> FailureProductIdList { get; private set; }

        public static QueryError FailedOnQueryRequest(string message)
        {
            var result = new QueryError(message);
            result.IsFailedOnQueryRequest = true;
            return result;
        }

        public static QueryError MaintenanceAppServer(string message)
        {
            var result = new QueryError(message);
            result.IsMaintenanceAppServer = true;
            return result;
        }

        public static QueryError StillPurchasing(List<string> failureProductIdList)
        {
            var result = new QueryError("ProductIds: " + string.Join(",", failureProductIdList.ToArray()));
            result.IsStillPurchasing = true;
            result.FailureProductIdList = failureProductIdList;
            return result;
        }

        public static QueryError FailedOnPlatformRequest(string message)
        {
            var result = new QueryError(message);
            result.IsFailedOnPlatformRequest = true;
            return result;
        }

        public static QueryError InvalidOperation(string message)
        {
            var result = new QueryError(message);
            result.IsInvalidOperation = true;
            return result;
        }

        QueryError(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
            if (IsFailedOnQueryRequest)
            {
                return string.Format("FailedOnQueryRequest: {0}", Message);
            }
            if (IsStillPurchasing)
            {
                return string.Format("StillPurchasing: {0}", Message);
            }
            if (IsFailedOnQueryRequest)
            {
                return string.Format("FailedOnQueryRequest: {0}", Message);
            }
            if (IsFailedOnPlatformRequest)
            {
                return string.Format("FailedOnPlatformRequest: {0}", Message);
            }
            if (IsInvalidOperation)
            {
                return string.Format("InvalidOperation: {0}", Message);
            }
            if (IsMaintenanceAppServer)
            {
                return string.Format("IsMaintenanceAppServer: {0}", Message);
            }
            return Message;
        }
    }
}

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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jackpot.Billing
{
    public class RestoreError
    {
        /// <summary>
        /// リストア処理のリクエストが失敗したことを示します
        /// </summary>
        /// <value><c>true</c> if this instance is failed on platform request; otherwise, <c>false</c>.</value>
        public bool IsFailedOnPlatformRequest { get; set; }

        /// <summary>
        /// エラーメッセージを示します
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }

        public static RestoreError FailedOnPlatformRequest(string message)
        {
            var result = new RestoreError(message);
            result.IsFailedOnPlatformRequest = true;
            return result;
        }

        RestoreError(string message)
        {
            Message = message;
        }
    }
}

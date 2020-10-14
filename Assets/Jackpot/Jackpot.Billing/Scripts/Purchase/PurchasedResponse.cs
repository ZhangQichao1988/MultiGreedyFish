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
using System.Collections.Generic;

namespace Jackpot.Billing
{
    /// <summary>
    /// サーバレスポンスの判定用クラスです
    /// 決済・仮想通貨管理ライブラリ(ringoame)
    /// のサーバーレスポンスを受け取れるようにしています
    /// </summary>
    public class PurchasedResponse
    {
        public PurchasedResponseKind ResultCode { get; private set; }

        public string Message { get; private set; }

        public List<string> SkippedTransactionIds { get; private set; }

        public PurchasedResponse(Dictionary<string, object> responseDic)
        {
            if (responseDic.ContainsKey("result_code"))
            {
                ResultCode = (PurchasedResponseKind) int.Parse(responseDic["result_code"].ToString());
            }
            else
            {
                ResultCode = PurchasedResponseKind.Unknown;
            }
            if (responseDic.ContainsKey("message"))
            {
                Message = (string) responseDic["message"];
            }
            SkippedTransactionIds = new List<string>();
            if (responseDic.ContainsKey("skipped_transaction_ids"))
            {
                foreach (var id in (List<object>) responseDic["skipped_transaction_ids"])
                {
                    SkippedTransactionIds.Add((string) id);
                }
            }
        }

        /// <summary>
        /// サーバレスポンスの結果コードのエラー判定を行います
        /// </summary>
        public bool IsProcessedFailure()
        {
            if (ResultCode == PurchasedResponseKind.ProcessedSuccessfully
                || ResultCode == PurchasedResponseKind.AlreadyProcessed)
            {
                return false;
            }
            return true;
        }
    }
}

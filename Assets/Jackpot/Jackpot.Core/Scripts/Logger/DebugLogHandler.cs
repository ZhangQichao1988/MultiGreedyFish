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
using UnityEngine;

namespace Jackpot
{
    /// <summary>
    /// Debugログ出力するためのLoggerHandlerです。
    /// 直接使用する事はないはず。
    /// </summary>
    internal class DebugLogHandler : ILoggerHandler
    {
        readonly bool isDebugBuild;

        /// <summary>
        /// Loggerから呼ばれるbootstrap。
        /// 設計としてあまりいけてないがプロダクションコードの負担を下げるべくやむなし
        /// </summary>
        public static void Initialize()
        {
            if (!Debug.isDebugBuild)
            {
                return;
            }
            Logger.AddHandler(new DebugLogHandler(Debug.isDebugBuild));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.DebugLogHandler"/> class.
        /// </summary>
        public DebugLogHandler(bool isDebugBuild)
        {
            this.isDebugBuild = isDebugBuild;
        }

        public void HandleLog(LoggerReport report)
        {
            // うっかりLogHandler登録しちゃった時の為に
            if (!isDebugBuild)
            {
                return;
            }
            try
            {
                Platform.Log(report);
            }
            catch (Exception e)
            {
                Debug.LogError("DebugLogHandler Error: " + e.ToString());
                Debug.LogError(report.ToString());
            }
        }
    }
}


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
    /// Loggerによって出力されるValue Objectです
    /// </summary>
    public class LoggerReport
    {
        #region Properties

        /// <summary>
        /// ログ出力された時間を示します
        /// </summary>
        /// <value>The date.</value>
        public DateTime Date { get; private set; }

        /// <summary>
        /// ログ出力に使用されたLogLevelを示します
        /// </summary>
        /// <value>The log level.</value>
        public LogLevel LogLevel { get; private set; }

        /// <summary>
        /// ログ出力がどのオブジェクトから実施されたかを示します
        /// </summary>
        /// <value>The name of the caller.</value>
        public string CallerName { get; private set; }

        /// <summary>
        /// ログメッセージを示します
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.LoggerReport"/> class.
        /// </summary>
        /// <param name="level">Level.</param>
        /// <param name="callerName">Caller name.</param>
        /// <param name="message">Message.</param>
        public LoggerReport(LogLevel level, string callerName, string message)
        {
            this.Date = DateTime.Now;
            this.LogLevel = level;
            this.CallerName = callerName;
            this.Message = message;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// LoggerReportを"[date][loglevel][callerName]message"なフォーマットで文字列出力します
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="Jackpot.LoggerReport"/>.</returns>
        public override string ToString()
        {
            return string.Format(
                "[{0}][{1}][{2}]{3}",
                this.Date.ToString(),
                this.LogLevel.ToString(),
                this.CallerName,
                this.Message
            );
        }

        #endregion
    }
}

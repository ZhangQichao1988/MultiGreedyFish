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
    /// 実装クラスはログ出力機能を提供します
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// LogLevel.Debugでログ出力を行います
        /// </summary>
        /// <param name="message">Message.</param>
        void Debug(string message);

        /// <summary>
        /// LogLevel.Debugでログ出力を行います。string.Format相当の機能を含みます
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="moreObjects">More objects.</param>
        void Debug(object obj, params object[] moreObjects);

        /// <summary>
        /// LogLevel.Infoでログ出力を行います
        /// </summary>
        /// <param name="message">Message.</param>
        void Info(string message);

        /// <summary>
        /// LogLevel.Infoでログ出力を行います。string.Format相当の機能を含みます
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="moreObjects">More objects.</param>
        void Info(object obj, params object[] moreObjects);

        /// <summary>
        /// LogLevel.Warnでログ出力を行います
        /// </summary>
        /// <param name="message">Message.</param>
        void Warn(string message);

        /// <summary>
        /// LogLevel.Warnでログ出力を行います。string.Format相当の機能を含みます
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="moreObjects">More objects.</param>
        void Warn(object obj, params object[] moreObjects);

        /// <summary>
        /// LogLevel.Errorでログ出力を行います
        /// </summary>
        /// <param name="message">Message.</param>
        void Error(string message);

        /// <summary>
        /// LogLevel.Errorでログ出力を行います。string.Format相当の機能を含みます
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="moreObjects">More objects.</param>
        void Error(object obj, params object[] moreObjects);

        /// <summary>
        /// LogLevel.Fatalでログ出力を行います
        /// </summary>
        /// <param name="message">Message.</param>
        void Fatal(string message);

        /// <summary>
        /// LogLevel.Fatalでログ出力を行います。string.Format相当の機能を含みます
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="moreObjects">More objects.</param>
        void Fatal(object obj, params object[] moreObjects);
    }
}

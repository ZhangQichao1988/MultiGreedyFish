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
 
/// <summary>
/// ログ出力レベルを示すenumです
/// ログ出力レベルは、Jackpot.Logger.SetLogLevel()にて指定します
/// <see cref="Jackpot.Logger.SetLogLevel"/>
/// </summary>
public enum LogLevel : int
{
    /// <summary>
    /// すべてのログを出力します
    /// </summary>
    All = 255,
    /// <summary>
    /// ILogger.Debug, ILogger.Info, ILogger.Warn, ILogger.Error, ILogger.Fatalのログを出力します
    /// </summary>
    Debug = 128,
    /// <summary>
    /// ILogger.Info, ILogger.Warn, ILogger.Error, ILogger.Fatalのログを出力します
    /// </summary>
    Info = 16,
    /// <summary>
    /// ILogger.Warn, ILogger.Error, ILogger.Fatalのログを出力します
    /// </summary>
    Warn = 8,
    /// <summary>
    /// ILogger.Error, ILogger.Fatalのログを出力します
    /// </summary>
    Error = 4,
    /// <summary>
    /// ILogger.Fatalのログを出力します
    /// </summary>
    Fatal = 2,
    /// <summary>
    /// Log出力を無効にします
    /// </summary>
    Off = 0
}


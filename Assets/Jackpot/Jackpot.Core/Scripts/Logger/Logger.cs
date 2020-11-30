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
using UnityEngine;

namespace Jackpot
{
    /// <summary>
    /// ログ出力を行うクラスです
    /// Loggerオブジェクトのファクトリメソッドおよび各種設定機能を持ちます
    /// </summary>
    /// <remarks>
    /// <code>
    /// using Jackpot;
    /// public class SampleBehaviour : MonoBehaviour
    /// {
    ///     static readonly ILogger logger = Logger.Get<SampleBehaviour>();
    ///     void Start()
    ///     {
    ///         
    ///         logger.Debug("start"); // => [MM/dd/yyyy HH:mm:ss][Debug][SampleBehaviour]start
    ///         logger.Error("Error cause: {0}", "foo"); // => [MM/dd/yyyy HH:mm:ss][Debug][SampleBehaviour]Error cause: foo
    ///     }
    /// }
    /// </code>
    /// UnityをDevelopmentBuildしたとき, LogLevelはAllに、そうでない場合はLogLevelはFatalに指定されます
    /// 各ビルド環境で出力レベルを変更したい場合, Logger.SetLogLevel()で変更できます
    /// <code>
    /// using Jackpot;
    /// public class EntryPoint : MonoBehaviour
    /// {
    ///     void Awake()
    ///     {
    ///         Logger.SetLogLevel(LogLevel.ERROR); // => Logger#Error(), Logger#Fatal()のログのみ出力
    ///         Logger.SetLogLevel(LogLevel.Off); // ログ出力を行わない
    ///     }
    /// }
    /// </code>
    /// <see cref="Jackpot.Logger.SetLogLevel"/>
    /// </remarks>
    public class Logger : ILogger
    {
        #region Constants

        const string NullStringMessage = "(Null string)";
        const string EmptyStringMessage = "(Empty string)";
        const string NullObjectMessage = "(Null object)";

        #endregion

        #region Static Fields

        static readonly Dictionary<string, ILogger> loggers = new Dictionary<string, ILogger>();
        static readonly List<ILoggerHandler> handlers = new List<ILoggerHandler>();
        static LogLevel logLevel = LogLevel.All;
        static Action bootStrap = BootStrapCore;

        #endregion

        #region Fields

        string callerName;

        #endregion

        #region Public Static Methods

        /// <summary>
        /// 指定された型に紐づくILoggerオブジェクトを返却します
        /// </summary>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static ILogger Get<T>()
        {
            return Get(typeof(T));
        }

        /// <summary>
        /// 指定された型に紐づくILoggerオブジェクトを返却します
        /// </summary>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static ILogger Get(Type type)
        {
            string callerName = type.FullName;
            return Get(callerName);
        }

        /// <summary>
        /// 名前に紐づくILoggerオブジェクトを返却します(static class等で使用)
        /// </summary>
        /// <param name="callerName">Caller name.</param>
        public static ILogger Get(string callerName = "Default")
        {
            if (string.IsNullOrEmpty(callerName))
            {
                callerName = "Default";
            }

            if (!loggers.ContainsKey(callerName) || loggers[callerName] == null)
            {
                Logger logger = new Logger(callerName);
                loggers[callerName] = logger;
            }
            return loggers[callerName];
        }

        /// <summary>
        /// 新しいLoggerHandlerオブジェクトを登録します
        /// </summary>
        /// <param name="handler">Handler.</param>
        public static void AddHandler(ILoggerHandler handler)
        {
            if (bootStrap != null)
            {
                bootStrap();
            }
            if (handlers.Contains(handler))
            {
                return;
            }
            handlers.Add(handler);
        }

        /// <summary>
        /// 指定のLoggerHandlerが登録済かを示します
        /// </summary>
        /// <returns><c>true</c>, if handler was contained, <c>false</c> otherwise.</returns>
        /// <param name="handler">Handler.</param>
        public static bool ContainsHandler(ILoggerHandler handler)
        {
            if (bootStrap != null)
            {
                bootStrap();
            }
            return handlers.Contains(handler);
        }

        /// <summary>
        /// 登録済のLoggerHandlerを解除します
        /// </summary>
        /// <param name="handler">Handler.</param>
        public static void RemoveHandler(ILoggerHandler handler)
        {
            if (bootStrap != null)
            {
                bootStrap();
            }
            if (!handlers.Contains(handler))
            {
                return;
            }
            handlers.Remove(handler);
        }

        /// <summary>
        /// 登録済のLoggerHandlerを全解除します
        /// </summary>
        public static void RemoveAllHandlers()
        {
            if (bootStrap != null)
            {
                bootStrap();
            }
            handlers.Clear();
        }

        /// <summary>
        /// LogLevelを設定します
        /// </summary>
        /// <see cref="Jackpot.LogLevel"/> 
        /// <param name="level">Level.</param>
        public static void SetLogLevel(LogLevel level)
        {
            if (bootStrap != null)
            {
                bootStrap();
            }
            logLevel = level;
        }

        #endregion

        #region Public Methods

        #region Debug log

        public void Debug(string message)
        {
            Log(LogLevel.Debug, this.callerName, FormatMessage(message));
        }

        public void Debug(object obj, params object[] moreObjects)
        {
            Log(LogLevel.Debug, this.callerName, FormatObjects(obj, moreObjects));
        }

        #endregion

        #region Infomation log

        public void Info(string message)
        {
            Log(LogLevel.Info, this.callerName, FormatMessage(message));
        }

        public void Info(object obj, params object[] moreObjects)
        {
            Log(LogLevel.Info, this.callerName, FormatObjects(obj, moreObjects));
        }

        #endregion

        #region Warning log

        public void Warn(string message)
        {
            Log(LogLevel.Warn, this.callerName, FormatMessage(message));
        }

        public void Warn(object obj, params object[] moreObjects)
        {
            Log(LogLevel.Warn, this.callerName, FormatObjects(obj, moreObjects));
        }

        #endregion

        #region Error log

        public void Error(string message)
        {
            Log(LogLevel.Error, this.callerName, FormatMessage(message));
        }

        public void Error(object obj, params object[] moreObjects)
        {
            Log(LogLevel.Error, this.callerName, FormatObjects(obj, moreObjects));
        }

        #endregion

        #region Fatal log

        public void Fatal(string message)
        {
            Log(LogLevel.Fatal, this.callerName, FormatMessage(message));
        }

        public void Fatal(object obj, params object[] moreObjects)
        {
            Log(LogLevel.Fatal, this.callerName, FormatObjects(obj, moreObjects));
        }

        #endregion

        #endregion

        #region Private Constructor

        Logger(string callerName)
        {
            this.callerName = callerName;
        }

        #endregion

        #region private methods

        void Log(LogLevel level, string callerName, string message)
        {
            if (bootStrap != null)
            {
                BootStrap();
            }
            if (handlers.Count == 0)
            {
                return;
            }
            if (level > logLevel)
            {
                return;
            }
            var report = new LoggerReport(level, callerName, message);
            foreach (var handler in handlers)
            {
                handler.HandleLog(report);
            }
        }

        string FormatMessage(string message)
        {
            if (message == null)
            {
                return NullStringMessage;
            }
            if (message == string.Empty)
            {
                return EmptyStringMessage;
            }
            return message;
        }

        string FormatObject(object obj)
        {
            if (obj == null)
            {
                return NullObjectMessage;
            }
            if (obj is string)
            {
                return FormatMessage((string) obj);
            }
            return obj.ToString();
        }

        string FormatObjects(object obj, params object[] moreObjects)
        {
            if (moreObjects == null || moreObjects.Length <= 0)
            {
                return FormatObject(obj);
            }
            if (obj is string && ((string) obj).Contains("{0}"))
            {
                return string.Format((string) obj, moreObjects);
            }

            StringBuilder builder = new StringBuilder();
            builder.Append(FormatObject(obj));
            if (moreObjects != null && moreObjects.Length > 0)
            {
                foreach (object moreObject in moreObjects)
                {
                    builder.Append(", ");
                    builder.Append(FormatObject(moreObject));
                }
            }
            return builder.ToString();
        }

        #endregion

        #region Initializer

        /// <summary>
        /// メインスレッド以外でLoggerを使用する場合、メインスレッドでこのメソッドを先に実行してください
        /// メインスレッドのみの利用の場合は実行する必要はありません
        /// </summary>
        public static void BootStrap()
        {
            if (bootStrap == null)
            {
                return;
            }
            bootStrap();
        }

        static void BootStrapCore()
        {
            // とにかくいけてない
            // UnityEngine.Debug.isDebugBuildがMain Threadでしか呼べないばっかりに
            // こういう実装しないと
            // EntryPointなMonoBehaviourのstaticフィールドにLogger Objectを持ちたくても持てない
            // Logger.Initialize()みたいなのは極力プロダクションコードに呼ばせるべきではない
            bootStrap = null;
            logLevel = UnityEngine.Debug.isDebugBuild ? LogLevel.All : LogLevel.Fatal;
            DebugLogHandler.Initialize();
        }

        #endregion
    }
}

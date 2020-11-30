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
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;

#endif

namespace Jackpot
{
    public class MacPlatform
    {
        [DllImport("KJMUnityPlugins")]
        static extern void _KJMCore_Init(bool inEditor);

        [DllImport("KJMUnityPlugins")]
        static extern string _KJMCore_GetBundleIdentifier();

        [DllImport("KJMUnityPlugins")]
        static extern string _KJMCore_GetMainBundlePlistInfo(string key);

        [DllImport("KJMUnityPlugins")]
        static extern string _KJMCore_GetHomeDirectory();

        [DllImport("KJMUnityPlugins")]
        static extern string _KJMCore_VerifyExcludedPathFromBackup(string fullPath);

        [DllImport("KJMUnityPlugins")]
        static extern string _KJMCore_ExcludePathFromBackup(string fullPath);

        [DllImport("KJMUnityPlugins")]
        static extern void _KJMCore_Log(string message);

        [DllImport("KJMUnityPlugins")]
        static extern string _KJMCore_GetCurrentLocaleIdentifier();

        [DllImport("KJMUnityPlugins")]
        static extern string _KJMCore_GetCurrentLocaleCountryCode();

        [DllImport("KJMUnityPlugins")]
        static extern string _KJMCore_GetCurrentLocaleLanguageCode();

        [DllImport("KJMUnityPlugins")]
        static extern string _KJMCore_GetOSVersion();

        [DllImport("KJMUnityPlugins")]
        static extern int _KJMCore_GetBatteryStatus();

        [DllImport("KJMUnityPlugins")]
        static extern float _KJMCore_GetBatteryLevel();

        [DllImport("KJMUnityPlugins")]
        static extern long _KJMCore_GetDiskFreeSpace();

        const string BundleIdentifierKey = "Jackpot.Editor.BundleIdentifier";
        const string BundleVersionKey = "Jackpot.Editor.BundleVersion";
        const string PersistentDirectoryPathKey = "Jackpot.Editor.PersistentDirectoryPath";
        const string InternalPersistentDirectoryPathKey = "Jackpot.Editor.InternalPersistentDirectoryPath";
        const string ExternalPersistentDirectoryPathKey = "Jackpot.Editor.ExternalPersistentDirectoryPath";
        const string CacheDirectoryPathKey = "Jackpot.Editor.CacheDirectoryPath";

        public static string GetBundleIdentifier()
        {
            string result = ApplicationCache.Instance.Get<string>(BundleIdentifierKey, string.Empty);
            if (string.IsNullOrEmpty(result))
            {
                result = _KJMCore_GetBundleIdentifier();
                ApplicationCache.Instance.Set<string>(BundleIdentifierKey, result);
            }
            return result;
        }

        public static string GetBundleVersion()
        {
            string result = ApplicationCache.Instance.Get<string>(BundleVersionKey, string.Empty);
            if (string.IsNullOrEmpty(result))
            {
                result = _KJMCore_GetMainBundlePlistInfo("CFBundleVersion");
                ApplicationCache.Instance.Set<string>(BundleVersionKey, result);
            }
            return result;
        }

        public static void Log(string message, string tag = "")
        {
            if (!string.IsNullOrEmpty(tag))
            {
                message = string.Format("[{0}] {1}", tag, message);
            }
            _KJMCore_Log(message);
        }

        public static void Log(LoggerReport report)
        {
            _KJMCore_Log(report.ToString());
        }

        public static long GetDiskFreeSpace()
        {
            return _KJMCore_GetDiskFreeSpace();
        }

        /// <summary>
        /// 永続化の為のディレクトリパスを取得します
        /// </summary>
        /// <returns>The persistent directory path.</returns>
        /// <param name="suffix">Suffix.</param>
        public static string GetPersistentDirectoryPath(string suffix = "")
        {
            string result = ApplicationCache.Instance.Get<string>(PersistentDirectoryPathKey, string.Empty);
            if (string.IsNullOrEmpty(result))
            {
                var applicationSupportDirectory = GetInternalPersistentDirectoryPath();
                // setup Application Support directory
                CreateDirectoryIfNeeded(applicationSupportDirectory);
                var verifyExcludedError = _KJMCore_VerifyExcludedPathFromBackup(applicationSupportDirectory);
                if (!string.IsNullOrEmpty(verifyExcludedError))
                {
                    var excludeError = _KJMCore_ExcludePathFromBackup(applicationSupportDirectory);
                    if (!string.IsNullOrEmpty(excludeError))
                    {
                        Logger.Get<MacPlatform>().Error(excludeError);
                    }
                }
                result = applicationSupportDirectory + "/files";
                ApplicationCache.Instance.Set<string>(PersistentDirectoryPathKey, result);
            }

            suffix = (suffix ?? string.Empty).Trim().Trim('/');
            if (suffix.Length > 0)
            {
                result += "/" + suffix;
            }
            CreateDirectoryIfNeeded(result);
            return result;
        }

        /// <summary>
        /// 永続化の為の内部ディレクトリパスを取得します
        /// </summary>
        /// <remarks>
        /// このメソッドでは、内部ディレクトリパスのみを返します。
        /// ( + "/files"は付加しません)
        /// GetPersistentDirectoryPath()とは返すパスが違うことに注意してください。
        /// </remarks>
        /// <returns>The internal persistent directory path.</returns>
        public static string GetInternalPersistentDirectoryPath()
        {
            string result = ApplicationCache.Instance.Get<string>(InternalPersistentDirectoryPathKey, string.Empty);
            if (string.IsNullOrEmpty(result))
            {
                var homeDirectory = _KJMCore_GetHomeDirectory();
                result = homeDirectory + "/Library/Application Support";
                ApplicationCache.Instance.Set<string>(InternalPersistentDirectoryPathKey, result);
            }
            return result;
        }

        /// <summary>
        /// 永続化の為の外部ディレクトリパスを取得します
        /// </summary>
        /// <remarks>
        /// 本来は、外部ディレクトリパスを返すメソッドです。
        /// 現状、枠組みだけ作っていて内部ディレクトリパスを返すようにしています。
        /// </remarks>
        /// <returns>The external persistent directory path.</returns>
        public static string GetExternalPersistentDirectoryPath()
        {
            string result = ApplicationCache.Instance.Get<string>(ExternalPersistentDirectoryPathKey, string.Empty);
            if (string.IsNullOrEmpty(result))
            {
                result = GetInternalPersistentDirectoryPath();
                ApplicationCache.Instance.Set<string>(ExternalPersistentDirectoryPathKey, result);
            }
            return result;
        }

        public static string GetCacheDirectoryPath(string suffix = "")
        {
            string result = ApplicationCache.Instance.Get<string>(CacheDirectoryPathKey, string.Empty);
            if (string.IsNullOrEmpty(result))
            {
                string homeDirectory = _KJMCore_GetHomeDirectory();
                result = homeDirectory + "/Library/Caches/files";
                ApplicationCache.Instance.Set<string>(CacheDirectoryPathKey, result);
            }

            suffix = (suffix ?? string.Empty).Trim().Trim('/');
            if (suffix.Length > 0)
            {
                result += "/" + suffix;
            }
            CreateDirectoryIfNeeded(result);
            return result;
        }

        public static string GetLocaleIdentifier()
        {
            return _KJMCore_GetCurrentLocaleIdentifier();
        }

        public static string GetLocaleCountryCode()
        {
            return _KJMCore_GetCurrentLocaleCountryCode();
        }

        public static string GetLocaleLanguageCode()
        {
            return _KJMCore_GetCurrentLocaleLanguageCode();
        }

        public static string GetDeviceTimeZone()
        {
            return TimeZone.CurrentTimeZone.StandardName;
        }

        public static bool IsIos()
        {
            return false;
        }

        public static bool IsAndroid()
        {
            return false;
        }

        public static bool IsEditor()
        {
            return true;
        }

        public static Version GetOSVersion()
        {
            return Version.Of(_KJMCore_GetOSVersion());
        }

        public static float GetUsedMemory()
        {
#if UNITY_5_6_OR_NEWER
            return Profiler.GetTotalAllocatedMemoryLong() / (1024f * 1024f);
#else
            return Profiler.GetTotalAllocatedMemory() / (1024f * 1024f);
#endif
        }

        public static void LoadResemaraDetectionIdentifier(Action<string> onLoad)
        {
            onLoad(SystemInfo.deviceUniqueIdentifier);
        }

        public static bool IsRooted()
        {
            return false;
        }

        public static bool IsDebuggerAttached()
        {
            var result = IsUsbDebuggable();
            return result ? result : SubscribeAssetStateLog();
        }

        public static bool IsUsbDebuggable()
        {
            return false;
        }

        public static bool IsEmulator()
        {
            return GetInvalidDeviceProperties().Count > 0;
        }

        public static bool IsWritable(string path)
        {
            return false;
        }

        public static ICollection<SimpleAnalysisEntry> GetInvalidDeviceProperties()
        {
            return new List<SimpleAnalysisEntry>();
        }

        public static bool SubscribeAssetStateLog()
        {
            return false;
        }

        public static string GenerateAssetStateLog(string seed)
        {
            return string.Empty;
        }

        public static BatteryStatusKind GetBatteryStatus()
        {
            var status = (BatteryStatusKind) Enum.ToObject(
                typeof(BatteryStatusKind),
                _KJMCore_GetBatteryStatus()
            );
            return status;
        }

        public static int GetBatteryLevel()
        {
            return (int) (_KJMCore_GetBatteryLevel() * 100f);
        }

        /// <summary>
        /// バッテリー温度の取得
        /// </summary>
        /// <returns>The battery temperature.</returns>
        public static float GetBatteryTemperature()
        {
            return 0.0f;
        }

        /// <summary>
        /// バッテリー電圧（mV）の取得
        /// </summary>
        /// <returns>The battery voltage.</returns>
        public static int GetBatteryVoltage()
        {
            return 0;
        }

        /// <summary>
        /// 温度状態の取得
        /// </summary>
        /// <remarks>
        /// サポート対象外のため-1の値を返す
        /// </remarks>>
        /// <returns></returns>
        public static int GetThermalState()
        {
            return -1;
        }

        /// <summary>
        /// 最高フレームレートの取得
        /// </summary>
        /// <remarks>
        /// サポート対象外のため−1の値を返す
        /// </remarks>
        /// <returns></returns>
        public static int GetMaximumFramesPerSecond()
        {
            return -1;
        }

        public static float GetFreeMemory()
        {
            return 0;
        }

        public static float GetTotalMemory()
        {
            return 0;
        }

        public static bool LaunchApp(string urlScheme, string appIdOrPackageName)
        {
            return false;
        }

        public static bool StoreReview()
        {
            return false;
        }

        static void CreateDirectoryIfNeeded(string directoryPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            if (!directoryInfo.Exists)
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        static MacPlatform()
        {
            _KJMCore_Init(true);
            MacUnitySendMessage.Initialize();
        }
        
        /// <summary>
        /// AndroidのToast表示
        /// </summary>
        /// <remarks>
        /// Androidのみなのでとりあえず処理なし
        /// </remarks>
        /// <param name="message">Toastに表示するメッセージ</param>
        /// <param name="isLong">Toastの表示時間をLongにするフラグ</param>
        public static void ShowToastMessage(string message, bool isLong)
        {
        }
    }
}

#endif

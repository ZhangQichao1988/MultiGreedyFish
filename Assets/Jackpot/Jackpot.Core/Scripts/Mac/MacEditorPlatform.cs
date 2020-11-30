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
#if UNITY_EDITOR_OSX
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
    public class MacEditorPlatform
    {
        [DllImport("KJMUnityPlugins")]
        static extern void _KJMCore_Init(bool inEditor);

        [DllImport("KJMUnityPlugins")]
        static extern string _KJMCore_GetCurrentLocaleIdentifier();

        [DllImport("KJMUnityPlugins")]
        static extern string _KJMCore_GetCurrentLocaleCountryCode();

        [DllImport("KJMUnityPlugins")]
        static extern string _KJMCore_GetCurrentLocaleLanguageCode();

        [DllImport("KJMUnityPlugins")]
        static extern string _KJMCore_GetOSVersion();

        const string BundleIdentifierKey = "Jackpot.Editor.BundleIdentifier";
        const string BundleVersionKey = "Jackpot.Editor.BundleVersion";
        const string PersistentDirectoryPathKey = "Jackpot.Editor.PersistentDirectoryPath";
        const string InternalPersistentDirectoryPathKey = "Jackpot.Editor.InternalPersistentDirectoryPath";
        const string ExternalPersistentDirectoryPathKey = "Jackpot.Editor.ExternalPersistentDirectoryPath";
        const string CacheDirectoryPathKey = "Jackpot.Editor.CacheDirectoryPath";

        public static string GetBundleIdentifier()
        {
#if UNITY_5_6_OR_NEWER
            return UnityEditor.PlayerSettings.applicationIdentifier;
#else
            return UnityEditor.PlayerSettings.bundleIdentifier;
#endif
        }

        public static string GetBundleVersion()
        {
            return UnityEditor.PlayerSettings.bundleVersion;
        }

        public static void Log(string message, string tag = "")
        {
            if (!string.IsNullOrEmpty(tag))
            {
                message = string.Format("[{0}] {1}", tag, message);
            }
            Debug.Log(message);
        }

        public static void Log(LoggerReport report)
        {
            switch (report.LogLevel)
            {
                case LogLevel.Fatal:
                case LogLevel.Error:
                    Debug.LogError(report.ToString());
                    return;
                case LogLevel.Warn:
                    Debug.LogWarning(report.ToString());
                    return;
                default:
                    Debug.Log(report.ToString());
                    return;
            }
        }

        public static long GetDiskFreeSpace()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(GetPersistentDirectoryPath());
            DriveInfo driveInfo = new DriveInfo(directoryInfo.Root.FullName);
            long result = (driveInfo.TotalFreeSpace / 1024);
            return result;
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
                result = GetInternalPersistentDirectoryPath();
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
        /// <returns>The internal persistent directory path.</returns>
        public static string GetInternalPersistentDirectoryPath()
        {
            var result = ApplicationCache.Instance.Get<string>(InternalPersistentDirectoryPathKey, string.Empty);
            if (string.IsNullOrEmpty(result))
            {
                var combined = Path.Combine(Application.dataPath, "../home/persistent/files");
                var info = new FileInfo(combined);
                result = info.FullName;
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
                var combined = Path.Combine(Application.dataPath, "../home/caches/files");
                var info = new FileInfo(combined);
                result = info.FullName;
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

        public static double GetUsedMemory()
        {
#if UNITY_5_6_OR_NEWER
            return Profiler.GetTotalAllocatedMemoryLong() / (1024d * 1024d);
#else
            return Profiler.GetTotalAllocatedMemory() / (1024d * 1024d);
#endif
        }

        public static double GetFreeMemory()
        {
            return 0;
        }

        public static double GetTotalMemory()
        {
            return 0;
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
            return BatteryStatusKind.Unknown;
        }

        public static int GetBatteryLevel()
        {
            return 0;
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

        public static bool LaunchApp(string urlScheme, string appIdOrPackageName)
        {
            return false;
        }

        public static bool StoreReview()
        {
            return false;
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

        static void CreateDirectoryIfNeeded(string directoryPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            if (!directoryInfo.Exists)
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        static MacEditorPlatform()
        {
            _KJMCore_Init(true);
            MacUnitySendMessage.Initialize();
        }
    }
}

#endif

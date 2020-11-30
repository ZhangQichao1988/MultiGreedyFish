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

#if UNITY_IOS
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Jackpot
{
    /// <summary>
    /// iOS上での実行時Jackpot.Platformは代わりにこのクラスを使用します
    /// </summary>
    internal partial class IosPlatform
    {
#region Extern Functions

        [DllImport("__Internal")]
        static extern long _KJPCore_GetFreeSpace();

        [DllImport("__Internal")]
        static extern string _KJPCore_GetCurrentLocaleIdentifier();

        [DllImport("__Internal")]
        static extern string _KJPCore_GetCurrentLocaleCountryCode();

        [DllImport("__Internal")]
        static extern string _KJPCore_GetCurrentLocaleLanguageCode();

        [DllImport("__Internal")]
        static extern string _KJPCore_GetDeviceTimeZone();

        [DllImport("__Internal")]
        static extern string _KJPCore_GetBundleIdentifier();

        [DllImport("__Internal")]
        static extern string _KJPCore_GetMainBundlePlistInfo(string key);

        [DllImport("__Internal")]
        static extern string _KJPCore_GetHomeDirectory();

        [DllImport("__Internal")]
        static extern string _KJPCore_VerifyExcludedPathFromBackup(string fullPath);

        [DllImport("__Internal")]
        static extern string _KJPCore_ExcludePathFromBackup(string fullPath);

        [DllImport("__Internal")]
        static extern void _KJPCore_Log(string message);

        [DllImport("__Internal")]
        static extern string _KJPCore_GetOSVersion();

        [DllImport("__Internal")]
        static extern uint _KJPCore_GetUsedMemory();

        [DllImport("__Internal")]
        static extern long _KJPCore_GetFreeMemory();

        [DllImport("__Internal")]
        static extern ulong _KJPCore_GetTotalMemory();

        [DllImport("__Internal")]
        static extern string _KJPCore_LoadResemaraDetectionIdentifier();

        [DllImport("__Internal")]
        static extern void _KJPCore_SubscribeData(out bool output, out bool error);

        [DllImport("__Internal")]
        static extern int _KJPCore_GetBatteryStatus();

        [DllImport("__Internal")]
        static extern float _KJPCore_GetBatteryLevel();

        [DllImport("__Internal")]
        static extern bool _KJPCore_LaunchApp(string urlScheme, string appId);

        [DllImport("__Internal")]
        static extern bool _KJPCore_StoreReview();

        [DllImport("__Internal")]
        static extern void _KJACore_AssetStateLogGenerateV2(StringBuilder buf, int bufsize, string seed);

        [DllImport("__Internal")]
        static extern int _KJPCore_GetThermalState();

        [DllImport(("__Internal"))]
        static extern int _KJPCore_GetMaximumFramesPerSecond();

#endregion

#region  Enums

        public enum ThermalState
        {
            Unknown = -1, // 取得不可(※サポート対象外)
            Nominal = 0, // 正常範囲内
            Fair = 1, // わずかに上昇
            Serious = 2, // 高い
            Critical = 3, // システム性能に大きな影響を与えているため、デバイスを冷却する必要があります
        }

#endregion

#region Constants

        const string BundleIdentifierKey = "Jackpot.Ios.BundleIdentifier";
        const string BundleVersionKey = "Jackpot.Ios.BundleVersion";
        const string PersistentDirectoryPathKey = "Jackpot.Ios.PersistentDirectoryPath";
        const string InternalPersistentDirectoryPathKey = "Jackpot.Ios.InternalPersistentDirectoryPath";
        const string ExternalPersistentDirectoryPathKey = "Jackpot.Ios.ExternalPersistentDirectoryPath";
        const string CacheDirectoryPathKey = "Jackpot.Ios.CacheDirectoryPath";

#endregion

#region Public Functions

        /// <summary>
        /// BundleIdentifierを取得します
        /// </summary>
        /// <returns>The bundle identifier.</returns>
        public static string GetBundleIdentifier()
        {
            string result = ApplicationCache.Instance.Get<string>(BundleIdentifierKey, string.Empty);
            if (string.IsNullOrEmpty(result))
            {
                result = _KJPCore_GetBundleIdentifier();
                ApplicationCache.Instance.Set<string>(BundleIdentifierKey, result);
            }

            return result;
        }

        /// <summary>
        /// BundleVersionを取得します
        /// </summary>
        /// <returns>The bundle version.</returns>
        public static string GetBundleVersion()
        {
            string result = ApplicationCache.Instance.Get<string>(BundleVersionKey, string.Empty);
            if (string.IsNullOrEmpty(result))
            {
                result = _KJPCore_GetMainBundlePlistInfo("CFBundleVersion");
                ApplicationCache.Instance.Set<string>(BundleVersionKey, result);
            }

            return result;
        }

        /// <summary>
        /// NSLogを使用したログ出力を実行します
        /// </summary>
        /// <remarks>
        /// リリースビルド時にLogを出力しない、といった制御はしていないので
        /// そういう需要がある場合、JackPort.Loggerを使用したログ出力を推奨します
        /// </remarks>
        /// <param name="message">Message.</param>
        /// <param name="tag">Tag.</param>
        public static void Log(string message, string tag = "")
        {
            if (!string.IsNullOrEmpty(tag))
            {
                message = string.Format("[{0}] {1}", tag, message);
            }

            _KJPCore_Log(message);
        }

        /// <summary>
        /// Log the specified report.
        /// </summary>
        /// <param name="report">Report.</param>
        public static void Log(LoggerReport report)
        {
            _KJPCore_Log(report.ToString());
        }

        public static long GetDiskFreeSpace()
        {
            return _KJPCore_GetFreeSpace();
        }

        /// <summary>
        /// 永続化の為のディレクトリパスを取得します
        /// </summary>
        /// <remarks>
        /// "[アプリケーションのホームディレクトリ]/Library/Application Support/files"のパスを返します。
        /// suffixが指定されている場合は、上記パス + suffixのディレクトリを作成してパスを返します。
        /// これらのパスは本来iCloudの保存対象ですが、当メソッドを使用してパスを取得した場合、保存対象から外れます。
        /// </remarks>>
        /// <returns>The persistent directory path.</returns>
        /// <param name="suffix">Suffix.</param>
        public static string GetPersistentDirectoryPath(string suffix = "")
        {
            string result = ApplicationCache.Instance.Get<string>(PersistentDirectoryPathKey, string.Empty);
            if (string.IsNullOrEmpty(result))
            {
                var applicationSupportDirectory = GetInternalPersistentDirectoryPath();

                // setup Application Support directory
                if (!DirectoryExists(applicationSupportDirectory))
                {
                    CreateDirectoryIfNeeded(applicationSupportDirectory);
                    ExcludePathFromBackup(applicationSupportDirectory);
                }

                result = applicationSupportDirectory + "/files";
                ApplicationCache.Instance.Set<string>(PersistentDirectoryPathKey, result);
            }

            suffix = (suffix ?? string.Empty).Trim().Trim('/');
            if (suffix.Length > 0)
            {
                result += "/" + suffix;
            }

            if (!DirectoryExists(result))
            {
                CreateDirectoryIfNeeded(result);
                ExcludePathFromBackup(result);
            }

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
                var homeDirectory = _KJPCore_GetHomeDirectory();
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
            string result = ApplicationCache.Instance.Get<string>(CacheDirectoryPathKey);
            if (string.IsNullOrEmpty(result))
            {
                string homeDirectory = _KJPCore_GetHomeDirectory();
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
            return _KJPCore_GetCurrentLocaleIdentifier();
        }

        public static string GetLocaleCountryCode()
        {
            return _KJPCore_GetCurrentLocaleCountryCode();
        }

        public static string GetLocaleLanguageCode()
        {
            return _KJPCore_GetCurrentLocaleLanguageCode();
        }

        public static string GetDeviceTimeZone()
        {
            return _KJPCore_GetDeviceTimeZone();
        }

        public static bool IsIos()
        {
            return true;
        }

        public static bool IsAndroid()
        {
            return false;
        }

        public static bool IsEditor()
        {
            return false;
        }

        public static Version GetOSVersion()
        {
            return Version.Of(_KJPCore_GetOSVersion());
        }

        public static double GetUsedMemory()
        {
            return _KJPCore_GetUsedMemory() / (1024d * 1024d);
        }

        public static double GetFreeMemory()
        {
            return _KJPCore_GetFreeMemory() / 1024d;
        }

        public static double GetTotalMemory()
        {
            return _KJPCore_GetTotalMemory() / 1024d;
        }

        /// <summary>
        /// リセマラ判定用の端末固有のIDを取得します
        /// ネイティブプラグイン側ではUUID+BundleIDのMD5ハッシュ値をキーチェーンに保存して返却しています。
        /// </summary>
        public static void LoadResemaraDetectionIdentifier(Action<string> onLoad)
        {
            onLoad(_KJPCore_LoadResemaraDetectionIdentifier());
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

        public static bool SubscribeAssetStateLog()
        {
            bool ret, error;
            _KJPCore_SubscribeData(out ret, out error);
            return !error && ret;
        }

        public static bool IsEmulator()
        {
            return GetInvalidDeviceProperties().Count > 0;
        }

        /// <summary>
        /// 指定のパスが書き込み可能か
        /// </summary>
        /// <remarks>
        /// 本来は、指定のパスが書き込み可能かの値を返します。
        /// 現状、枠組みだけ作っていてfalseの値を返すようにしています。
        /// </remarks>
        /// <param name="path">path</param>
        /// <returns>The specified path is writable?</returns>
        public static bool IsWritable(string path)
        {
            return false;
        }

        public static ICollection<SimpleAnalysisEntry> GetInvalidDeviceProperties()
        {
            return new List<SimpleAnalysisEntry>();
        }

        public static BatteryStatusKind GetBatteryStatus()
        {
            var status = (BatteryStatusKind) Enum.ToObject(
                typeof(BatteryStatusKind),
                _KJPCore_GetBatteryStatus()
            );
            return status;
        }

        public static int GetBatteryLevel()
        {
            return (int) (_KJPCore_GetBatteryLevel() * 100f);
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
        /// ThermalStateのenumと比較を推奨
        /// </remarks>>
        /// <returns>The thermal status.</returns>
        public static int GetThermalState()
        {
            return _KJPCore_GetThermalState();
        }

        /// <summary>
        /// 最高フレームレートの取得
        /// </summary>
        /// <returns>The maximum frames per second.</returns>
        public static int GetMaximumFramesPerSecond()
        {
            return _KJPCore_GetMaximumFramesPerSecond();
        }


        public static bool LaunchApp(string urlScheme, string appId)
        {
            return _KJPCore_LaunchApp(urlScheme, appId);
        }

        public static bool StoreReview()
        {
            return _KJPCore_StoreReview();
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

#endregion

#region Private Methods

        static bool DirectoryExists(string directoryPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            return directoryInfo.Exists;
        }

        static void CreateDirectoryIfNeeded(string directoryPath)
        {
            if (!DirectoryExists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        public static string GenerateAssetStateLog(string seed)
        {
            StringBuilder buf = new StringBuilder(1024);
            _KJACore_AssetStateLogGenerateV2(buf, buf.Capacity, seed);
            return buf.ToString();
        }

        static void ExcludePathFromBackup(string directoryPath)
        {
            var verifyExcludedError = _KJPCore_VerifyExcludedPathFromBackup(directoryPath);
            if (!string.IsNullOrEmpty(verifyExcludedError))
            {
                var excludeError = _KJPCore_ExcludePathFromBackup(directoryPath);
                if (!string.IsNullOrEmpty(excludeError))
                {
                    Logger.Get<IosPlatform>().Error(excludeError);
                }
            }
        }

#endregion
    }
}
#endif

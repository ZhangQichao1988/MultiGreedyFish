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
using System.IO;
using UnityEngine;
#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif
#if UNITY_EDITOR_OSX
using Impl = Jackpot.MacEditorPlatform;

#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
using Impl = Jackpot.UnityWindowsPlatform;

#elif UNITY_EDITOR
using Impl = Jackpot.UnityEditorPlatform;

#elif UNITY_IOS
using Impl = Jackpot.IosPlatform;

#elif UNITY_ANDROID
using Impl = Jackpot.AndroidPlatform;

#elif UNITY_STANDALONE_OSX
using Impl = Jackpot.MacPlatform;

#else
using Impl = Jackpot.UnityEditorPlatform;
#endif

namespace Jackpot
{
    /// <summary>
    /// プラットフォーム依存なインターフェースを持つクラスです
    /// </summary>
    public partial class Platform
    {
#region Public Functions

        /// <summary>
        /// BundleIdentifierを返却します
        /// </summary>
        /// <returns>The bundle identifier.</returns>
        public static string GetBundleIdentifier()
        {
            return Impl.GetBundleIdentifier();
        }

        /// <summary>
        /// BundleVersionを返却します
        /// </summary>
        /// <returns>The bundle version.</returns>
        public static string GetBundleVersion()
        {
            return Impl.GetBundleVersion();
        }

        /// <summary>
        /// ログ出力を行います
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="tag">Tag.</param>
        public static void Log(string message, string tag = "")
        {
            Impl.Log(message, tag);
        }

        /// <summary>
        /// Log the specified report.
        /// </summary>
        /// <param name="report">Report.</param>
        public static void Log(LoggerReport report)
        {
            Impl.Log(report);
        }

        /// <summary>
        /// ディスクの空き領域(KB)を返却します
        /// </summary>
        /// <returns>The disk free space.</returns>
        public static long GetDiskFreeSpace()
        {
            return Impl.GetDiskFreeSpace();
        }

        /// <summary>
        /// 永続化の為のディレクトリを返却します。iCloudでバックアップされません
        /// </summary>
        /// <returns>The persistent directory path.</returns>
        /// <param name="suffix">Suffix.</param>
        public static string GetPersistentDirectoryPath(string suffix = "")
        {
            return Impl.GetPersistentDirectoryPath(suffix);
        }

        /// <summary>
        /// 永続化の為の内部ディレクトリを返却します。
        /// </summary>
        /// <returns>The Internal persistent directory path.</returns>
        public static string GetInternalPersistentDirectoryPath()
        {
            return Impl.GetInternalPersistentDirectoryPath();
        }

        /// <summary>
        /// 永続化の為の外部ディレクトリを返却します
        /// </summary>
        /// <returns>The External persistent directory path.</returns>
        public static string GetExternalPersistentDirectoryPath()
        {
            return Impl.GetExternalPersistentDirectoryPath();
        }

        /// <summary>
        /// キャッシュディレクトリを返却します。OS側から削除される事を想定する必要があります
        /// </summary>
        /// <returns>The cache directory path.</returns>
        /// <param name="suffix">Suffix.</param>
        public static string GetCacheDirectoryPath(string suffix = "")
        {
            return Impl.GetCacheDirectoryPath(suffix);
        }

        /// <summary>
        /// 端末に設定されているロケール文字列を返却します。OSごとに異なる値が返却される事に注意してください
        /// </summary>
        /// <returns>The locale identifier.</returns>
        public static string GetLocaleIdentifier()
        {
            return Impl.GetLocaleIdentifier();
        }

        /// <summary>
        /// 端末に設定されている国コードを返却します。OSごとに異なる値が返却される事に注意してください
        /// </summary>
        /// <returns>The locale country code.</returns>
        public static string GetLocaleCountryCode()
        {
            return Impl.GetLocaleCountryCode();
        }

        /// <summary>
        /// 端末に設定されている言語コードを返却します。OSごとに異なる値が返却される事に注意してください
        /// </summary>
        /// <returns>The locale language code.</returns>
        public static string GetLocaleLanguageCode()
        {
            return Impl.GetLocaleLanguageCode();
        }

        /// <summary>
        /// 端末のタイムゾーンを返却します
        /// </summary>
        /// <returns>The device time zone.</returns>
        public static string GetDeviceTimeZone()
        {
            return Impl.GetDeviceTimeZone();
        }

        /// <summary>
        /// 実行プラットフォームがiOSか否かを示します
        /// </summary>
        /// <returns><c>true</c> if is ios; otherwise, <c>false</c>.</returns>
        public static bool IsIos()
        {
            return Impl.IsIos();
        }

        /// <summary>
        /// 実行プラットフォームがAndroidか否かを示します
        /// </summary>
        /// <returns><c>true</c> if is android; otherwise, <c>false</c>.</returns>
        public static bool IsAndroid()
        {
            return Impl.IsAndroid();
        }

        /// <summary>
        /// 実行プラットフォームがUnityEditorか否かを示します
        /// </summary>
        /// <returns><c>true</c> if is editor; otherwise, <c>false</c>.</returns>
        public static bool IsEditor()
        {
            return Impl.IsEditor();
        }

        /// <summary>
        /// 実行プラットフォームのOSVersionを返却します
        /// </summary>
        /// <returns>The OS version.</returns>
        public static Version GetOSVersion()
        {
            return Impl.GetOSVersion();
        }

        /// <summary>
        /// 端末上で対象のアプリがどれだけの実メモリを使用しているか返します
        /// </summary>
        /// <returns>Used memory. Unit is Mega Byte.</returns>
        public static double GetUsedMemory()
        {
            return Impl.GetUsedMemory();
        }

        /// <summary>
        /// メモリの空き領域(KB)を返却します
        /// </summary>
        /// <returns>The memory free space</returns>
        public static double GetFreeMemory()
        {
            return Impl.GetFreeMemory();
        }

        /// <summary>
        /// 総メモリの領域(KB)を返却します
        /// </summary>
        /// <returns>The memory total space</returns>
        public static double GetTotalMemory()
        {
            return Impl.GetTotalMemory();
        }

        /// <summary>
        /// リセマラ判定用IDを取得します
        /// </summary>
        /// <returns>The Device UniqueId.</returns>
        public static void LoadResemaraDetectionIdentifier(Action<string> onLoad)
        {
            Impl.LoadResemaraDetectionIdentifier(onLoad);
        }

        /// <summary>
        /// 実行環境の端末がroot化されているか否かを示します
        /// </summary>
        /// <remarks>
        /// Androidのみ対応しています。他プラットフォームではfalseを返却します
        /// </remarks>
        /// <returns><c>true</c> if is rooted; otherwise, <c>false</c>.</returns>
        public static bool IsRooted()
        {
            return Impl.IsRooted();
        }

        /// <summary>
        /// [このAPIはr60にて削除されます] 実行アプリに対し、デバッガがアタッチされているか否かを示します
        /// </summary>
        /// <remarks>
        /// Platform.IsUsbDebuggable()と、Platform.SubscribeAssetStateLog()の両方を実施します
        /// </remarks>
        /// <returns><c>true</c> if is debugger attached; otherwise, <c>false</c>.</returns>
        [Obsolete("This method will remove at r60. Instead, please use Jackpot.Platform.IsUsbDebuggable(), Jackpot.Platform.IsEmulator(), Jackpot.Platform.SubscribeAssetStateLog()")]
        public static bool IsDebuggerAttached()
        {
            return Impl.IsDebuggerAttached();
        }

        /// <summary>
        /// 実行アプリに対し、USBデバッグが可能な状態にあるか否かを示します
        /// </summary>
        /// <remarks>
        /// 現在はAndroidのJDWP互換なデバッガのアタッチ検出のみ対応しています。
        /// </remarks>
        /// <returns><c>true</c> if is usb debuggable; otherwise, <c>false</c>.</returns>
        public static bool IsUsbDebuggable()
        {
            return Impl.IsUsbDebuggable();
        }

        /// <summary>
        /// アプリがエミュレータ上で実行されているか否かを示します。
        /// </summary>
        /// <remarks>
        /// Androidのみ対応しています。他プラットフォームではfalseを返却します
        /// 1つでも無効なシステムプロパティが存在した場合、エミュレータ判定されますので
        /// 緩和したい場合や、詳細に解析したい場合は<see cref="GetInvalidDeviceProperties()"/> を使用してください。
        /// </remarks>
        /// <returns><c>true</c> if is emulator; otherwise, <c>false</c>.</returns>
        public static bool IsEmulator()
        {
            return Impl.IsEmulator();
        }

        /// <summary>
        /// 指定のパスが書き込み可能か
        /// </summary>
        /// <param name="path">path</param>
        /// <returns>The specified path is writable?</returns>
        public static bool IsWritable(string path)
        {
            return Impl.IsWritable(path);
        }

        /// <summary>
        /// 無効なデバイスプロパティ一覧を取得します。
        /// </summary>
        /// <returns>The invalid device properties.</returns>
        public static ICollection<SimpleAnalysisEntry> GetInvalidDeviceProperties()
        {
            return Impl.GetInvalidDeviceProperties();
        }

        /// <summary>
        /// 実行アプリのプロセスに対し、他プロセスがアタッチされているか否かを示します
        /// </summary>
        /// <remarks>
        /// gdb/lldbの様なプロセスにアタッチするアプリケーションのアタッチ検出に利用します
        /// </remarks>
        /// <returns><c>true</c> if is process attached; otherwise, <c>false</c>.</returns>
        public static bool SubscribeAssetStateLog()
        {
            return Impl.SubscribeAssetStateLog();
        }

        /// <summary>
        /// アプリ内に含まれるバイナリの情報を含む文字列を生成します
        /// </summary>
        /// <remarks>
        /// Androidのみ対応しています。他プラットフォームでは空文字列を返却します
        /// </remarks>
        /// <returns>The asset state log.</returns>
        /// <param name="seed">Seed.</param>
        public static string GenerateAssetStateLog(string seed)
        {
            return Impl.GenerateAssetStateLog(seed);
        }

        /// <summary>
        /// バッテリーの充電状態を返却します
        /// </summary>
        /// <returns>The battery status.</returns>
        public static BatteryStatusKind GetBatteryStatus()
        {
            return Impl.GetBatteryStatus();
        }

        /// <summary>
        /// バッテリーの残量を返却します
        /// </summary>
        /// <returns>The battery level.</returns>
        public static int GetBatteryLevel()
        {
            return Impl.GetBatteryLevel();
        }

        /// <summary>
        /// バッテリー温度の取得
        /// </summary>
        /// <returns>The battery temperature.</returns>
        public static float GetBatteryTemperature()
        {
            return Impl.GetBatteryTemperature();
        }

        /// <summary>
        /// バッテリー電圧（mV）の取得
        /// </summary>
        /// <returns>The battery voltage.</returns>
        public static int GetBatteryVoltage()
        {
            return Impl.GetBatteryVoltage();
        }

        /// <summary>
        /// URLスキーマ/パッケージ名からアプリを起動します。
        /// アプリが端末にインストールされていない場合はFalseを返します。
        /// </summary>
        /// <remarks>
        /// iOSの場合は空文字を、Androidの場合はアプリのパッケージ名を渡して下さい。
        /// </remarks>
        /// <returns><c>true</c>, if app was launched, <c>false</c> otherwise.</returns>
        /// <param name="urlScheme">URL scheme.</param>
        /// <param name="appIdOrPackageName">App identifier or package name.</param>
        public static bool LaunchApp(string urlScheme, string appIdOrPackageName)
        {
            return Impl.LaunchApp(urlScheme, appIdOrPackageName);
        }

        /// <summary>
        /// iOS標準のレビュー誘導ダイアログを表示します。iOS10.3未満の端末を使用している場合はflaseを返却します。
        /// </summary>
        /// <remarks>
        /// iOSのみ対応しています。iOS10.3未満の端末を使用している場合、他プラットフォームではflaseを返却します。
        /// </remarks>
        /// <returns><c>true</c>, if review was stored, <c>false</c> otherwise.</returns>
        public static bool StoreReview()
        {
            return Impl.StoreReview();
        }

        /// <summary>
        /// 温度状態の取得
        /// </summary>
        /// <remarks>
        /// iOSのみ対応。 (iOS11~)
        /// </remarks>>
        /// <returns></returns>
        public static int GetThermalState()
        {
            return Impl.GetThermalState();
        }

        /// <summary>
        /// 最高フレームレートの取得
        /// </summary>
        /// <remarks>
        /// iOSのみ対応。
        /// </remarks>>
        /// <returns></returns>
        public static int GetMaximumFramesPerSecond()
        {
            return Impl.GetMaximumFramesPerSecond();
        }
        
        /// <summary>
        /// AndroidのToast表示
        /// </summary>
        /// <remarks>
        /// Androidのみ対応。
        /// </remarks>
        /// <param name="message">Toastに表示するメッセージ</param>
        public static void ShowToastMessage(string message)
        {
            ShowToastMessage(message, false);
        }

        /// <summary>
        /// AndroidのToast表示
        /// </summary>
        /// <remarks>
        /// Androidのみ対応。
        /// </remarks>
        /// <param name="message">Toastに表示するメッセージ</param>
        /// <param name="isLong">Toastの表示時間をLongにするフラグ</param>
        public static void ShowToastMessage(string message, bool isLong)
        {
            Impl.ShowToastMessage(message, isLong);
        }

#endregion
    }

#if UNITY_EDITOR && !UNITY_EDITOR_WIN

    public class UnityEditorPlatform
    {
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
            UnityEngine.Debug.Log(message);
        }

        public static void Log(LoggerReport report)
        {
            UnityEngine.Debug.Log(report.ToString());
        }

        public static long GetDiskFreeSpace()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(GetPersistentDirectoryPath());
            DriveInfo driveInfo = new DriveInfo(directoryInfo.Root.FullName);
            long result = driveInfo.TotalFreeSpace / 1024;
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
            return "ja_JP";
        }

        public static string GetLocaleCountryCode()
        {
            return "JP";
        }

        public static string GetLocaleLanguageCode()
        {
            return "ja";
        }

        public static string GetDeviceTimeZone()
        {
            return System.TimeZoneInfo.Local.Id;
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
            return Version.Of("");
        }

        public static double GetUsedMemory()
        {
#if UNITY_5_6_OR_NEWER
            return Profiler.GetTotalAllocatedMemoryLong() / (1024d * 1024d);
#else
            return Profiler.GetTotalAllocatedMemory() / (1024d * 1024d);
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

        public static string GenerateAssetStateLog(string seed)
        {
            return string.Empty;
        }

        public static bool SubscribeAssetStateLog()
        {
            return false;
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
    }

#endif
}

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
#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Jackpot
{
    /// <summary>
    /// Android上での実行時Jackpot.Platformは代わりにこのクラスを使用します
    /// </summary>
    partial class AndroidPlatform
    {
        static AndroidJavaObject resemaraObject;

        private static AndroidJavaClass bridge;

        #region Extern Functions

        [DllImport("jackpot-core")]
        static extern bool _KJACore_AssetStateLogAvailable();

        [DllImport("jackpot-core")]
        static extern void _KJACore_AssetStateLogSubscribe(out bool output, out bool err);

        [DllImport("jackpot-core")]
        static extern void _KJACore_AssetStateLogGenerate(StringBuilder buf, int bufsize, string seed);

        [DllImport("jackpot-core")]
        static extern void _KJACore_AssetStateLogGenerateV2(StringBuilder buf, int bufsize, string seed);

        #endregion

        #region Constants

        const string BundleIdentifierKey = "Jackpot.Android.BundleIdentifier";
        const string BundleVersionKey = "Jackpot.Android.BundleVersion";
        const string PersistentDirectoryPathKey = "Jackpot.Android.PersistentDirectoryPath";
        const string InternalPersistentDirectoryPathKey = "Jackpot.Android.InternalPersistentDirectoryPath";
        const string ExternalPersistentDirectoryPathKey = "Jackpot.Android.ExternalPersistentDirectoryPath";
        const string CacheDirectoryPathKey = "Jackpot.Android.CacheDirectoryPath";

        #endregion

        #region Public Functions

        /// <summary>
        /// Bundle Identifierを返却します
        /// </summary>
        /// <returns>The bundle identifier.</returns>
        public static string GetBundleIdentifier()
        {
            var result = ApplicationCache.Instance.Get<string>(BundleIdentifierKey, string.Empty);
            if (string.IsNullOrEmpty(result))
            {
                MainThreadDispatcher.SendAndWait(() =>
                {
                    AndroidUtility.UsingPackageInfo(packageInfo => result = packageInfo.Get<string>("packageName"));
                    ApplicationCache.Instance.Set<string>(BundleIdentifierKey, result);
                });
            }
            return result;
        }

        /// <summary>
        /// Bundle Vesionを返却します
        /// </summary>
        /// <returns>The bundle version.</returns>
        public static string GetBundleVersion()
        {
            var result = ApplicationCache.Instance.Get<string>(BundleVersionKey, string.Empty);
            if (string.IsNullOrEmpty(result))
            {
                MainThreadDispatcher.SendAndWait(() =>
                {
                    AndroidUtility.UsingPackageInfo(packageInfo => result = packageInfo.Get<string>("versionName"));
                    ApplicationCache.Instance.Set<string>(BundleVersionKey, result);
                });
            }
            return result;
        }

        /// <summary>
        /// android.util.Log.dを使用したログ出力を行います
        /// </summary>
        /// <remarks>
        /// リリースビルド時にLogを出力しない、といった制御はしていないので
        /// そういう需要がある場合、JackPort.Loggerを使用したログ出力を推奨します
        /// </remarks>
        /// <param name="message">Message.</param>
        /// <param name="tag">Tag.</param>
        public static void Log(string message, string tag = "")
        {
            if (string.IsNullOrEmpty(tag))
            {
                tag = GetBundleIdentifier();
            }
            UsingLog(log => log.CallStatic<int>("d", tag, message));
        }

        /// <summary>
        /// android.util.Log.dと、LoggerReportを使用したログ出力を行います
        /// </summary>
        /// <param name="report">Report.</param>
        public static void Log(LoggerReport report)
        {
            if (report == null)
            {
                return;
            }
            UsingLog(log => log.CallStatic<int>("d", report.CallerName, report.ToString()));
        }

        /// <summary>
        /// 端末の空き容量(KB)を取得します
        /// </summary>
        /// <returns>The disk free space.</returns>
        public static long GetDiskFreeSpace()
        {
            long result = 0;
            MainThreadDispatcher.SendAndWait(() =>
            {
                using (var unityBridge = new AndroidJavaClass("com.klab.jackpot.UnityBridge"))
                {
                    Interlocked.Exchange(ref result, unityBridge.CallStatic<long>("getDiskFreeSpace"));
                }
            });
            return result;
        }

        /// <summary>
        /// 永続化の為のディレクトリパスを取得します
        /// <remarks>
        /// 外部ディレクトリパスが存在し書込み可能な場合は、外部ディレクトリパス + "/files"を返します。
        /// 外部ディレクトリパスが存在しない、もしくは書き込みが不可な場合は、内部ディレクトリパス + "/files"を返します。
        /// suffixが指定されている場合は上記パス + suffixのディレクトリを作成してパスを返します。
        /// </remarks>>
        /// </summary>
        /// <returns>The persistent directory path.</returns>
        /// <param name="suffix">Suffix.</param>
        public static string GetPersistentDirectoryPath(string suffix = "")
        {
            var result = ApplicationCache.Instance.Get<string>(PersistentDirectoryPathKey, string.Empty);
            if (string.IsNullOrEmpty(result))
            {
                MainThreadDispatcher.SendAndWait(() =>
                {
                    result = GetExternalPersistentDirectoryPath();
                    var canWrite = IsWritable(result);
                    if (string.IsNullOrEmpty(result) || !canWrite)
                    {
                        result = GetInternalPersistentDirectoryPath();
                    }

                    result += "/files";
                    ApplicationCache.Instance.Set<string>(PersistentDirectoryPathKey, result);
                });
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
            var result = ApplicationCache.Instance.Get<string>(InternalPersistentDirectoryPathKey, string.Empty);
            if (string.IsNullOrEmpty(result))
            {
                MainThreadDispatcher.SendAndWait(() =>
                {
                    using (var unityBridge = new AndroidJavaClass("com.klab.jackpot.UnityBridge"))
                    {
                        result = unityBridge.CallStatic<string>("getInternalPersistentDirectoryPath");
                        ApplicationCache.Instance.Set<string>(InternalPersistentDirectoryPathKey, result);
                    }
                });
            }

            return result;
        }

        /// <summary>
        /// 永続化の為の外部ディレクトリパスを取得します
        /// </summary>
        /// <remarks>
        /// このメソッドでは、外部ディレクトリパスのみを返します。
        /// ( + "/files"は付加しません)
        /// GetPersistentDirectoryPath()とは返すパスが違うことに注意してください。
        /// </remarks>
        /// <returns>The internal persistent directory path.</returns>
        public static string GetExternalPersistentDirectoryPath()
        {
            var result = ApplicationCache.Instance.Get<string>(ExternalPersistentDirectoryPathKey, string.Empty);
            if (string.IsNullOrEmpty(result))
            {
                MainThreadDispatcher.SendAndWait(() =>
                {
                    using (var unityBridge = new AndroidJavaClass("com.klab.jackpot.UnityBridge"))
                    {
                        result = unityBridge.CallStatic<string>("getExternalPersistentDirectoryPath");
                        ApplicationCache.Instance.Set<string>(ExternalPersistentDirectoryPathKey, result);
                    }
                });
            }

            return result;
        }

        /// <summary>
        /// キャッシュの為のディレクトリパスを取得します
        /// Androidでは1MBの容量制限があるので、注意して使用してください
        /// </summary>
        /// <remarks>
        /// OS側からファイルを破棄されるので、使用する場合は、ファイルの存在有無を必ず確認して使用するようにしてください。
        /// </remarks>
        /// <returns>The cache directory path.</returns>
        /// <param name="suffix">Suffix.</param>
        public static string GetCacheDirectoryPath(string suffix = "")
        {
            string result = ApplicationCache.Instance.Get<string>(CacheDirectoryPathKey, string.Empty);
            if (string.IsNullOrEmpty(result))
            {
                MainThreadDispatcher.SendAndWait(() =>
                {
                    using (var unityBridge = new AndroidJavaClass("com.klab.jackpot.UnityBridge"))
                    {
                        var dir = unityBridge.CallStatic<string>("getCacheDirectory");
                        result = dir + "/files";
                        ApplicationCache.Instance.Set<string>(CacheDirectoryPathKey, result);
                    }
                });
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
            var result = string.Empty;
            MainThreadDispatcher.SendAndWait(() =>
            {
                using (var unityBridge = new AndroidJavaClass("com.klab.jackpot.UnityBridge"))
                {
                    result = unityBridge.CallStatic<string>("getDefaultLocaleString");
                }
            });
            return result;
        }

        public static string GetLocaleCountryCode()
        {
            var result = string.Empty;
            MainThreadDispatcher.SendAndWait(() =>
            {
                using (var unityBridge = new AndroidJavaClass("com.klab.jackpot.UnityBridge"))
                {
                    result = unityBridge.CallStatic<string>("getDefaultLocaleCountry");
                }
            });
            return result;
        }

        public static string GetLocaleLanguageCode()
        {
            var result = string.Empty;
            MainThreadDispatcher.SendAndWait(() =>
            {
                using (var unityBridge = new AndroidJavaClass("com.klab.jackpot.UnityBridge"))
                {
                    result = unityBridge.CallStatic<string>("getDefaultLocaleLanguage");
                }
            });
            return result;
        }

        /// <summary>
        /// 端末のタイムゾーンを取得します
        /// </summary>
        /// <returns>The device time zone.</returns>
        public static string GetDeviceTimeZone()
        {
            var result = string.Empty;
            MainThreadDispatcher.SendAndWait(() =>
            {
                using (var unityBridge = new AndroidJavaClass("com.klab.jackpot.UnityBridge"))
                {
                    Interlocked.Exchange(ref result, unityBridge.CallStatic<string>("getDeviceTimeZone"));
                }
            });
            return result;
        }

        public static bool IsIos()
        {
            return false;
        }

        public static bool IsAndroid()
        {
            return true;
        }

        public static bool IsEditor()
        {
            return false;
        }

        public static Version GetOSVersion()
        {
            var result = string.Empty;
            MainThreadDispatcher.SendAndWait(() =>
            {
                using (var unityBridge = new AndroidJavaClass("com.klab.jackpot.UnityBridge"))
                {
                    result = unityBridge.CallStatic<string>("getOsVersion");
                }
            });
            return Version.Of(result);
        }

        public static double GetUsedMemory()
        {
            long result = 0;
            MainThreadDispatcher.SendAndWait(() =>
            {
                using (var unityBridge = new AndroidJavaClass("com.klab.jackpot.UnityBridge"))
                {
                    result = unityBridge.CallStatic<long>("getUsedMemory");
                }
            });

            return result / 1024d;
        }

        public static double GetFreeMemory()
        {
            long result = 0;
            MainThreadDispatcher.SendAndWait(() =>
            {
                using (var unityBridge = new AndroidJavaClass("com.klab.jackpot.UnityBridge"))
                {
                    result = unityBridge.CallStatic<long>("getFreeMemory");
                }
            });

            return result / 1024d;
        }

        public static double GetTotalMemory()
        {
            long result = 0;
            MainThreadDispatcher.SendAndWait(() =>
            {
                using (var unityBridge = new AndroidJavaClass("com.klab.jackpot.UnityBridge"))
                {
                    result = unityBridge.CallStatic<long>("getTotalMemory");
                }
            });

            return result / 1024d;
        }

        /// <summary>
        /// リセマラ判定用の端末固有のIDを取得します
        /// ネイティブプラグイン側では広告ID(AdvertigingID)を取得しています。
        /// ただ、素の広告IDを使用する場合Googleのポリシーに抵触してしまう
        /// 場合がある為、広告IDはMD5ハッシュ化した状態で提供しています
        /// </summary>
        public static void LoadResemaraDetectionIdentifier(Action<string> action)
        {
            using (var unityBridge = new AndroidJavaClass("com.klab.jackpot.UnityBridge"))
            {
                var listenr = new AndroidPlatformListener();
                listenr.OnLoadResemaraDetectionIdentifierComplete += action;

                resemaraObject = unityBridge.CallStatic<AndroidJavaObject>(
                    "getResemaraDetectionId",
                    new object[] { listenr }
                );
            }
        }

        public static bool IsRooted()
        {
            var result = false;
            MainThreadDispatcher.SendAndWait(() =>
            {
                result = _KJACore_AssetStateLogAvailable();
            });
            return result;
        }

        public static bool IsDebuggerAttached()
        {
            var result = IsUsbDebuggable();
            return result ? result : SubscribeAssetStateLog();
        }

        public static bool IsUsbDebuggable()
        {
            var result = false;
            MainThreadDispatcher.SendAndWait(() =>
            {
                using (var debug = new AndroidJavaObject("android.os.Debug"))
                {
                    result = debug.CallStatic<bool>("isDebuggerConnected");
                }
            });
            return result;
        }

        public static bool IsEmulator()
        {
            return GetInvalidDeviceProperties().Count > 0;
        }

        /// <summary>
        /// 指定のパスが書き込み可能か
        /// </summary>
        /// <param name="path">path</param>
        /// <returns>The specified path is writable?</returns>
        public static bool IsWritable(string path)
        {
            var result = false;

            MainThreadDispatcher.SendAndWait(() =>
            {
                using (var debug = new AndroidJavaObject("com.klab.jackpot.UnityBridge"))
                {
                    result = debug.CallStatic<bool>("isWritable", path);
                }
            });

            return result;
        }

        public static ICollection<SimpleAnalysisEntry> GetInvalidDeviceProperties()
        {
            var logger = Logger.Get(typeof(AndroidPlatform));
            var results = new List<SimpleAnalysisEntry>();
            MainThreadDispatcher.SendAndWait(() =>
            {
                // emulator detection
                using (var build = new AndroidJavaObject("android.os.Build"))
                {
                    var brand = build.GetStatic<string>("BRAND");
                    if (brand == "generic" || brand == "generic_x86")
                    {
                        logger.Debug("Emulator detected from BRAND: {0}", brand);
                        results.Add(new SimpleAnalysisEntry("BRAND", brand, false));
                    }

                    var device = build.GetStatic<string>("DEVICE");
                    if (device == "generic" || device == "generic_x86" || device == "vbox86p")
                    {
                        logger.Debug("Emulator detected from for DEVICE: {0}", device);
                        results.Add(new SimpleAnalysisEntry("DEVICE", device, false));
                    }

                    var fingerprint = build.GetStatic<string>("FINGERPRINT");
                    if (fingerprint.StartsWith("generic"))
                    {
                        logger.Debug("Emulator detected from FINGERPRINT: {0}", fingerprint);
                        results.Add(new SimpleAnalysisEntry("FINGERPRINT", fingerprint, false));
                    }

                    var hardware = build.GetStatic<string>("HARDWARE");
                    if (hardware == "goldfish" || hardware == "vbox86")
                    {
                        logger.Debug("Emulator detected from HARDWARE: {0}", hardware);
                        results.Add(new SimpleAnalysisEntry("HARDWARE", hardware, false));
                    }

                    var manufacturer = build.GetStatic<string>("MANUFACTURER");
                    if (manufacturer == "unknown" || manufacturer == "Genymotion")
                    {
                        logger.Debug("Emulator detected from MANUFACTURER: {0}", manufacturer);
                        results.Add(new SimpleAnalysisEntry("MANUFACTURER", manufacturer, false));
                    }

                    var model = build.GetStatic<string>("MODEL");
                    if (model == "sdk" || model == "google_sdk" || model == "Emulator" || model == "Android SDK built for x86")
                    {
                        logger.Debug("Emulator detected from MODEL: {0}", model);
                        results.Add(new SimpleAnalysisEntry("MODEL", model, false));
                    }

                    var product = build.GetStatic<string>("PRODUCT");
                    if (product == "sdk" || product == "google_sdk" || product == "sdk_x86" || product == "vbox86p")
                    {
                        logger.Debug("Emulator detected from PRODUCT: {0}", product);
                        results.Add(new SimpleAnalysisEntry("PRODUCT", product, false));
                        return;
                    }
                }
            });
            return results;
        }

        public static bool SubscribeAssetStateLog()
        {
            var result = false;
            MainThreadDispatcher.SendAndWait(() =>
            {
                var output = false;
                var error = false;
                _KJACore_AssetStateLogSubscribe(out output, out error);
                if (!error)
                {
                    result = output;
                }
            });
            return result;
        }

        public static string GenerateAssetStateLog(string seed)
        {
            var buf = new StringBuilder(1024);
            #if ENABLE_IL2CPP
            _KJACore_AssetStateLogGenerateV2(buf, buf.Capacity, seed);
            #else
            _KJACore_AssetStateLogGenerate(buf, buf.Capacity, seed);
            #endif
            return buf.ToString();
        }

        /// <summary>
        /// バッテリー充電状態を取得
        /// </summary>
        /// <returns></returns>
        public static BatteryStatusKind GetBatteryStatus()
        {
            var status = BatteryStatusKind.Unknown;
            status = (BatteryStatusKind) Enum.ToObject(
                typeof(BatteryStatusKind),
                CallStatic<int>("getBatteryStatus")
            );
            return status;
        }

        /// <summary>
        /// バッテリー残量を取得
        /// </summary>
        /// <returns></returns>
        public static int GetBatteryLevel()
        {
            return CallStatic<int>("getBatteryLevel");
        }

        /// <summary>
        /// バッテリー温度の取得
        /// </summary>
        /// <remarks>
        /// 摂氏（℃）の単位で値を返します。
        /// </remarks>>
        /// <returns>The battery temperature.</returns>
        public static float GetBatteryTemperature()
        {
            return CallStatic<float>("getBatteryTemperature");
        }

        /// <summary>
        /// バッテリー電圧の取得
        /// </summary>
        /// <remarks>
        /// ミリボルト（mV）の単位で値を返します。
        /// </remarks>
        /// <returns>The battery voltage.</returns>
        public static int GetBatteryVoltage()
        {
            return CallStatic<int>("getBatteryVoltage");
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
        /// Androidのみ対応。
        /// </remarks>
        /// <param name="message">Toastに表示するメッセージ</param>
        /// <param name="isLong">Toastの表示時間をLongにするフラグ</param>
        public static void ShowToastMessage(string message, bool isLong)
        {
            MainThreadDispatcher.Post(() =>
            {
                CallStatic("showToastMessage", new object[] {message, isLong});
            });
        }

        public static bool LaunchApp(string urlScheme, string packageName)
        {
            bool result = false;
            MainThreadDispatcher.SendAndWait(() =>
            {
                using (var unityBridge = new AndroidJavaClass("com.klab.jackpot.UnityBridge"))
                {
                    result = unityBridge.CallStatic<bool>("LaunchApp", new object[] { urlScheme, packageName });
                }
            });

            return result;
        }

        public static bool StoreReview()
        {
            return false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// NativeBridgeを取得
        /// </summary>
        /// <returns></returns>
        static AndroidJavaClass GetNativeBridge()
        {
            if (bridge == null)
            {
                MainThreadDispatcher.SendAndWait(() =>
                {
                    bridge = new AndroidJavaClass("com.klab.jackpot.UnityBridge");
                });
            }

            return bridge;
        }

        static void CreateDirectoryIfNeeded(string directoryPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            if (!directoryInfo.Exists)
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        static void UsingLog(Action<AndroidJavaClass> callback)
        {
            if (callback == null)
            {
                return;
            }

            MainThreadDispatcher.SendAndWait(() =>
            {
                using (var log = new AndroidJavaClass("android.util.Log"))
                {
                    callback(log);
                }
            });
        }

        static void CallStatic(string method)
        {
            CallStatic(method, null);
        }

        static void CallStatic(string method, object[] args)
        {
            using (new JniAttachSection())
            {
                if (args == null)
                {
                    GetNativeBridge().CallStatic(method);
                }
                else
                {
                    GetNativeBridge().CallStatic(method, args);
                }
            }
        }

        static T CallStatic<T>(string method)
        {
            return CallStatic<T>(method, null);
        }

        static T CallStatic<T>(string method, object[] args)
        {
            var result = default(T);
            using (new JniAttachSection())
            {
                if (args == null)
                {
                    result = GetNativeBridge().CallStatic<T>(method);
                }
                else
                {
                    result = GetNativeBridge().CallStatic<T>(method, args);
                }
            }

            return result;
        }

        #endregion

    }
}
#endif

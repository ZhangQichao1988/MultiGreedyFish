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
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
#if !NET_STANDARD_2_0
using Microsoft.Win32;
#endif
#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif

namespace Jackpot
{
    public class UnityWindowsPlatform
    {
        const string BundleIdentifierKey = "Jackpot.Editor.BundleIdentifier";
        const string BundleVersionKey = "Jackpot.Editor.BundleVersion";
        const string PersistentDirectoryPathKey = "Jackpot.Editor.PersistentDirectoryPath";
        const string InternalPersistentDirectoryPathKey = "Jackpot.Editor.InternalPersistentDirectoryPath";
        const string ExternalPersistentDirectoryPathKey = "Jackpot.Editor.ExternalPersistentDirectoryPath";
        const string CacheDirectoryPathKey = "Jackpot.Editor.CacheDirectoryPath";

        [DllImport("kernel32")]
        private extern static bool GetDiskFreeSpaceEx(
            string lpDirectoryName,
            out ulong freeBytesAvailableToCaller,
            out ulong totalNumberOfBytes,
            out ulong totalNumberOfFreeBytes);

        [DllImport("kernel32")]
        private static extern int GetSystemDefaultLCID();

        public static string GetBundleIdentifier()
        {
#if UNITY_EDITOR
#if UNITY_5_6_OR_NEWER
            return UnityEditor.PlayerSettings.applicationIdentifier;
#else
            return UnityEditor.PlayerSettings.bundleIdentifier;
#endif
#else
            return "com.klab.jackpot"; //dummy
#endif
        }

        public static string GetBundleVersion()
        {
#if UNITY_EDITOR
            return UnityEditor.PlayerSettings.bundleVersion;
#else
            return "1.0.0"; // dummy
#endif
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

            ulong diskFree = 0;
            ulong diskTotalNOB = 0;
            ulong diskTotalNOFB = 0;
            GetDiskFreeSpaceEx(directoryInfo.Root.FullName, out diskFree, out diskTotalNOB, out diskTotalNOFB);
            long result = (long) diskTotalNOFB / 1024;

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
            return new CultureInfo(GetSystemDefaultLCID()).ToString();
        }

        public static string GetLocaleCountryCode()
        {
            return CountryCode.LcidToIso3166P1Alpha2(GetSystemDefaultLCID(), "");
        }

        public static string GetLocaleLanguageCode()
        {
            return new CultureInfo(GetSystemDefaultLCID()).TwoLetterISOLanguageName;
        }

        public static string GetDeviceTimeZone()
        {
            return OlsonMapper.Find(ConvertEnglishTimeZoneName(System.TimeZone.CurrentTimeZone.StandardName));
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
            return Version.Of(System.Environment.OSVersion.Version.ToString());
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

        static void CreateDirectoryIfNeeded(string directoryPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            if (!directoryInfo.Exists)
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        static string ConvertEnglishTimeZoneName(string timeZoneId)
        {
            if (string.IsNullOrEmpty(timeZoneId))
            {
                throw new ArgumentNullException(string.Format("timeZoneId was null. timeZoneId:{0}", timeZoneId));
            }

            using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones"))
            {
                if (key == null)
                {
                    return string.Empty;
                }

                var subKeyNames = key.GetSubKeyNames();
                foreach (var subKeyName in subKeyNames)
                {
                    using (var subKey = key.OpenSubKey(subKeyName))
                    {
                        if (subKey == null)
                        {
                            continue;
                        }
                        if (timeZoneId.Contains((string) subKey.GetValue("Std", "")))
                        {
                            return subKeyName;
                        }
                    }
                }
            }
            return string.Empty;
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

    // レジストリ アクセスの代替クラスです。.NET Standard 2.0 でプラットフォーム依存コードがなくなっているためです。
    // ConvertEnglishTimeZoneName メソッドの実行のために HKLM に対する読み取り専用になっていますので、
    // 必要に応じて定義済みキーの追加、Desired に対するアクセス許可の追加、メソッド追加、RegCreateKeyEx、RegDeleteKey、RegDeleteValue などの追加をしてください。
#if NET_STANDARD_2_0
    /// <summary>
    /// レジストリのルート キーの定義クラス。レジストリへのアクセスは通常このクラスから実行してください。
    /// </summary>
    public static class Registry
    {
        /// <summary>
        /// HKEY_LOCAL_MACHINE の定義済みキー
        /// </summary>
        static readonly UIntPtr hklm = new UIntPtr(0x80000002u);

        /// <summary>
        /// HKEY_LOCAL_MACHINE
        /// </summary>
        public static RegistryKey LocalMachine { get; private set; }

        static Registry()
        {
            LocalMachine = RegistryKey.OpenBaseKey(hklm);
        }
    }

    /// <summary>
    /// レジストリ キーにアクセスするためのクラス。
    /// 通常は <see cref="Registry"/> クラスからのアクセスを想定しています。必要がある場合は必ず <see cref="RegistryKey.OpenBaseKey(UIntPtr)"/> メソッドから呼び出してください。
    /// </summary>
    public class RegistryKey : IDisposable
    {
        [DllImport("advapi32.dll")]
        private static extern int RegOpenKeyEx(UIntPtr hKey, string pSubKey, int ulOptions, int samDesired, out UIntPtr phkResult);

        [DllImport("advapi32.dll")]
        private static extern int RegCloseKey(UIntPtr hKey);

        [DllImport("advapi32.dll")]
        private static extern int RegEnumKeyEx(UIntPtr hKey, int dwIndex, System.Text.StringBuilder pName, ref int pcbName, UIntPtr reserved, UIntPtr pClass, UIntPtr pcbClass, out long pftLastWrite);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegQueryValueExW")]
        private static extern int RegQueryValueEx(UIntPtr hKey, string lpValueName, int pReserved, out uint pType, IntPtr lpData, ref int lpcData);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "FormatMessageW")]
        private static extern uint FormatMessage(uint dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId, IntPtr lpBuffer, int nSize, IntPtr arguments);

        const int Desired = 0x20019 | 0x0100; // KEY_READ, KEY_EXECUTE | KEY_WOW64_64KEY
        const int MaxSubKeyNameSize = 256 + 1; // サブキー名の最大文字数
        private UIntPtr handle;

        // ルートキー名
        static string[] hKeyName = {
            "HKEY_CLASSES_ROOT", // 0x80000000u
            "HKEY_CURRENT_USER", // 0x80000001u
            "HKEY_LOCAL_MACHINE", // 0x80000002u
            "HKEY_USERS", // 0x80000003u
            "", // 0x80000004u, HKEY_PERFORMANCE_DATA
            "HKEY_CURRENT_CONFIG", // 0x80000005u
            "" // 0x80000006u, HKEY_DYN_DATA
        };

        /// <summary>
        /// レジストリ キーのパス。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// サブキーを取得します。
        /// </summary>
        /// <param name="name">取得したいサブキーのパス。</param>
        /// <returns>サブキー</returns>
        public RegistryKey OpenSubKey(string name)
        {
            var registryKey = new RegistryKey();
            var result = RegOpenKeyEx(this.handle,
                name,
                0,
                Desired,
                out registryKey.handle);

            if (result != 0)
            {
                var errorMessage = GetErrorCode(result);
                throw new Exception(errorMessage);
            }

            registryKey.Name = this.Name + "\\" + name;

            return registryKey;
        }

        /// <summary>
        /// すべてのサブキーの名前が格納されている文字列の配列を取得します。
        /// </summary>
        /// <returns>現在のキーのサブキーの名前を格納する文字列の配列。</returns>
        public string[] GetSubKeyNames()
        {
            var keyNames = new List<string>();
            for (var i = 0;; i++)
            {
                var nameSize = MaxSubKeyNameSize;
                var name = new System.Text.StringBuilder(MaxSubKeyNameSize);
                var lastWriteDate = 0L;
                var result = RegEnumKeyEx(this.handle,
                    i,
                    name,
                    ref nameSize,
                    UIntPtr.Zero,
                    UIntPtr.Zero,
                    UIntPtr.Zero,
                    out lastWriteDate);

                if (result != 0)
                {
                    if (result == 259) // ERROR_NO_MORE_ITEMS
                    {
                        break;
                    }

                    // TODO: okada-s Error Code
                    continue;
                }

                keyNames.Add(name.ToString());
            }

            return keyNames.ToArray();
        }

        /// <summary>
        /// 指定した名前に関連付けられている値を取得します。
        /// </summary>
        /// <param name="name">取得する値の名前。</param>
        /// <param name="defaultValue">値が見つからなかったときに返す値。</param>
        /// <returns></returns>
        public object GetValue(string name, string defaultValue = "")
        {
            var dwType = 0u;
            var valueCapacity = 0;

            // valueCapacity が正しい値になってからもう一度取得することで Value が取れる
            var result = RegQueryValueEx(this.handle,
                name,
                0,
                out dwType,
                IntPtr.Zero,
                ref valueCapacity);

            var pResult = Marshal.AllocHGlobal(valueCapacity);

            result = RegQueryValueEx(this.handle,
                name,
                0,
                out dwType,
                pResult,
                ref valueCapacity);

            if (result != 0)
            {
                return defaultValue;
            }
            var value = PtrToValue(dwType, pResult, valueCapacity);
            Marshal.FreeHGlobal(pResult);

            return value;
        }

        /// <summary>
        /// ルートキーを取得します。通常これを直接呼ぶことはありません。
        /// </summary>
        /// <param name="baseKey">定義済みキー。</param>
        /// <returns></returns>
        public static RegistryKey OpenBaseKey(UIntPtr baseKey)
        {
            var registryKey = new RegistryKey();
            var result = RegOpenKeyEx(baseKey, "", 0, Desired, out registryKey.handle);

            if (result != 0)
            {
                var errorMessage = GetErrorCode(result);
                throw new Exception(errorMessage);
            }

            // index の取り方は reference code 参照: https://referencesource.microsoft.com/#mscorlib/microsoft/win32/registrykey.cs,629
            var index = ((int) baseKey) & 0x0FFFFFFF;
            // index の out of range は考慮していない
            registryKey.Name = hKeyName[index];
            return registryKey;
        }

        static string GetErrorCode(int errorCode)
        {
            const int capacity = 1024;

            var pResult = Marshal.AllocHGlobal(capacity);
            var messageResult = FormatMessage(
                0x00001000, // FORMAT_MESSAGE_FROM_SYSTEM
                IntPtr.Zero,
                (uint) errorCode,
                0,
                pResult,
                capacity,
                IntPtr.Zero);

            var message = Marshal.PtrToStringAuto(pResult);

            Marshal.FreeHGlobal(pResult);

            return ($"error({errorCode}) {message}");
        }

        /// <summary>
        /// レジストリの値を型に応じて変換、コピーして返します
        /// REG_MULTI_SZ, REG_DWORD, REG_QWORD以外はbyte配列で返します
        /// </summary>
        /// <param name="type">レジストリ値の型</param>
        /// <param name="value">レジストリ値</param>
        /// <param name="size">値のサイズ</param>
        /// <returns></returns>
        static object PtrToValue(uint type, IntPtr value, int size)
        {
            object ret;
            switch (type)
            {
                case 1: // REG_SZ
                    ret = Marshal.PtrToStringAuto(value);
                    break;
                case 4: // REG_DWORD
                case 5: // REG_DWORD_BIG_ENDIAN
                    ret = Marshal.ReadInt32(value);
                    break;
                case 11: // REG_QWORD
                    ret = Marshal.ReadInt64(value);
                    break;
                default:
                    // OTHER
                    byte[] arr = new byte[size];
                    Marshal.Copy(value, arr, 0, size);
                    ret = arr;
                    break;
            }
            return ret;
        }

        public void Dispose()
        {
            RegCloseKey(handle);
        }

        ~RegistryKey()
        {
            Dispose();
        }
    }
#endif
}
#endif

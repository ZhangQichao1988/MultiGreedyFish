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

using UnityEditor.iOS.Xcode;
using System.Collections.Generic;
using System.Linq;

namespace XcodeTools
{
    public class EntitlementsPlist : PlistFile
    {
        public string ApsEnvironmentKey { get { return "aps-environment"; } }
        public string WifiInformationKey { get { return "com.apple.developer.networking.wifi-info"; } }
        public string UserFontsKey { get { return "com.apple.developer.user-fonts"; } }
        public string HealthKitKey { get { return "com.apple.developer.healthkit"; } }
        public string HomeKitKey { get { return "com.apple.developer.homekit"; } }
        public string InterAppAudioKey { get { return "inter-app-audio"; } }
        public string WirelessConfigurationKey { get { return "com.apple.external-accessory.wireless-configuration"; } }
        public string SignInWithAppleKey { get { return "com.apple.developer.applesignin"; } }

        public EntitlementsPlist() : base()
        {
        }

        public EntitlementsPlist(string path) : base(path)
        {
            if (System.IO.File.Exists(path))
            {
                Plist.ReadFromFile(path);
            }
        }

        /// <summary>
        /// APs Environmentをセットする.
        /// </summary>
        /// <param name="isProduction">isProduction</param>
        public void SetApsEnvironment(bool isProduction)
        {
            SetString(ApsEnvironmentKey, isProduction ? "production" : "development");
        }

        /// <summary>
        /// WiFi Informationをセットする.
        /// </summary>
        /// <param name="isEnabled">isEnabled</param>
        public void SetAccessWifiInformation(bool isEnabled)
        {
            SetBoolean(WifiInformationKey, isEnabled);
        }

        /// <summary>
        /// Sign In With Appleをセットする
        /// </summary>
        public void SetSignInWithApple()
        {
            var value = new[] { "Default" };
            AddString(SignInWithAppleKey, value);
        }

        /// <summary>
        /// Fontsをセットする
        /// </summary>
        /// <param name="isAppUsage">isAppUsage</param>
        /// <param name="isSystemInstallation">isSystemInstallation</param>
        public void SetUserFonts(bool isAppUsage, bool isSystemInstallation)
        {
            var list = new List<string>();

            if (isAppUsage)
            {
                list.Add("app-usage");
            }

            if (isSystemInstallation)
            {
                list.Add("system-installation");
            }
            
            AddString(UserFontsKey, list);
        }

        /// <summary>
        /// Health Kitをセットする
        /// </summary>
        /// <param name="isEnable">isEnable</param>
        public void SetHealthKit(bool isEnable)
        {
            SetBoolean(HealthKitKey, isEnable);
        }

        /// <summary>
        /// Home Kitをセットする
        /// </summary>
        /// <param name="isEnable">isEnable</param>
        public void SetHomeKit(bool isEnable)
        {
            SetBoolean(HomeKitKey, isEnable);
        }

        /// <summary>
        /// Inter App Audioをセットする
        /// </summary>
        /// <param name="isEnable">isEnable</param>
        public void SetInterAppAudio(bool isEnable)
        {
            SetBoolean(InterAppAudioKey, isEnable);
        }

        /// <summary>
        /// Wireless Configurationをセットする
        /// </summary>
        /// <param name="isEnable">isEnable</param>
        public void SetWirelessConfiguration(bool isEnable)
        {
            SetBoolean(WirelessConfigurationKey, isEnable);
        }

        /// <summary>
        /// Entitlementsファイルにあるkeyを削除
        /// 既に存在しない場合は何もしない
        /// </summary>
        /// <param name="key">削除するkey名</param>
        /// <returns>成否</returns>
        public bool Remove(string key)
        {
            if (Plist.root[key] == null)
            {
                return false;
            }

            Plist.root[key] = null;
            return true;
        }

        /// <summary>
        /// EntitlementsファイルへValueとしてStringを追加
        /// 既に追加されていれば何もしない
        /// </summary>
        /// <param name="key">追加するkey名</param>
        /// <param name="values">追加する値</param>
        void AddString(string key, IEnumerable<string> values)
        {
            PlistElementArray plistElement = Plist.root.values.ContainsKey(key) ? 
                Plist.root.values[key].AsArray() : Plist.root.CreateArray(key);

            foreach (var value in values)
            {
                if (plistElement.values.Any(element => element.AsString() == value))
                {
                    continue;
                }
                plistElement.AddString(value);
            }
        }
    }
}

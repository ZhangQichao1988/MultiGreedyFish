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
using UnityEngine;
using System;

#if UNITY_ANDROID
namespace Jackpot
{
    /// <summary>
    /// Android permission.
    /// </summary>
    public partial class AndroidRequestPermission
    {
        /// <summary>
        /// パーミッションが許可された際に設定するeventです。
        /// </summary>
        public static event Action GrantedRequestPermission;

        /// <summary>
        /// パーミッションが許可されなかった際に設定するeventです。
        /// </summary>
        public static event Action DeniedRequestPermission;

        /// <summary>
        /// パーミッション許可の説明ダイアログを表示する必要がある際に設定するeventです。
        /// </summary>
        public static event Action<int, string> NeedExplainRequestPermission;

        static AndroidJavaObject plugin;

        static AndroidRequestPermission()
        {
            plugin = new AndroidJavaClass("com.klab.jackpot.UnityPermissionBridge");
        }

        /// <summary>
        /// プラグインを開放します。
        /// </summary>
        public static void Finish()
        {
            plugin.CallStatic("finish");
        }

        /// <summary>
        /// 権限取得処理を開始します
        /// </summary>
        /// <param name="permission">Permission.</param>
        public static void StartRequestPermission(string permission)
        {
            plugin.CallStatic(
                "startRequestPermission",
                new object[] {
                    permission,
                    new AndroidRequestPermissionListener()
                }
            );
        }

        /// <summary>
        /// 権限要求説明ダイアログ表示後に再度権限取得処理を行います
        /// </summary>
        public static void RequestPermission(string permission)
        {
            plugin.CallStatic("requestPermission", new object[] { permission });
        }

        /// <summary>
        /// 設定画面へ遷移します
        /// </summary>
        public static void OpenSettings()
        {
            plugin.CallStatic("openSettings");
        }
    }
}
#endif

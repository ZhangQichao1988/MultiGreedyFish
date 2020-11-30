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
using UnityEngine;
using System.Collections.Generic;

namespace Jackpot
{
    /// <summary>
    /// Android向けプラグインを作成する際に役立つユーティリティーです
    /// </summary>
    public static class AndroidUtility
    {

        /// <summary>
        /// Usings the UnityPlayer.currentActivity object.
        /// </summary>
        /// <param name="callback">Callback.</param>
        public static void UsingUnityPlayerActivity(Action<AndroidJavaObject> callback)
        {
            if (callback == null)
            {
                return;
            }
            using (new JniAttachSection())
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                callback(currentActivity);
            }
        }

        /// <summary>
        /// Usings the ApplicationContext object.
        /// </summary>
        /// <param name="callback">Callback.</param>
        public static void UsingApplicationContext(Action<AndroidJavaObject> callback)
        {
            if (callback == null)
            {
                return;
            }
            UsingUnityPlayerActivity(activity =>
            {
                using (var applicationContext = activity.Call<AndroidJavaObject>("getApplicationContext"))
                {
                    callback(applicationContext);
                }
            });
        }

        /// <summary>
        /// Usings the PackageInfo object.
        /// </summary>
        /// <param name="callback">Callback.</param>
        public static void UsingPackageInfo(Action<AndroidJavaObject> callback)
        {
            if (callback == null)
            {
                return;
            }
            UsingUnityPlayerActivity(activity =>
            {
                using (var applicationContext = activity.Call<AndroidJavaObject>("getApplicationContext"))
                using (var packageManager = applicationContext.Call<AndroidJavaObject>("getPackageManager"))
                using (var packageInfo = packageManager.Call<AndroidJavaObject>(
                                             "getPackageInfo",
                                             applicationContext.Call<string>("getPackageName"),
                                             0
                                         ))
                {
                    callback(packageInfo);
                }
            });
        }

        /// <summary>
        /// Dictionaries to java.util.HashMap.
        /// </summary>
        /// <returns>The to hash map.</returns>
        /// <param name="dict">Dict.</param>
        public static AndroidJavaObject DictionaryToHashMap(Dictionary<string, string> dict)
        {
            using (new JniAttachSection())
            {
                var results = new AndroidJavaObject("java.util.HashMap");
                if (dict != null && dict.Count > 0)
                {
                    var javaStringSignature = "java.lang.String";
                    var putMethodPtr = AndroidJNIHelper.GetMethodID(
                                           results.GetRawClass(),
                                           "put",
                                           "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;"
                                       );
                    foreach (var pair in dict)
                    {
                        var args = new object[2];
                        var key = new AndroidJavaObject(javaStringSignature, pair.Key);
                        var value = new AndroidJavaObject(javaStringSignature, pair.Value);
                        args[0] = key;
                        args[1] = value;
                        AndroidJNI.CallObjectMethod(
                            results.GetRawObject(),
                            putMethodPtr,
                            AndroidJNIHelper.CreateJNIArgArray(args)
                        );
                    }
                }
                return results;
            }
        }
    }
}
#endif

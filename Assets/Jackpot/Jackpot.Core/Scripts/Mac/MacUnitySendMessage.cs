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
using System.Runtime.InteropServices;
using UnityEngine;

namespace Jackpot
{
    /// <summary>
    /// Macプラグインからのコールバックを受け取るクラスです
    /// </summary>
    /// <remarks>
    /// 使用するためには、MacUnitySendMessage.Initialize()を実施する必要があります
    /// </remarks>
    public class MacUnitySendMessage
    {
        delegate void KJMUnitySendMessage(IntPtr objectName, IntPtr methodName, IntPtr value);
        static bool initialized;

        [DllImport("KJMUnityPlugins")]
        static extern void _KJMCore_SetUnitySendMessageCallback([MarshalAs(UnmanagedType.FunctionPtr)]KJMUnitySendMessage callbackMethod);

        public static void Initialize()
        {
            if (initialized)
            {
                return;
            }
            initialized = true;
            _KJMCore_SetUnitySendMessageCallback((IntPtr objectNameIntPtr, IntPtr methodNameIntPtr, IntPtr valueIntPtr) =>
            {
                var objectName = Marshal.PtrToStringAuto(objectNameIntPtr);
                var methodName = Marshal.PtrToStringAuto(methodNameIntPtr);
                var value = Marshal.PtrToStringAuto(valueIntPtr);
                MainThreadDispatcher.Post(() =>
                {
                    var target = GameObject.Find(objectName);
                    if (target != null)
                    {
                        target.SendMessage(methodName, value);
                    }
                });
            });
        }
    }
}
#endif

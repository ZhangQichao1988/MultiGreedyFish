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
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

namespace Jackpot
{
    /// <summary>
    /// UnityEngine::AndroidJNI利用時に、安全に現在のスレッドをVMにアタッチ/デタッチするためのクラスです。
    /// </summary>
    /// <remarks>
    /// <p>まず例として、以下のコードを実行するとアプリがクラッシュします</p>
    /// <code>
    /// new Thread(() =>
    /// {
    ///     using (var log = new AndroidJavaObject("android.util.Log")) // JNI ERROR (app bug): accessed stale local reference XXX
    ///     {
    ///         log.CallStatic<int>("d", "MyApp", "message");
    ///     }
    /// }).Start();
    /// </code>
    /// <p>AndroidJNIを利用するには、現在のスレッドをVMにアタッチする必要があります。
    /// スレッドをアタッチしたままではVMを安全に終了することができない為、デタッチする必要があります。
    /// いつデタッチしたら良いかは、自前で管理せねばなりません。</p>
    /// <p>AndroidJniAttachSectionは、スレッド毎に自身の生成カウントを管理しています。
    /// オブジェクト生成時に生成カウントをインクリメントし、生成カウントが1になった時にアタッチ処理を行い
    /// オブジェクト開放時に生成カウントをデクリメントし、生成カウントが0になった時に、デタッチ処理を行います。</p>
    /// <code>
    /// new Thread(() =>
    /// {
    ///     using (new JniAttachSection()) // Attach current thread
    ///     using (var log = new AndroidJavaObject("android.util.Log")) // No JNI ERROR
    ///     {
    ///         log.CallStatic<int>("d", "MyApp", "message");
    ///     }
    ///     // detach current thread
    /// }).Start();
    /// </code>
    /// <code>
    /// new Thread(() =>
    /// {
    ///     using (new JniAttachSection()) // Attach current thread
    ///     using (var player = new AndroidJavaObject("com.unity3d.player.UnityPlayer"))
    ///     {
    ///         // Do something
    ///         using (new JniAttachSection()) // No-attach. Aleady attached
    ///         using (var activity = player.GetStatc<AndroidJavaObject>("currentActivity"))
    ///         {
    ///             // Do something
    ///         }
    ///         // No-detach. Still in JniAttachSection's scope
    ///     }
    ///     // detach current thread
    /// }).Start();
    /// </code>
    /// <p>UnityMainスレッドにおいては、そもそもデタッチするとUnityEngineがクラッシュしてしまうので、
    /// このクラス上でアタッチ、デタッチ処理は行われません(スルーされます)。</p>
    /// </remarks>
    public class JniAttachSection : IDisposable
    {
        /// <summary>スレッドの識別番号とセクションカウント</summary>
        static Dictionary<int, int> threadTable = new Dictionary<int, int>();

        /// <summary>排他処理オブジェクト</summary>
        static object lockObject = new object();


        readonly bool mainThread;
        bool disposed;

        public JniAttachSection()
        {
            mainThread = MainThreadDispatcher.IsMainThread;
            AcquireSection();
        }

        ~JniAttachSection()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            ReleaseSection();
            disposed = true;
        }

        void AcquireSection()
        {
            if (mainThread)
            {
                return;
            }

            int threadId = Thread.CurrentThread.ManagedThreadId;

            lock (lockObject)
            {
                // キーが存在しなければ追加
                if (!threadTable.ContainsKey(threadId))
                {
                    threadTable.Add(threadId, 1);
                    AndroidJNI.AttachCurrentThread();
                }
                else
                {
                    ++threadTable[threadId];
                }
            }
        }

        void ReleaseSection()
        {
            if (mainThread)
            {
                return;
            }

            int threadId = Thread.CurrentThread.ManagedThreadId;

            lock (lockObject)
            {
                if (threadTable.ContainsKey(threadId))
                {
                    if (--threadTable[threadId] == 0)
                    {
                        AndroidJNI.DetachCurrentThread();
                        threadTable.Remove(threadId);
                    }
                }
                else
                {
                    throw new InvalidOperationException("Section to release does not exist.");
                }
            }
        }
    }
}
#endif
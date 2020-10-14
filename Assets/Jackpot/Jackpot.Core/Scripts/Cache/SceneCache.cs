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
using UnityEngine;

namespace Jackpot
{
    /// <summary>
    /// SceneCacheのSingletonインスタンスが生成されたSceneが破棄されるまで生存するMemory Objectです
    /// </summary>
    /// <remarks>
    /// 特定のシーンのみでデータを保持、破棄いたい場合に使用します
    /// UnityEditor上でHierarchyに直接配置しないようにしてください
    /// </remarks>
    public class SceneCache : MonoBehaviour, IMemory
    {

#region Properties

        /// <summary>
        /// SceneCacheのSingletonインスタンスを示します
        /// </summary>
        /// <value>The instance.</value>
        public static IMemory Instance
        {
            get
            {
                lock (lifecycleLock)
                {
                    if (isAlive)
                    {
                        return instance;
                    }
                    MainThreadDispatcher.SendAndWait(() =>
                    {
                        if (isAlive)
                        {
                            return;
                        }
                        instance = new GameObject("SceneCache").AddComponent<SceneCache>();
                        instance.memory = new ConcurrentMemory();
                        isAlive = true;
                    });
                    return instance;
                }
            }
        }

        public static bool IsAlive { get { return isAlive; } }

#endregion

#region Fields

        static readonly object lifecycleLock = new object();
        static SceneCache instance;
        static bool isAlive;

        IMemory memory;

#endregion

#region Unity Methods

        void OnDestroy()
        {
            lock (lifecycleLock)
            {
                instance = null;
                isAlive = false;
            }
        }

#endregion

#region Public Methods

        public TValue Get<TValue>(string key)
        {
            return memory.Get<TValue>(key);
        }

        public TValue Get<TValue>(string key, TValue defaultValue)
        {
            return memory.Get<TValue>(key, defaultValue);
        }

        public bool TryGet<TValue>(string key, out TValue value)
        {
            return memory.TryGet<TValue>(key, out value);
        }

        public bool Contains(string key)
        {
            return memory.Contains(key);
        }

        public bool ContainsKey(string key)
        {
            return memory.ContainsKey(key);
        }

        public void Set<TValue>(string key, TValue value)
        {
            memory.Set<TValue>(key, value);
        }

        public void Remove(string key)
        {
            memory.Remove(key);
        }

        public void RemoveAll()
        {
            memory.RemoveAll();
        }

#endregion
    }
}


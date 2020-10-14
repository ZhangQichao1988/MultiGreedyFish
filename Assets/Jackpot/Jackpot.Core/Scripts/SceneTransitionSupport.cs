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

#if !UNITY_4_6 && !UNITY_4_7 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2
using UnityEngine.SceneManagement;
#endif

namespace Jackpot
{
    /// <summary>
    /// 実装Objectはシーン切り替え、及びシーン間のパラメータの受け渡し機能を提供します
    /// </summary>
    /// <remarks>
    /// 以下にサンプルコードを示します
    /// <code>
    /// using UnityEngine;
    /// using Jackpot;
    /// 
    /// public class AScene : MonoBehaviour, ISceneTransitionSupport
    /// {
    ///     void Update()
    ///     {
    ///         if (Input.GetButtonUp("Fire1")
    ///         {
    ///             // BSceneに遷移しながら、nameパラメータを渡す
    ///             var params = new Memory();
    ///             params.Set<string>("name", "Unity");
    ///             this.ChangeScene("BScene", params);
    ///         }
    ///     }
    /// }
    /// 
    /// // BSceneのentry-point
    /// public class BScene : MonoBehaviour, ISceneTransitionSupport
    /// {
    ///     void Awake()
    ///     {
    ///         var params = this.GetParameter();
    ///         Debug.Log(this.GetPrevSceneName()); // => AScene
    ///         var name = params.Get<string>("name");
    ///         Debug.Log("Hello, " + name + "!"); // => Hello, Unity!
    ///     }
    ///     void Update()
    ///     {
    ///         if (Input.GetButtonUp("Fire1")
    ///         {
    ///             // CSceneへの遷移に必要なパラメータ(後述)を作成
    ///             var params = new CSceneParameter("fooooo!");
    ///             this.ChangeScene<CSceneParameter>("CScene", params);
    ///         }
    ///     }
    /// }
    /// 
    /// // CSceneの遷移に必要なパラメータ
    /// public class CSceneParameter : Memory
    /// {
    ///     public Message { get; private set; }
    /// 
    ///     public CSceneParam(string message)
    ///     {
    ///         Message = message;
    ///     }
    /// }
    /// 
    /// // CSceneのentry-point
    /// public class CScene : MonoBehaviour, ISceneTransitionSupport
    /// {
    ///     void Awake()
    ///     {
    ///         // 型安全にCScene用のパラメータを取得
    ///         var params = this.GetParameter<CSceneParameter>();
    ///         Debug.Log(params.Message); // => foooooo!
    ///     }
    /// }
    /// </code>
    /// </remarks>
    public interface ISceneTransitionSupport
    {
    }

    /// <summary>
    /// Jackpot.ISceneTransitionSupportを実装したObjectが持つメソッド群です
    /// <see cref="Jackpot.ISceneTransitionSupport" />
    /// </summary>
    public static class SceneTransitionSupport
    {
#region Constants

        const string SceneParameterPrefix = "SceneTransitionSupport.Scene.";
        const string PrevSceneKey = "SceneTransitionSupport.PrevScene";
        const string PrevSceneNameKey = "SceneTransitionSupport.PrevSceneName";

#endregion

#region Non-Generic

        /// <summary>
        /// シーンを同期的に切り替えます。パラメータの受け渡しができます
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="sceneName">Scene name.</param>
        /// <param name="parameter">Parameter.</param>
        public static void ChangeScene(this ISceneTransitionSupport self, string sceneName, IMemory parameter = null)
        {
            ChangeSceneCore(sceneName, parameter);
        }

        /// <summary>
        /// シーンを非同期に切り替えます。パラメータの受け渡しができます
        /// </summary>
        /// <returns>The scene async.</returns>
        /// <param name="self">Self.</param>
        /// <param name="sceneName">Scene name.</param>
        /// <param name="parameter">Parameter.</param>
        public static AsyncOperation ChangeSceneAsync(
            this ISceneTransitionSupport self,
            string sceneName,
            IMemory parameter = null)
        {
            return ChangeSceneAsyncCore(sceneName, parameter);
        }

        /// <summary>
        /// シーンに渡されたパラメータを返却します。sceneNameを指定した場合、最後にそのシーンに渡されたパラメータを返却します
        /// </summary>
        /// <returns>The scene parameter.</returns>
        /// <param name="self">Self.</param>
        /// <param name="sceneName">Scene name.</param>
        public static IMemory GetSceneParameter(this ISceneTransitionSupport self, string sceneName = null)
        {
            return GetSceneParameterCore(sceneName);
        }

        /// <summary>
        /// シーンに渡すパラメータを更新します。シーンを二つ以上跨いでパラメータを渡したい場合に使用します
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="sceneName">Scene name.</param>
        /// <param name="parameter">Parameter.</param>
        public static void StoreSceneParameter(this ISceneTransitionSupport self, string sceneName, IMemory parameter)
        {
            StoreSceneParameterCore(sceneName, parameter);
        }

        /// <summary>
        /// 現在のシーンに渡されたパラメータを削除します。sceneNameを指定した場合、そのシーンのパラメータを削除します
        /// </summary>
        /// <param name="">.</param>
        public static void DeleteSceneParameter(this ISceneTransitionSupport self, string sceneName = null)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                
#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
                sceneName = Application.loadedLevelName;
#else
                sceneName = SceneManager.GetActiveScene().name;
#endif
            }
            ApplicationCache.Instance.Remove(SceneParameterKey(sceneName));
        }

        /// <summary>
        /// 現在のシーンの前のシーン名を返却します。初回起動時は起動シーン名が返却されます
        /// </summary>
        /// <returns>The previous scene name.</returns>
        /// <param name="self">Self.</param>
        public static string GetPrevSceneName(this ISceneTransitionSupport self)
        {
            var parameter = GetSceneParameterCore();

#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
            return parameter.Get<string>(PrevSceneNameKey, Application.loadedLevelName);
#else
            return parameter.Get<string>(PrevSceneNameKey, SceneManager.GetActiveScene().name);
#endif
        }

        /// <summary>
        /// 現在のシーンの前のシーン番号を返却します。初回起動時は起動シーンの番号が返却されます
        /// </summary>
        /// <returns>The previous scene.</returns>
        /// <param name="self">Self.</param>
        public static int GetPrevScene(this ISceneTransitionSupport self)
        {
            var parameter = GetSceneParameterCore();

#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
            return parameter.Get<int>(PrevSceneKey, Application.loadedLevel);    
#else
            return parameter.Get<int>(PrevSceneKey, SceneManager.GetActiveScene().buildIndex);
#endif
        }

#endregion

#region Generic

        /// <summary>
        /// シーンを同期的に切り替えます。その際パラメータを型安全に渡します
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="sceneName">Scene name.</param>
        /// <param name="parameter">Parameter.</param>
        /// <typeparam name="TParam">The 1st type parameter.</typeparam>
        public static void ChangeScene<TParam>(this ISceneTransitionSupport self, string sceneName, TParam parameter) where TParam : IMemory
        {
            ChangeSceneCore(sceneName, parameter);
        }

        /// <summary>
        /// シーンを非同期に切り替えます。その際パラメータを型安全に渡します
        /// </summary>
        /// <returns>The scene async.</returns>
        /// <param name="self">Self.</param>
        /// <param name="sceneName">Scene name.</param>
        /// <param name="parameter">Parameter.</param>
        /// <typeparam name="TParam">The 1st type parameter.</typeparam>
        public static AsyncOperation ChangeSceneAsync<TParam>(
            this ISceneTransitionSupport self,
            string sceneName,
            TParam parameter) where TParam : IMemory
        {
            return ChangeSceneAsyncCore(sceneName, parameter);
        }

        /// <summary>
        /// シーンに渡されたパラメータを型引数にキャストしつつ取得します
        /// </summary>
        /// <returns>The scene async.</returns>
        /// <param name="self">Self.</param>
        /// <param name="sceneName">Scene name.</param>
        /// <param name="parameter">Parameter.</param>
        /// <typeparam name="TParam">The 1st type parameter.</typeparam>
        public static TParam GetSceneParameter<TParam>(this ISceneTransitionSupport self, string sceneName = null) where TParam : IMemory
        {
            return (TParam) GetSceneParameterCore(sceneName);
        }

        /// <summary>
        /// シーンに渡すパラメータを型安全に更新します
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="sceneName">Scene name.</param>
        /// <param name="parameter">Parameter.</param>
        /// <typeparam name="TParam">The 1st type parameter.</typeparam>
        public static void StoreSceneParameter<TParam>(
            this ISceneTransitionSupport self,
            string sceneName,
            TParam parameter) where TParam : IMemory
        {
            StoreSceneParameterCore(sceneName, parameter);
        }

#endregion

#region Private Methods

        static void ChangeSceneCore(string sceneName, IMemory parameter = null)
        {
            StoreSceneParameterCore(sceneName, parameter);

#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
            Application.LoadLevel(sceneName);
#else
            SceneManager.LoadScene(sceneName);
#endif
        }

        static AsyncOperation ChangeSceneAsyncCore(string sceneName, IMemory parameter = null)
        {
            StoreSceneParameterCore(sceneName, parameter);

#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
            return Application.LoadLevelAsync(sceneName);
#else
            return SceneManager.LoadSceneAsync(sceneName);
#endif
        }

        static IMemory GetSceneParameterCore(string sceneName = null)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                
#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
                sceneName = Application.loadedLevelName;
#else
                sceneName = SceneManager.GetActiveScene().name;
#endif
            }
            var result = ApplicationCache.Instance.Get<IMemory>(SceneParameterKey(sceneName));
            if (result == null)
            {
                result = new Memory();
            }
            return result;
        }

        static void StoreSceneParameterCore(string sceneName, IMemory parameter)
        {
            if (parameter == null)
            {
                parameter = GetSceneParameterCore(sceneName);
            }


#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
            parameter.Set<int>(PrevSceneKey, Application.loadedLevel);
            parameter.Set<string>(PrevSceneNameKey, Application.loadedLevelName);
#else
            parameter.Set<int>(PrevSceneKey, SceneManager.GetActiveScene().buildIndex);
            parameter.Set<string>(PrevSceneNameKey, SceneManager.GetActiveScene().name);
#endif
            ApplicationCache.Instance.Set<IMemory>(SceneParameterKey(sceneName), parameter);
        }

        static string SceneParameterKey(string sceneName)
        {
            ValidateSceneName(sceneName);
            return SceneParameterPrefix + sceneName;
        }

        static void ValidateSceneName(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName) || sceneName.Trim().Length == 0)
            {
                throw new ArgumentException("scene name is not valid");
            }
        }

#endregion
    }
}


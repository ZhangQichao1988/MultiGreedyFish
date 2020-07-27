using UnityEngine;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// 游戏管理器,维护Scene生命周期
/// </summary>
public class BlSceneManager : MonoBehaviour
{
    class PreloadSceneData
    {
        public Type cls;
        public System.Object parms;
    }

    static BaseScene currentScene;

    private static BlSceneManager Instance { get; set; }

    static PreloadSceneData preloadInfo;

    public static void LoadSceneByClass(string name, Type sceneClass, System.Object data = null)
    {
        if (currentScene != null)
        {
            currentScene.Destory();
            currentScene =  null;
        }
        preloadInfo = new PreloadSceneData(){
            cls = sceneClass,
            parms = data
        };
        LoadSceneAsync(name, LoadSceneMode.Single);
    }

    public static GameObject GetCachedPrefab(string path)
    {
        if (currentScene != null)
        {
            return currentScene.cachedObject[path] as GameObject;
        }
        return null;
    }

    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="sceneName">场景名</param>
    /// <param name="mode">0:Single,1:Additive</param>
    public static void LoadSceneAsync(string sceneName, LoadSceneMode mode)
    {
        LoadingMgr.Show(LoadingMgr.LoadingType.Progress);
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, mode);
    }

    /// <summary>
    /// 异步卸载场景
    /// </summary>
    /// <param name="sceneName"></param>
    public static void UnloadSceneAsync(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
    }

    public static BaseScene GetCurrentScene()
    {
        return currentScene;
    }

    /// <summary>
    /// 激活场景
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    public static bool SetActiveScene(string sceneName)
    {
        Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
        return UnityEngine.SceneManagement.SceneManager.SetActiveScene(scene);
    }

    public void Awake()
	{
		if(Instance == null)
		{
			Instance = this;
		}
    }

    private void Start()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
		SceneManager.sceneUnloaded += OnSceneUnloaded;
	}

    void Update()
    {
        if (currentScene != null)
        {
            currentScene.Update();
        }
    }

    private void OnDestroy()
	{
		SceneManager.sceneUnloaded -= OnSceneUnloaded;
		SceneManager.sceneLoaded -= OnSceneLoaded;

		if (Instance != null)
		{
			Instance = null;
		}
	}

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
        // on scene loaded call
        int block = ResourceManager.Begin(OnSceneAssetLoaded, OnSceneAssetProgress);
        if (currentScene != null)
        {
            currentScene.Cache(block);
        }
        ResourceManager.End();
	}
    
    void OnSceneAssetLoaded(int block)
    {
        if (currentScene != null)
        {
            currentScene.Create();
        }
        LoadingMgr.Hide(LoadingMgr.LoadingType.Progress);
    }
    
    void OnSceneAssetProgress(int block, float progress)
    {
        LoadingMgr.SetProgress(progress);
    }

	void OnSceneUnloaded(Scene scene)
	{

        GC.Collect();

        Resources.UnloadUnusedAssets();

        ResourceManager.UnloadAll();

		// on scene unloaded call
        if (preloadInfo != null)
        {
            currentScene = Activator.CreateInstance(preloadInfo.cls) as BaseScene;
            currentScene.Init(preloadInfo.parms);
        }
        preloadInfo = null;
	}
}


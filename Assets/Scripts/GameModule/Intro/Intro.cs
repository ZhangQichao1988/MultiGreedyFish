using System.Collections;
using UnityEngine;
using NetWorkModule;


public class Intro : MonoBehaviour
{
    public static Intro Instance
    {
        get;
        set;
    }
    
    private ClickEffect ClickEffect;

    static BlUIManager uiManager;

    public static BlUIManager UIManager
    {
        get
        {
            return uiManager;
        }
    }

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        gameObject.AddComponent<NetWorkManager>();
        gameObject.AddComponent<ResourceManager>();
        uiManager = gameObject.AddComponent<BlUIManager>();
        gameObject.AddComponent<BlSceneManager>();
        gameObject.AddComponent<EffectManager>();
        
        ClickEffect = gameObject.AddComponent<ClickEffect>();
    }

    private void OnDestroy()
    {
        if (Instance != null)
        {
            Instance = null;
        }
    }

    IEnumerator Start()
    {
        ClickEffect.Initialize();
        NetWorkHandler.InitHttpNetWork();
#if CONSOLE_ENABLE 
		DebugMenu.Instance.StartDebug();
#endif

        yield return new WaitForEndOfFrame();
        UIManager.Init();
        UIBase.Open("ArtResources/UI/Prefabs/Title");
    }
    

    public string GetAppVer()
    {
        return Application.version;
    }

    /// <summary>
    /// 重新启动游戏
    /// </summary>
    public void Restart()
    {
        Destroy(gameObject);
        System.GC.Collect();
        NetWorkHandler.Dispose();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Intro");
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void Quit()
    {
#if !UNITY_EDITOR
        Application.Quit();
#else
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
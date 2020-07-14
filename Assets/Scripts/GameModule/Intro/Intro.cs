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
        gameObject.AddComponent<BlUIManager>();
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
        yield return new WaitForEndOfFrame();
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
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
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
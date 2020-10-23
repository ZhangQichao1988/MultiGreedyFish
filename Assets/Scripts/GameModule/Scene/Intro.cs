using System.Collections;
using UnityEngine;
using NetWorkModule;
using TimerModule;
using System;


public class Intro : MonoBehaviour
{
    public static Intro Instance
    {
        get;
        set;
    }
    
    private ClickEffect clickEffect;

    [NonSerialized]
    public AdsController AdsController;

    [NonSerialized]
    public FireBaseController FireBaseCtrl;

    static BlUIManager uiManager;

    public static BlUIManager UIManager
    {
        get
        {
            return uiManager;
        }
    }

    private void SetLandscape()
    {
        Screen.orientation = ScreenOrientation.Landscape;//如果屏幕是竖屏,则立刻旋转至横屏

        //设置只允许横屏旋转
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToLandscapeLeft = true;

        Screen.orientation = ScreenOrientation.AutoRotation;//再设置为允许自动旋转
    }

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        SetLandscape();
        DontDestroyOnLoad(gameObject);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        gameObject.AddComponent<NetWorkManager>();
        gameObject.AddComponent<ResourceManager>();
        uiManager = gameObject.AddComponent<BlUIManager>();
        gameObject.AddComponent<BlSceneManager>();
        gameObject.AddComponent<EffectManager>();
        gameObject.AddComponent<TimerManager>();
        
        AdsController = gameObject.AddComponent<AdsController>();

        FireBaseCtrl = gameObject.AddComponent<FireBaseController>();
        
        clickEffect = gameObject.AddComponent<ClickEffect>();

    }

    private void OnDestroy()
    {
        if (Instance != null)
        {
            Instance = null;
        }
    }

    static int a = 0;

    IEnumerator Start()
    {
        BillingManager.Initialize();
        clickEffect.Initialize();
        NetWorkHandler.InitHttpNetWork();
        GameServiceController.Init();
#if CONSOLE_ENABLE 
		DebugMenu.Instance.StartDebug();
#endif

        yield return new WaitForEndOfFrame();
        UIManager.Init();
        //UIBase.Open("ArtResources/UI/Prefabs/Title");
#if DUMMY_DATA
        StartLogin();
#else
        LoadingMgr.Show(LoadingMgr.LoadingType.Progress, "检查是否有更新...");
        UpdateFlowController.StartUpdateFlow(StartLogin);
#endif
    }
    
    void StartLogin()
    {
        UserLoginFlowController.StartLoginFlow(()=>{
                BlSceneManager.LoadSceneByClass(SceneId.HOME_SCENE, typeof(HomeScene));
            });
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
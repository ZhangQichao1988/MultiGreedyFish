using System.Collections;
using UnityEngine;
using NetWorkModule;
using TimerModule;
using System;
using IFix.Core;
using System.IO;
using UnityEngine.Audio;
using Firebase.Analytics;

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
        Application.targetFrameRate = 60;

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

        // 设置读取
        AppConst.languageMode = (LanguageDataTableProxy.LanguageMode)PlayerPrefs.GetInt(AppConst.PlayerPrefabsOptionLangauge, (int)AppConst.languageMode);
        AppConst.BgmValue = PlayerPrefs.GetFloat(AppConst.PlayerPrefabsOptionBgmValue, AppConst.BgmValue);
        AppConst.SeValue = PlayerPrefs.GetFloat(AppConst.PlayerPrefabsOptionSeValue, AppConst.SeValue);
        AppConst.IsEco = PlayerPrefs.GetInt(AppConst.PlayerPrefabsOptionIsEco, AppConst.IsEco);
        AppConst.NotShowAdvert = PlayerPrefs.GetInt(AppConst.PlayerPrefabsOptionIsShowAdvert, AppConst.NotShowAdvert);

        // 读取教学阶段
        TutorialControl.currStep = (TutorialControl.Step)PlayerPrefs.GetInt(AppConst.PlayerPrefabsTutorialStep, (int)TutorialControl.Step.GotoTutorialBattle);
        //TutorialControl.currStep = TutorialControl.Step.Completed;

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

    GameObject repairBtn;

    IEnumerator Start()
    {
        BillingManager.Initialize();
        clickEffect.Initialize();
        NetWorkHandler.InitHttpNetWork(new CommonNetErrorHandler());
        GameServiceController.Init();
#if CONSOLE_ENABLE 
		DebugMenu.Instance.StartDebug();
#endif

        yield return new WaitForEndOfFrame();
        UIManager.Init();
        

        if (AppConst.ServerType == ESeverType.OFFLINE)
        {
            StartLogin();
        }
        else
        {
            try
            {
                LoadingMgr.Show(LoadingMgr.LoadingType.Progress, LanguageDataTableProxy.GetText(100, "update...")) ;
                UpdateFlowController.StartUpdateFlow(StartLogin);
            }
            catch (Exception ex)
            {
                repairBtn = UIBase.Open("ArtResources/UI/Prefabs/Msg/RepairBtn", UIBase.UILayers.POPUP);
            }
        }
    }
    
    void StartLogin()
    {
        //更新完毕 补丁嵌入逻辑
        var patchPath = Path.Combine(Application.persistentDataPath + "/patch", GetPlatformName(), "Assembly-CSharp.patch.bytes");
        if (File.Exists(patchPath))
        {
            FileStream stream = new FileStream(patchPath, FileMode.Open, FileAccess.Read);
            PatchManager.Load(stream);
        }

        UserLoginFlowController.StartLoginFlow(()=>{
                if (repairBtn != null)
                {
                    Destroy(repairBtn);
                    repairBtn = null;
                }
            if (TutorialControl.IsStep(TutorialControl.Step.GotoTutorialBattle))
            {
                PlayerModel.Instance.BattleStart();
                P4_Response realResponse = new P4_Response();
                realResponse.BattleId = "tutorial_1";
                StageModel.Instance.SetStartBattleRes(realResponse);
                BlSceneManager.LoadSceneByClass(SceneId.BATTLE_SCENE, typeof(BattleScene));
                FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventTutorialBegin);
            }
            else
            {
                BlSceneManager.LoadSceneByClass(SceneId.HOME_SCENE, typeof(HomeScene));
            }
        });
    }

    string GetPlatformName()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                return "android";
            case RuntimePlatform.IPhonePlayer:
                return "ios";
            default:
                return "editor";
        }
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
        PlayerModel.Instance.Dispose();
        BaseDataTableProxyMgr.Destory();
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
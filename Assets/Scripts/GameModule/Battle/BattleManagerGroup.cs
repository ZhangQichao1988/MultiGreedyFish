using Firebase.Analytics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManagerGroup : MonoBehaviour
{

    enum GameStep 
    {
        None,
        Title,
        Battle,
        Result,
    }
    static BattleManagerGroup instance = null;

    public InGameUIPanel inGameUIPanel = null;
    public FishManager fishManager = null;
    public AquaticManager aquaticManager = null;
    public ShellManager shellManager = null;
    public PoisonRing poisonRing = null;
    public CameraFollow cameraFollow = null;
    public BattleTime battleTime = null;

    public Text resultText = null;

    Animator animator = null;
    public bool isPause = false;
    private int battleRanking = 0;
    private bool isResult = false;

    private int playerKilledCnt = 0;

    // 教学相关
    public enum TutorialStep
    { 
        None,
        Start,
        BabyFishCreateWaiting,
        BadyFishTips,
        BadyFishMissionChecking,
        BadyFishMissionCompleted,

        JellyFishMissionChecking,
        JellyFishMissionCompleted,

        ShellMissionChecking,
        ShellMissionCompleted,

        TortoiseMissionChecking,
        TortoiseMissionCompleted,

        SkillMissionChecking,
        SkillMissionCompleted,

        KillSharkMissionChecking,
        KillSharkMissionCompleted,

        AquaticMissionChecking,
        AquaticMissionCompleted,

        CompletedAll,

        
    };
    public Animator animTutorialMission;
    public Text textMission1;
    public Image imgMissionComplete1;
    public GameObject goFingerMove;
    public GameObject goFingerSkill;
    private TutorialStep tutorialStep = TutorialStep.None;
    private float tutorialTime = 0f;
    private int tutorialCntTarget = 0;
    private int tutorialCntCurrent = 0;
    private bool playerIsShield = false;
    private void Awake()
	{
        instance = this;
        animator = GetComponent<Animator>();
        BattleScene bs = BlSceneManager.GetCurrentScene() as BattleScene;
        if (TutorialControl.IsStep(TutorialControl.Step.GotoTutorialBattle))
        {
            tutorialStep = TutorialStep.Start;
        }
        else
        {
            GotoBattle();
            goFingerMove.SetActive(false);
            goFingerSkill.SetActive(false);
        }
    }
    static public BattleManagerGroup GetInstance()
    { return instance; }


    public void AddPlayerKilledCnt()
    {
        ++playerKilledCnt;
    }
    public void GotoBattle()
    {
        //resultRoot.SetActive(false);
        //titleBtn.SetActive(false);
        //battleControl.SetActive(true) ;
        animator.SetTrigger("ReadyStart");
        poisonRing.gameObject.SetActive(true);
        poisonRing.Init();
        cameraFollow.Init();

        fishManager.Clean();
        inGameUIPanel.Init();
        fishManager.CreateOtherFish();
        this.isPause = true;
    }

    public void GotoHome()
    {
        NetWorkHandler.GetDispatch().AddListener<P5_Response>(GameEvent.RECIEVE_P5_RESPONSE, OnRecvBattleResult);
        NetWorkHandler.RequestBattleResult(StageModel.Instance.battleId, battleRanking);
        StageModel.Instance.battleRanking = battleRanking;

        var levelInfo = PlayerModel.Instance.GetCurrentPlayerFishLevelInfo();
        FirebaseAnalytics.LogEvent("battle_end",
                                                        new Parameter(FirebaseAnalytics.ParameterCharacter, levelInfo.FishId.ToString()),
                                                        new Parameter(FirebaseAnalytics.ParameterScore, levelInfo.RankLevel.ToString()),
                                                        new Parameter(FirebaseAnalytics.ParameterLevel, levelInfo.FishLevel.ToString()),
                                                        new Parameter(FirebaseAnalytics.ParameterIndex, StageModel.Instance.battleRanking.ToString()),
                                                        new Parameter(FirebaseAnalytics.ParameterValue, (long)playerKilledCnt)
                                                        );
    }

    void OnRecvBattleResult<T>(T response)
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P5_RESPONSE);
        var realResponse = response as P5_Response;
        if (realResponse.Result.Code == NetworkConst.CODE_OK)
        {
            SetBattlePause();
            BlSceneManager.LoadSceneByClass(SceneId.HOME_SCENE, typeof(HomeScene), "BattleResult");
            StageModel.Instance.resultResponse = realResponse;
            //BattleResult battleResult = UIBase.Open<BattleResult>("ArtResources/UI/Prefabs/BattleResult");

            //battleResult.Setup(realResponse);
            //var playerFishLevelInfo = PlayerModel.Instance.GetCurrentPlayerFishLevelInfo();
            //playerFishLevelInfo.RankLevel += realResponse.GainRankLevel;
        }
        else
        {
            Debug.Log("战斗报酬通信错误！");
        }

    }
    public void SetPlayPoint()
    {
        animator.SetTrigger("PlayPoint");
    }
    public void GotoResult(int rank)
    {
        if (isResult) { return; }
        isResult = true;
        animator.SetTrigger("Win");
        if (rank == 1)
        {
            animator.SetTrigger("BattleEnd");
        }
        battleRanking = rank;
        resultText.text = string.Format( LanguageDataTableProxy.GetText(1), rank.ToString() );
    }

    public void BattleEnd()
    {
        animator.SetTrigger("BattleEnd");
        //Time.timeScale = 0f;
    }

	public void SetBattleStart()
    {
        this.isPause = false;
    }

	public void SetBattlePause()
	{
		this.isPause = true;
	}

	private void Update()
	{
        TutorialUpdate();
        if (isPause) { return; }
        fishManager.CustomUpdate();
        poisonRing.CustomUpdate();
    }

    private void OnApplicationPause(bool focus)
    {
        //进入程序状态更改为前台
        if (focus)
        {
            isPause = true;
            UIPopupBattleToHome.Open();
        }
        //else
        //{
        //    //离开程序进入到后台状态
        //}
    }
    private void TutorialUpdate()
    {
        if (tutorialStep == TutorialStep.None) { return; }
        tutorialTime -= Time.deltaTime;
        switch (tutorialStep)
        {
            case TutorialStep.Start:
                cameraFollow.Init();
                fishManager.Clean();
                inGameUIPanel.Init();
                inGameUIPanel.SetSkillBtnDisplay(false);
                goFingerMove.SetActive(true);
                shellManager.ShowTargetIcon(false);
                aquaticManager.ShowTargetIcon(false);
                this.isPause = true;
                tutorialTime = 0.5f;
                tutorialStep = TutorialStep.BabyFishCreateWaiting;
                break;
            case TutorialStep.BabyFishCreateWaiting: // 稍等一下再布置宝宝鱼
                if (tutorialTime < 0f)
                {
                    PBEnemyDataInfo[] pBEnemyDataInfos = new PBEnemyDataInfo[] { new PBEnemyDataInfo() { FishId = 0, FishLevel = 1, FishCountMax = 50, FishCountMin = 50 } };
                    fishManager.CreateEnemy(pBEnemyDataInfos);
                    fishManager.SetShine( FishBase.FishType.Enemy, true);
                    this.isPause = false;
                    tutorialTime = 1f;
                    tutorialStep = TutorialStep.BadyFishTips;
                }
                break;
            case TutorialStep.BadyFishTips: // 布置完宝宝鱼弹出提示窗口
                if (tutorialTime < 0f) 
                {
                    this.isPause = true;
                    animTutorialMission.SetBool("Show", true);
                    tutorialStep = TutorialStep.BadyFishMissionChecking;
                    tutorialCntTarget = 3;
                    tutorialCntCurrent = 0;
                    textMission1.text = string.Format(LanguageDataTableProxy.GetText(900), tutorialCntCurrent, tutorialCntTarget);
                    imgMissionComplete1.gameObject.SetActive(false);

                    OpenTips();
                }
                break;
            case TutorialStep.BadyFishMissionChecking:
                goFingerMove.SetActive(!inGameUIPanel.IsMove());
                break;
            case TutorialStep.BadyFishMissionCompleted:
                if (tutorialTime < 0f)
                {
                    goFingerMove.SetActive(false);
                    fishManager.Clean(FishBase.FishType.Enemy);
                    PBEnemyDataInfo[] pBEnemyDataInfos = new PBEnemyDataInfo[] { new PBEnemyDataInfo() { FishId = 4, FishLevel = 5, FishCountMax = 30, FishCountMin = 30 } };
                    fishManager.CreateEnemy(pBEnemyDataInfos);
                    fishManager.SetShine(FishBase.FishType.Enemy, true);
                    tutorialCntTarget = 2;
                    tutorialCntCurrent = 0;
                    textMission1.text = string.Format(LanguageDataTableProxy.GetText(901), tutorialCntCurrent, tutorialCntTarget);

                    tutorialStep = TutorialStep.JellyFishMissionChecking;
                    imgMissionComplete1.gameObject.SetActive(false);
                    animTutorialMission.SetBool("Show", true);
                    OpenTips();
                }
                break;
            case TutorialStep.JellyFishMissionCompleted:
                if (tutorialTime < 0f)
                {
                    fishManager.Clean(FishBase.FishType.Enemy);
                    shellManager.ShowTargetIcon(true);
                    this.isPause = true;
                    animTutorialMission.SetBool("Show", true);
                    tutorialStep = TutorialStep.ShellMissionChecking;
                    tutorialCntTarget = 2;
                    tutorialCntCurrent = 0;
                    textMission1.text = string.Format(LanguageDataTableProxy.GetText(902), tutorialCntCurrent, tutorialCntTarget);
                    imgMissionComplete1.gameObject.SetActive(false);
                    OpenTips();
                }
                break;
            case TutorialStep.ShellMissionCompleted:
                if (tutorialTime < 0f)
                {
                    shellManager.ShowTargetIcon(false);
                    playerIsShield = true;
                    this.isPause = true;
                    PBEnemyDataInfo[] pBEnemyDataInfos = new PBEnemyDataInfo[] { new PBEnemyDataInfo() { FishId = 5, FishLevel = 5, FishCountMax = 20, FishCountMin = 20 } };
                    fishManager.CreateEnemy(pBEnemyDataInfos);
                    fishManager.CreateBoss();
                    animTutorialMission.SetBool("Show", true);
                    tutorialStep = TutorialStep.TortoiseMissionChecking;
                    tutorialCntTarget = 5;
                    tutorialCntCurrent = 0;
                    textMission1.text = string.Format(LanguageDataTableProxy.GetText(904), tutorialCntCurrent, tutorialCntTarget);
                    imgMissionComplete1.gameObject.SetActive(false);

                    OpenTips();
                }
                break;
            case TutorialStep.TortoiseMissionChecking:
                if (inGameUIPanel.Player.isShield != playerIsShield)
                {
                    playerIsShield = inGameUIPanel.Player.isShield;
                    fishManager.SetShine(FishBase.FishType.Enemy, !inGameUIPanel.Player.isShield);
                    fishManager.SetShine(FishBase.FishType.Boss, inGameUIPanel.Player.isShield);
                }
                break;
            
            case TutorialStep.TortoiseMissionCompleted:
                if (tutorialTime < 0f)
                {
                    this.isPause = true;
                    goFingerSkill.SetActive(true);
                    animTutorialMission.SetBool("Show", true);
                    tutorialStep = TutorialStep.SkillMissionChecking;
                    inGameUIPanel.SetSkillBtnDisplay(true);
                    tutorialCntTarget = 1;
                    tutorialCntCurrent = 0;
                    textMission1.text = string.Format(LanguageDataTableProxy.GetText(906), tutorialCntCurrent, tutorialCntTarget);
                    imgMissionComplete1.gameObject.SetActive(false);

                    OpenTips();
                }
                break;
            case TutorialStep.SkillMissionCompleted:
                if (tutorialTime < 0f)
                {
                    goFingerSkill.SetActive(false);
                    animTutorialMission.SetBool("Show", true);
                    tutorialStep = TutorialStep.KillSharkMissionChecking;
                    tutorialCntTarget = 1;
                    tutorialCntCurrent = 0;
                    textMission1.text = string.Format(LanguageDataTableProxy.GetText(905), tutorialCntCurrent, tutorialCntTarget);
                    imgMissionComplete1.gameObject.SetActive(false);

                    //OpenTips();
                }
                break;
            case TutorialStep.SkillMissionChecking:
                // 一直可以发动技能
                if (inGameUIPanel.Player.fishSkill.currentGauge < 1f)
                {
                    inGameUIPanel.Player.fishSkill.currentGauge = 1f;
                }
                break;
            case TutorialStep.KillSharkMissionCompleted:
                if (tutorialTime < 0f)
                {
                    aquaticManager.ShowTargetIcon(true);
                    this.isPause = true;
                    animTutorialMission.SetBool("Show", true);
                    tutorialStep = TutorialStep.AquaticMissionChecking;
                    tutorialCntTarget = 1;
                    tutorialCntCurrent = 0;
                    textMission1.text = string.Format(LanguageDataTableProxy.GetText(907), tutorialCntCurrent, tutorialCntTarget);
                    imgMissionComplete1.gameObject.SetActive(false);

                    OpenTips();
                }
                break;
            case TutorialStep.AquaticMissionCompleted:
                if (tutorialTime < 0f)
                {
                    aquaticManager.ShowTargetIcon(false);
                    tutorialStep = TutorialStep.CompletedAll;
                    TutorialControl.NextStep();
                    this.isPause = true;
                    UIPopupBattleTutorialCompleted.Open();
                }
                break;
        }
    }
    public void OpenTips()
    {
        this.isPause = true;
        UIPopupBattleTpis.Type type = UIPopupBattleTpis.Type.BadyFish;
        switch (tutorialStep)
        {
            case TutorialStep.BadyFishMissionChecking: type = UIPopupBattleTpis.Type.BadyFish; break;
            case TutorialStep.JellyFishMissionChecking: type = UIPopupBattleTpis.Type.JellyFish; break;
            case TutorialStep.ShellMissionChecking: type = UIPopupBattleTpis.Type.Shell; break;
            case TutorialStep.TortoiseMissionChecking: type = UIPopupBattleTpis.Type.Tortoise; break;
            case TutorialStep.AquaticMissionChecking: type = UIPopupBattleTpis.Type.Aquatic; break;
            case TutorialStep.SkillMissionChecking: type = UIPopupBattleTpis.Type.Skill; break;
        }
        UIPopupBattleTpis.Open(type, () =>
        {
            this.isPause = false;
        });
    }
    public bool IsTutorialStep(TutorialStep tutorialStep)
    {
        return this.tutorialStep == tutorialStep;
    }
    public void AddTutorialCnt(TutorialStep tutorialStep)
    {
        if (tutorialStep != this.tutorialStep) { return; }
        int languageId = 0;
        ++tutorialCntCurrent;
        if (tutorialCntCurrent == tutorialCntTarget) 
        {
            animTutorialMission.SetBool("Show", false);
            imgMissionComplete1.gameObject.SetActive(true);
            FirebaseAnalytics.SetCurrentScreen("tutorial", this.tutorialStep.ToString());
            ++this.tutorialStep;
            tutorialTime = 1f;
        }
        switch (tutorialStep)
        {
            case TutorialStep.BadyFishMissionChecking: languageId = 900; break;
            case TutorialStep.JellyFishMissionChecking: languageId = 901; break;
            case TutorialStep.ShellMissionChecking: languageId = 902; break;
            case TutorialStep.TortoiseMissionChecking: languageId = 904; break;
            case TutorialStep.SkillMissionChecking: languageId = 906; break;
            case TutorialStep.AquaticMissionChecking: languageId = 907; break;
            case TutorialStep.KillSharkMissionChecking: languageId = 905; break;
        }
        textMission1.text = string.Format(LanguageDataTableProxy.GetText(languageId), tutorialCntCurrent, tutorialCntTarget);
    }
#if UNITY_EDITOR && CONSOLE_ENABLE
    public void GotoTutorialNext()
    {
        ++this.tutorialStep;
    }
#endif
}

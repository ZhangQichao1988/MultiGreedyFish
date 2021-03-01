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
    private void Awake()
	{
        instance = this;
        animator = GetComponent<Animator>();
        BattleScene bs = BlSceneManager.GetCurrentScene() as BattleScene;
        GotoBattle();
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
        fishManager.CreateEnemy();
        this.isPause = true;
    }

    public void GotoHome()
    {
        NetWorkHandler.GetDispatch().AddListener<P5_Response>(GameEvent.RECIEVE_P5_RESPONSE, OnRecvBattleResult);
        NetWorkHandler.RequestBattleResult(StageModel.Instance.battleId, battleRanking);
        StageModel.Instance.battleRanking = battleRanking;

        var levelInfo = PlayerModel.Instance.GetCurrentPlayerFishLevelInfo();
        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelEnd,
                                                        new Parameter(FirebaseAnalytics.ParameterCharacter, levelInfo.FishId),
                                                        new Parameter(FirebaseAnalytics.ParameterScore, levelInfo.RankLevel),
                                                        new Parameter(FirebaseAnalytics.ParameterLevel, levelInfo.FishLevel),
                                                        new Parameter(FirebaseAnalytics.ParameterScore, StageModel.Instance.battleRanking),
                                                        new Parameter(FirebaseAnalytics.ParameterValue, playerKilledCnt)
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

            PlayerModel.Instance.gainGold = realResponse.GainGold;
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
        if (isPause) { return; }
        fishManager.CustomUpdate();
        poisonRing.CustomUpdate();
    }
}

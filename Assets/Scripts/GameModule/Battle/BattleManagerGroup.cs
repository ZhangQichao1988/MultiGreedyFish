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

    public Text resultText = null;

    Animator animator = null;
    private bool isPause = false;
    private int battleRanking = 0;
    private void Awake()
	{
        instance = this;
        animator = GetComponent<Animator>();
        BattleScene bs = BlSceneManager.GetCurrentScene() as BattleScene;
        GotoBattle();
    }
    static public BattleManagerGroup GetInstance()
    { return instance; }

    

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
        NetWorkHandler.RequestBattleResult(battleRanking);
        
    }

    void OnRecvBattleResult<T>(T response)
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P5_RESPONSE);
        var realResponse = response as P5_Response;
        if (realResponse.Result.Code == NetworkConst.CODE_OK)
        {
            BattleResult battleResult = UIBase.Open<BattleResult>("ArtResources/UI/Prefabs/BattleResult");
            int rewardGold = realResponse.Player.Gold - PlayerModel.Instance.player.Gold;
            int rewardBattleRanking = PlayerModel.Instance.GetPlayerFishLevelInfo(realResponse.Player).RankLevel - PlayerModel.Instance.GetCurrentPlayerFishLevelInfo().RankLevel;
            battleResult.Setup(rewardGold, rewardBattleRanking);
            PlayerModel.Instance.player = realResponse.Player;
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
        if (rank == 1)
        {
            //this.isPause = true;
            animator.SetTrigger("Win");
            animator.SetTrigger("BattleEnd");
        }
        else
        {
            animator.SetTrigger("Lose");
        }
        //resultRoot.SetActive(true);
        //battleControl.SetActive(false);
        battleRanking = rank;
        resultText.text = string.Format( LanguageDataTableProxy.GetText(1), rank.ToString() );
    }

    public void BattleEnd()
    {
        animator.SetTrigger("BattleEnd");
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

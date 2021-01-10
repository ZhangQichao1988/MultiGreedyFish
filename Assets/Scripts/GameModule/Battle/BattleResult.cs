using UnityEngine;
using System.Collections.Generic;
using Google.Protobuf;
using UnityEngine.UI;
using TimerModule;

public class BattleResult : UIBase
{
    public Text textRewardGold;
    public Text textRewardGoldAdvert;
    public Text textRewardRank;
    public Text textBattleRanking;
    public Text textStreakCnt;

    public Text textBattleRankingReward;
    public Text textRankUpReward;
    public Text textTotalReward;

    public FishStatusFishControl fishControl;
    public GaugeRank gaugeRank;

    public GameObject goRankupRewardRoot;

    public Button btnGetRewardAdvert;

    // 动画演出用参数
    public float AddRankRate;
    public float AddStreakRankRate;
    public float AddBattleRankingRewardRate;
    public float AddRankUpRewardRate;

    Animator animator;
    private int rankStart;
    private float preRankGaugeRate;

    private P5_Response response;
    PBPlayerFishLevelInfo levelInfo;
    int retryTimes;

    protected override string uiName { get { return "BattleResult"; } }

    public override void Init()
    {
        base.Init();
        Setup();

    }
    protected override void OnRegisterEvent()
    {
        NetWorkHandler.GetDispatch().AddListener<P8_Response>(GameEvent.RECIEVE_P8_RESPONSE, OnRecvReward);
    }

    protected override void OnUnRegisterEvent()
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P8_RESPONSE);
    }

    void OnRecvReward(P8_Response res)
    {
        if (res.Result.Code == NetWorkResponseCode.SUCEED)
        {
            PlayerModel.Instance.player.Gold += res.RewardMoney;
            BackToHome();
        }
        else if (res.Result.Code == NetWorkResponseCode.NEED_RETRY)
        {
            //双倍领取如果 双倍奖励没有验证通过的话 会返回这个 客户端作重试处理
            if (++retryTimes < AdsController.RewardRetryTimes)
            {
                LoadingMgr.Show(LoadingMgr.LoadingType.Repeat);
                TimerManager.AddTimer((int)eTimerType.RealTime, retryTimes, (obj)=>{
                    LoadingMgr.Hide(LoadingMgr.LoadingType.Repeat);
                    NetWorkHandler.RequestGetBattleBounds(StageModel.Instance.battleId, true);
                }, null);
            }
            else
            {
                //重试失败 作普通奖励逻辑
                NetWorkHandler.RequestGetBattleBounds(StageModel.Instance.battleId, false);
            }
        }
    }

    /// <summary>
    /// 普通报酬
    /// </summary>
    public void OnClickGetReward()
    {
        NetWorkHandler.RequestGetBattleBounds(StageModel.Instance.battleId, false);
    }

    /// <summary>
    /// 双倍报酬
    /// </summary>
    public void OnClickGetRewardAdvert()
    {
        Intro.Instance.AdsController.OnAdRewardGetted = ()=>{
            LoadingMgr.Show(LoadingMgr.LoadingType.Repeat);
            TimerManager.AddTimer((int)eTimerType.RealTime, AdsController.RewardWaitTime, (obj)=>{
                LoadingMgr.Hide(LoadingMgr.LoadingType.Repeat);
                NetWorkHandler.RequestGetBattleBounds(StageModel.Instance.battleId, true);
            }, null);
        };
        Intro.Instance.AdsController.Show();
    }

    void BackToHome()
    {
        var homeScene = BlSceneManager.GetCurrentScene() as HomeScene;
        homeScene.GotoSceneUI("Home");
        //Close();
        //BlSceneManager.LoadSceneByClass(SceneId.HOME_SCENE, typeof(HomeScene), "BattleResult");
    }

    public void Setup()
    {
        animator = GetComponent<Animator>();

        response = StageModel.Instance.resultResponse;
        
        levelInfo = PlayerModel.Instance.GetCurrentPlayerFishLevelInfo();
        
        // rank条
        gaugeRank.Refash(levelInfo);
        preRankGaugeRate = gaugeRank.sliderRankLevel.value;

        // 中间的鱼
        fishControl.CreateFishModel(levelInfo.FishId);

        btnGetRewardAdvert.gameObject.SetActive( response.GainGold > 0 );

        rankStart = levelInfo.RankLevel;
        int totalGold = response.GainGold + response.GainRankLevelupBonusGold;

        textBattleRankingReward.text = response.GainGold.ToString();

        // 荣誉提升奖励
        goRankupRewardRoot.SetActive(response.GainRankLevelupBonusGold > 0);
        if (goRankupRewardRoot.activeSelf)
        {
            textRankUpReward.text = response.GainRankLevelupBonusGold.ToString();
        }

        textBattleRanking.text = string.Format(LanguageDataTableProxy.GetText(9), StageModel.Instance.battleRanking);
        
        int currentWin = response.FightFish.CurrentWin;
        levelInfo.CurrentWin = currentWin;
        if (currentWin > 1)
        {
            textStreakCnt.text = string.Format(LanguageDataTableProxy.GetText(62), currentWin);
            textStreakCnt.gameObject.SetActive(true);
        }
        else
        {
            textStreakCnt.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // rank条更新
        if(AddStreakRankRate <= 1.5f)
        {
            int rankUp = response.GainRankLevel;
            int streakRankUp = response.ContWinRankAdded;
            int totalRankUp = (int)(Mathf.Lerp(0, rankUp, AddRankRate) + Mathf.Lerp(0, streakRankUp, AddStreakRankRate));
            if (totalRankUp < 0)
            {
                textRewardRank.text = "-" + totalRankUp;
            }
            else
            {
                textRewardRank.text = "+" + totalRankUp;
            }
            levelInfo.RankLevel = totalRankUp;
            gaugeRank.Refash(levelInfo);
            if (preRankGaugeRate > gaugeRank.sliderRankLevel.value)
            {
                animator.SetTrigger("RankUp");
            }
        }

        // 明细显示
        if (AddBattleRankingRewardRate <= 1.5f || AddRankUpRewardRate <= 1.5f)
        {
            int AddBattleRankingReward = (int)Mathf.Lerp(0, response.GainGold, AddBattleRankingRewardRate);
            int AddRankUpReward = (int)Mathf.Lerp(0, response.GainRankLevelupBonusGold, AddRankUpRewardRate);
            int totalGold = AddBattleRankingReward + AddRankUpReward;
            textTotalReward.text = totalGold.ToString();
            textRewardGold.text = string.Format(LanguageDataTableProxy.GetText(8), totalGold);
            textRewardGoldAdvert.text = string.Format(LanguageDataTableProxy.GetText(8), totalGold * ConfigTableProxy.Instance.GetDataById(1001).intValue); 

        }
    }
}
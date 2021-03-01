using UnityEngine;
using System.Collections.Generic;
using Google.Protobuf;
using UnityEngine.UI;
using TimerModule;
using Firebase.Analytics;

public class BattleResult : UIBase
{

    public Text textRewardGold;
    public Text textRewardGoldAdvert;

    public Text textRewardRank;

    public Text textGoldPool;
    public Text textRewardRankStreak;
    public Text textBattleRanking;
    public Text textStreakCnt;

    public Text textBattleRankingReward;
    public Text textRankUpReward;
    public Text textStreakReward;
    public Text textTotalReward;

    public FishStatusFishControl fishControl;
    public GaugeRank gaugeRank;

    public GameObject goRewardRoot;
    public GameObject goStreakRankupRewardRoot;

    public Button btnGetReward;
    public Button btnGetRewardAdvert;

    // 动画演出用参数
    public float AddBattleRankingRewardRate;
    public float AddRankUpRewardRate;
    public float AddContWinRewardRate;

    Animator animator;
    private int rankStart;
    private float preRankGaugeRate;

    private P5_Response response;
    PBPlayerFishLevelInfo levelInfo;
    int retryTimes;
    AudioSource countAudioSource;

    // 演出相关
    int animStep = 0;
    float animTime = 0f;
    public Text textStreakAddRankLevel;
    public Text textTotalAddRankLevel;
    public GameObject goAddGoldBattleRanking;
    public GameObject goAddGoldRankUp;
    public GameObject goAddGoldStreak;


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
            PlayerModel.Instance.MissionActionTriggerAdd(3, 1);
            PlayerModel.Instance.MissionActionTriggerAdd(14, res.RewardMoney);
            BackToHome();
        }
        else if (res.Result.Code == NetWorkResponseCode.ADS_NEED_RETRY)
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
        Intro.Instance.AdsController.Show(GameHelper.AdmobCustomGenerator(AdmobEvent.BattleReward));
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

        if (levelInfo.RankLevel + response.GainRankLevel < 0)
        {
            response.GainRankLevel = -levelInfo.RankLevel;
        }

        // 提升段位积分
        if (response.GainRankLevel < 0)
        {
            textRewardRank.text = response.GainRankLevel.ToString();
        }
        else
        {
            textRewardRank.text = "+" + response.GainRankLevel;
        }
        // rank条
        gaugeRank.Refash(levelInfo);
        preRankGaugeRate = gaugeRank.sliderRankLevel.value;

        // 中间的鱼
        fishControl.CreateFishModel(levelInfo.FishId);

        btnGetRewardAdvert.gameObject.SetActive( response.GainGold > 0 );

        rankStart = levelInfo.RankLevel;
        //int totalGold = response.GainGold + response.GainRankLevelupBonusGold;

        //textBattleRankingReward.text = response.GainGold.ToString();

        //// 荣誉提升奖励
        //goRankupRewardRoot.SetActive(response.GainRankLevelupBonusGold > 0);
        //if (goRankupRewardRoot.activeSelf)
        //{
        //    textRankUpReward.text = response.GainRankLevelupBonusGold.ToString();
        //}

        textBattleRanking.text = string.Format(LanguageDataTableProxy.GetText(9), StageModel.Instance.battleRanking);
        textGoldPool.text = response.GoldPoolCurrGold.ToString();
        if (StageModel.Instance.battleRanking <= 3)
        {
            PlayerModel.Instance.MissionActionTriggerAdd(4, 1);
        }
        


    }

    private void Update()
    {
        int tmp = 0;
        animTime -= Time.deltaTime;
        switch (animStep)
        {
            case 0: // 稍等一下再开始
                animTime = 1f;
                animStep = 1;
                break;
            case 1: // 显示第几名
                if (animTime > 0f) { return; }
                goRewardRoot.SetActive(true);
                if (response.GainRankLevel > 0)
                {
                    textTotalAddRankLevel.gameObject.SetActive(true);
                }
                animTime = 0.5f;
                animStep = 2;
                countAudioSource = SoundManager.PlaySE(1004);
                break;
            case 2:
                if (response.GainRankLevel > 0)
                {
                    textTotalAddRankLevel.text = "+" + (int)Mathf.Lerp(response.GainRankLevel, 0f,  animTime * 2f);
                }
                if (animTime <= 0f) 
                {
                    countAudioSource.Stop();
                    // 判定是否2连胜以上
                    int currentWin = response.FightFish.CurrentWin;
                    if (currentWin > 0)
                    {
                        PlayerModel.Instance.MissionActionTrigger(2, currentWin);
                    }
                    levelInfo.CurrentWin = currentWin;
                    if (currentWin >= 2)
                    {
                        textStreakCnt.text = string.Format(LanguageDataTableProxy.GetText(62), currentWin);
                        textStreakAddRankLevel.text = "+" + response.ContWinRankAdded;
                        goStreakRankupRewardRoot.SetActive(true);
                        animTime = 0.5f;
                        countAudioSource = SoundManager.PlaySE(1004);
                        animStep = 5;
                    }
                    else
                    {
                        animTime = 0.5f;
                        animStep = 6;
                    }
                }
                break;
            case 5:// 连胜显示
                textTotalAddRankLevel.text = "+" + (response.GainRankLevel + (int)Mathf.Lerp(response.ContWinRankAdded, 0f, animTime * 2f));
                if (animTime <= 0f)
                {
                    countAudioSource.Stop();
                    textTotalAddRankLevel.text = "+" + (response.GainRankLevel + response.ContWinRankAdded);
                    animTime = 0.5f;
                    animStep = 6;
                }
                break;
            case 6: // 加算经验条前等一等
                if (animTime <= 0f)
                {
                    animTime = 0.5f;
                    animStep = 10;
                    countAudioSource = SoundManager.PlaySE(1004);
                }
                break;
            case 10:// 加算经验条
                int totalRankLevel = (int)Mathf.Lerp(0f, (response.GainRankLevel + response.ContWinRankAdded), animTime * 2f);
                if (totalRankLevel > 0)
                {
                    textTotalAddRankLevel.text = "+" + totalRankLevel;
                }
                else if (totalRankLevel < 0)
                {
                    textTotalAddRankLevel.text = totalRankLevel.ToString();
                }
                else
                {
                    textTotalAddRankLevel.gameObject.SetActive(false);
                }
                levelInfo.RankLevel = rankStart + response.GainRankLevel + response.ContWinRankAdded - totalRankLevel;
                if (animTime <= 0f)
                {
                    countAudioSource.Stop();
                    levelInfo.RankLevel = rankStart + response.GainRankLevel + response.ContWinRankAdded;
                    textTotalAddRankLevel.gameObject.SetActive(false);
                    // 显示金币明细（战斗排名）
                    textBattleRankingReward.text = "0";
                    goAddGoldBattleRanking.SetActive(true);
                    countAudioSource = SoundManager.PlaySE(1002);
                    animTime = 0.5f;
                    animStep = 15;
                }
                gaugeRank.Refash(levelInfo);
                if (response.GainRankLevel > 0 && preRankGaugeRate > gaugeRank.sliderRankLevel.value)
                {
                    if (gaugeRank.rankId >= 16)
                    {   // 传说
                        PlayerModel.Instance.MissionActionTrigger(35, 1);
                    }
                    else if (gaugeRank.rankId >= 11)
                    {   // 黄金
                        PlayerModel.Instance.MissionActionTrigger(34, 1);
                    }
                    else if(gaugeRank.rankId >= 6)
                    {   // 白银
                        PlayerModel.Instance.MissionActionTrigger(33, 1);
                    }
                    animator.SetTrigger("RankUp");
                }
                break;
            case 15: // 显示金币明细（战斗排名）
                AddBattleRankingRewardRate = 1 - animTime * 2f;
                tmp = (int)Mathf.Lerp(0, response.GainGold, AddBattleRankingRewardRate);
                textBattleRankingReward.text = tmp.ToString();
                textGoldPool.text = (response.GoldPoolCurrGold - tmp).ToString();
                if (animTime <= 0f)
                {
                    countAudioSource.Stop();
                    if (response.GainRankLevelupBonusGold > 0)
                    {   // 是否有段位升级奖励
                        textRankUpReward.text = "0";
                        goAddGoldRankUp.SetActive(true);
                        animTime = 0.5f;
                        countAudioSource = SoundManager.PlaySE(1002);
                        animStep = 20;
                    }
                    else if (response.ContWinGoldAdded > 0)
                    {   // 是否有连胜奖励
                        animStep = 25;
                    }
                    else
                    {
                        animStep = 30;
                    }
                }
                break;
            case 20: // 段位升级金币加算
                AddRankUpRewardRate = 1 - animTime * 2f;
                textRankUpReward.text = ((int)Mathf.Lerp(0, response.GainRankLevelupBonusGold, AddRankUpRewardRate)).ToString();
                if (animTime <= 0f)
                {
                    countAudioSource.Stop();
                    animStep = 25;
                }
                break;
            case 25:// 连胜金币加算初始化
                textStreakReward.text = "0";
                goAddGoldStreak.SetActive(true);
                countAudioSource = SoundManager.PlaySE(1002);
                animTime = 0.5f;
                animStep = 26;
                break;
            case 26:
                AddContWinRewardRate = 1 - animTime * 2f;
                textStreakReward.text = ((int)Mathf.Lerp(0, response.ContWinGoldAdded, AddContWinRewardRate)).ToString();

                if (animTime <= 0f)
                {
                    countAudioSource.Stop();
                    animStep = 30;
                }
                break;
            case 30:
                btnGetReward.interactable = true;
                btnGetRewardAdvert.interactable = true;
                animStep = 40;
                break;
        }


        //// rank条更新
        //if(AddStreakRankRate <= 1f)
        //{
        //    int rankUp = response.GainRankLevel;
        //    int streakRankUp = response.ContWinRankAdded;
        //    int totalRankUp = (int)(Mathf.Lerp(0, rankUp, AddRankRate) + Mathf.Lerp(0, streakRankUp, AddStreakRankRate));
        //    if (totalRankUp < 0)
        //    {
        //        textRewardRank.text = "-" + totalRankUp;
        //    }
        //    else
        //    {
        //        textRewardRank.text = "+" + totalRankUp;
        //    }
        //    levelInfo.RankLevel = totalRankUp;
        //    gaugeRank.Refash(levelInfo);

        //}

        //// 明细显示
        //if (AddBattleRankingRewardRate <= 1f || AddRankUpRewardRate <= 1f)
        //{
        int AddBattleRankingReward = (int)Mathf.Lerp(0, response.GainGold, AddBattleRankingRewardRate);
        int AddRankUpReward = (int)Mathf.Lerp(0, response.GainRankLevelupBonusGold, AddRankUpRewardRate);
        int AddContWinReward = (int)Mathf.Lerp(0, response.ContWinGoldAdded, AddContWinRewardRate);
        int totalGold = AddBattleRankingReward + AddRankUpReward + AddContWinReward;
        textTotalReward.text = totalGold.ToString();
        textRewardGold.text = string.Format(LanguageDataTableProxy.GetText(8), totalGold);
        textRewardGoldAdvert.text = string.Format(LanguageDataTableProxy.GetText(8), totalGold * ConfigTableProxy.Instance.GetDataById(1001).intValue);

        //}
    }
}
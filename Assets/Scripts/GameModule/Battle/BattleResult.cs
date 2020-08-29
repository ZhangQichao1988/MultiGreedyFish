using UnityEngine;
using System.Collections.Generic;
using Google.Protobuf;
using UnityEngine.UI;
using TimerModule;

public class BattleResult : UIBase
{
    [SerializeField]
    Text textRewardGold;

    [SerializeField]
    Text textRewardGoldAdvert;

    [SerializeField]
    Text textRewardRank;

    int retryTimes;

    protected override string uiName { get { return "BattleResult"; } }

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
            // TODO:将来要在Home界面做加算演出
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
                    NetWorkHandler.RequestGetBattleBounds(StageModel.Instance.BattleId, true);
                }, null);
            }
            else
            {
                //重试失败 作普通奖励逻辑
                NetWorkHandler.RequestGetBattleBounds(StageModel.Instance.BattleId, false);
            }
        }
    }

    /// <summary>
    /// 普通报酬
    /// </summary>
    public void OnClickGetReward()
    {
        NetWorkHandler.RequestGetBattleBounds(StageModel.Instance.BattleId, false);
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
                NetWorkHandler.RequestGetBattleBounds(StageModel.Instance.BattleId, true);
            }, null);
        };
        Intro.Instance.AdsController.Show();
    }

    void BackToHome()
    {
        Close();
        BlSceneManager.LoadSceneByClass(SceneId.HOME_SCENE, typeof(HomeScene));
    }

    public void Setup(P5_Response response)
    {
        // TODO:明细显示时分离
        int gold = response.GainGold + response.GainRankLevelupBonusGold;
        int rankUp = response.GainRankLevel;

        Debug.Assert(textRewardGold != null, "BattleResult.Setup()_1");
        textRewardGold.text = string.Format( LanguageDataTableProxy.GetText(8), gold);

        Debug.Assert(textRewardGoldAdvert != null, "BattleResult.Setup()_2");
        textRewardGoldAdvert.text = string.Format(LanguageDataTableProxy.GetText(8), gold * ConfigTableProxy.Instance.GetDataByKey("BattleRewardGoldAdvertRate"));

        Debug.Assert(textRewardRank != null, "BattleResult.Setup()_3");
        textRewardRank.text = string.Format(LanguageDataTableProxy.GetText(9), rankUp);
    }
}
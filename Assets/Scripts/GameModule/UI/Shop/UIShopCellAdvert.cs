
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TimerModule;

public class UIShopCellAdvert : UIShopCell
{
    public GameObject goNew;

    private int AdvertRewardLimitCnt;

    private void Awake()
    {
        AdvertRewardLimitCnt = ConfigTableProxy.Instance.GetDataById(3100).intValue;
    }
    public override void UpdateData(System.Object data)
    {
        string itemName = ItemDataTableProxy.GetItemName(1);
        var rewardList = ItemDataTableProxy.GetRewardList(ConfigTableProxy.Instance.GetDataById(3101).stringValue);
        textReward.text = string.Format( LanguageDataTableProxy.GetText(204), itemName, rewardList[0].amount);

        Refresh();
    }

    public override void Refresh()
    {
        textName.text = string.Format(LanguageDataTableProxy.GetText(205), AdvertRewardLimitCnt - PlayerModel.Instance.player.AdvertRewardRemainingCnt);
        buyBtn.interactable = PlayerModel.Instance.player.AdvertRewardRemainingCnt < AdvertRewardLimitCnt;
        banObject.SetActive(!buyBtn.interactable);
        goNew.SetActive(buyBtn.interactable);
        textBan.text = LanguageDataTableProxy.GetText(202);
    }

    public override void OnCellClick()
    {
        Intro.Instance.AdsController.OnAdRewardGetted = ()=>{
            LoadingMgr.Show(LoadingMgr.LoadingType.Repeat);
            TimerManager.AddTimer((int)eTimerType.RealTime, AdsController.RewardWaitTime, (obj)=>{
                LoadingMgr.Hide(LoadingMgr.LoadingType.Repeat);
                GetRewardDirect(true);
            }, null);
        };
        Intro.Instance.AdsController.Show(GameHelper.AdmobCustomGenerator(AdmobEvent.DiamondReward, DailyRewardType.FREE_DIAMOND.ToString()));


    }

    void GetRewardDirect(bool isfirst = false)
    {
        if (isfirst)
        {
            NetWorkHandler.GetDispatch().AddListener<P22_Response>(GameEvent.RECIEVE_P22_RESPONSE, OnRecvGetAdvertRemainingCnt);
            NetWorkHandler.RequestGetAdvertRemainingCnt();
        }
        else
        {
            LoadingMgr.Show(LoadingMgr.LoadingType.Repeat);
            AdsController.RewardHttpRetryTimes++;
            TimerManager.AddTimer((int)eTimerType.RealTime, AdsController.RewardWaitTime, (obj)=>{
                    LoadingMgr.Hide(LoadingMgr.LoadingType.Repeat);
                    NetWorkHandler.GetDispatch().AddListener<P22_Response>(GameEvent.RECIEVE_P22_RESPONSE, OnRecvGetAdvertRemainingCnt);
                    NetWorkHandler.RequestGetAdvertRemainingCnt();
                }, null);
        }
        
    }

    void OnRecvGetAdvertRemainingCnt(P22_Response res)
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P22_RESPONSE);
        Debug.Log("OnRecvGetAdvertRemainingCnt!");
        if (res.Result.Code == NetWorkResponseCode.SUCEED)
        {
            PlayerModel.Instance.player.AdvertRewardRemainingCnt = res.AdvertRewardRemainingCnt;
            Refresh();
            var rewardVO = RewardMapVo.From(res.Content);
            var homeScene = BlSceneManager.GetCurrentScene() as HomeScene;
            PlayerModel.Instance.UpdateAssets(rewardVO, "get_free_diamond");
            homeScene.OnGettedItemNormal(rewardVO);
            AdsController.RewardHttpRetryTimes = 0;
        }
        else if (res.Result.Code == NetWorkResponseCode.DOUBLE_REWADR_ERROR && AdsController.RewardHttpRetryTimes < 2)
        {
            //双倍错误 进行重发
            GetRewardDirect();
        }
        else
        {
            //todo l10n
            MsgBox.OpenTips(res.Result.Desc);
            AdsController.RewardHttpRetryTimes = 0;
        }
    }

}
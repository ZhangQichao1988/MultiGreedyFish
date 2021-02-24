using NetWorkModule;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TimerModule;

public class UIMissionItem : SimpleScrollingCell
{
    public Text textProcess;
    public Text textRemainingTime;
    public Text textBody;
    public Text textStatus;
    public Text textGold;
    public GameObject goRewardBtn;
    public GameObject goMask;
    public GameObject goTimeout;

    PBMission pBMission;
    float backupTime;
    long remainingTime;
    bool isReach;
    ItemDataTableProxy.RewardData rewardData;
    public void Setup(PBMission pBMission)
    {
        this.pBMission = pBMission;
        goTimeout.SetActive(false);
        isReach = pBMission.CurrTrigger >= pBMission.Trigger;
        textProcess.text = string.Format(LanguageDataTableProxy.GetText(700), pBMission.CurrTrigger, pBMission.Trigger);
        backupTime = Time.realtimeSinceStartup;
        //remainingTime = (float)Clock.Timestamp % 86400;
        long secTime = 0;
        switch (pBMission.Type)
        {
            case MissionType.MissionDaily:
                secTime = Clock.SecOfDay;
                remainingTime = secTime - (long)Clock.Timestamp % secTime;
                break;
            case MissionType.MissionWeekly:
                secTime = Clock.SecOfWeek;
                remainingTime = secTime - (long)Clock.Timestamp % secTime;
                break;
            default:
                remainingTime = 0;
                break;
        }

        var actionData =  MissionActionDataTableProxy.Instance.GetDataById(pBMission.ActionId);
        textBody.text = string.Format( LanguageDataTableProxy.GetText(actionData.desc), pBMission.Trigger);

        // 已完成
        if (isReach)
        {
            goMask.SetActive(pBMission.IsComplete);
            goRewardBtn.SetActive(!pBMission.IsComplete);
            goMask.SetActive(pBMission.IsComplete);
            if (pBMission.IsComplete)
            {
                textRemainingTime.text = LanguageDataTableProxy.GetText(708);
                textRemainingTime.gameObject.SetActive(true);
            }
            else
            {
                textRemainingTime.gameObject.SetActive(false);
            }
        }
        else
        {
            goMask.SetActive(false);
            goRewardBtn.SetActive(false);
            goMask.SetActive(false);
            textRemainingTime.gameObject.SetActive(pBMission.Type != MissionType.MissionAchievement);
            textRemainingTime.gameObject.SetActive(true);

        }

        // 报酬图标显示
        var listReward = ItemDataTableProxy.GetRewardList(pBMission.Reward);
        rewardData = listReward[0];
        textGold.text = rewardData.amount.ToString();
    }
    public void GetReward()
    {
        if (AppConst.NotShowAdvert == 0)
        {
            UIGetReward.Open(rewardData, GameHelper.AdmobCustomGenerator(AdmobEvent.MissionReward, pBMission.MissionId.ToString()),
            (isDouble) =>
            {
                NetWorkHandler.GetDispatch().AddListener<P21_Response>(GameEvent.RECIEVE_P21_RESPONSE, OnRecvGetBonus);
                NetWorkHandler.RequestGetMissionBonus(pBMission.MissionId, isDouble);
            });
        }
        else
        {
            NetWorkHandler.GetDispatch().AddListener<P21_Response>(GameEvent.RECIEVE_P21_RESPONSE, OnRecvGetBonus);
            NetWorkHandler.RequestGetMissionBonus(pBMission.MissionId);
        }
    }

    void GetRewardDirect()
    {
        LoadingMgr.Show(LoadingMgr.LoadingType.Repeat);
        AdsController.RewardHttpRetryTimes++;
        TimerManager.AddTimer((int)eTimerType.RealTime, AdsController.RewardWaitTime, (obj)=>{
                LoadingMgr.Hide(LoadingMgr.LoadingType.Repeat);
                NetWorkHandler.GetDispatch().AddListener<P21_Response>(GameEvent.RECIEVE_P21_RESPONSE, OnRecvGetBonus);
                NetWorkHandler.RequestGetMissionBonus(pBMission.MissionId, true);
            }, null);
    }

    void OnRecvGetBonus<T>(T response)
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P21_RESPONSE);
        Debug.Log("OnRecvGetBonus!");
        var res = response as P21_Response;
        if (res.Result.Code == NetWorkResponseCode.SUCEED)
        {
            var rewardVO = RewardMapVo.From(res);
            var homeScene = BlSceneManager.GetCurrentScene() as HomeScene;
            PlayerModel.Instance.UpdateAssets(rewardVO);
            homeScene.OnGettedItemNormal(rewardVO);
            if (res.NewMission == null)
            { // 没有更新任务
                pBMission.IsComplete = true;
            }
            else
            {
                int index = PlayerModel.Instance.pBMissions.IndexOf(pBMission);
                Debug.Assert(index >= 0, "UIMissionItem.OnRecvGetBonus()_1");
                PlayerModel.Instance.pBMissions.Remove(pBMission);
                pBMission = res.NewMission;
                PlayerModel.Instance.pBMissions.Insert(index, pBMission);
            }
            Setup(pBMission);
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
    private void Update()
    {
        if (pBMission != null && pBMission.Type != MissionType.MissionAchievement && !isReach)
        {   
            int nowRemainingTime = (int)(remainingTime - Time.realtimeSinceStartup - backupTime);
            if (nowRemainingTime > Clock.SecOfDay)
            {
                textRemainingTime.text = string.Format(LanguageDataTableProxy.GetText(702), nowRemainingTime / Clock.SecOfDay);
            }
            else if (nowRemainingTime > 3600)
            {
                textRemainingTime.text = string.Format(LanguageDataTableProxy.GetText(705), nowRemainingTime / 3600);
            }
            else if (nowRemainingTime > 60)
            {
                textRemainingTime.text = string.Format(LanguageDataTableProxy.GetText(706), nowRemainingTime / 60);
            }
            else if (nowRemainingTime > 0)
            {
                textRemainingTime.text = string.Format(LanguageDataTableProxy.GetText(707), nowRemainingTime);
            }
            else
            {
                goMask.SetActive(true);
                goTimeout.SetActive(true);
                textRemainingTime.gameObject.SetActive(false);
            }
        }

    }
}

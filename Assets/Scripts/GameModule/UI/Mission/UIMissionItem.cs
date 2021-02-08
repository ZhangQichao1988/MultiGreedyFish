using NetWorkModule;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UIMissionItem : SimpleScrollingCell
{
    public Text textProcess;
    public Text textRemainingTime;
    public Text textBody;
    public GameObject goRewardRoot;
    public GameObject goRewardBtn;
    public GameObject goMask;
    public GameObject goTimeout;
    public GameObject goComplete;

    PBMission pBMission;
    float backupTime;
    float remainingTime;
    bool isReach;
    public void Setup(PBMission pBMission)
    {
        this.pBMission = pBMission;
        goTimeout.SetActive(false);
        isReach = pBMission.CurrTrigger >= pBMission.Trigger;
        textProcess.text = string.Format(LanguageDataTableProxy.GetText(700), pBMission.CurrTrigger, pBMission.Trigger);
        backupTime = Time.realtimeSinceStartup;
        //remainingTime = (float)Clock.Timestamp % 86400;
        int secTime = 0;
        switch (pBMission.Type)
        {
            case MissionType.MissionDaily:
                secTime = Clock.SecOfDay;
                remainingTime = secTime - (float)1612662112 % secTime;
                break;
            case MissionType.MissionWeekly:
                secTime = Clock.SecOfWeek;
                remainingTime = secTime - (float)1612662112 % secTime;
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
            goComplete.SetActive(pBMission.IsComplete);
            textRemainingTime.text = LanguageDataTableProxy.GetText(701);
        }
        else
        {
            goMask.SetActive(false);
            goRewardBtn.SetActive(false);
            goComplete.SetActive(false);
            if (pBMission.Type == MissionType.MissionAchievement)
            {
                textRemainingTime.gameObject.SetActive(false);
            }
        }

        // 报酬图标显示
        GameObjectUtil.DestroyAllChildren(goRewardRoot);
        var asset = ResourceManager.LoadSync<GameObject>(Path.Combine(AssetPathConst.uiRootPath, "PlayerRanking/PlayerRankingRewardItemItem"));
        var listReward = ItemDataTableProxy.GetRewardList(pBMission.Reward);
        GameObject tmp;
        UIPlayerRankingRewardItemItem uIPlayerRankingRewardItemItem;
        ItemData itemData;
        foreach (var note in listReward)
        {
            tmp = GameObjectUtil.InstantiatePrefab(asset.Asset, goRewardRoot);
            uIPlayerRankingRewardItemItem = tmp.GetComponent<UIPlayerRankingRewardItemItem>();
            itemData = ItemDataTableProxy.Instance.GetDataById(note.id);
            uIPlayerRankingRewardItemItem.Init(itemData.resIcon, note.amount);
        }
    }
    public void GetReward()
    {
        NetWorkHandler.GetDispatch().AddListener<P21_Response>(GameEvent.RECIEVE_P21_RESPONSE, OnRecvGetBonus);
        NetWorkHandler.RequestGetMissionBonus(pBMission.MissionId);

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
                pBMission = res.NewMission;
            }
            Setup(pBMission);
        }
        else
        {
            //todo l10n
            MsgBox.OpenTips(res.Result.Desc);
        }
    }
    private void Update()
    {
        if (pBMission.Type != MissionType.MissionAchievement && !isReach)
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
            }
        }

    }
}

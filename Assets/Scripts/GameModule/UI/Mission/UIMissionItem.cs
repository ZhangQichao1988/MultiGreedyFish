using NetWorkModule;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UIMissionItem : SimpleScrollingCell
{
    // 一天的秒数
    static readonly int secOfDay = 86400;
    // 一周的秒数
    static readonly int secOfWeek = secOfDay * 7;
    public Text textProcess;
    public Text textRemainingTime;
    public Text textBody;
    public GameObject goRewardRoot;
    public GameObject goRewardBtn;
    public GameObject goMask;
    public GameObject goTimeout;

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
                secTime = secOfDay;
                break;
            case MissionType.MissionWeekly:
                secTime = secOfWeek;
                break;
        }
        remainingTime = secTime - (float)1612662112 % secTime;

        var actionData =  MissionActionDataTableProxy.Instance.GetDataById(pBMission.ActionId);
        textBody.text = string.Format( LanguageDataTableProxy.GetText(actionData.desc), pBMission.Trigger);

        // 已完成
        if (isReach)
        {
            goMask.SetActive(pBMission.IsComplete);
            goRewardBtn.SetActive(!pBMission.IsComplete);
            textRemainingTime.text = LanguageDataTableProxy.GetText(701);
        }
        else
        {
            goMask.SetActive(false);
            goRewardBtn.SetActive(false);

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
        if (!isReach)
        {   
            int nowRemainingTime = (int)(remainingTime - Time.realtimeSinceStartup - backupTime);
            if (nowRemainingTime > 3600)
            {
                textRemainingTime.text = string.Format(LanguageDataTableProxy.GetText(702), nowRemainingTime / 3600 + "h");
            }
            else if (nowRemainingTime > 60)
            {
                textRemainingTime.text = string.Format(LanguageDataTableProxy.GetText(702), nowRemainingTime / 60 + "m");
            }
            else if (nowRemainingTime > 0)
            {
                textRemainingTime.text = string.Format(LanguageDataTableProxy.GetText(702), nowRemainingTime + "s");
            }
            else
            {
                goMask.SetActive(true);
                goTimeout.SetActive(true);
            }
        }

    }
}

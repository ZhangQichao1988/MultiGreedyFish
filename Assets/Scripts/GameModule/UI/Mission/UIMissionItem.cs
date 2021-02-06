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

    PBMission pBMission;
    float backupTime;
    float remainingTime;
    bool isReach;
    private void Awake()
    {
    }
    public void Setup(PBMission pBMission)
    {
        this.pBMission = pBMission;
        isReach = pBMission.CurrTrigger >= pBMission.Trigger;
        if (isReach)
        {
            textProcess.text = LanguageDataTableProxy.GetText(701);
        }
        else 
        {
            textProcess.text = string.Format(LanguageDataTableProxy.GetText(700), pBMission.CurrTrigger, pBMission.Trigger);
        }
        backupTime = Time.realtimeSinceStartup;
        remainingTime = 1601719931 % 86400;
        var actionData =  MissionActionDataTableProxy.Instance.GetDataById(pBMission.ActionId);
        textBody.text = string.Format( LanguageDataTableProxy.GetText(actionData.desc), pBMission.Trigger);

        // 已完成
        if (isReach)
        {
            goMask.SetActive(pBMission.IsComplete);
            goRewardBtn.SetActive(!pBMission.IsComplete);
        }
        else
        {
            goMask.SetActive(false);
        }

        // 报酬图标显示
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

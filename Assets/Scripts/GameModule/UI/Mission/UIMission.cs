using UnityEngine;
using System.Collections.Generic;
using Google.Protobuf;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using System;
using UnityEngine.Audio;

public class UIMission : UIBase
{
    static public UIMission instance;

    public GameObject goMission;
    public GameObject goAchievement;
    public GameObject goNewMission;
    public GameObject goNewAchievement;
    public SimpleScrollingView scrollingView;

    List<UIMissionItem> listMissionItem = new List<UIMissionItem>();
    P20_Response res;
    public override void Init()
    {
        instance = this;
        scrollingView.Init(AssetPathConst.uiRootPath + "Mission/MissionItem");
        base.Init();
    }
    public override void OnEnter(object obj)
    {
        base.OnEnter(obj);
        FetchMission( 0);
    }

    public void ApplyNewIcon()
    {
        bool isNewMission = false;
        bool isNewAchievement = false;
        foreach (var note in PlayerModel.Instance.pBMissions)
        {
            if ( note.CurrTrigger >= note.Trigger && !note.IsComplete)
            {
                if (!isNewMission && note.Type != MissionType.MissionAchievement)
                {
                    isNewMission = true;
                }
                else if (!isNewAchievement && note.Type == MissionType.MissionAchievement)
                {
                    isNewAchievement = true;
                }
            }
        }
        goNewMission.SetActive(isNewMission);
        goNewAchievement.SetActive(isNewAchievement);
    }

    // 获取任务列表
    public void FetchMission(int missionType)
    {
        ApplyNewIcon();

        List<PBMission> MissionList =  new List<PBMission>(PlayerModel.Instance.pBMissions);
        switch (missionType)
        {
            case 0:
                MissionList = MissionList.FindAll((a) => a.Type != global::MissionType.MissionAchievement);
                goMission.SetActive(true);
                goAchievement.SetActive(false);
                break;
            case 1:
                MissionList = MissionList.FindAll((a) => a.Type == global::MissionType.MissionAchievement);
                goMission.SetActive(false);
                goAchievement.SetActive(true);
                break;
        }

        // 每日-》每周-》成就
        //MissionList.Sort((a, b) => { return a.Type - b.Type; });
        MissionList.Sort((a, b) => { return a.MissionId - b.MissionId; });

        // 获得过奖励的靠后,没拿报酬的靠前
        List<PBMission> list1 = MissionList.Where<PBMission>((a) => a.CurrTrigger >= a.Trigger && !a.IsComplete).ToList();
        List<PBMission> list2 = MissionList.Where<PBMission>((a) => a.CurrTrigger < a.Trigger && !a.IsComplete).ToList();
        List<PBMission> list3 = MissionList.Where<PBMission>((a) => a.CurrTrigger >= a.Trigger && a.IsComplete).ToList();
        MissionList.Clear();
        MissionList.AddRange(list1);
        MissionList.AddRange(list2);
        MissionList.AddRange(list3);

        var listCell = scrollingView.Fill(MissionList.Count);

        UIMissionItem uIItem;
        listMissionItem.Clear();
        for (int i = 0; i < listCell.Count; ++i)
        {
            uIItem = listCell[i] as UIMissionItem;
            listMissionItem.Add(uIItem);
        }

        for (int i = 0; i < listMissionItem.Count; ++i)
        {
            listMissionItem[i].Setup(MissionList[i]);
        }
    }
    private void OnDestroy()
    {
        instance = null;
    }
}
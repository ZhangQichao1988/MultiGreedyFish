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

    public SimpleScrollingView scrollingView;

    List<UIMissionItem> listMissionItem = new List<UIMissionItem>();
    P20_Response res;
    public override void Init()
    {
        base.Init();
    }
    public override void OnEnter(object obj)
    {
        base.OnEnter(obj);
        FetchMission();
    }

    // 获取任务列表
    public void FetchMission()
    {
        var MissionList = PlayerModel.Instance.pBMissions;

        // 每日-》每周-》成就
        //MissionList.Sort((a, b) => { return a.Type - b.Type; });
        MissionList.Sort((a, b) => { return a.MissionId - b.MissionId; });


        //// 获得过奖励的靠后
        //MissionList.Sort((a, b) =>
        //{
        //    if (a.IsComplete && !b.IsComplete)
        //    {
        //        return 1;
        //    }
        //    else if (!a.IsComplete && b.IsComplete)
        //    {
        //        return -1;
        //    }
        //    return 0;
        //});

        // 获得过奖励的靠后,没拿报酬的靠前
        List<PBMission> list1 = MissionList.Where<PBMission>((a) => a.CurrTrigger >= a.Trigger && !a.IsComplete).ToList();
        List<PBMission> list2 = MissionList.Where<PBMission>((a) => a.CurrTrigger < a.Trigger && !a.IsComplete).ToList();
        List<PBMission> list3 = MissionList.Where<PBMission>((a) => a.CurrTrigger >= a.Trigger && a.IsComplete).ToList();
        MissionList.Clear();
        MissionList.AddRange(list1);
        MissionList.AddRange(list2);
        MissionList.AddRange(list3);

        if (listMissionItem.Count <= 0)
        {
            scrollingView.Init(AssetPathConst.uiRootPath + "Mission/MissionItem");
            var listCell = scrollingView.Fill(MissionList.Count);

            UIMissionItem uIItem;
            for (int i = 0; i < listCell.Count; ++i)
            {
                uIItem = listCell[i] as UIMissionItem;
                listMissionItem.Add(uIItem);
            }
        }
        for (int i = 0; i < listMissionItem.Count; ++i)
        {
            listMissionItem[i].Setup(MissionList[i]);
        }
    }
}
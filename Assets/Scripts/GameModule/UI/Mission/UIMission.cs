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
    // 设置昵称
    public void FetchMission()
    {
        NetWorkHandler.GetDispatch().AddListener<P20_Response>(GameEvent.RECIEVE_P20_RESPONSE, OnRecvFetchMision);
        NetWorkHandler.RequestGetMissionList();

    }
    void OnRecvFetchMision<T>(T response)
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P20_RESPONSE);
        res = response as P20_Response;
        
        if (listMissionItem.Count <= 0)
        {
            scrollingView.Init(AssetPathConst.uiRootPath + "Mission/MissionItem");
            var listCell = scrollingView.Fill(res.MissionList.Count);

            UIMissionItem uIItem;
            for (int i = 0; i < listCell.Count; ++i)
            {
                uIItem = listCell[i] as UIMissionItem;
                listMissionItem.Add(uIItem);
            }
        }
        for (int i = 0; i < listMissionItem.Count; ++i)
        {
            listMissionItem[i].Setup( res.MissionList[i]);
        }
    }
}
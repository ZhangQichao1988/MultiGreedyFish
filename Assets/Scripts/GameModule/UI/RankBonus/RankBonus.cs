using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RankBonus : UIBase
{
    public Transform transContent = null;

    List<RankBonusItem> aryFishEditorItem = null;
    List<RankBonusDataInfo> rankBonusDataInfos;
    P16_Response p16_Response;

    public override void OnEnter(System.Object parms)
    {
        Refash();
    }
    public override void Init()
    {
        base.Init();
        NetWorkHandler.GetDispatch().AddListener<P16_Response>(GameEvent.RECIEVE_P16_RESPONSE, OnRecvGetGettedRankBonusIds);
        NetWorkHandler.RequestGetGettedRankBonusIds();

    }
    void OnRecvGetGettedRankBonusIds<T>(T response)
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P16_RESPONSE);
        p16_Response = response as P16_Response;

        aryFishEditorItem = new List<RankBonusItem>();

        GameObject go;
        RankBonusItem rankBonusItem;
        rankBonusDataInfos = RankBonusDataTableProxy.Instance.GetAll();
        foreach (var note in rankBonusDataInfos)
        {
            var asset = ResourceManager.LoadSync<GameObject>(Path.Combine(AssetPathConst.uiRootPath, "RankBonus/RankBonusItem"));
            go = GameObjectUtil.InstantiatePrefab(asset.Asset, transContent.gameObject);
            rankBonusItem = go.GetComponent<RankBonusItem>();
            aryFishEditorItem.Add(rankBonusItem);
        }
        Refash();

    }
    void Refash()
    {
        int playerTotalRankLevel = PlayerModel.Instance.GetTotalRankLevel();
        bool firstReach = false;
        RankBonusItem.Status status;

        foreach (var note in rankBonusDataInfos)
        {
            if (playerTotalRankLevel >= note.rankLevel)
            {
                status = p16_Response.GettedBonusIds.Contains(note.ID) ? RankBonusItem.Status.Getted : RankBonusItem.Status.NoGet;
            }
            else
            {
                if (!firstReach)
                {
                    firstReach = true;
                    status = RankBonusItem.Status.Next;
                }
                else
                {
                    status = RankBonusItem.Status.NoReach;
                }
            }
        }
    }
 }

using System;
using UnityEditor;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIPlayerRankingGetRewardDialog : UIBase
{
    public Text textTitle;
    public UIRewardCell uiRewardCell;
    //[SerializeField]
    //SimpleScrollingView scrollingView;

    public override void Init()
    {
        //scrollingView.Init(AssetPathConst.shopRewardCellPath);
    }

    public override void OnEnter(System.Object parms)
    {
        var rankReward = parms as PBRankReward;
        var batchData = RankingBatchDataTableProxy.Instance.GetDataById(rankReward.RankBatch);
        string rankStr = RankingRewardDataTableProxy.Instance.GetRankingStr(batchData.groupId, rankReward.Rank, rankReward.RankRate);
        textTitle.text = string.Format( LanguageDataTableProxy.GetText(630), rankStr);
        var items = RewardItemVo.FromList(rankReward.Content);
        PlayerModel.Instance.ProcessReward(items);
        UIHomeResource.Instance.UpdateAssets();
        uiRewardCell.UpdateData(items[0]);
        //var uiObjs = scrollingView.Fill(items.Count);
        //uiObjs.ForEach(cell=>{
        //    var idx = uiObjs.IndexOf(cell);
        //    cell.UpdateData(items[idx]);
        //});
    }

}

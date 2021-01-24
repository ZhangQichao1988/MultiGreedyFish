using System;
using UnityEditor;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIPlayerRankingRewardList : UIBase
{
    
    [SerializeField]
    SimpleScrollingView scrollingView;

    public override void Init()
    {
        scrollingView.Init(AssetPathConst.playerRankingRewardItemPrefabPath);
    }

    public override void OnEnter(System.Object parms)
    {
        var groupId = (int)parms;
        var listReward = RankingRewardDataTableProxy.Instance.GetRewardList(groupId);
        var uiObjs = scrollingView.Fill(listReward.Count);
        uiObjs.ForEach(cell=>{
            var idx = uiObjs.IndexOf(cell);
            cell.UpdateData(listReward[idx]);
        });
    }

    public void OnSure()
    {
        this.Close();
    }

}

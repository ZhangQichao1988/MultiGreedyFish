using System;
using UnityEditor;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIReward : UIBase
{
    
    [SerializeField]
    SimpleScrollingView scrollingView;

    public override void Init()
    {
        scrollingView.Init(AssetPathConst.shopRewardCellPath);
    }

    public override void OnEnter(System.Object parms)
    {
        var items = parms as List<RewardItemVo>;
        var uiObjs = scrollingView.Fill(items.Count);
        uiObjs.ForEach(cell=>{
            var idx = uiObjs.IndexOf(cell);
            cell.UpdateData(items[idx]);
        });
    }

    public void OnSure()
    {
        this.Close();
    }

}

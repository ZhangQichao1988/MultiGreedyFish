using System;
using UnityEditor;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIReward : UIBase
{
    public UIRewardCell rewardCell;

    public override void OnEnter(System.Object parms)
    {
        var items = parms as List<RewardItemVo>;
        rewardCell.UpdateData(items[0]);

    }

}

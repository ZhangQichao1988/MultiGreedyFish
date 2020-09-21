using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardMapVo
{
    public bool IsTreasure;

    public List<RewardItemVo> Content;

    public List<RewardItemVo> TreasureContent;

    public static RewardMapVo From(P11_Response pRes)
    {
        var rvo = new RewardMapVo();
        rvo.Content = RewardItemVo.FromList(pRes.Content);
        rvo.IsTreasure = pRes.IsTreasure;
        rvo.TreasureContent = RewardItemVo.FromList(pRes.TreaContent);

        return rvo;
    }
}
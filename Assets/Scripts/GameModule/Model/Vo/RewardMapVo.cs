using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardMapVo
{
    public bool IsTreasure;

    public List<RewardItemVo> Content;

    public List<RewardItemVo> TreasureContent;

    /// <summary>
    /// 普通购买
    /// </summary>
    /// <param name="pRes"></param>
    /// <returns></returns>
    public static RewardMapVo From(P11_Response pRes)
    {
        var rvo = new RewardMapVo();
        rvo.Content = RewardItemVo.FromList(pRes.Content);
        rvo.IsTreasure = pRes.IsTreasure;
        rvo.TreasureContent = RewardItemVo.FromList(pRes.TreaContent);

        if (rvo.IsTreasure && rvo.Content.Count <= 1)
        {
            rvo.Content = null;
        }

        return rvo;
    }

    /// <summary>
    /// 氪金
    /// </summary>
    /// <param name="pRes"></param>
    /// <returns></returns>
    public static RewardMapVo From(P13_Response pRes)
    {
        var rvo = new RewardMapVo();
        rvo.Content = RewardItemVo.FromList(pRes.Content);
        rvo.IsTreasure = false;

        return rvo;
    }
    public static RewardMapVo From(IList<ProductContent> pRes)
    {
        var rvo = new RewardMapVo();
        rvo.Content = RewardItemVo.FromList(pRes);
        rvo.IsTreasure = false;

        return rvo;
    }
}
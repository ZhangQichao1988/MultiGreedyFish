using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// 基表代理
/// 每张基表对应一个proxy
/// </summary>
public class RankBonusDataTableProxy : BaseDataTableProxy<RankBonusDataTable, RankBonusDataInfo, RankBonusDataTableProxy>
{

    public RankBonusDataTableProxy() : base("JsonData/RankBonusData") {}

    public float GetRankBonusProcess(int currentRankLevel)
    {
        var list = GetAll();
        if (list[list.Count - 1].rankLevel < currentRankLevel) { return 1f; }

        int preRankLevel = 0;
        int nextRankLevel = list[0].rankLevel;
        
        for (int i = list.Count - 1; i >= 0; --i)
        {
            if (list[i].rankLevel < currentRankLevel)
            {
                nextRankLevel = list[i + 1].rankLevel;
                preRankLevel = list[i].rankLevel;
                break;
            }
        }
        return (float)(currentRankLevel - preRankLevel) / (float)(nextRankLevel - preRankLevel);
    }

}
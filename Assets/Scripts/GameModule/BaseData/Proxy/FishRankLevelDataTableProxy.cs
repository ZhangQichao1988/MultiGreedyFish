using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// 基表代理
/// 每张基表对应一个proxy
/// </summary>
public class FishRankLevelDataTableProxy : BaseDataTableProxy<FishRankLevelDataTable, FishRankLevelDataInfo, FishRankLevelDataTableProxy>
{

    public FishRankLevelDataTableProxy() : base("JsonData/FishRankLevelData") {}

    public float GetFishRankLevelProcess(int currentRankLevel)
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
            }
        }
        return (float)(currentRankLevel - preRankLevel) / (float)(nextRankLevel - preRankLevel);
    }

    public void GetFishRankLevelData(int currentRankLevel, out FishRankLevelDataInfo currentRank, out FishRankLevelDataInfo nextRank)
    {
        var list = GetAll();
        if (list[list.Count - 1].rankLevel < currentRankLevel)
        {
            currentRank = list[list.Count - 1];
            nextRank = null;
        }

        for (int i = list.Count - 1; i >= 0; --i)
        {
            if (list[i].rankLevel <= currentRankLevel)
            {
                nextRank = list[i + 1];
                currentRank = list[i];
                return;
            }
        }
        Debug.LogError("FishRankLevelDataTableProxy.GetFishRankLevelData()_1");
        currentRank = null;
        nextRank = null;
    }

}
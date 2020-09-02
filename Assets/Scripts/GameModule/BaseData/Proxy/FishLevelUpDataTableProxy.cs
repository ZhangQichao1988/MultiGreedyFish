using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 基表代理
/// 每张基表对应一个proxy
/// </summary>
public class FishLevelUpDataTableProxy : BaseDataTableProxy<FishLevelUpDataTable, FishLevelUpDataInfo, FishLevelUpDataTableProxy>
{

    public FishLevelUpDataTableProxy() : base("JsonData/FishLevelUpData") {}

    public int GetFishAtk(int fishId, int level)
    {
        var fishData = FishDataTableProxy.Instance.GetDataById(fishId);
        return GetFishAtk(fishData, level);
    }
    public int GetFishAtk(FishDataInfo fishData, int level)
    {
        float upRate = ConfigTableProxy.Instance.GetDataByKey("FishLevelUpRate");
        return (int)(fishData.atk + fishData.atk * upRate * Mathf.Pow(level, 2));
    }
    public int GetFishHp(FishDataInfo fishData, int level)
    {
        float upRate = ConfigTableProxy.Instance.GetDataByKey("FishLevelUpRate");
        return (int)(fishData.life + fishData.life * upRate * Mathf.Pow(level, 2));
    }
}
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

    public int GetFishAtk(int fishId, int level, float battleLevel)
    {
        var fishData = FishDataTableProxy.Instance.GetDataById(fishId);
        return GetFishAtk(fishData, level, battleLevel);
    }
    public int GetFishAtk(FishDataInfo fishData, float level, float battleLevel)
    {
        --level;
        --battleLevel;
        float upRate = ConfigTableProxy.Instance.GetDataByKey("FishLevelUpRate");
        float atk = fishData.atk + fishData.atkAdd * level;
        return (int)(atk + atk * upRate * Mathf.Pow(battleLevel, 2));
    }
    public int GetFishHp(FishDataInfo fishData, float level, float battleLevel)
    {
        --level;
        --battleLevel;
        float life = fishData.life + fishData.lifeAdd * level;
        float upRate = ConfigTableProxy.Instance.GetDataByKey("FishLevelUpRate");
        return (int)(life + life * upRate * Mathf.Pow(battleLevel, 2));
    }
}
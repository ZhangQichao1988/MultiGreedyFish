using System;
using System.Collections.Generic;

[Serializable]
public class FishRankLevelDataTable : BaseDataTable<FishRankLevelDataInfo> {}


[Serializable]
public class FishRankLevelDataInfo : IQueryById
{
    public int rankLevel;
    public int getGold;
    public string rankIcon;
}
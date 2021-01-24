using System;

[Serializable]
public class RankBonusDataTable : BaseDataTable<RankBonusDataInfo> {}


[Serializable]
public class RankBonusDataInfo : IQueryById
{
    public int rankLevel;
    public string productContent;
    public string rankIcon;

    
}
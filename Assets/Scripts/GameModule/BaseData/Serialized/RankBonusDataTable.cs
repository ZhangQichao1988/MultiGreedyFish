using System;
using System.Collections.Generic;

[Serializable]
public class RankBonusDataTable : BaseDataTable<RankBonusDataInfo> {}


[Serializable]
public class RankBonusDataInfo : IQueryById
{
    public int rankLevel;
    public int itemId;
}
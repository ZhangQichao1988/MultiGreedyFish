using System;
using System.Collections.Generic;

[Serializable]
public class FishLevelUpDataTable : BaseDataTable<FishLevelUpDataInfo> {}


[Serializable]
public class FishLevelUpDataInfo : IQueryById
{
    public int useChip;
    public int useGold;
}
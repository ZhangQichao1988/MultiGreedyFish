using System;
using System.Collections.Generic;

[Serializable]
public class GoldPoolDataTable : BaseDataTable<GoldPoolDataInfo> {}


[Serializable]
public class GoldPoolDataInfo : IQueryById
{
    public int level;
    public int gainPreSec;
    public int maxGold;

}
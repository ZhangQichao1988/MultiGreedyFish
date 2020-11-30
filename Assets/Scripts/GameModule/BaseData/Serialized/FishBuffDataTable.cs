using System;
using System.Collections.Generic;

[Serializable]
public class FishBuffDataTable : BaseDataTable<FishBuffDataInfo> {}


[Serializable]
public class FishBuffDataInfo : IQueryById
{
    public string buffType;
    public string aryParam;
}
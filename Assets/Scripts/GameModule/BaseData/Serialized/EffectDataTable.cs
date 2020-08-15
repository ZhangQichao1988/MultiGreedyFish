using System;
using System.Collections.Generic;

[Serializable]
public class EffectDataTable : BaseDataTable<EffectDataInfo> {}


[Serializable]
public class EffectDataInfo : IQueryById
{
    public string prefabPath;
    public float duration;
    public int cacheCount;
}
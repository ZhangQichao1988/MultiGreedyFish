using System;
using System.Collections.Generic;

[Serializable]
public class FishSkillDataTable : BaseDataTable<FishSkillDataInfo> {}


[Serializable]
public class FishSkillDataInfo : IQueryById
{
    public int name;
    public string skillType;
    public int effectId;
    public string aryParam;
}
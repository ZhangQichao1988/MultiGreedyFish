using System;
using System.Collections.Generic;

[Serializable]
public class MissionActionDataTable : BaseDataTable<MissionActionData> {}


[Serializable]
public class MissionActionData : IQueryById
{
    public int desc;
}
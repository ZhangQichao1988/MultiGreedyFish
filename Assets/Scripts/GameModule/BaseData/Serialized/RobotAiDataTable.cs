using System;
using System.Collections.Generic;

[Serializable]
public class RobotAiDataTable : BaseDataTable<RobotAiDataInfo> {}


[Serializable]
public class RobotAiDataInfo : IQueryById
{
    public string aiType;
    public string aryParam;
}
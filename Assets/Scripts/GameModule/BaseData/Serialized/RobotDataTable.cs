using System;
using System.Collections.Generic;

[Serializable]
public class RobotDataTable : BaseDataTable<RobotDataInfo>{}


[Serializable]
public class RobotDataInfo : IQueryById
{
    public int fishId;
    public string name;
    public int aiId;
}
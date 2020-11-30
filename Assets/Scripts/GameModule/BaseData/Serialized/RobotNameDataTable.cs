using System;
using System.Collections.Generic;

[Serializable]
public class RobotNameDataTable : BaseDataTable<RobotNameDataInfo> {}


[Serializable]
public class RobotNameDataInfo : IQueryById
{
    public string name;
}
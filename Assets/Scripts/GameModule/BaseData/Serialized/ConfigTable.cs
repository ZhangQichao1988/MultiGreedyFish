using System;
using System.Collections.Generic;

[Serializable]
public class ConfigTable : BaseDataTable<ConfigInfo> {}


[Serializable]
public class ConfigInfo : IQueryById
{
    public string key;
    public float floatValue;
    public int intValue;
    public string stringValue;
}
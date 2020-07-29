using System;
using System.Collections.Generic;

[Serializable]
public class FunctionDataTable : BaseDataTable<FunctionData>{}


[Serializable]
public class FunctionData
{
    public int ID;
    public float Function;
    public string Desc;
}
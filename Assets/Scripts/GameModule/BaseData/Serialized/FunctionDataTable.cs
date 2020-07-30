using System;
using System.Collections.Generic;

[Serializable]
public class FunctionDataTable : BaseDataTable<FunctionData>{}


[Serializable]
public class FunctionData : IQueryById
{
    public float Function;
    public string Desc;
}
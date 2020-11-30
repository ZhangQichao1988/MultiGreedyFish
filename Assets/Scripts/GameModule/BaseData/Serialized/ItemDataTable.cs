using System;
using System.Collections.Generic;

[Serializable]
public class ItemDataTable : BaseDataTable<ItemData> {}


[Serializable]
public class ItemData : IQueryById
{
    public string type;
    public string resIcon;
    public int extra;
    public int rari;
}
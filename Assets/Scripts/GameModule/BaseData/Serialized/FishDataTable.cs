using System;
using System.Collections.Generic;

[Serializable]
public class FishDataTable : BaseDataTable<FishDataInfo>{}


[Serializable]
public class FishDataInfo
{
    public int ID;
    public string name;
    public int atk;
    public int life;
    public float moveSpeed;
    public int skillId;
}
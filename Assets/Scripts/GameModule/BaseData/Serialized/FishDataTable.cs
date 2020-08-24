using System;
using System.Collections.Generic;

[Serializable]
public class FishDataTable : BaseDataTable<FishDataInfo>{}


[Serializable]
public class FishDataInfo : IQueryById
{
    public string name;
    public string prefabPath;
    public int atk;
    public int life;
    public float moveSpeed;
    public int skillId;
    public int isPlayerFish;
}
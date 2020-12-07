using System;
using System.Collections.Generic;

[Serializable]
public class FishDataTable : BaseDataTable<FishDataInfo>{}


[Serializable]
public class FishDataInfo : IQueryById
{
    public int name;
    public int comment;
    public string prefabPath;
    public int atk;
    public int atkAdd;
    public int life;
    public int lifeAdd;
    public float moveSpeed;
    public int skillId;
    public int rare;
    public int isPlayerFish;
}
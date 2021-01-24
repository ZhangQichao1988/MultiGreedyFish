using System;
using System.Collections.Generic;

[Serializable]
public class RankingRewardDataTable : BaseDataTable<RankingRewardData> {}


[Serializable]
public class RankingRewardData : IQueryById
{
    public int ranking;
    public int percent;
    public string productContent;
    public int groupId;
}
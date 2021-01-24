using System;
using System.Collections.Generic;

[Serializable]
public class RankingBatchDataTable : BaseDataTable<RankingBatchData> {}


[Serializable]
public class RankingBatchData : IQueryById
{
    public int batch;
    public string startTime;
    public string endTime;
    public int rankType;
    public int groupId;
}
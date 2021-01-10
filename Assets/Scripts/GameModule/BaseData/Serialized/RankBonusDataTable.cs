using System;
using System.Collections.Generic;

[Serializable]
public class RankBonusDataTable : BaseDataTable<RankBonusDataInfo> {}


[Serializable]
public class RankBonusDataInfo : IQueryById
{
    public int rankLevel;
    public string productContent;
    public int itemId;
    public int amount;
    public string rankIcon;
    RankBonusDataInfo()
    {
        var obj = MiniJSON.Json.Deserialize(productContent) as Dictionary<string, object>;
        itemId = (int)obj["id"];
        amount = (int)obj["amount"];
    }
}
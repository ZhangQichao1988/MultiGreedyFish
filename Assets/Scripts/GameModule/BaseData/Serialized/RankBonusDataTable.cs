using System;
using System.Collections.Generic;

[Serializable]
public class RankBonusDataTable : BaseDataTable<RankBonusDataInfo> {}


[Serializable]
public class RankBonusDataInfo : IQueryById
{
    public int rankLevel;
    public string productContent;
    public string rankIcon;

    public class RankBonusItemDataInfo
    {
        public int id;
        public int amount;
        public RankBonusItemDataInfo(string productContent)
        {
            var obj = MiniJSON.Json.Deserialize(productContent) as List<object>;
            object obj0 = obj[0];
            var dic = obj0 as Dictionary<string, object>;

            id = Convert.ToInt32(dic["id"]);
            amount = Convert.ToInt32(dic["amount"]);
        }
    }
}
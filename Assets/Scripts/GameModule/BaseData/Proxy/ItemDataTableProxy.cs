using System;
using System.Collections.Generic;

public class ItemDataTableProxy : BaseDataTableProxy<ItemDataTable, ItemData, ItemDataTableProxy>
{
    static readonly int SHOP_LANG_ST_ID = 40000;

    public ItemDataTableProxy() : base("JsonData/ItemData") {}
    public string GetItemName(int id)
    {
        return LanguageDataTableProxy.GetText(SHOP_LANG_ST_ID + id);
    }
    static public List<RewardData> GetRewardList(string productContent)
    {
        List<RewardData> rewardDatas = new List<RewardData>();
        var obj = MiniJSON.Json.Deserialize(productContent) as List<object>;
        foreach (var note in obj)
        {
            object obj0 = note;
            var dic = obj0 as Dictionary<string, object>;
            rewardDatas.Add(new RewardData() { amount = Convert.ToInt32(dic["amount"]), id = Convert.ToInt32(dic["id"]) });
            //id = Convert.ToInt32(dic["id"]);
            //amount = Convert.ToInt32(dic["amount"]);
        }
        return rewardDatas;
    }
    public class RewardData
    {
        public int id;
        public int amount;
    }
}
using System;
using System.Collections.Generic;

[Serializable]
public class ShopBillingItemTable : BaseDataTable<ShopBillingItem> {}


[Serializable]
public class ShopBillingItem : IQueryById
{
    public string type;
    public string resIcon;
    public string refresh;
    public int price;
    public string platformId;
    public string platform;
    public string productContent;
    public string beginAt;
    public string endAt;

    public int limitAmount;
    public int rate;
}


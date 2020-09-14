using System;
using System.Collections.Generic;

[Serializable]
public class ShopBillingItemTable : BaseDataTable<ShopBillingItem> {}


[Serializable]
public class ShopBillingItem : IQueryById
{
    public string type;
    public string resIcon;
    public int price;
    public string platformId;
    public string productContent;
    public long beginAt;
    public long endAt;

    public int limitAmount;
}


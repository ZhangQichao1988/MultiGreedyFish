using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardItemVo
{
    public ProductContent pbContent;

    public ItemData masterDataItem;

    public string Name
    {
        get
        {
            return ItemDataTableProxy.Instance.GetItemName(masterDataItem.ID);
        }
    }

    public string ResIcon
    {
        get
        {
            return masterDataItem.resIcon;
        }
    }

    public int Amount
    {
        get
        {
            return pbContent.Amount;
        }
    }

    public static List<RewardItemVo> FromList(IList<ProductContent> items)
    {
        if (items == null)
        {
            return null;
        }
        var result = new List<RewardItemVo>();
        foreach (var item in items)
        {
            result.Add(FromItem(item));
        }

        return result;
    }   

    public static RewardItemVo FromItem(ProductContent item)
    {
        var result = new RewardItemVo();
        result.pbContent = item;
        result.masterDataItem = ItemDataTableProxy.Instance.GetDataById(item.ContentId);

        return result;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShopItemVo
{
    public static int SHOP_LANG_ST_ID = 30000;
    public ShopBillingProduct pbItems;

    public ShopBillingItem masterDataItem;

    public string Name
    {
        get
        {
            return LanguageDataTableProxy.GetText(SHOP_LANG_ST_ID + masterDataItem.ID);
        }
    }
    public string ResIcon
    {
        get
        {
            return masterDataItem.resIcon;
        }
    }

    public string Price
    {
        get
        {
            return masterDataItem.price.ToString();
        }
    }

    public PayType Paytype
    {
        get
        {
            return pbItems.PayType;
        }
    }

    public bool CanBuy
    {
        get
        {
            if (pbItems.LimitDetail == null)
            {
                return true;
            }
            else
            {
                return pbItems.LimitDetail.LimitedRemainingAmount > 0;
            }
        }
    }

    public void UpdateBuyNum(int addNum)
    {
        if (pbItems.LimitDetail != null)
        {
            pbItems.LimitDetail.LimitedRemainingAmount = Mathf.Max(0, pbItems.LimitDetail.LimitedRemainingAmount - addNum);
        }
    }

    public static List<ShopItemVo> FromList(IList<ShopBillingProduct> items)
    {
        var result = new List<ShopItemVo>();
        foreach (var item in items)
        {
            result.Add(FromItem(item));
        }

        return result;
    }   

    public static ShopItemVo FromItem(ShopBillingProduct item)
    {
        var result = new ShopItemVo();
        result.pbItems = item;
        result.masterDataItem = ShopBillingItemTableProxy.Instance.GetDataById(item.Id);

        return result;
    }
}
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ShopModel : BaseModel<ShopModel>
{
    public List<ShopItem> GoldItems;
    public List<ShopItem> DiamondItems;
    public List<ShopItem> OtherItems;
    public ShopModel() : base()
    {
        GoldItems = new List<ShopItem>(){
            new ShopItem(){ maxNum = 2, name = "金币宝箱", price = "12345"},
            new ShopItem(){ maxNum = 2, name = "金币宝箱2", price = "12345"},
            new ShopItem(){ maxNum = 2, name = "金币宝箱3", price = "12345"},
            new ShopItem(){ maxNum = 2, name = "金币宝箱4", price = "12345"},
            new ShopItem(){ maxNum = 2, name = "金币宝箱5", price = "12345"},
            new ShopItem(){ maxNum = 2, name = "金币宝箱6", price = "12345"},
            new ShopItem(){ maxNum = 2, name = "金币宝箱7", price = "12345"},
            new ShopItem(){ maxNum = 2, name = "金币宝箱8", price = "12345"},
            new ShopItem(){ maxNum = 2, name = "金币宝箱9", price = "12345"},
            new ShopItem(){ maxNum = 2, name = "金币宝箱10", price = "12345"}
        };

        DiamondItems = new List<ShopItem>(){
            new ShopItem(){ name = "钻石宝箱", price = "12345"},
            new ShopItem(){ name = "钻石宝箱2", price = "12345"},
            new ShopItem(){ name = "钻石宝箱3", price = "12345"},
            new ShopItem(){ name = "钻石宝箱4", price = "12345"},
            new ShopItem(){ name = "钻石宝箱5", price = "12345"},
            new ShopItem(){ maxNum = 2, name = "钻石宝箱6", price = "12345"},
            new ShopItem(){ maxNum = 2, name = "钻石宝箱7", price = "12345"},
            new ShopItem(){ maxNum = 2, name = "钻石宝箱8", price = "12345"},
            new ShopItem(){ maxNum = 2, name = "钻石宝箱9", price = "12345"},
            new ShopItem(){ maxNum = 2, name = "钻石宝箱10", price = "12345"},
            new ShopItem(){ maxNum = 2, name = "钻石宝箱11", price = "12345"},
            new ShopItem(){ maxNum = 2, name = "钻石宝箱12", price = "12345"},
            new ShopItem(){ maxNum = 2, name = "钻石宝箱13", price = "12345"},
            new ShopItem(){ maxNum = 2, name = "钻石宝箱14", price = "12345"},
            new ShopItem(){ maxNum = 2, name = "钻石宝箱15", price = "12345"}
        };

        OtherItems = new List<ShopItem>(){
            new ShopItem(){ name = "其他宝箱", price = "12345"},
            new ShopItem(){ maxNum = 2, name = "其他宝箱2", price = "12345"},
            new ShopItem(){ maxNum = 2, name = "其他宝箱3", price = "12345"},
            new ShopItem(){ maxNum = 2, name = "其他宝箱4", price = "12345"},
            new ShopItem(){ maxNum = 2, name = "其他宝箱5", price = "12345"},
            new ShopItem(){ maxNum = 2, name = "其他宝箱6", price = "12345"},
            new ShopItem(){ maxNum = 2, name = "其他宝箱7", price = "12345"}
        };
    }

    public List<ShopItem> GetShopItemByType(UIShop.ShopType type)
    {
        if (type == UIShop.ShopType.Gold)
        {
            return GoldItems;
        }
        else if (type == UIShop.ShopType.Diamond)
        {
            return DiamondItems;
        }
        else
        {
            return OtherItems;
        }
    }
}
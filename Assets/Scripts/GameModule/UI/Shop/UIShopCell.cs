
using UnityEngine;
using UnityEngine.UI;

public class UIShopCell : SimpleScrollingCell
{
    public Text text;

    public Image images;

    ShopItemVo shopData;


    public override void UpdateData(System.Object data)
    {
        shopData = data as ShopItemVo;
        text.text = shopData.Name;
        Debug.Log(AssetPathConst.itemIconPath + shopData.ResIcon);
        images.sprite = ResourceManager.LoadSync<Sprite>(AssetPathConst.itemIconPath + shopData.ResIcon).Asset;
    }

    public void OnCellClick()
    {
        if (shopData.CanBuy)
        {
            ShopModel.Instance.BuyItemNormal(shopData);
        }
        else
        {
            MsgBox.OpenTips("暂时不能购买此商品");
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class UIShopCell : SimpleScrollingCell
{
    public Text text;

    public Image buyIcon;

    public Text priceText;

    public Image images;

    ShopItemVo shopData;


    public override void UpdateData(System.Object data)
    {
        shopData = data as ShopItemVo;
        text.text = shopData.Name;
        images.sprite = ResourceManager.LoadSync<Sprite>(AssetPathConst.itemIconPath + shopData.ResIcon).Asset;
        priceText.text = shopData.Price;
        buyIcon.sprite = shopData.Paytype == PayType.Gold ? ResourceManager.LoadSync<Sprite>(AssetPathConst.texCommonPath + "UI_goldcoin").Asset :
                                                ResourceManager.LoadSync<Sprite>(AssetPathConst.texCommonPath + "UI_diamond").Asset;
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
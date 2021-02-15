
using UnityEngine;
using UnityEngine.UI;

public class UIShopCell : SimpleScrollingCell
{
    public Text text;

    public Image buyIcon;
    public Button buyBtn;

    public Text priceText;
    public Text originPriceText;
    public Text offText;

    public Image images;

    public GameObject banObject;


    ShopItemVo shopData;


    public override void UpdateData(System.Object data)
    {
        shopData = data as ShopItemVo;
        text.text = shopData.Name;
        AssetRef<Sprite> assRef = ResourceManager.LoadSync<Sprite>(AssetPathConst.itemIconPath + shopData.ResIcon);
        if (assRef != null)
        {
            images.sprite = assRef.Asset;
        }
        priceText.text = shopData.Paytype == PayType.Money ? shopData.BillingFormatPrice : shopData.Price.ToString();
        float proceOff = float.Parse(priceText.text);
        if (proceOff < shopData.OriginPrice)
        {
            proceOff = 1 - proceOff / shopData.OriginPrice;
            offText.text = (int)(proceOff * 100) + "%OFF";
            originPriceText.text = shopData.OriginPrice.ToString();
        }
        else
        {
            offText.gameObject.SetActive(false);
            originPriceText.gameObject.SetActive(false);
        }
        buyIcon.sprite = shopData.Paytype == PayType.Gold ? ResourceManager.LoadSync<Sprite>(AssetPathConst.texCommonPath + "UI_goldcoin").Asset :
                                                ResourceManager.LoadSync<Sprite>(AssetPathConst.texCommonPath + "UI_diamond").Asset;


        buyBtn.interactable = shopData.CanBuy;
        banObject.SetActive(!shopData.CanBuy);

    }

    public void OnCellClick()
    {
        if (shopData.CanBuy)
        {
            ShopModel.Instance.BuyItemNormal(shopData);
        }
        else
        {
            MsgBox.OpenTips("达到商品的购买上限");
        }
    }
}
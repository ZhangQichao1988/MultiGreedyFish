
using UnityEngine;
using UnityEngine.UI;

public class UIShopCell : SimpleScrollingCell
{
    public Text text;

    public Image buyIcon;
    public Button buyBtn;

    public Text priceText;
    public Text priceMoneyText;
    public Text priceRateText;
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

        int proceOff;
        if (shopData.Paytype == PayType.Money)
        {
            priceMoneyText.text = shopData.BillingFormatPrice;
            priceMoneyText.gameObject.SetActive(true);
            priceText.gameObject.SetActive(false);
            proceOff = int.Parse(shopData.BillingPrice);
            buyIcon.gameObject.SetActive(false);
        }
        else
        {
            priceText.text = shopData.Price.ToString();
            priceText.gameObject.SetActive(true);
            priceMoneyText.gameObject.SetActive(false);
            proceOff = shopData.Price;
            buyIcon.sprite = shopData.Paytype == PayType.Gold ? ResourceManager.LoadSync<Sprite>(AssetPathConst.texCommonPath + "UI_goldcoin").Asset :
                                        ResourceManager.LoadSync<Sprite>(AssetPathConst.texCommonPath + "UI_diamond").Asset;

        }

        if ( shopData.PriceRate < 1)
        {
            proceOff = (int)((float)proceOff / shopData.PriceRate);
            offText.text = (int)((1-shopData.PriceRate) * 100) + "%OFF";
            priceRateText.text = proceOff.ToString();
        }
        else
        {
            offText.gameObject.SetActive(false);
            priceRateText.gameObject.SetActive(false);
        }



        buyBtn.interactable = shopData.CanBuy;
        banObject.SetActive(!shopData.CanBuy);

    }

    public override void Refresh()
    {
        if (shopData != null)
        {
            buyBtn.interactable = shopData.CanBuy;
            banObject.SetActive(!shopData.CanBuy);
        }
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
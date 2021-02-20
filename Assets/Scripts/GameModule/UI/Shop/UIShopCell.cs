
using UnityEngine;
using UnityEngine.UI;

public class UIShopCell : SimpleScrollingCell
{
    public Text text;
    public Text textReward;

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

        string itemName = ItemDataTableProxy.GetItemName(shopData.pbItems.ProductContent[0].ContentId);
        textReward.text = string.Format( LanguageDataTableProxy.GetText(204), itemName, shopData.pbItems.ProductContent[0].Amount);

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
            proceOff = (int)float.Parse(shopData.BillingPrice);
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

        Debug.LogWarning("can buy item " + shopData.CanBuy);
        Debug.LogWarningFormat("pb info {0} {1} {2}", shopData.pbItems.Id, shopData.pbItems.PlatformProductId, shopData.pbItems.LimitDetail != null ? shopData.pbItems.LimitDetail.LimitedRemainingAmount.ToString() : "null");

        Refresh();

    }

    public override void Refresh()
    {
        if (shopData != null)
        {
            if (shopData.pbItems.LimitDetail != null)
            {
                text.text = string.Format(shopData.Name, shopData.pbItems.LimitDetail.LimitedRemainingAmount);
            }
            else
            {
                text.text = shopData.Name;
            }
            buyBtn.interactable = shopData.CanBuy;
            banObject.SetActive(!shopData.CanBuy);
        }
    }

    public virtual void OnCellClick()
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
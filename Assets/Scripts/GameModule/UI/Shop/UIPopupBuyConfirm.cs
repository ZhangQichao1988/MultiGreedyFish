using System;
using UnityEditor;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class UIPopupBuyConfirm : UIBase
{

    public Image itemIcon;
    public Image buyIcon;
    public Text priceText;
    public Text priceMoneyText;
    public Text textReward;

    ShopItemVo shopItem;

    static public void Open(ShopItemVo shopItem)
    {
        var ui = UIBase.Open<UIPopupBuyConfirm>("ArtResources/UI/Prefabs/PopupBuyConfirm", UILayers.POPUP);
        ui.Init(shopItem);
    }
    public void Init(ShopItemVo shopItem)
    {
        this.shopItem = shopItem;

        string itemName = ItemDataTableProxy.GetItemName(shopItem.pbItems.ProductContent[0].ContentId);
        textReward.text = string.Format(LanguageDataTableProxy.GetText(204), itemName, shopItem.pbItems.ProductContent[0].Amount);

        AssetRef<Sprite> assRef = ResourceManager.LoadSync<Sprite>(AssetPathConst.itemIconPath + shopItem.ResIcon);
        if (assRef != null)
        {
            itemIcon.sprite = assRef.Asset;
        }

        if (shopItem.Paytype == PayType.Money)
        {
            priceMoneyText.text = shopItem.BillingFormatPrice;
            priceMoneyText.gameObject.SetActive(true);
            priceText.gameObject.SetActive(false);
            buyIcon.gameObject.SetActive(false);
        }
        else
        {
            priceText.text = shopItem.Price.ToString();
            priceText.gameObject.SetActive(true);
            priceMoneyText.gameObject.SetActive(false);
            buyIcon.sprite = shopItem.Paytype == PayType.Gold ? ResourceManager.LoadSync<Sprite>(AssetPathConst.texCommonPath + "UI_goldcoin").Asset :
                                            ResourceManager.LoadSync<Sprite>(AssetPathConst.texCommonPath + "UI_diamond").Asset;

        }
    }
    public void OnClickBtn()
    {
        ShopModel.Instance.BuyItemNormal(shopItem);
        Close();
    }


}

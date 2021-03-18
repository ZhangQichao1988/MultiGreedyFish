
using NetWorkModule;
using UnityEngine;
using UnityEngine.UI;

public class UIShopCell : SimpleScrollingCell
{
    public Text textName;
    public Text textReward;

    public Image buyIcon;
    public Button buyBtn;

    public Text priceText;
    public Text priceMoneyText;
    public Text priceRateText;
    public Text offText;

    public Image images;

    public GameObject banObject;
    public Text textBan;

    long remainingTime = long.MaxValue;
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
                textName.text = string.Format(shopData.Name, shopData.pbItems.LimitDetail.LimitedRemainingAmount);
            }
            else
            {
                textName.text = shopData.Name;
            }
            buyBtn.interactable = shopData.CanBuy;
            banObject.SetActive(!shopData.CanBuy);
            if (!shopData.CanBuy)
            {   // 购买上限
                textBan.text = LanguageDataTableProxy.GetText(202);
            }
            
        }
    }

    public virtual void OnCellClick()
    {
        bool isEnough = true;
        UIPopupGotoResGet.ResType resType = UIPopupGotoResGet.ResType.DIAMOND;
        switch (shopData.Paytype)
        {
            case PayType.Diamond:
                isEnough = PlayerModel.Instance.player.Diamond >= shopData.Price;
                resType = UIPopupGotoResGet.ResType.DIAMOND;
                break;
            case PayType.Gold:
                isEnough = PlayerModel.Instance.player.Gold >= shopData.Price;
                resType = UIPopupGotoResGet.ResType.GOLD;
                break;
        }
        if (isEnough)
        {
            if (shopData.CanBuy)
            {
                UIPopupBuyConfirm.Open(shopData);
            }
            else
            {
                MsgBox.OpenTips("达到商品的购买上限");
            }
        }
        else
        {
            UIPopupGotoResGet.Open(resType, null, false);
        }
    }
    private void Update()
    {
        if (shopData != null && shopData.pbItems.IsRad && shopData.pbItems.RefreshType != "NONE")
        {
            long tmp = 0;
            switch (shopData.pbItems.RefreshType)
            {
                case "DAY":
                    tmp = Clock.GetRemaingingTimeWithDaily();
                    break;
                case "WEEK":
                    tmp = Clock.GetRemaingingTimeWithWeekly();
                    break;
                default:
                    tmp = 0;
                    break;
            }

            if (remainingTime >= tmp)
            {
                remainingTime = tmp;
                textName.text = string.Format(shopData.Name, Clock.GetRemainingTimeStr(remainingTime));

            }
            else
            {
                buyBtn.interactable = false;
                banObject.SetActive(true);
                textBan.text = LanguageDataTableProxy.GetText(207);
                textName.text = "";
            }
        }
    }
}
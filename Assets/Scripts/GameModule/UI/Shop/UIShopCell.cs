
using UnityEngine;
using UnityEngine.UI;

public class UIShopCell : SimpleScrollingCell
{
    public Text text;

    public Image images;

    ShopItem shopData;


    public override void UpdateData(System.Object data)
    {
        shopData = data as ShopItem;
        text.text = shopData.name;
    }

    public void OnCellClick()
    {
        Debug.Log("click buy");
    }
}
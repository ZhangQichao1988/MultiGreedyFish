using System;
using UnityEditor;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : UIBase
{
    [SerializeField]
    SimpleScrollingView scrollingView;

    private GameObject btnTagGold;
    private GameObject selTagGold;


    private GameObject btnTagDiamond;
    private GameObject selTagDiamond;


    private GameObject btnTagOther;
    private GameObject selTagOther;

    public enum ShopType
    {
        Gold,
        Diamond,
        Other
    }

    public override void Init()
    {
        btnTagGold = GameObjectUtil.FindChildGameObject(gameObject, "shop_tag/tag_gold");
        selTagGold = GameObjectUtil.FindChildGameObject(gameObject, "shop_tag/sel_gold");

        btnTagDiamond = GameObjectUtil.FindChildGameObject(gameObject, "shop_tag/tag_diamond");
        selTagDiamond = GameObjectUtil.FindChildGameObject(gameObject, "shop_tag/sel_diamond");

        btnTagOther = GameObjectUtil.FindChildGameObject(gameObject, "shop_tag/tag_other");
        selTagOther = GameObjectUtil.FindChildGameObject(gameObject, "shop_tag/sel_other");

        scrollingView.Init(AssetPathConst.shopItemCellPath);
    }

    public override void OnEnter(System.Object parms)
    {
        string sType = parms == null ? ShopType.Gold.ToString() : parms.ToString();
        sType = Enum.GetNames(typeof(ShopType)).Contains(sType) ? sType : ShopType.Gold.ToString();
        OnClickTag(sType);

    }

    void AllActiveBtn()
    {
        btnTagGold.SetActive(true);
        btnTagDiamond.SetActive(true);
        btnTagOther.SetActive(true);
    }

    void AllDeActiveSel()
    {
        selTagGold.SetActive(false);
        selTagDiamond.SetActive(false);
        selTagOther.SetActive(false);
    }

    public void OnClickTag(string tag)
    {
        ShopType tagType = (ShopType)Enum.Parse(typeof(ShopType), tag);
        AllActiveBtn();
        AllDeActiveSel();
        switch (tagType)
        {
            case ShopType.Gold:
                btnTagGold.SetActive(false);
                selTagGold.SetActive(true);
                break;
            case ShopType.Diamond:
                btnTagDiamond.SetActive(false);
                selTagDiamond.SetActive(true);
                break;
            case ShopType.Other:
                btnTagOther.SetActive(false);
                selTagOther.SetActive(true);
                break;
            default:
                break;
        }
        InitContent(tagType);
    }

    void InitContent(ShopType type)
    {
        //tmp data;
        var items = ShopModel.Instance.GetShopItemByType(type);
        var uiObjs = scrollingView.Fill(items.Count);
        uiObjs.ForEach(cell=>{
            var idx = uiObjs.IndexOf(cell);
            cell.UpdateData(items[idx]);
        });
    }

    private void OnDestroy()
    {
        
    }
}

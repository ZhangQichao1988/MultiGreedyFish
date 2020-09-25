using System;
using UnityEditor;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIShop : UIBase
{
    [SerializeField]
    SimpleScrollingView scrollingView;


    private GameObject btnTagDiamond;
    private GameObject selTagDiamond;


    private GameObject btnTagOther;
    private GameObject selTagOther;

    private Dictionary<ShopType, bool> requestFlag;


    public override void Init()
    {

        btnTagDiamond = GameObjectUtil.FindChildGameObject(gameObject, "shop_tag/tag_diamond");
        selTagDiamond = GameObjectUtil.FindChildGameObject(gameObject, "shop_tag/sel_diamond");

        btnTagOther = GameObjectUtil.FindChildGameObject(gameObject, "shop_tag/tag_other");
        selTagOther = GameObjectUtil.FindChildGameObject(gameObject, "shop_tag/sel_other");

        scrollingView.Init(AssetPathConst.shopItemCellPath);
    }

    public override void OnEnter(System.Object parms)
    {
        requestFlag = new Dictionary<ShopType, bool>();
        ShopType sType = parms == null ? ShopType.Other : (ShopType)Enum.Parse( typeof(ShopType), parms.ToString());
        OnClickTag(sType.ToString());
    }

    void OnGetted(System.Object type)
    {
        requestFlag[(ShopType)type] = true;
        InitContent((ShopType)type);
    }

    protected override void OnRegisterEvent()
    {
        ShopModel.Instance.AddListener(ShopEvent.ON_GETTED_SHOP_LIST, OnGetted);
    }

    protected override void OnUnRegisterEvent()
    {
        ShopModel.Instance.RemoveListener(ShopEvent.ON_GETTED_SHOP_LIST, OnGetted);
    }

    void AllActiveBtn()
    {
        btnTagDiamond.SetActive(true);
        btnTagOther.SetActive(true);
    }

    void AllDeActiveSel()
    {
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
            case ShopType.Pay:
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
        ShopModel.Instance.RequestShopItem(tagType, !requestFlag.ContainsKey(tagType));
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

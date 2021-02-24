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

    [SerializeField]
    UIShopCellAdvert cellAdvert;

    private GameObject btnTagDiamond;
    //private GameObject selTagDiamond;


    private GameObject btnTagOther;
    //private GameObject selTagOther;

    private Dictionary<ShopType, bool> requestFlag;


    public override void Init()
    {

        btnTagDiamond = GameObjectUtil.FindChildGameObject(gameObject, "shop_tag/tag_diamond");
        //selTagDiamond = GameObjectUtil.FindChildGameObject(gameObject, "shop_tag/sel_diamond");

        btnTagOther = GameObjectUtil.FindChildGameObject(gameObject, "shop_tag/tag_other");
        //selTagOther = GameObjectUtil.FindChildGameObject(gameObject, "shop_tag/sel_other");

        scrollingView.Init(AssetPathConst.shopItemCellPath);
        base.Init();
    }

    public override void OnEnter(System.Object parms)
    {
        requestFlag = new Dictionary<ShopType, bool>();
        ShopType sType = parms == null ? ShopType.Other : (ShopType)Enum.Parse( typeof(ShopType), parms.ToString());
        OnClickTag(sType.ToString());
    }

    void OnGetted(System.Object type)
    {
        Debug.Log("OnGetted " + (ShopType)type);
        requestFlag[(ShopType)type] = true;
        InitContent((ShopType)type);
    }

    void OnBuyEnd(System.Object vo)
    {
        //购买完成刷新cell
        if (scrollingView != null)
        {
            scrollingView.Refresh();
        }
    }

    protected override void OnRegisterEvent()
    {
        ShopModel.Instance.AddListener(ShopEvent.ON_GETTED_SHOP_LIST, OnGetted);
        ShopModel.Instance.AddListener(ShopEvent.ON_GETTED_ITEM, OnBuyEnd);
        
    }

    protected override void OnUnRegisterEvent()
    {
        ShopModel.Instance.RemoveListener(ShopEvent.ON_GETTED_SHOP_LIST, OnGetted);
        ShopModel.Instance.RemoveListener(ShopEvent.ON_GETTED_ITEM, OnBuyEnd);
    }

    //void AllActiveBtn()
    //{
    //    btnTagDiamond.SetActive(true);
    //    btnTagOther.SetActive(true);
    //}

    //void AllDeActiveSel()
    //{
    //    selTagDiamond.SetActive(false);
    //    selTagOther.SetActive(false);
    //}

    public void OnClickTag(string tag)
    {
        ShopType tagType = (ShopType)Enum.Parse(typeof(ShopType), tag);
        //AllActiveBtn();
        //AllDeActiveSel();
        switch (tagType)
        {
            case ShopType.Pay:
                btnTagDiamond.SetActive(true);
                btnTagOther.SetActive(false);
                //selTagDiamond.SetActive(true);
                break;
            case ShopType.Other:
                btnTagDiamond.SetActive(false);
                btnTagOther.SetActive(true);
                //selTagOther.SetActive(true);
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
        if (items.Count <= 0)
        {
            MsgBox.OpenTips("无法读取商店物品");
            scrollingView.Fill(0);
            return;
        }

        //// 添加看广告获钻石商品
        //var limitDetail = new LimitedDetail();
        //limitDetail.LimitAmount = ConfigTableProxy.Instance.GetDataById(3100).intValue;

        //ShopItemVo advertRewardItem = ShopItemVo.FromItem(new ShopBillingProduct() { LimitDetail = limitDetail,  });
        //advertRewardItem.Name = LanguageDataTableProxy.GetText(205);

        //items.Insert(0, advertRewardItem);
        if (type == ShopType.Other)
        {
            var cellObj = ResourceManager.LoadSync<GameObject>(AssetPathConst.shopItemAdvertCellPath).Asset;
            var go = GameObjectUtil.InstantiatePrefab(cellObj, scrollingView.content.gameObject);
            cellAdvert = go.GetComponent<UIShopCellAdvert>();
            cellAdvert.UpdateData(null);
        }

        var uiObjs = scrollingView.Fill(items.Count);

        uiObjs.ForEach(cell=>{
            var idx = uiObjs.IndexOf(cell);
            cell.UpdateData(items[idx]);
        });

        if (type == ShopType.Other)
        {
            scrollingView.InsertCell(cellAdvert, 0);
        }


    }

    private void OnDestroy()
    {
        
    }
}

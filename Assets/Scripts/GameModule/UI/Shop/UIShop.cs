using System;
using UnityEditor;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIShop : UIBase
{
    //[SerializeField]
    //SimpleScrollingView scrollingView;

    [SerializeField]
    UIShopCellAdvert cellAdvert;

    public ContentSizeFitter contentSizeFitter;

    public GameObject goCampaignRoot;
    public GameObject goCampaignSpace;
    public GameObject goTreasureRoot;
    public GameObject goTreasureSpace;
    public GameObject goNormalRoot;

    private GameObject btnTagDiamond;
    //private GameObject selTagDiamond;


    private GameObject btnTagOther;
    //private GameObject selTagOther;

    private Dictionary<ShopType, bool> requestFlag;
    private List<UIShopCell> uIShopCells;

    public override void Init()
    {

        uIShopCells = new List<UIShopCell>();
        btnTagDiamond = GameObjectUtil.FindChildGameObject(gameObject, "shop_tag/tag_diamond");
        //selTagDiamond = GameObjectUtil.FindChildGameObject(gameObject, "shop_tag/sel_diamond");

        btnTagOther = GameObjectUtil.FindChildGameObject(gameObject, "shop_tag/tag_other");
        //selTagOther = GameObjectUtil.FindChildGameObject(gameObject, "shop_tag/sel_other");

        //scrollingView.Init(AssetPathConst.shopItemCellPath);
        base.Init();
    }

    public override void OnEnter(System.Object parms)
    {
        // 为了自动设定size功能正常
        contentSizeFitter.gameObject.SetActive(false);

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
        foreach (var note in uIShopCells)
        {
            note.Refresh();
        }
        //if (scrollingView != null)
        //{
        //    scrollingView.Refresh();
        //}
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
        foreach (var note in uIShopCells)
        {
            Destroy(note.gameObject);
        }
        uIShopCells.Clear();
        var items = ShopModel.Instance.GetShopItemByType(type);


        if (items.Count <= 0)
        {
            MsgBox.OpenTips("无法读取商店物品");
            //scrollingView.Fill(0);
            return;
        }

        // 精选区域
        GameObject cellObj, tmp;
        UIShopCell uIShopCell;
        List<ShopItemVo> listTmps;

        //// 添加看广告获钻石商品
        if (type == ShopType.Other)
        {
            cellObj = ResourceManager.LoadSync<GameObject>(AssetPathConst.shopItemAdvertCellPath).Asset;
            tmp = GameObjectUtil.InstantiatePrefab(cellObj, goCampaignRoot);
            UIShopCellAdvert uIShopCellAdvert = tmp.GetComponent<UIShopCellAdvert>();
            uIShopCellAdvert.UpdateData(null);
            uIShopCells.Add(uIShopCellAdvert);

            // 精选商品
            listTmps = items.FindAll((a) => a.Priority < 100);
            cellObj = ResourceManager.LoadSync<GameObject>(AssetPathConst.shopItemCampaignCellPath).Asset;
            foreach (var note in listTmps)
            {
                tmp = GameObjectUtil.InstantiatePrefab(cellObj, goCampaignRoot);
                uIShopCell = tmp.GetComponent<UIShopCell>();
                uIShopCell.UpdateData(note);
                uIShopCells.Add(uIShopCell);
            }
            // 宝箱
            listTmps = items.FindAll((a) => a.Priority >= 100 && a.Priority < 200);
            cellObj = ResourceManager.LoadSync<GameObject>(AssetPathConst.shopItemTreasureCellPath).Asset;
            foreach (var note in listTmps)
            {
                tmp = GameObjectUtil.InstantiatePrefab(cellObj, goTreasureRoot);
                uIShopCell = tmp.GetComponent<UIShopCell>();
                uIShopCell.UpdateData(note);
                uIShopCells.Add(uIShopCell);
            }
            goCampaignSpace.SetActive(true);
            goTreasureSpace.SetActive(true);
        }
        else
        {
            goCampaignSpace.SetActive(false);
            goTreasureSpace.SetActive(false);
        }
        
        // 普通
        listTmps = items.FindAll((a) => a.Priority >= 200 && a.Priority < 300);
        cellObj = ResourceManager.LoadSync<GameObject>(AssetPathConst.shopItemCellPath).Asset;
        foreach (var note in listTmps)
        {
            tmp = GameObjectUtil.InstantiatePrefab(cellObj, goNormalRoot);
            uIShopCell = tmp.GetComponent<UIShopCell>();
            uIShopCell.UpdateData(note);
            uIShopCells.Add(uIShopCell);
        }
        // 为了自动设定size功能正常
        contentSizeFitter.gameObject.SetActive(true);

    }
    private void LateUpdate()
    {
        contentSizeFitter.enabled = false;
        contentSizeFitter.enabled = true;
    }

}

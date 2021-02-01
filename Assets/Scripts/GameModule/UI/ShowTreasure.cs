using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class ShowTreasure : UIBase
{
    public Text textItemName;
    public Text textItemCount;
    public Image imageItem;
    public GameObject goGrid;
    public Image imageRadiation;
    public Image imageRadiationAll;
    public Image imageLight;
    public Image imageCricle;
    public Image imageTreansure;


    int showItemIndex = 0;
    bool isShowAll = false;
    float showAllTime = 0f;
    float radiationAngle = 0f;
    List<ShowTreansureItem> listItems = new List<ShowTreansureItem>();
    Animator animator;
    RewardMapVo mapVo;

    public Image image;

    public override void OnEnter(object obj)
    {
        base.OnEnter(obj);

        imageRadiation.gameObject.SetActive(false);
        imageRadiationAll.gameObject.SetActive(false);

        animator = GetComponent<Animator>();
        mapVo = obj as RewardMapVo;
        string imgPath = mapVo.Content[0].masterDataItem.resIcon;
        imageTreansure.sprite = ResourceManager.LoadSync<Sprite>(AssetPathConst.itemIconPath + imgPath).Asset;

        // 物品一览
        GameObject tmp;
        ShowTreansureItem showTreansureItem;
        tmp = ResourceManager.LoadSync<GameObject>("ArtResources/UI/Prefabs/ShowTreansureItem").Asset;
        foreach (var note in mapVo.TreasureContent)
        {
            showTreansureItem = GameObjectUtil.InstantiatePrefab(tmp, goGrid, false).GetComponent<ShowTreansureItem>();
            listItems.Add(showTreansureItem);
            showTreansureItem.Init(note.masterDataItem.resIcon, note.Amount);
        }
    }
    public static void ShowGettedItem(RewardMapVo res)
    {
        if (res != null && res.TreasureContent.Count > 0)
        {
            UIBase.Open("ArtResources/UI/Prefabs/ShowTreansure", UILayers.POPUP, res);
        }
    }
    public void ShowItem()
    {
        if (showItemIndex >= mapVo.TreasureContent.Count)
        {
            imageRadiation.gameObject.SetActive(false);
            imageRadiationAll.gameObject.SetActive(true);
            if (!isShowAll)
            {
                imageCricle.color = new Color(0f, 0.65f, 0.88f, 1f);
                animator.SetTrigger("ShowAll");
                showItemIndex = 0;
                isShowAll = true;
            }
            else
            {
                Close();
            }
            return;
        }
        if (mapVo.TreasureContent[showItemIndex].masterDataItem.type != "cPiece")
        {   // 不是鱼碎片的话不显示放射特效
            imageRadiation.gameObject.SetActive(false);
            imageLight.gameObject.SetActive(false);
        }
        else
        {
            imageRadiation.gameObject.SetActive(true);
            var fishData = FishDataTableProxy.Instance.GetDataById(mapVo.TreasureContent[showItemIndex].masterDataItem.extra);
            Color color = new Color(0.12f, 0.24f, 0.63f, 0.7f);
            switch (fishData.rare)
            {
                case 3:
                    color = new Color(0.92f, 0.28f, 0.58f, 0.7f);
                    break;
                case 2:
                    color = new Color(0.92f, 0.87f, 0.24f, 0.7f);
                    break;
            }
            imageCricle.color = color;
            imageLight.color = color;
            imageLight.gameObject.SetActive(true);
        }
        animator.SetTrigger("ShowItem");
        textItemName.text = ItemDataTableProxy.Instance.GetItemName(mapVo.TreasureContent[showItemIndex].masterDataItem.ID);
        string imgPath = mapVo.TreasureContent[showItemIndex].masterDataItem.resIcon;
        imageItem.sprite = ResourceManager.LoadSync<Sprite>(AssetPathConst.itemIconPath + imgPath).Asset;
        textItemCount.text = "x" + mapVo.TreasureContent[showItemIndex].Amount;

        showItemIndex++;
    }
    private void Update()
    {
        radiationAngle += Time.deltaTime * 3f;
        image.material.SetFloat("_RealTime", radiationAngle * 0.05f);
        if (imageRadiation.gameObject.activeSelf) 
        {
            imageRadiation.transform.rotation = Quaternion.AngleAxis(radiationAngle, Vector3.forward);
        }
        if (imageRadiationAll.gameObject.activeSelf)
        {
            imageRadiationAll.transform.rotation = Quaternion.AngleAxis(radiationAngle, Vector3.forward);
        }

        if (!isShowAll || showItemIndex >= listItems.Count) { return; }
        showAllTime += Time.deltaTime;
        if (showAllTime > 0.2f)
        {
            showAllTime = 0f;
            listItems[showItemIndex].Show();
            ++showItemIndex;

        }
    }
}
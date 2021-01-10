using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RankBonusItem : MonoBehaviour
{
    public enum Status
    { 
        Getted,           // 已获取报酬
        Next,              // 下一个目标 
        NoGet,           // 达成目标但未获取报酬
        NoReach,       // 未达成目标
    };

    public Text textItemName;
    public Text textRankPoint;

    public Image imageItem;
    public Image imageRankIcon;

    public GameObject goTick;
    public GameObject textNextBonus;

    private Animator animator;
    private RankBonusDataInfo dataInfo;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void Refash(Status status)
    {
        switch (status)
        {
            case Status.Getted:
                goTick.SetActive(true);
                textNextBonus.SetActive(false);
                animator.enabled = false;
                imageItem.material.DisableKeyword("BOOLEAN_9AA876E6_ON");
                break;
            case Status.Next:
                goTick.SetActive(false);
                textNextBonus.SetActive(true);
                animator.enabled = false;
                imageItem.material.EnableKeyword("BOOLEAN_9AA876E6_ON");
                break;
            case Status.NoGet:
                goTick.SetActive(false);
                textNextBonus.SetActive(false);
                animator.enabled = true;
                imageItem.material.DisableKeyword("BOOLEAN_9AA876E6_ON");
                break;
            case Status.NoReach:
                goTick.SetActive(false);
                textNextBonus.SetActive(false);
                animator.enabled = false;
                imageItem.material.EnableKeyword("BOOLEAN_9AA876E6_ON");
                break;
        }
    }
    public void Init(RankBonusDataInfo rankBonusDataInfo, Status status)
    {
        dataInfo = rankBonusDataInfo;
        var itemData = ItemDataTableProxy.Instance.GetDataById(dataInfo.itemId);

        textItemName.text = ItemDataTableProxy.Instance.GetItemName(itemData.ID);
        textRankPoint.text = dataInfo.rankLevel.ToString();

        // 奖励物品图标显示
        string itemImgPath = string.Format(AssetPathConst.ItemPath, itemData.resIcon);
        var spAsset = ResourceManager.LoadSync<Sprite>(itemImgPath);
        Debug.Assert(spAsset != null, "Not found ItemImage:" + itemImgPath);
        imageItem.sprite = spAsset.Asset;

        // 段位图标
        spAsset = ResourceManager.LoadSync<Sprite>(string.Format(AssetPathConst.texCommonPath, dataInfo.rankIcon));
        imageRankIcon.sprite = spAsset.Asset;


    }
}

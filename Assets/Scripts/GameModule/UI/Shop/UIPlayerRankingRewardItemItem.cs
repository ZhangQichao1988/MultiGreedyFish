using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerRankingRewardItemItem : MonoBehaviour
{
    public Image imageItem;
    public Text textCount;
    public void Init(string resIcon, int count)
    {
        string itemImgPath = Path.Combine(AssetPathConst.itemIconPath, resIcon);
        var spAsset = ResourceManager.LoadSync<Sprite>(itemImgPath);
        Debug.Assert(spAsset != null, "Not found ItemImage:" + itemImgPath);
        imageItem.sprite = spAsset.Asset;

        textCount.text = count.ToString();
    }
}

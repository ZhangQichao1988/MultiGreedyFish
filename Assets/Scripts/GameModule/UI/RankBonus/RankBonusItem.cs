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

    public Animator animator;
    public RankBonusDataInfo dataInfo;
    private Material material;

    public void Refash(Status status)
    {
        if (material == null)
        {
            material = new Material(imageItem.material);
            imageItem.material = material;
        }
        switch (status)
        {
            case Status.Getted:
                goTick.SetActive(true);
                textNextBonus.SetActive(false);
                animator.enabled = false;
                imageItem.material.DisableKeyword("GRAY_SCALE");
                break;
            case Status.Next:
                goTick.SetActive(false);
                textNextBonus.SetActive(true);
                animator.enabled = false;
                imageItem.material.EnableKeyword("GRAY_SCALE");
                break;
            case Status.NoGet:
                goTick.SetActive(false);
                textNextBonus.SetActive(false);
                animator.enabled = true;
                imageItem.material.DisableKeyword("GRAY_SCALE");
                break;
            case Status.NoReach:
                goTick.SetActive(false);
                textNextBonus.SetActive(false);
                animator.enabled = false;
                imageItem.material.EnableKeyword("GRAY_SCALE");
                break;
        }
    }
    public void Init(RankBonusDataInfo rankBonusDataInfo)
    {
        dataInfo = rankBonusDataInfo;
        var itemDataInfo = new RankBonusDataInfo.RankBonusItemDataInfo(dataInfo.productContent);
        var itemData = ItemDataTableProxy.Instance.GetDataById(itemDataInfo.id);

        textItemName.text = ItemDataTableProxy.Instance.GetItemName(itemData.ID);
        textRankPoint.text = dataInfo.rankLevel.ToString();

        // 奖励物品图标显示
        string itemImgPath = Path.Combine(AssetPathConst.itemIconPath, itemData.resIcon);
        var spAsset = ResourceManager.LoadSync<Sprite>(itemImgPath);
        Debug.Assert(spAsset != null, "Not found ItemImage:" + itemImgPath);
        imageItem.sprite = spAsset.Asset;

        // 段位图标
        spAsset = ResourceManager.LoadSync<Sprite>(Path.Combine(AssetPathConst.texCommonPath, dataInfo.rankIcon));
        imageRankIcon.sprite = spAsset.Asset;
    }
    public void OnClickGetBonus()
    {
        NetWorkHandler.GetDispatch().AddListener<P16_Response>(GameEvent.RECIEVE_P16_RESPONSE, OnRecvGetBonus);
        NetWorkHandler.RequestGetRankBonus();

    }
    void OnRecvGetBonus<T>(T response)
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P16_RESPONSE);
        Debug.Log("OnRecvGetBonus!");
        var res = response as P16_Response;
        var rewardVO = RewardMapVo.From(res);
        var homeScene = BlSceneManager.GetCurrentScene() as HomeScene;
        PlayerModel.Instance.UpdateAssets(rewardVO);
        homeScene.OnGettedItemNormal(rewardVO);
    }
    private void OnDestroy()
    {
        Destroy(material);
    }
}

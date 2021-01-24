using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerRankingRewardItem : SimpleScrollingCell
{
    public Text textRanking;
    public GameObject goGrid;

    public override void UpdateData(System.Object data)
    {
        var rewardData = data as RankingRewardData;
        if (rewardData.ranking > 0)
        {
            textRanking.text = rewardData.ranking.ToString();
        }
        else
        {
            textRanking.text = rewardData.percent + "%";
        }

        var asset = ResourceManager.LoadSync<GameObject>(Path.Combine(AssetPathConst.uiRootPath, "PlayerRanking/PlayerRankingRewardItemItem"));
        var listReward = ItemDataTableProxy.GetRewardList(rewardData.productContent);
        GameObject tmp;
        UIPlayerRankingRewardItemItem uIPlayerRankingRewardItemItem;
        ItemData itemData;
        foreach (var note in listReward)
        {
            tmp = GameObjectUtil.InstantiatePrefab(asset.Asset, goGrid);
            uIPlayerRankingRewardItemItem = tmp.GetComponent<UIPlayerRankingRewardItemItem>();
            itemData = ItemDataTableProxy.Instance.GetDataById(note.id);
            uIPlayerRankingRewardItemItem.Init(itemData.resIcon, note.amount);
        }
        
    }
}
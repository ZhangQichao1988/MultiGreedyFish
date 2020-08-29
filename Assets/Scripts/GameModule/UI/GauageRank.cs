using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GauageRank : MonoBehaviour
{
    public Text textRankLevel;
    public Text textRank;
    public Image rankIcon;

    Slider sliderRankLevel;


    private void Awake()
    {
        sliderRankLevel = GetComponent<Slider>();
    }
    public void Refash(PBPlayerFishLevelInfo pBPlayerFishLevelInfo)
    {
        FishRankLevelDataInfo currentRankData, nextRankData;
        FishRankLevelDataTableProxy.Instance.GetFishRankLevelData(pBPlayerFishLevelInfo.RankLevel, out currentRankData, out nextRankData);
        sliderRankLevel.value = nextRankData == null ? 1f : (float)(pBPlayerFishLevelInfo.RankLevel - currentRankData.rankLevel) / (float)(nextRankData.rankLevel - currentRankData.rankLevel);
        textRankLevel.text = pBPlayerFishLevelInfo.RankLevel.ToString();
        textRank.text = currentRankData.ID.ToString();
        rankIcon.sprite = ResourceManager.LoadSync<Sprite>(Path.Combine(AssetPathConst.texCommonPath, currentRankData.rankIcon)).Asset;

    }
}

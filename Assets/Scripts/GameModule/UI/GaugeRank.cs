using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GaugeRank : MonoBehaviour
{
    public Text textRankLevel;
    public Text textRank;
    public Image rankIcon;

    public Slider sliderRankLevel;

    public int rankId;
    private void Awake()
    {
        sliderRankLevel = GetComponent<Slider>();
    }
    public void Refash(PBPlayerFishLevelInfo pBPlayerFishLevelInfo, bool applyIcon = true)
    {
        FishRankLevelDataInfo currentRankData, nextRankData;
        FishRankLevelDataTableProxy.Instance.GetFishRankLevelData(pBPlayerFishLevelInfo.RankLevel, out currentRankData, out nextRankData);
        sliderRankLevel.value = nextRankData == null ? 1f : (float)(pBPlayerFishLevelInfo.RankLevel - currentRankData.rankLevel) / (float)(nextRankData.rankLevel - currentRankData.rankLevel);
        textRankLevel.text = pBPlayerFishLevelInfo.RankLevel.ToString();
        textRank.text = currentRankData.ID.ToString();
        rankId = currentRankData.ID;
        rankIcon.gameObject.SetActive(pBPlayerFishLevelInfo.FishLevel > 0);
        if (rankIcon.IsActive() && applyIcon)
        {
            rankIcon.sprite = ResourceManager.LoadSync<Sprite>(Path.Combine(AssetPathConst.texCommonPath, currentRankData.rankIcon)).Asset;
        }
    }
}

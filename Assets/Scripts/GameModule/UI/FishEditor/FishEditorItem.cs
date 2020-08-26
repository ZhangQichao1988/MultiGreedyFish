using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FishEditorItem : MonoBehaviour
{
    Button button;

    public Image image;

    public Slider sliderRankLevel;
    public Text textRankLevel;
    public Text textRank;
    public Image rankIcon;

    public Text textFishName;
    public Text textFishChip;

    public Slider sliderFishLevel;
    public Text textFishLevel;

    FishDataInfo fishData;

    public PBPlayerFishLevelInfo pBPlayerFishLevelInfo;
    public void Refash(PBPlayerFishLevelInfo pBPlayerFishLevelInfo)
    {
        this.pBPlayerFishLevelInfo = pBPlayerFishLevelInfo;

        // 数据更新
        FishRankLevelDataInfo currentRankData, nextRankData;
        FishRankLevelDataTableProxy.Instance.GetFishRankLevelData(pBPlayerFishLevelInfo.RankLevel, out currentRankData, out nextRankData);
        sliderRankLevel.value = nextRankData == null ? 1f : (float)(pBPlayerFishLevelInfo.RankLevel - currentRankData.rankLevel) / (float)(nextRankData.rankLevel - currentRankData.rankLevel);
        textRankLevel.text = pBPlayerFishLevelInfo.RankLevel.ToString();
        textRank.text = currentRankData.ID.ToString();
        rankIcon.sprite = ResourceManager.LoadSync<Sprite>(Path.Combine( AssetPathConst.texCommonPath, currentRankData.rankIcon)).Asset;
        

        textFishName.text = fishData.name;

        var fishLevelUpData = FishLevelUpDataTableProxy.Instance.GetDataById(pBPlayerFishLevelInfo.FishLevel);
        sliderFishLevel.value = (float)pBPlayerFishLevelInfo.FishChip / (float)fishLevelUpData.useChip;
        textFishChip.text = string.Format("{0}/{1}", pBPlayerFishLevelInfo.FishChip, fishLevelUpData.useChip);
        textFishLevel.text = string.Format("Lv.{0}", pBPlayerFishLevelInfo.FishLevel) ;
    }
    public void Init(PBPlayerFishLevelInfo pBPlayerFishLevelInfo)
    {
        fishData = FishDataTableProxy.Instance.GetDataById(pBPlayerFishLevelInfo.FishId);
        Refash(pBPlayerFishLevelInfo);
        var spAsset = ResourceManager.LoadSync<Sprite>(string.Format(AssetPathConst.fishIconPath, pBPlayerFishLevelInfo.FishId));
        image.sprite = spAsset.Asset;

        button = GetComponent<Button>();
        button.onClick.AddListener(
            () =>
            {
                var homeScene = BlSceneManager.GetCurrentScene() as HomeScene;
                var ui = homeScene.GotoSceneUI("FishStatus") as UIFishStatus;
                ui.Setup(pBPlayerFishLevelInfo);
            });

    }
}

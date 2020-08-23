using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishEditorItem : MonoBehaviour
{
    Button button;

    public Image image;
    public Text textRankLevel;
    public Text textFishName;
    public Text textFishChip;
    public Text textFishLevel;

    FishDataInfo fishData;

    public PBPlayerFishLevelInfo pBPlayerFishLevelInfo;
    public void Refash(PBPlayerFishLevelInfo pBPlayerFishLevelInfo)
    {
        this.pBPlayerFishLevelInfo = pBPlayerFishLevelInfo;

        // 数据更新
        textRankLevel.text = pBPlayerFishLevelInfo.RankLevel.ToString();
        textFishName.text = fishData.name;

        var fishLevelUpData = FishLevelUpDataTableProxy.Instance.GetDataById(pBPlayerFishLevelInfo.FishLevel);
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

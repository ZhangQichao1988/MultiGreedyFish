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

    public GauageRank gauageRank;
    public GauageLevel gauageLevel;

    public Text textRankLevel;
    public Text textRank;
    public Image rankIcon;

    public Text textFishName;
    public Text textFishLevel;

    FishDataInfo fishData;

    public PBPlayerFishLevelInfo pBPlayerFishLevelInfo;
    public void Refash(PBPlayerFishLevelInfo pBPlayerFishLevelInfo)
    {
        this.pBPlayerFishLevelInfo = pBPlayerFishLevelInfo;

        gauageRank.Refash(pBPlayerFishLevelInfo);
        gauageLevel.Refash(pBPlayerFishLevelInfo);

        textFishName.text = LanguageDataTableProxy.GetText( fishData.name );
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

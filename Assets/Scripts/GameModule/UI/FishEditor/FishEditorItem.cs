using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishEditorItem : MonoBehaviour
{
    public Image image;
    public Button button;

    public PBPlayerFishLevelInfo pBPlayerFishLevelInfo;
    public void Refash(PBPlayerFishLevelInfo pBPlayerFishLevelInfo)
    {
        this.pBPlayerFishLevelInfo = pBPlayerFishLevelInfo;

        // 数据更新

    }
    public void Init(PBPlayerFishLevelInfo pBPlayerFishLevelInfo)
    {
        Refash(pBPlayerFishLevelInfo);
        var spAsset = ResourceManager.LoadSync<Sprite>(string.Format(AssetPathConst.fishIconPath, pBPlayerFishLevelInfo.FishId));
        image = GetComponent<Image>();
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

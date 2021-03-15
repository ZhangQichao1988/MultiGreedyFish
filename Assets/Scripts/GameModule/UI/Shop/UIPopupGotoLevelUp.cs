using System;
using UnityEditor;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TimerModule;

public class UIPopupGotoLevelUp : UIBase
{
    public Image imageFishIcon;
    public Text textFishLv;

    public Image imageFishIconAfter;
    public Text textFishLvAfter;

    PBPlayerFishLevelInfo levelInfo;
    static public void Open()
    {
        var ui = UIBase.Open<UIPopupGotoLevelUp>("ArtResources/UI/Prefabs/PopupGotoLevelUp", UILayers.POPUP);
        ui.Setup();
        
    }
    public void Setup()
    {
        levelInfo = PlayerModel.Instance.GetCurrentPlayerFishLevelInfo();
        imageFishIcon.sprite = ResourceManager.LoadSync<Sprite>(string.Format(AssetPathConst.fishIconPath, levelInfo.FishId)).Asset;
        imageFishIconAfter.sprite = imageFishIcon.sprite;

        textFishLv.text = string.Format(LanguageDataTableProxy.GetText(800), levelInfo.FishLevel);
        textFishLvAfter.text = string.Format(LanguageDataTableProxy.GetText(800), levelInfo.FishLevel + 1);
        //isShowAdvert.isOn = false;
        //isShowAdvert.onValueChanged.AddListener(isOn =>
        //{
        //    AppConst.NotShowAdvert = isOn ? 1 : 0;
        //    UIHome.Instance.SetShowAdvertSwitch();
        //});
    }

    public void OnBtn()
    {
        var homeScene = BlSceneManager.GetCurrentScene() as HomeScene;
        var ui = homeScene.GotoSceneUI("FishStatus") as UIFishStatus;
        ui.Setup(levelInfo);
        Close();
    }
}

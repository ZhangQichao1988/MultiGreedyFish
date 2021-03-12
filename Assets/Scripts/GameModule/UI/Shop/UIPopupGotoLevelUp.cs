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
    public Image imageReward;
    public Text textReward;

    public Image imageRewardDouble;
    public Text textRewardDouble;


    static public void Open()
    {
        var ui = UIBase.Open<UIPopupGotoLevelUp>("ArtResources/UI/Prefabs/PopupGotoLevelUp", UILayers.POPUP);
        ui.Setup();
    }
    public void Setup()
    {
        var fishLvInfo = PlayerModel.Instance.GetCurrentPlayerFishLevelInfo();
        //imageReward.sprite = ResourceManager.LoadSync<Sprite>(AssetPathConst.itemIconPath + resIcon).Asset;
        //imageRewardDouble.sprite = ResourceManager.LoadSync<Sprite>(AssetPathConst.itemIconPath + resIcon).Asset;

        //textReward.text = "x" + rewardData.amount;
        //textRewardDouble.text = "x" + rewardData.amount * 2;
        //isShowAdvert.isOn = false;
        //isShowAdvert.onValueChanged.AddListener(isOn =>
        //{
        //    AppConst.NotShowAdvert = isOn ? 1 : 0;
        //    UIHome.Instance.SetShowAdvertSwitch();
        //});
    }

    public void OnNormal()
    {
        this.Close();
    }
    public void OnDouble()
    {
        Intro.Instance.AdsController.OnAdRewardGetted = ()=>{
            LoadingMgr.Show(LoadingMgr.LoadingType.Repeat);
            TimerManager.AddTimer((int)eTimerType.RealTime, AdsController.RewardWaitTime, (obj)=>{
                LoadingMgr.Hide(LoadingMgr.LoadingType.Repeat);
                this.Close();
            }, null);
        };
    }
}

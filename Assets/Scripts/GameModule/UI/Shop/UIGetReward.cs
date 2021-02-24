using System;
using UnityEditor;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TimerModule;

public class UIGetReward : UIBase
{
    public Image imageReward;
    public Text textReward;

    public Image imageRewardDouble;
    public Text textRewardDouble;

    public Toggle isShowAdvert;

    public string customData;

    private Action<bool> normalCb;

    static public void Open(ItemDataTableProxy.RewardData rewardData, string admobCustom, Action<bool> callback)
    {
        var ui = UIBase.Open<UIGetReward>("ArtResources/UI/Prefabs/GetReward", UILayers.POPUP);
        ui.Init(rewardData, admobCustom, callback);
    }
    public void Init(ItemDataTableProxy.RewardData rewardData, string customData, Action<bool> callback)
    {
        this.normalCb = callback;
        this.customData = customData;
        string resIcon = ItemDataTableProxy.Instance.GetDataById(rewardData.id).resIcon;
        imageReward.sprite = ResourceManager.LoadSync<Sprite>(AssetPathConst.itemIconPath + resIcon).Asset;
        imageRewardDouble.sprite = ResourceManager.LoadSync<Sprite>(AssetPathConst.itemIconPath + resIcon).Asset;

        textReward.text = "x" + rewardData.amount;
        textRewardDouble.text = "x" + rewardData.amount * 2;
        isShowAdvert.isOn = false;
        isShowAdvert.onValueChanged.AddListener(isOn =>
        {
            AppConst.NotShowAdvert = isOn ? 1 : 0;
            PlayerPrefs.SetInt(AppConst.PlayerPrefabsOptionIsShowAdvert, AppConst.NotShowAdvert);
        });
    }

    public void OnNormal()
    {
        this.normalCb(false);
        this.Close();
    }
    public void OnDouble()
    {
        Intro.Instance.AdsController.OnAdRewardGetted = ()=>{
            LoadingMgr.Show(LoadingMgr.LoadingType.Repeat);
            TimerManager.AddTimer((int)eTimerType.RealTime, AdsController.RewardWaitTime, (obj)=>{
                LoadingMgr.Hide(LoadingMgr.LoadingType.Repeat);
                this.normalCb(true);
                this.Close();
            }, null);
        };
        Intro.Instance.AdsController.Show(this.customData);
    }
}

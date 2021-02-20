using System;
using UnityEditor;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class UIGetReward : UIBase
{
    public Image imageReward;
    public Text textReward;

    public Image imageRewardDouble;
    public Text textRewardDouble;

    public Toggle isShowAdvert;

    private Action normalCb;
    private Action doubleCb;

    static public void Open(ItemDataTableProxy.RewardData rewardData, Action normalCb, Action doubleCb)
    {
        var ui = UIBase.Open<UIGetReward>("ArtResources/UI/Prefabs/GetReward", UILayers.POPUP);
        ui.Init(rewardData, normalCb, doubleCb);
    }
    public void Init(ItemDataTableProxy.RewardData rewardData, Action normalCb, Action doubleCb)
    {
        this.normalCb = normalCb;
        this.doubleCb = doubleCb;
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
        normalCb();
        this.Close();
    }
    public void OnDouble()
    {
        doubleCb();
        this.Close();
    }
}

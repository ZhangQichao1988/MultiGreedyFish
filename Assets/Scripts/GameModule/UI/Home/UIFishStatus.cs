using UnityEngine;
using System.Collections.Generic;
using Google.Protobuf;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using System;

public class UIFishStatus : UIBase
{
    public FishStatusFishControl fishControl;
    public GauageLevel gauageLevel;
    public GauageRank gauageRank;
    public Text textFishName;
    public Text textFishComment;
    public Text textFishSkillName;
    public Text textLevelupUseGold;

    public GameObject[] goListAtkValue;
    public GameObject[] goListHpValue;
    public GameObject[] goListSpdValue;

    private PBPlayerFishLevelInfo playerFishLevelInfo;

    public void Setup(PBPlayerFishLevelInfo playerFishLevelInfo)
    {
        if (this.playerFishLevelInfo == null || this.playerFishLevelInfo.FishId != playerFishLevelInfo.FishId)
        { CreateFishModel(playerFishLevelInfo.FishId); }

        this.playerFishLevelInfo = playerFishLevelInfo;
        var fishData = FishDataTableProxy.Instance.GetDataById(playerFishLevelInfo.FishId);
        textFishName.text = LanguageDataTableProxy.GetText(fishData.name);
        textFishComment.text = LanguageDataTableProxy.GetText(fishData.comment);

        // 属性
        float rate;
        int intRate;
        rate = Mathf.InverseLerp(0f, ConfigTableProxy.Instance.GetDataByKey("FishAtkMax"), fishData.atk);
        intRate = Mathf.Clamp((int)(rate * 5) + 1, 1, 5);
        for (int i = 0; i < 5; ++i)
        {
            goListAtkValue[i].SetActive(i < intRate);
        }

        rate = Mathf.InverseLerp(0f, ConfigTableProxy.Instance.GetDataByKey("FishHpMax"), fishData.life);
        intRate = Mathf.Clamp((int)(rate * 5) + 1, 1, 5);
        for (int i = 0; i < 5; ++i)
        {
            goListHpValue[i].SetActive(i < intRate);
        }

        rate = Mathf.InverseLerp(0f, ConfigTableProxy.Instance.GetDataByKey("FishSpdMax"), fishData.moveSpeed);
        intRate = Mathf.Clamp((int)(rate * 5) + 1, 1, 5);
        for (int i = 0; i < 5; ++i)
        {
            goListSpdValue[i].SetActive(i < intRate);
        }


        var fishSkillData = FishSkillDataTableProxy.Instance.GetDataById(fishData.skillId);
        textFishSkillName.text = LanguageDataTableProxy.GetText(fishSkillData.name);

        var fishLevelData = FishLevelUpDataTableProxy.Instance.GetDataById(playerFishLevelInfo.FishLevel);
        textLevelupUseGold.text = fishLevelData.useGold.ToString();

        gauageLevel.Refash(playerFishLevelInfo);
        gauageRank.Refash(playerFishLevelInfo);
    }
    private void CreateFishModel(int fishId)
    {
        var fishBaseData = FishDataTableProxy.Instance.GetDataById(fishId);
        var asset = ResourceManager.LoadSync(Path.Combine(AssetPathConst.fishPrefabRootPath + fishBaseData.prefabPath), typeof(GameObject));
        GameObject go =  GameObjectUtil.InstantiatePrefab(asset.Asset as GameObject, fishControl.gameObject);
        fishControl.SetFishModel(go);
    }

    public void OnClickFishSelect()
    {
        NetWorkHandler.GetDispatch().AddListener<P6_Response>(GameEvent.RECIEVE_P6_RESPONSE, OnRecvFishSelect);
        NetWorkHandler.RequestFightFishSet(playerFishLevelInfo.FishId);
    }

    void OnRecvFishSelect<T>(T response)
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P6_RESPONSE);
        var realResponse = response as P6_Response;
        PlayerModel.Instance.player.FightFish = playerFishLevelInfo.FishId;
        ((HomeScene)BlSceneManager.GetCurrentScene()).GotoSceneUI("Home");
    }

    public void OpenLevelUpDialog()
    {
        UIBase.Open<UIFishLevelUp>(Path.Combine( AssetPathConst.uiRootPath, "PopupFishLevelUp"), UILayers.POPUP);
    }
}
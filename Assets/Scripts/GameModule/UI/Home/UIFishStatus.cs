using UnityEngine;
using System.Collections.Generic;
using Google.Protobuf;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using System;

public class UIFishStatus : UIBase
{
    public Button btnSelectFish;
    public Text textSelectFish;

    public Button btnLvUp;

    public FishStatusFishControl fishControl;
    public GaugeLevel gauageLevel;
    public GaugeRank gauageRank;
    public Text textFishName;
    public Text textFishComment;
    public Text textFishSkillName;
    public Text textLevelupUseGold;
    public Text textLevelupUseChip;

    public Text textCurrentFishLevel;
    public Text textCurrentFishLevelEffect;

    public Text textLvUpBtn;

    public GameObject goLevelUpRoot;
    public GameObject goTextMaxLevel;

    public Animator animatorLvupEffect;

    public GameObject goNew;

    public GameObject[] goListAtkValue;
    public GameObject[] goListHpValue;
    public GameObject[] goListSpdValue;

    private PBPlayerFishLevelInfo playerFishLevelInfo;
    private int showFishLevel;

    public override void OnEnter(System.Object obj)
    {
        fishControl.OnEnter();
    }
        public void Setup(PBPlayerFishLevelInfo playerFishLevelInfo)
    {
        if (this.playerFishLevelInfo == null || this.playerFishLevelInfo.FishId != playerFishLevelInfo.FishId)
        { fishControl.CreateFishModel(playerFishLevelInfo.FishId); }

        this.playerFishLevelInfo = playerFishLevelInfo;
        var fishData = FishDataTableProxy.Instance.GetDataById(playerFishLevelInfo.FishId);

        textFishName.text = LanguageDataTableProxy.GetText(fishData.name);
        textFishComment.text = LanguageDataTableProxy.GetText(fishData.comment);

        // 属性
        float rate;
        int intRate;
        rate = Mathf.InverseLerp(   ConfigTableProxy.Instance.GetDataById(2000).intValue,
                                                    ConfigTableProxy.Instance.GetDataById(2001).intValue, 
                                                    fishData.atkAdd);
        intRate = Mathf.Clamp((int)(rate * 5) + 1, 1, 5);
        for (int i = 0; i < 5; ++i)
        {
            goListAtkValue[i].SetActive(i < intRate);
        }

        rate = Mathf.InverseLerp(   ConfigTableProxy.Instance.GetDataById(2002).intValue,
                                                    ConfigTableProxy.Instance.GetDataById(2003).intValue, 
                                                    fishData.lifeAdd);
        intRate = Mathf.Clamp((int)(rate * 5) + 1, 1, 5);
        for (int i = 0; i < 5; ++i)
        {
            goListHpValue[i].SetActive(i < intRate);
        }

        rate = Mathf.InverseLerp(   ConfigTableProxy.Instance.GetDataById(2004).floatValue,
                                                    ConfigTableProxy.Instance.GetDataById(2005).floatValue, 
                                                    fishData.moveSpeed);
        intRate = Mathf.Clamp((int)(rate * 5) + 1, 1, 5);
        for (int i = 0; i < 5; ++i)
        {
            goListSpdValue[i].SetActive(i < intRate);
        }


        var fishSkillData = FishSkillDataTableProxy.Instance.GetDataById(fishData.skillId);
        textFishSkillName.text = LanguageDataTableProxy.GetText(fishSkillData.name);

        var fishLevelData = FishLevelUpDataTableProxy.Instance.GetDataById(playerFishLevelInfo.FishLevel);

        if (fishLevelData.useGold < 0)
        {
            goLevelUpRoot.SetActive(false);
            goTextMaxLevel.SetActive(true);
            btnLvUp.interactable = false;
        }
        else
        {
            goLevelUpRoot.SetActive(true);
            goTextMaxLevel.SetActive(false);
            btnLvUp.interactable = true;
            textLevelupUseGold.text = fishLevelData.useGold.ToString();
            textLevelupUseChip.text = fishLevelData.useChip.ToString();

            goNew.SetActive(PlayerModel.Instance.player.Gold >= fishLevelData.useGold && playerFishLevelInfo.FishChip >= fishLevelData.useChip);

        }

        bool canSelectFish = playerFishLevelInfo.FishLevel > 0;
        textLvUpBtn.text = LanguageDataTableProxy.GetText(canSelectFish ? 15 : 18);
        btnSelectFish.interactable = canSelectFish;
        textSelectFish.text = LanguageDataTableProxy.GetText(canSelectFish ? 16 : 17);

        showFishLevel = playerFishLevelInfo.FishLevel;
        textCurrentFishLevel.text = canSelectFish ? 
                                                    string.Format(LanguageDataTableProxy.GetText(10), showFishLevel) :
                                                    LanguageDataTableProxy.GetText(17);
        textCurrentFishLevelEffect.text = textCurrentFishLevel.text;
        gauageLevel.Refash(playerFishLevelInfo);
        gauageRank.Refash(playerFishLevelInfo);

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
        var uiFishLevelUp = UIBase.Open<UIFishLevelUp>(Path.Combine( AssetPathConst.uiRootPath, "PopupFishLevelUp"), UILayers.POPUP);
        uiFishLevelUp.Setup(playerFishLevelInfo, ()=> { animatorLvupEffect.SetTrigger("show"); });
    }
    public void Reflash()
    {
        Setup(PlayerModel.Instance.GetPlayerFishLevelInfo(playerFishLevelInfo.FishId));
    }
}
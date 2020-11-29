using UnityEngine;
using System.Collections.Generic;
using Google.Protobuf;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using System;

public class UIFishLevelUp : UIBase
{

    private PBPlayerFishLevelInfo playerFishLevelInfo;

    public Text textLvUp;
    public Text textTitle;
    public Text textLevelupUseGold;

    public Text textAtkValue;
    public Text textAtkPlus;

    public Text textHpValue;
    public Text textHpPlus;

    public void Setup(PBPlayerFishLevelInfo playerFishLevelInfo)
    {
        this.playerFishLevelInfo = playerFishLevelInfo;
        var fishLevelData = FishLevelUpDataTableProxy.Instance.GetDataById(playerFishLevelInfo.FishLevel);
        textLevelupUseGold.text = fishLevelData.useGold.ToString();

        bool isLvUp = playerFishLevelInfo.FishLevel > 0;
        textTitle.text = isLvUp ? 
                                string.Format(LanguageDataTableProxy.GetText(50), playerFishLevelInfo.FishLevel + 1) :
                                LanguageDataTableProxy.GetText(51);
        textLvUp.text = LanguageDataTableProxy.GetText(isLvUp ? 15 : 18); 

        int currentValue, plus;
        var fishData = FishDataTableProxy.Instance.GetDataById(playerFishLevelInfo.FishId);
        // 攻击力
        currentValue = FishLevelUpDataTableProxy.Instance.GetFishAtk(fishData, playerFishLevelInfo.FishLevel);
        textAtkValue.text = currentValue.ToString();
        plus = FishLevelUpDataTableProxy.Instance.GetFishAtk(fishData, playerFishLevelInfo.FishLevel + 1) - currentValue;
        textAtkPlus.text = "+" + plus.ToString();
        // 体力
        currentValue = FishLevelUpDataTableProxy.Instance.GetFishHp(fishData, playerFishLevelInfo.FishLevel);
        textHpValue.text = currentValue.ToString();
        plus = FishLevelUpDataTableProxy.Instance.GetFishHp(fishData, playerFishLevelInfo.FishLevel + 1) - currentValue;
        textHpPlus.text = "+" + plus.ToString();

    }

    public void OnClickFishLevelUp()
    {
        NetWorkHandler.GetDispatch().AddListener<P7_Response>(GameEvent.RECIEVE_P7_RESPONSE, OnRecvFishLevelUp);
        NetWorkHandler.RequesFishLevelUp(playerFishLevelInfo.FishId);
    }

    void OnRecvFishLevelUp<T>(T response)
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P7_RESPONSE);
        var realResponse = response as P7_Response;
        PlayerModel.Instance.SetPlayerFishLevelInfo(playerFishLevelInfo.FishId, realResponse.FishInfo);
        Close();
    }

}
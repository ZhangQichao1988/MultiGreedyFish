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
    private FishLevelUpDataInfo dataInfo;

    public Text textLvUp;
    public Text textTitle;
    public Text textLevelupUseGold;

    public Text textAtkValue;
    public Text textAtkPlus;

    public Text textHpValue;
    public Text textHpPlus;

    public Text textChip;

    public Action onSuccess = null;

    public void Setup(PBPlayerFishLevelInfo playerFishLevelInfo, Action onSuccess)
    {
        this.onSuccess = onSuccess;
        this.playerFishLevelInfo = playerFishLevelInfo;
        dataInfo = FishLevelUpDataTableProxy.Instance.GetDataById(playerFishLevelInfo.FishLevel);
        textLevelupUseGold.text = dataInfo.useGold.ToString();

        bool isLvUp = playerFishLevelInfo.FishLevel > 0;
        textTitle.text = isLvUp ? 
                                string.Format(LanguageDataTableProxy.GetText(50), playerFishLevelInfo.FishLevel + 1) :
                                LanguageDataTableProxy.GetText(51);
        textLvUp.text = LanguageDataTableProxy.GetText(isLvUp ? 15 : 18); 

        int currentValue, plus;
        var fishData = FishDataTableProxy.Instance.GetDataById(playerFishLevelInfo.FishId);
        // 攻击力
        currentValue = FishLevelUpDataTableProxy.Instance.GetFishAtk(fishData, playerFishLevelInfo.FishLevel, 1);
        textAtkValue.text = currentValue.ToString();
        plus = FishLevelUpDataTableProxy.Instance.GetFishAtk(fishData, playerFishLevelInfo.FishLevel + 1, 1) - currentValue;
        textAtkPlus.text = "+" + plus.ToString();
        // 体力
        currentValue = FishLevelUpDataTableProxy.Instance.GetFishHp(fishData, playerFishLevelInfo.FishLevel, 1);
        textHpValue.text = currentValue.ToString();
        plus = FishLevelUpDataTableProxy.Instance.GetFishHp(fishData, playerFishLevelInfo.FishLevel + 1, 1) - currentValue;
        textHpPlus.text = "+" + plus.ToString();

        textChip.text = dataInfo.useChip.ToString();
    }

    public void OnClickFishLevelUp()
    {
        if (dataInfo.useGold > PlayerModel.Instance.player.Gold)
        {
            UIPopupGotoResGet.Open(UIPopupGotoResGet.ResType.GOLD, ()=> { Close(); });
        }
        else if (dataInfo.useChip > playerFishLevelInfo.FishChip)
        {
            UIPopupGotoResGet.Open(UIPopupGotoResGet.ResType.CHIP, () => { Close(); });
        }
        else
        {
            NetWorkHandler.GetDispatch().AddListener<P7_Response>(GameEvent.RECIEVE_P7_RESPONSE, OnRecvFishLevelUp);
            NetWorkHandler.RequesFishLevelUp(playerFishLevelInfo.FishId);
        }
    }

    void OnRecvFishLevelUp<T>(T response)
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P7_RESPONSE);
        var realResponse = response as P7_Response;
        if (realResponse.Result.Code == NetworkConst.CODE_OK)
        {
            if (realResponse.FishInfo.FishLevel == 1)
            {   // 解锁
                PlayerModel.Instance.MissionActionTriggerAdd(16, 1);
            }
            PlayerModel.Instance.MissionActionTriggerAdd(13, 1);
            PlayerModel.Instance.MissionActionTriggerAdd(15, realResponse.FishInfo.FishLevel);
            PlayerModel.Instance.SetPlayerFishLevelInfo(playerFishLevelInfo.FishId, realResponse.FishInfo);
            PlayerModel.Instance.player.Gold -= dataInfo.useGold;
            PlayerModel.Instance.MissionActionTriggerAdd(5, dataInfo.useGold);
            UIHomeResource.Instance.UpdateAssets();

            
            onSuccess();
            Close();
        }

    }

}
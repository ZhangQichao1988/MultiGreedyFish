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

    public Text textLevelupUseGold;

    public void Setup(PBPlayerFishLevelInfo playerFishLevelInfo)
    {
        this.playerFishLevelInfo = playerFishLevelInfo;
        var fishLevelData = FishLevelUpDataTableProxy.Instance.GetDataById(playerFishLevelInfo.FishLevel);
        textLevelupUseGold.text = fishLevelData.useGold.ToString();
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
        if (realResponse.Result.Code == NetWorkResponseCode.SUCEED)
        {
             PlayerModel.Instance.SetPlayerFishLevelInfo(playerFishLevelInfo.FishId, realResponse.FishInfo);
            Close();
        }
        else
        {
            MsgBox.Open("networkerror", realResponse.Result.Desc);
        }
    }

}
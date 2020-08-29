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

    public void Setup(PBPlayerFishLevelInfo playerFishLevelInfo)
    {

    }

    public void OnClickFishLevelUp()
    {
        NetWorkHandler.GetDispatch().AddListener<P6_Response>(GameEvent.RECIEVE_P6_RESPONSE, OnRecvFishLevelUp);
        NetWorkHandler.RequestFightFishSet(playerFishLevelInfo.FishId);
    }

    void OnRecvFishLevelUp<T>(T response)
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P6_RESPONSE);
        var realResponse = response as P6_Response;
        PlayerModel.Instance.player.FightFish = playerFishLevelInfo.FishId;
        ((HomeScene)BlSceneManager.GetCurrentScene()).GotoSceneUI("Home");
    }

}
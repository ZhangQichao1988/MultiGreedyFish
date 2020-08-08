using UnityEngine;
using System.Collections.Generic;
using Google.Protobuf;

public class Home : UIBase
{
    public void Awake()
    {
        UIBase.Close("BattleResult");
    }
    public void OnClickLogin()
    {
        GameServiceController.GetPlatformToken((token)=>{
            token = token == null ? "" : token ;
            UserLoginFlowController.ProcessLoginLogic(token);
        });
    }

    public void OnClickBattle()
    {
        NetWorkHandler.GetDispatch().AddListener<P4_Response>(GameEvent.RECIEVE_P4_RESPONSE, OnRecvBattle);
        NetWorkHandler.RequestBattle();

        
    }

    void OnRecvBattle<T>(T response)
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P4_RESPONSE);
        var realResponse = response as P4_Response;
        if (realResponse.Result.Code == NetworkConst.CODE_OK)
        {
            Close();
            DataBank.stageInfo = realResponse.StageInfo;
            BlSceneManager.LoadSceneByClass(SceneId.BATTLE_SCENE, typeof(BattleScene));
        }
        else
        {
            Debug.Log("战斗通信错误！");
        }

    }
}
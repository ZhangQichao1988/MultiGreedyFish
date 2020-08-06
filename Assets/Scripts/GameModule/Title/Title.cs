using UnityEngine;
using System.Collections.Generic;
using Google.Protobuf;

public class Title : UIBase
{
    public void OnClickLogin()
    {
        GameServiceController.GetPlatformToken((token)=>{
            token = "tesadfasfa";
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
            BlSceneManager.LoadSceneByClass(SceneId.BATTLE_SCENE, typeof(BattleScene), realResponse);
        }
        else
        {
            Debug.Log("战斗通信错误！");
        }

    }
}
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
        Close();
        BlSceneManager.LoadSceneByClass(SceneId.BATTLE_SCENE, typeof(BattleScene));
    }

}
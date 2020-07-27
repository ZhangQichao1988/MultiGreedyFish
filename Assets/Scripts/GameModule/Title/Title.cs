using UnityEngine;
using Google.Protobuf;

public class Title : UIBase
{
    public void OnClickLogin()
    {
        Debug.Log("Click ");
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
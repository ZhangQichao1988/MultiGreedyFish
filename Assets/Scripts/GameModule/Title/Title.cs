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
        UIBase.Destroy(this.gameObject);
        BlSceneManager.LoadSceneAsync("BattleScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
        //UnityEngine.SceneManagement.SceneManager.LoadScene("BattleScene");
    }

}
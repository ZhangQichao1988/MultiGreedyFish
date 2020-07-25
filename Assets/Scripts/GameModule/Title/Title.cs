using UnityEngine;
using Google.Protobuf;

public class Title : UIBase
{
    public void OnClickTitle()
    {
        Debug.Log("Click ");
        GameServiceController.GetPlatformToken((token)=>{
            token = "tesadfasfa";
            UserLoginFlowController.ProcessLoginLogic(token);
        });
    }
    
}
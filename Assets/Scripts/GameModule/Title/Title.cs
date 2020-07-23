using UnityEngine;
using Google.Protobuf;

public class Title : UIBase
{
    public void OnClickTitle()
    {
        Debug.Log("Click ");
        GameServiceController.GetPlatformToken((token)=>{
            UserLoginFlowController.ProcessLoginLogic(token);
        });
    }
    
}
using UnityEngine;
using Google.Protobuf;

public class Title : UIBase
{
    public void OnClickTitle()
    {
        Debug.Log("Click ");
        UserLoginFlowController.ProcessLoginLogic();
    }
    
}
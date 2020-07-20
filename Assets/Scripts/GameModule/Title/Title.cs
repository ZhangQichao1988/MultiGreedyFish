using UnityEngine;

public class Title : UIBase
{
    public void OnClickTitle()
    {
        Debug.Log("Click ");
        NetWorkHandler.GetDispatch().AddListener<P0_Response>(GameEvent.RECIEVE_P0_REQUEST, OnRecvStartUp);
        NetWorkHandler.RequestStartUp();
    }

    void OnRecvStartUp<T>(T response)
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P0_REQUEST);
        NetWorkHandler.Login();
    }
    
}
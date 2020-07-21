using UnityEngine;
using Google.Protobuf;

public class Title : UIBase
{
    public void OnClickTitle()
    {
        Debug.Log("Click ");
        NetWorkHandler.GetDispatch().AddListener<P0_Response>(GameEvent.RECIEVE_P0_RESPONSE, OnRecvStartUp);
        NetWorkHandler.RequestStartUp();
    }

    void OnRecvStartUp<T>(T response)
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P0_RESPONSE);
        NetWorkHandler.GetDispatch().AddListener<P2_Response>(GameEvent.RECIEVE_P2_RESPONSE, OnRecvLogin);
        NetWorkHandler.RequestLogin();
    }

    void OnRecvLogin<T>(T response)
    {
        var realResponse = response as P2_Response;
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P2_RESPONSE);
        GameTimeUtil.SetServerTime(realResponse.ServerTime);
        

        NetWorkHandler.GetDispatch().AddListener<P3_Response>(GameEvent.RECIEVE_P3_RESPONSE, OnRecvGetPlayerInfo);
        NetWorkHandler.RequestGetPlayer();
    }

    void OnRecvGetPlayerInfo<T>(T response)
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P3_RESPONSE);
        
        
    }
    
}
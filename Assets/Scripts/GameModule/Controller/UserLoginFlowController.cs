using UnityEngine;

/// <summary>
/// 用户登录相关流程 控制器
/// </summary>
public class UserLoginFlowController
{
    public static void ProcessLoginLogic()
    {
        if (string.IsNullOrEmpty( PlayerPrefs.GetString(NetworkConst.AUTH_KEY) ) )
        {
            NetWorkHandler.GetDispatch().AddListener<P0_Response>(GameEvent.RECIEVE_P0_RESPONSE, OnRecvStartUp);
            NetWorkHandler.RequestStartUp();
        }
        else
        {
            NetWorkHandler.GetDispatch().AddListener<P2_Response>(GameEvent.RECIEVE_P2_RESPONSE, OnRecvLogin);
            NetWorkHandler.RequestLogin();
        }
    }

    static void OnRecvStartUp<T>(T response)
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P0_RESPONSE);
        NetWorkHandler.GetDispatch().AddListener<P2_Response>(GameEvent.RECIEVE_P2_RESPONSE, OnRecvLogin);
        NetWorkHandler.RequestLogin();
    }

    static void OnRecvLogin<T>(T response)
    {
        var realResponse = response as P2_Response;
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P2_RESPONSE);
        GameTimeUtil.SetServerTime(realResponse.ServerTime);
        

        NetWorkHandler.GetDispatch().AddListener<P3_Response>(GameEvent.RECIEVE_P3_RESPONSE, OnRecvGetPlayerInfo);
        NetWorkHandler.RequestGetPlayer();
    }

    static void OnRecvGetPlayerInfo<T>(T response)
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P3_RESPONSE);
    }
}
using UnityEngine;
using System;

/// <summary>
/// 用户登录相关流程 控制器
/// </summary>
public class UserLoginFlowController
{
    static Action finishCb;
    public static void StartLoginFlow(Action callback)
    {
        finishCb = callback;
        GameServiceController.GetPlatformToken((token)=>{
            token = token == null ? "" : token ;
            ProcessLoginLogic(token);
        });
    }

    public static void ProcessLoginLogic(string platformToken)
    {
        Debug.LogFormat("[GetPlatform token]{0}", platformToken);
        if (string.IsNullOrEmpty( PlayerPrefs.GetString("TEST_LOGIN") ) )
        {
            //正常登录
            if (string.IsNullOrEmpty( PlayerPrefs.GetString(NetworkConst.AUTH_KEY) ) )
            {
                if (!string.IsNullOrEmpty(platformToken))
                {
                    //连协check
                    ProcessLinkedCheck(
                        platformToken,
                        LinkServiceType
                    );
                }
                else
                {
                    ProcessStartUp();
                }
            }
            else
            {
                ProcessLogin();
            }
        }
        else
        {
            //debug login
            NetWorkHandler.GetDispatch().AddListener<P17_Response, P17_Request>(GameEvent.RECIEVE_P17_RESPONSE, OnRecvDebugLogin);
            NetWorkHandler.RequestDebugLogin(PlayerPrefs.GetString("TEST_LOGIN"));
        }
    }

    static DataLinkServiceType LinkServiceType
    {
        get
        {
            return Application.platform == RuntimePlatform.Android ? DataLinkServiceType.GooglePlay : DataLinkServiceType.GameCenter;
        }
    }

    /// <summary>
    /// 连协
    /// </summary>
    /// <param name="token"></param>
    /// <param name="type"></param>
    static void ProcessLinkedCheck(string token, DataLinkServiceType type)
    {
        NetWorkHandler.GetDispatch().AddListener<P1_Response, P1_Request>(GameEvent.RECIEVE_P1_RESPONSE, OnRecvLinkedCheck);
        NetWorkHandler.RequestLoginWithThirdPlatform(token, type);
    }

    static void OnRecvLinkedCheck<T, V>(T response, V request)
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P1_RESPONSE);
        var linkedRes = response as P1_Response;
        var linkedReq = request as P1_Request;
        if (linkedRes.Result.Code == NetworkConst.CODE_OK)
        {
            ProcessLogin();
        }
        else
        {
            ProcessStartUp(linkedReq.Accesstoken);
        }
    }

    static void OnRecvDebugLogin<T, V>(T response, V request)
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P17_RESPONSE);
        var linkedRes = response as P17_Response;
        var linkedReq = request as P17_Request;
        if (linkedRes.Result.Code == NetworkConst.CODE_OK)
        {
            ProcessLogin();
        }
    }

    /// <summary>
    /// 登录
    /// </summary>
    static void ProcessLogin()
    {
        NetWorkHandler.GetDispatch().AddListener<P2_Response>(GameEvent.RECIEVE_P2_RESPONSE, OnRecvLogin);
        NetWorkHandler.RequestLogin();
    }

    /// <summary>
    /// 注册
    /// </summary>
    static void ProcessStartUp(string accToken = null)
    {
        NetWorkHandler.GetDispatch().AddListener<P0_Response>(GameEvent.RECIEVE_P0_RESPONSE, OnRecvStartUp);
        if (accToken == null)
        {
            NetWorkHandler.RequestStartUp();
        }
        else
        {
            NetWorkHandler.RequestStartUp(accToken, LinkServiceType);
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
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P2_RESPONSE);
        var realResponse = response as P2_Response;
        if (realResponse.Result.Code == NetworkConst.CODE_OK)
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventLogin);
            GameTimeUtil.SetServerTime(realResponse.ServerTime);
            PlayerModel.Instance.playerId = realResponse.PlayerId;
            NetWorkHandler.GetDispatch().AddListener<P3_Response>(GameEvent.RECIEVE_P3_RESPONSE, OnRecvGetPlayerInfo);
            NetWorkHandler.RequestGetPlayer();
        }
        else
        {
            //清除缓存
            PlayerPrefs.DeleteKey(NetworkConst.AUTH_KEY);
            ProcessStartUp();
        }
        
    }

    static void OnRecvGetPlayerInfo<T>(T response)
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P3_RESPONSE);
        Debug.Log("On Getted Userinfo!");
        var realResponse = response as P3_Response;

        Intro.Instance.FireBaseCtrl.SetUserId(realResponse.Player.PlayerId.ToString());
        PlayerModel.Instance.player = realResponse.Player;
        finishCb?.Invoke();
    }
}
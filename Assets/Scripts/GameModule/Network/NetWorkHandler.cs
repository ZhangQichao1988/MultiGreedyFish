using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetWorkModule;
using Google.Protobuf;


/// <summary>
/// 网络分发处理
/// </summary>
public class NetWorkHandler
{
    private static Dictionary<string, MessageParser> pbParserRef;
    private static EventDispatch dispatch;

    public static EventDispatch GetDispatch()
    {
        if (dispatch == null)
        {
            dispatch = new EventDispatch();
        }
        return dispatch;
    }

    public static void Dispose()
    {
        if (dispatch != null)
        {
            dispatch.RemoveAll();
            dispatch = null;
        }
    }

    public static void InitHttpNetWork()
    {
        pbParserRef = new Dictionary<string, MessageParser>(){
            {"P0_Request", P0_Request.Parser},
            {"P1_Request", P1_Request.Parser},
            {"P2_Request", P2_Request.Parser},
            {"P0_Response", P0_Response.Parser},
            {"P1_Response", P1_Response.Parser},
            {"P2_Response", P2_Response.Parser},
            {"P3_Response", P3_Response.Parser}
        };

        NetWorkManager.Instance.InitWithServerCallBack(new FishProtocol(), (int)MessageId.MidLogin, OnServerEvent);

        //register
        HttpDispatcher.Instance.AddObserver((int)MessageId.MidStartup, OnRecvStartup);
        HttpDispatcher.Instance.AddObserver((int)MessageId.MidLogin , OnRecvLogin);
        HttpDispatcher.Instance.AddObserver((int)MessageId.MidLoginWithPlatform, OnRecvLoginWithThirdPlatform);
        HttpDispatcher.Instance.AddObserver((int)MessageId.MidGetPlayerInfo, OnRecvGetPlayerInfo);
    }
    
    static void OnServerEvent(HttpDispatcher.EventType type, string msg, System.Object obj)
    {
        string errMsg = "";
        switch (type)
        {
            case HttpDispatcher.EventType.HttpError:
            case HttpDispatcher.EventType.SignatureError:
            case HttpDispatcher.EventType.Failed:
            case HttpDispatcher.EventType.KickOutLoginUser:
                errMsg = "got server error " + type;
                Debug.LogWarning(errMsg);
                LoadingMgr.Hide(LoadingMgr.LoadingType.Repeat);
                MsgBox.Open("网络错误", errMsg);
                break;
            case HttpDispatcher.EventType.Caution:
                errMsg = "got a warning";
                Debug.LogWarning(errMsg);
                LoadingMgr.Hide(LoadingMgr.LoadingType.Repeat);
                MsgBox.Open("网络错误", errMsg);
                break;
            case HttpDispatcher.EventType.HttpRequestSend:
                // http request send 
                LoadingMgr.Show(LoadingMgr.LoadingType.Repeat);
                TraceLog("Request", msg, obj as byte[]);
                break;
            case HttpDispatcher.EventType.HttpRecieve:
                // http request recieve 
                LoadingMgr.Hide(LoadingMgr.LoadingType.Repeat);
                TraceLog("Response", msg, obj as byte[]);
                break;
            default:
                break;
        }
    }

    static byte[] GetStreamBytes(IMessage pbMsg)
    {
        byte[] bytesDatas;
        using (MemoryStream stream = new MemoryStream())
        {
            pbMsg.WriteTo(stream);
            bytesDatas = stream.ToArray();
        }
        return bytesDatas;
    }

    /// <summary>
    /// P0 STARTUP
    /// </summary>
    public static void RequestStartUp(string authToken = null, DataLinkServiceType serviceType = DataLinkServiceType.None)
    {
        var request = new P0_Request();
        request.DeviceId = UnityEngine.SystemInfo.deviceUniqueIdentifier;
        request.DeviceName = UnityEngine.SystemInfo.deviceName;
        request.DevicePlatform = Application.platform.ToString();

        if (authToken != null)
        {
            request.Accesstoken = authToken;
            request.ServiceType = serviceType;
        }

        var randomKey = CryptographyUtil.RandomBytes(32);
        request.Mask = CryptographyUtil.GetMakData(randomKey);

        byte[] requestByteData = GetStreamBytes(request);
        NetWorkManager.Request("P0_Request", requestByteData , randomKey, false);
    }

    /// <summary>
    /// P2 Login
    /// </summary>
    /// <param name="authToken"></param>
    public static void RequestLogin()
    {
        var request = new P2_Request();
        request.AuthToken = PlayerPrefs.GetString(NetworkConst.AUTH_KEY);
        
        var randomKey = CryptographyUtil.RandomBytes(32);
        request.Mask = CryptographyUtil.GetMakData(randomKey);

        byte[] requestByteData = GetStreamBytes(request);
        NetWorkManager.Request("P2_Request", requestByteData , randomKey, false);
    }

    public static void RequestLoginWithThirdPlatform(string authToken, DataLinkServiceType serviceType)
    {
        var request = new P1_Request();
        request.Accesstoken = authToken;
        request.ServiceType = serviceType;
        var randomKey = CryptographyUtil.RandomBytes(32);
        request.Mask = CryptographyUtil.GetMakData(randomKey);

        byte[] requestByteData = GetStreamBytes(request);
        NetWorkManager.Request("P1_Request", requestByteData , randomKey, false);
    }


    //recieve callback
    static void OnRecvStartup(HttpDispatcher.NodeMsg msg)
    {
        var response = P0_Response.Parser.ParseFrom(msg.Body);
        PlayerPrefs.SetString(NetworkConst.AUTH_KEY, response.AuthToken);
        PlayerPrefs.Save();
        byte[] randKey = msg.CachedData as byte[];
        NetWorkManager.HttpClient.SaveSessionKey(response.AuthKey, randKey, true);
        GetDispatch().Dispatch<P0_Response>(GetDispatchKey(msg.Key), response);
    }

    static void OnRecvLogin(HttpDispatcher.NodeMsg msg)
    {
        var response = P2_Response.Parser.ParseFrom(msg.Body);

        if (response.Result.Code == NetworkConst.CODE_OK)
        {
            NetWorkManager.HttpClient.SaveSessionKey(response.SessionKey, msg.CachedData as byte[], false);
            NetWorkManager.HttpClient.SetPlayerId(response.PlayerId);
        }

        GetDispatch().Dispatch<P2_Response>(GetDispatchKey(msg.Key), response);
    }

    /// <summary>
    /// P3 获取玩家信息
    /// </summary>
    public static void RequestGetPlayer()
    {
        NetWorkManager.Request("P3_Request", null);
    }

    static string GetDispatchKey(int msgId)
    {
        return string.Format(GameEvent.RECIEVE_COMMON_RESPONSE, msgId);
    }

    static void OnRecvLoginWithThirdPlatform(HttpDispatcher.NodeMsg msg)
    {
        var response = P1_Response.Parser.ParseFrom(msg.Body);
        if (response.Result.Code == NetworkConst.CODE_OK)
        {
            PlayerPrefs.SetString(NetworkConst.AUTH_KEY, response.AuthToken);
            PlayerPrefs.Save();
            byte[] randKey = msg.CachedData as byte[];
            NetWorkManager.HttpClient.SaveSessionKey(response.AuthKey, randKey, true);
        }
        var request = pbParserRef[string.Format("P{0}_Request", msg.Key)].ParseFrom(ByteString.CopyFrom(msg.ReqMsg)) as P1_Request;
        GetDispatch().Dispatch<P1_Response, P1_Request>(GetDispatchKey(msg.Key), response, request);
    }

    static void OnRecvGetPlayerInfo(HttpDispatcher.NodeMsg msg)
    {
        var response = P3_Response.Parser.ParseFrom(msg.Body);
        GetDispatch().Dispatch<P3_Response>(GetDispatchKey(msg.Key), response);
    }

    static void TraceLog(string tag, string msg, byte[] data)
    {
#if CONSOLE_ENABLE 
        string requestData = "";
        string color = tag == "Request" ? "green" : "yellow";
        if (data != null)
        {
            MessageParser parser = pbParserRef[msg];
            var msgData = parser.ParseFrom(ByteString.CopyFrom(data));
            requestData = JsonFormatter.ToDiagnosticString(msgData);
        }
        string logData = string.Format("<color='{3}'>[{0}]</color> {1} \n{2}", tag, msg, requestData, color);
        Debug.Log(logData);
#endif
    }
}

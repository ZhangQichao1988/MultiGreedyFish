using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetWorkModule;
using Google.Protobuf;


/// <summary>
/// 网络分发处理
/// ps 网络层独立 不要处理业务逻辑
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

        NetWorkManager.Instance.Reset();
    }

    public static void InitHttpNetWork()
    {
        pbParserRef = new Dictionary<string, MessageParser>(){
            {"P0_Request", P0_Request.Parser},
            {"P1_Request", P1_Request.Parser},
            {"P2_Request", P2_Request.Parser},
            {"P5_Request", P5_Request.Parser},
            {"P6_Request", P6_Request.Parser},
            {"P7_Request", P7_Request.Parser},
            {"P8_Request", P8_Request.Parser},
            {"P9_Request", P9_Request.Parser},
            {"P0_Response", P0_Response.Parser},
            {"P1_Response", P1_Response.Parser},
            {"P2_Response", P2_Response.Parser},
            {"P3_Response", P3_Response.Parser},
            {"P4_Response", P4_Response.Parser},
            {"P5_Response", P5_Response.Parser},
            {"P6_Response", P6_Response.Parser},
            {"P7_Response", P7_Response.Parser},
            {"P8_Response", P8_Response.Parser},
            {"P9_Response", P9_Response.Parser},
        };

#if DUMMY_DATA
        NetWorkManager.Instance.InitWithServerCallBack(new FishProtocol(), (int)MessageId.MidLogin, OnServerEvent, new FishDummy(pbParserRef));
#else
        NetWorkManager.Instance.InitWithServerCallBack(new FishProtocol(), (int)MessageId.MidLogin, OnServerEvent);
#endif

        //register
        HttpDispatcher.Instance.AddObserver((int)MessageId.MidStartup, OnRecvStartup);
        HttpDispatcher.Instance.AddObserver((int)MessageId.MidLogin , OnRecvLogin);
        HttpDispatcher.Instance.AddObserver((int)MessageId.MidLoginWithPlatform, OnRecvLoginWithThirdPlatform);
        HttpDispatcher.Instance.AddObserver((int)MessageId.MidGetPlayerInfo, OnRecvGetPlayerInfo);
        HttpDispatcher.Instance.AddObserver((int)MessageId.MidStartFight, OnRecvBattle);
        HttpDispatcher.Instance.AddObserver((int)MessageId.MidEndFight, OnRecvBattleResult);
        HttpDispatcher.Instance.AddObserver((int)MessageId.MidSetFightFish, OnRecvFightFishSet);

        HttpDispatcher.Instance.AddObserver((int)MessageId.MidFishLevelUp, OnRecvFishLevelUp);
        HttpDispatcher.Instance.AddObserver((int)MessageId.MidBoundsGet, OnRecvBounsGet);
        HttpDispatcher.Instance.AddObserver((int)MessageId.MidModifyNick, OnRecvModifyNick);
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
            case HttpDispatcher.EventType.TimeOut:
                //超时
                LoadingMgr.Hide(LoadingMgr.LoadingType.Repeat);
                MsgBox.OpenConfirm("网络错误", "请求超时 请重试", ()=>{
                    RetrySend(obj);
                }, ()=>{
                    //goto title
                    Intro.Instance.Restart();
                });
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

    static void RetrySend(System.Object obj)
    {
        ReqData reqInfo = obj as ReqData;
        NetWorkManager.Request(reqInfo.Msg, reqInfo.Body, reqInfo.CachedData, reqInfo.NeedAuth);
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

#region ServerRequest
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

    /// <summary>
    /// P3 获取玩家信息
    /// </summary>
    public static void RequestGetPlayer()
    {
        NetWorkManager.Request("P3_Request", null);
    }

    public static void RequestBattle()
    {
        NetWorkManager.Request("P4_Request", null);
    }

    public static void RequestBattleResult(string battleId, int battleRanking)
    {
        var request = new P5_Request();
        request.BattleRanking = battleRanking;
        request.BattleId = battleId;
        
        byte[] requestByteData = GetStreamBytes(request);
        NetWorkManager.Request("P5_Request", requestByteData);
    }

    public static void RequestFightFishSet(int fishId)
    {
        var request = new P6_Request();
        request.FishId = fishId;
        
        byte[] requestByteData = GetStreamBytes(request);
        NetWorkManager.Request("P6_Request", requestByteData);

    }

    public static void RequesFishLevelUp(int fishId)
    {
        var request = new P7_Request();
        request.FishId = fishId;
        
        byte[] requestByteData = GetStreamBytes(request);
        NetWorkManager.Request("P7_Request", requestByteData);
    }

    public static void RequestGetBattleBounds(string battleId, bool isDouble)
    {
        var request = new P8_Request();
        request.BattleId = battleId;
        request.IsDouble = isDouble;
        
        byte[] requestByteData = GetStreamBytes(request);
        NetWorkManager.Request("P8_Request", requestByteData);

    }

    public static void RequestModifyNick(string nickName)
    {
        var request = new P9_Request();
        request.Nick = nickName;
        
        byte[] requestByteData = GetStreamBytes(request);
        NetWorkManager.Request("P9_Request", requestByteData);

    }

#endregion

#region ServerResponse
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

    static void OnRecvBattle(HttpDispatcher.NodeMsg msg)
    {
        var response = P4_Response.Parser.ParseFrom(msg.Body);
        GetDispatch().Dispatch<P4_Response>(GetDispatchKey(msg.Key), response);
    }
    static void OnRecvBattleResult(HttpDispatcher.NodeMsg msg)
    {
        var response = P5_Response.Parser.ParseFrom(msg.Body);
        GetDispatch().Dispatch<P5_Response>(GetDispatchKey(msg.Key), response);
    }

    static void OnRecvFightFishSet(HttpDispatcher.NodeMsg msg)
    {
        var response = P6_Response.Parser.ParseFrom(msg.Body);
        GetDispatch().Dispatch<P6_Response>(GetDispatchKey(msg.Key), response);
    }

    static void OnRecvFishLevelUp(HttpDispatcher.NodeMsg msg)
    {
        var response = P7_Response.Parser.ParseFrom(msg.Body);
        GetDispatch().Dispatch<P7_Response>(GetDispatchKey(msg.Key), response);
    }

    static void OnRecvBounsGet(HttpDispatcher.NodeMsg msg)
    {
        var response = P8_Response.Parser.ParseFrom(msg.Body);
        GetDispatch().Dispatch<P8_Response>(GetDispatchKey(msg.Key), response);
    }

    static void OnRecvModifyNick(HttpDispatcher.NodeMsg msg)
    {
        var response = P9_Response.Parser.ParseFrom(msg.Body);
        GetDispatch().Dispatch<P9_Response>(GetDispatchKey(msg.Key), response);
    }

#endregion

    public static string GetDispatchKey(int msgId)
    {
        return string.Format(GameEvent.RECIEVE_COMMON_RESPONSE, msgId);
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

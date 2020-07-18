using System;
using System.IO;
using UnityEngine;
using NetWorkModule;
using Google.Protobuf;


/// <summary>
/// 网络分发处理
/// </summary>
public class NetWorkHandler
{
    private static EventDispatch dispatch;

    public static EventDispatch GetDispatch()
    {
        if (dispatch == null)
        {
            dispatch = new EventDispatch();
        }
        return dispatch;
    }

    public static void InitHttpNetWork()
    {
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
                MsgBox.Open("网络错误", errMsg);
                break;
            case HttpDispatcher.EventType.Caution:
                errMsg = "got a warning";
                Debug.LogWarning(errMsg);
                MsgBox.Open("网络错误", errMsg);
                break;
            case HttpDispatcher.EventType.HttpRequestSend:
                // http request send 
                LoadingMgr.Show(LoadingMgr.LoadingType.Repeat);
                break;
            case HttpDispatcher.EventType.HttpRecieve:
                // http request recieve 
                LoadingMgr.Hide(LoadingMgr.LoadingType.Repeat);
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

    //request method
    public static void RequestStartUp()
    {
        var request = new P0_Request();
        request.DeviceId = UnityEngine.SystemInfo.deviceUniqueIdentifier;
        request.DeviceName = UnityEngine.SystemInfo.deviceName;
        request.DevicePlatform = Application.platform.ToString();

        var randomKey = CryptographyUtil.RandomBytes(32);
        request.Mask = CryptographyUtil.GetMakData(randomKey);

        byte[] requestByteData = GetStreamBytes(request);
        NetWorkManager.Request("P0_Request", requestByteData , false);
    }

    public static void Login(string authToken)
    {
        var request = new P1_Request();
        request.Accesstoken = authToken;
        
        var randomKey = CryptographyUtil.RandomBytes(32);
        request.Mask = CryptographyUtil.GetMakData(randomKey);

        byte[] requestByteData = GetStreamBytes(request);
        NetWorkManager.Request("P1_Request", requestByteData , false);
    }


    //recieve callback
    static void OnRecvStartup(HttpDispatcher.NodeMsg msg)
    {
        var response = P0_Response.Parser.ParseFrom(msg.Body);
        GetDispatch().Dispatch<P0_Response>(GameEvent.RECIEVE_P0_REQUEST, response);
    }

    static void OnRecvLogin(HttpDispatcher.NodeMsg msg)
    {
        var response = P1_Response.Parser.ParseFrom(msg.Body);
        GetDispatch().Dispatch<P1_Response>(GameEvent.RECIEVE_P0_REQUEST, response);
    }

    static void OnRecvLoginWithThirdPlatform(HttpDispatcher.NodeMsg msg)
    {

    }

    static void OnRecvGetPlayerInfo(HttpDispatcher.NodeMsg msg)
    {

    }
}
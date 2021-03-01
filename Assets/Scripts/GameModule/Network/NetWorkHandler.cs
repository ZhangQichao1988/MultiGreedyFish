using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetWorkModule;
using Google.Protobuf;
using Jackpot.Billing;


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

    static IErrorCodeProcesser errorCodeProcesser;
    public static void InitHttpNetWork(IErrorCodeProcesser errProcess)
    {
        errorCodeProcesser = errProcess;
        pbParserRef = new Dictionary<string, MessageParser>(){
            {"P0_Request", P0_Request.Parser},
            {"P1_Request", P1_Request.Parser},
            {"P2_Request", P2_Request.Parser},
            {"P5_Request", P5_Request.Parser},
            {"P6_Request", P6_Request.Parser},
            {"P7_Request", P7_Request.Parser},
            {"P8_Request", P8_Request.Parser},
            {"P9_Request", P9_Request.Parser},
            {"P10_Request", P10_Request.Parser},
            {"P11_Request", P11_Request.Parser},
            {"P12_Request", P12_Request.Parser},
            {"P13_Request", P13_Request.Parser},
            {"P15_Request", P15_Request.Parser},
            {"P16_Request", P16_Request.Parser},
            {"P17_Request", P17_Request.Parser},
            {"P19_Request", P19_Request.Parser},
            {"P21_Request", P21_Request.Parser},
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
            {"P10_Response", P10_Response.Parser},
            {"P11_Response", P11_Response.Parser},
            {"P12_Response", P12_Response.Parser},
            {"P13_Response", P13_Response.Parser},
            {"P14_Response", P14_Response.Parser},
            {"P15_Response", P15_Response.Parser},
            {"P16_Response", P16_Response.Parser},
            {"P17_Response", P17_Response.Parser},
            {"P18_Response", P18_Response.Parser},
            {"P19_Response", P19_Response.Parser},
            {"P20_Response", P20_Response.Parser},
            {"P21_Response", P21_Response.Parser},
            {"P22_Response", P22_Response.Parser},
            {"P23_Response", P23_Response.Parser},
        };

        if (AppConst.ServerType == ESeverType.OFFLINE)
        {
            NetWorkManager.Instance.InitWithServerCallBack(new FishProtocol(), (int)MessageId.MidLogin, OnServerEvent, new FishDummy(pbParserRef));
        }
        else
        {
            NetWorkManager.Instance.InitWithServerCallBack(new FishProtocol(), (int)MessageId.MidLogin, OnServerEvent);
        }

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
        HttpDispatcher.Instance.AddObserver((int)MessageId.MidGetShopitem, OnRecvGetShopItem);
        HttpDispatcher.Instance.AddObserver((int)MessageId.MidBuyNormal, OnRecvItemBuyNormal);

        HttpDispatcher.Instance.AddObserver((int)MessageId.MidPrePay, OnRecvPrePay);
        HttpDispatcher.Instance.AddObserver((int)MessageId.MidBuyPay, OnRecvBuyPay);
        HttpDispatcher.Instance.AddObserver((int)MessageId.MidDebugBilling, OnRecvDebugPay);

        HttpDispatcher.Instance.AddObserver((int)MessageId.MidGoldPoolRefresh, OnRecvGoldRef);
        HttpDispatcher.Instance.AddObserver((int)MessageId.MidRankRewardGet, OnRecvRewardGet);

        HttpDispatcher.Instance.AddObserver(17, OnDebugLoginEnd);

        HttpDispatcher.Instance.AddObserver((int)MessageId.MidUpdateGooldPool, OnRecvUpdateGooldPool);
        HttpDispatcher.Instance.AddObserver((int)MessageId.MidRankListGet, OnRecvRankList);

        HttpDispatcher.Instance.AddObserver((int)MessageId.MidMissionListGet, OnRecvMissionGet);
        HttpDispatcher.Instance.AddObserver((int)MessageId.MidCompleteMission, OnRecvMissionComplete);


        HttpDispatcher.Instance.AddObserver((int)MessageId.MidAdDiamond, OnRecvAdDiamond);
        HttpDispatcher.Instance.AddObserver((int)MessageId.MidGoldPoolRecover, OnRecvGoldPoolRecover);
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
                if (msg.Contains("P13_Request"))
                {
                    //支付协议超时
                    LoadingMgr.Hide(LoadingMgr.LoadingType.Repeat);
                }
                MsgBox.OpenConfirm("网络错误", "请求超时 请重试", ()=>{
                    Debug.Log("Clicked !!");
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
        NetWorkManager.Request(reqInfo.Msg, reqInfo.Body, reqInfo.CachedData, reqInfo.NeedAuth, reqInfo.RequestId);
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

    public static void RequestGetShopItem(ShopType stype)
    {
        var request = new P10_Request();
        request.ProductType = stype;
        
        byte[] requestByteData = GetStreamBytes(request);
        NetWorkManager.Request("P10_Request", requestByteData);

    }

    public static void RequestBuyNormal(ShopItemVo vo, int num = 1)
    {
        var request = new P11_Request();
        request.ShopItemId = vo.ID;
        request.ShopItemNum = num;
        
        byte[] requestByteData = GetStreamBytes(request);
        NetWorkManager.Request("P11_Request", requestByteData, vo);
    }

    public static void RequestBillingPreBuy(string productId)
    {
        var request = new P12_Request();
        request.PlatformId = productId;
        
        byte[] requestByteData = GetStreamBytes(request);
        NetWorkManager.Request("P12_Request", requestByteData, productId);

    }

    /*
        required string receipt			= 1;
        required string transactionId	= 2;
        required string price			= 3;
        required string formattedPrice	= 4;
        required Device platform 		= 5;
    */
    public static void RequestBillingBuy(string receipt, string transactionId, string price, string formattedPrice, Device device ,string productId)
    {
        var request = new P13_Request();
        request.Receipt = receipt;
        request.TransactionId = transactionId;
        request.Price = price;
        request.FormattedPrice = formattedPrice;
        request.Platform = device;
        
        byte[] requestByteData = GetStreamBytes(request);
        NetWorkManager.Request("P13_Request", requestByteData, productId);
    }

    public static void RequestDebugBilling(string productId)
    {
        var request = new P15_Request();
        request.PlatformId = productId;

        byte[] requestByteData = GetStreamBytes(request);
        NetWorkManager.Request("P15_Request", requestByteData, productId);
    }

    public static void RequestGoldPoolFetch()
    {
        NetWorkManager.Request("P14_Request", null);
    }
    public static void RequestGoldPoolLevelUp()
    {
        NetWorkManager.Request("P18_Request", null);
    }
    public static void RequestGoldPoolRecover()
    {
        NetWorkManager.Request("P23_Request", null);
    }

    /// <summary>
    /// 获取段位奖励
    /// </summary>
    public static void RequestGetRankBonus(int rankBonusDataId, bool isDouble = false)
    {
        var request = new P16_Request();
        request.RankBoundsId = rankBonusDataId;
        request.IsDouble = isDouble;

        byte[] requestByteData = GetStreamBytes(request);
        NetWorkManager.Request("P16_Request", requestByteData);
    }

    public static void RequestDebugLogin(string playerId)
    {
        var request = new P17_Request();
        request.PlayerId = playerId;
        var randomKey = CryptographyUtil.RandomBytes(32);
        request.Mask = CryptographyUtil.GetMakData(randomKey);

        byte[] requestByteData = GetStreamBytes(request);
        NetWorkManager.Request("P17_Request", requestByteData , randomKey, false);
    }

    public static void RequestGetRankList(PBRankType type)
    {
        var request = new P19_Request();
        request.RankType = type;

        byte[] requestByteData = GetStreamBytes(request);
        NetWorkManager.Request("P19_Request", requestByteData);
    }

    public static void RequestGetMissionList()
    {
        NetWorkManager.Request("P20_Request", null);
    }

    /// <summary>
    /// 获取任务奖励
    /// </summary>
    public static void RequestGetMissionBonus(int missionId, bool isDouble = false)
    {
        var request = new P21_Request();
        request.MissionId = missionId;
        request.IsDouble = isDouble;

        byte[] requestByteData = GetStreamBytes(request);
        NetWorkManager.Request("P21_Request", requestByteData);
    }

    /// <summary>
    /// 获取剩余观看广告次数
    /// </summary>
    public static void RequestGetAdvertRemainingCnt()
    {
        NetWorkManager.Request("P22_Request", null);
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
        errorCodeProcesser.Process(response.Result.Code);
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

        errorCodeProcesser.Process(response.Result.Code);
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
        errorCodeProcesser.Process(response.Result.Code);
        GetDispatch().Dispatch<P1_Response, P1_Request>(GetDispatchKey(msg.Key), response, request);
    }

    static void OnRecvGetPlayerInfo(HttpDispatcher.NodeMsg msg)
    {
        var response = P3_Response.Parser.ParseFrom(msg.Body);
        errorCodeProcesser.Process(response.Result.Code);
        GetDispatch().Dispatch<P3_Response>(GetDispatchKey(msg.Key), response);
    }

    static void OnRecvBattle(HttpDispatcher.NodeMsg msg)
    {
        var response = P4_Response.Parser.ParseFrom(msg.Body);
        errorCodeProcesser.Process(response.Result.Code);
        GetDispatch().Dispatch<P4_Response>(GetDispatchKey(msg.Key), response);
    }
    static void OnRecvBattleResult(HttpDispatcher.NodeMsg msg)
    {
        var response = P5_Response.Parser.ParseFrom(msg.Body);
        errorCodeProcesser.Process(response.Result.Code);
        GetDispatch().Dispatch<P5_Response>(GetDispatchKey(msg.Key), response);
    }

    static void OnRecvFightFishSet(HttpDispatcher.NodeMsg msg)
    {
        var response = P6_Response.Parser.ParseFrom(msg.Body);
        errorCodeProcesser.Process(response.Result.Code);
        GetDispatch().Dispatch<P6_Response>(GetDispatchKey(msg.Key), response);
    }

    static void OnRecvFishLevelUp(HttpDispatcher.NodeMsg msg)
    {
        var response = P7_Response.Parser.ParseFrom(msg.Body);
        errorCodeProcesser.Process(response.Result.Code);
        GetDispatch().Dispatch<P7_Response>(GetDispatchKey(msg.Key), response);
    }

    static void OnRecvBounsGet(HttpDispatcher.NodeMsg msg)
    {
        var response = P8_Response.Parser.ParseFrom(msg.Body);
        errorCodeProcesser.Process(response.Result.Code);
        GetDispatch().Dispatch<P8_Response>(GetDispatchKey(msg.Key), response);
    }

    static void OnRecvModifyNick(HttpDispatcher.NodeMsg msg)
    {
        var response = P9_Response.Parser.ParseFrom(msg.Body);
        errorCodeProcesser.Process(response.Result.Code);
        GetDispatch().Dispatch<P9_Response>(GetDispatchKey(msg.Key), response);
    }

    static void OnRecvGetShopItem(HttpDispatcher.NodeMsg msg)
    {
        var response = P10_Response.Parser.ParseFrom(msg.Body);
        var request = pbParserRef[string.Format("P{0}_Request", msg.Key)].ParseFrom(ByteString.CopyFrom(msg.ReqMsg)) as P10_Request;
        errorCodeProcesser.Process(response.Result.Code);
        GetDispatch().Dispatch<P10_Response, P10_Request>(GetDispatchKey(msg.Key), response, request);
    }

    static void OnRecvItemBuyNormal(HttpDispatcher.NodeMsg msg)
    {
        var response = P11_Response.Parser.ParseFrom(msg.Body);
        var request = pbParserRef[string.Format("P{0}_Request", msg.Key)].ParseFrom(ByteString.CopyFrom(msg.ReqMsg)) as P11_Request;
        errorCodeProcesser.Process(response.Result.Code);
        GetDispatch().Dispatch<P11_Response, P11_Request, ShopItemVo>(GetDispatchKey(msg.Key), response, request, msg.CachedData as ShopItemVo);
    }

    static void OnRecvPrePay(HttpDispatcher.NodeMsg msg)
    {
        var response = P12_Response.Parser.ParseFrom(msg.Body);
        var request = pbParserRef[string.Format("P{0}_Request", msg.Key)].ParseFrom(ByteString.CopyFrom(msg.ReqMsg)) as P12_Request;
        errorCodeProcesser.Process(response.Result.Code);
        GetDispatch().Dispatch<P12_Response, string>(GetDispatchKey(msg.Key), response, msg.CachedData.ToString());
    }


    static void OnRecvBuyPay(HttpDispatcher.NodeMsg msg)
    {
        var response = P13_Response.Parser.ParseFrom(msg.Body);
        var request = pbParserRef[string.Format("P{0}_Request", msg.Key)].ParseFrom(ByteString.CopyFrom(msg.ReqMsg)) as P13_Request;
        errorCodeProcesser.Process(response.Result.Code);
        GetDispatch().Dispatch<P13_Response, string>(GetDispatchKey(msg.Key), response, msg.CachedData.ToString());
    }

    static void OnRecvDebugPay(HttpDispatcher.NodeMsg msg)
    {
        var response = P15_Response.Parser.ParseFrom(msg.Body);
        errorCodeProcesser.Process(response.Result.Code);
        GetDispatch().Dispatch<P15_Response, string>(GetDispatchKey(msg.Key), response, msg.CachedData.ToString());
    }

    static void OnRecvGoldRef(HttpDispatcher.NodeMsg msg)
    {
        var response = P14_Response.Parser.ParseFrom(msg.Body);
        errorCodeProcesser.Process(response.Result.Code);
        GetDispatch().Dispatch<P14_Response>(GetDispatchKey(msg.Key), response);
    }

    static void OnRecvRewardGet(HttpDispatcher.NodeMsg msg)
    {
        var response = P16_Response.Parser.ParseFrom(msg.Body);
        errorCodeProcesser.Process(response.Result.Code);
        GetDispatch().Dispatch<P16_Response>(GetDispatchKey(msg.Key), response);
    }

    static void OnDebugLoginEnd(HttpDispatcher.NodeMsg msg)
    {
        var response = P17_Response.Parser.ParseFrom(msg.Body);
        if (response.Result.Code == NetworkConst.CODE_OK)
        {
            PlayerPrefs.SetString(NetworkConst.AUTH_KEY, response.AuthToken);
            PlayerPrefs.Save();
            byte[] randKey = msg.CachedData as byte[];
            NetWorkManager.HttpClient.SaveSessionKey(response.AuthKey, randKey, true);
        }
        var request = pbParserRef[string.Format("P{0}_Request", msg.Key)].ParseFrom(ByteString.CopyFrom(msg.ReqMsg)) as P17_Request;
        errorCodeProcesser.Process(response.Result.Code);
        GetDispatch().Dispatch<P17_Response, P17_Request>(GetDispatchKey(msg.Key), response, request);
    }

    static void OnRecvUpdateGooldPool(HttpDispatcher.NodeMsg msg)
    {
        var response = P18_Response.Parser.ParseFrom(msg.Body);
        errorCodeProcesser.Process(response.Result.Code);
        GetDispatch().Dispatch<P18_Response>(GetDispatchKey(msg.Key), response);
    }
    
    static void OnRecvRankList(HttpDispatcher.NodeMsg msg)
    {
        var response = P19_Response.Parser.ParseFrom(msg.Body);
        errorCodeProcesser.Process(response.Result.Code);
        GetDispatch().Dispatch<P19_Response>(GetDispatchKey(msg.Key), response);
    }
    
    static void OnRecvMissionGet(HttpDispatcher.NodeMsg msg)
    {
        var response = P20_Response.Parser.ParseFrom(msg.Body);
        errorCodeProcesser.Process(response.Result.Code);
        GetDispatch().Dispatch<P20_Response>(GetDispatchKey(msg.Key), response);
    }
    
    static void OnRecvMissionComplete(HttpDispatcher.NodeMsg msg)
    {
        var response = P21_Response.Parser.ParseFrom(msg.Body);
        errorCodeProcesser.Process(response.Result.Code);
        GetDispatch().Dispatch<P21_Response>(GetDispatchKey(msg.Key), response);
    }
    
    static void OnRecvAdDiamond(HttpDispatcher.NodeMsg msg)
    {
        var response = P22_Response.Parser.ParseFrom(msg.Body);
        errorCodeProcesser.Process(response.Result.Code);
        GetDispatch().Dispatch<P22_Response>(GetDispatchKey(msg.Key), response);
    }

    static void OnRecvGoldPoolRecover(HttpDispatcher.NodeMsg msg)
    {
        var response = P23_Response.Parser.ParseFrom(msg.Body);
        errorCodeProcesser.Process(response.Result.Code);
        GetDispatch().Dispatch<P23_Response>(GetDispatchKey(msg.Key), response);
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

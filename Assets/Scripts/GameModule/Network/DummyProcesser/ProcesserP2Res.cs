using NetWorkModule.Dummy;
using Google.Protobuf;
using UnityEngine;
using NetWorkModule;

/// <summary>
/// 登录
/// </summary>
public class ProcesserP2Res : BaseDummyProcesser<P2_Request, P2_Response>
{
    public override P2_Response ProcessRequest(int msgId, P2_Request pbData)
    {
        var response = GetResponseData();

        NetWorkManager.HttpClient.SetPlayerId(response.PlayerId);
        NetWorkManager.HttpClient.SaveSessionKey(response.SessionKey, new byte[32], false);

        return response;
    }

    P2_Response GetResponseData()
    {
        var res = new P2_Response();
        res.PlayerId = 999999L;
        res.ServerTime = System.DateTime.Now.Ticks;
        res.SessionKey = "ODg4OGFhYWE5OTk5YmJiYjg4ODhhYWFhOTk5OWJiYmI=";
        res.IsPlatformServiceLinked = true;
        res.Result = new PBResult(){
            Code = 0
        };

        return res;
    }
}
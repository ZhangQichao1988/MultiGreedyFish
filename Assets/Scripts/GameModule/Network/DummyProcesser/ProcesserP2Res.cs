using NetWorkModule.Dummy;
using Google.Protobuf;
using UnityEngine;
using NetWorkModule;

/// <summary>
/// 登录
/// </summary>
public class ProcesserP2Res : IDummyResponseProcesser
{
    public IMessage ProcessRequest(int resId, IMessage pbData)
    {
        var request = pbData as P2_Request;
        var response = GetResponseData();

        NetWorkManager.HttpClient.SetPlayerId(response.PlayerId);
        NetWorkManager.HttpClient.SaveSessionKey(response.SessionKey, new byte[32], false);

        return response;
    } 

    public void DispatchRes(int resId, IMessage request, IMessage response)
    {
        NetWorkHandler.GetDispatch().Dispatch<P2_Response>(NetWorkHandler.GetDispatchKey(resId), response as P2_Response);
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
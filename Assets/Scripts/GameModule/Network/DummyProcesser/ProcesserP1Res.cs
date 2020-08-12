using NetWorkModule.Dummy;
using Google.Protobuf;
using UnityEngine;

/// <summary>
/// 三方登录
/// </summary>
public class ProcesserP1Res : BaseDummyProcesser<P1_Request, P1_Response>
{
    public override P1_Response ProcessRequest(int msgId, P1_Request pbData)
    {
        var response = GetResponseData();
        PlayerPrefs.SetString(NetworkConst.AUTH_KEY, response.AuthToken);
        PlayerPrefs.Save();

        return response;
    } 

    public override void DispatchRes(int msgId, P1_Request request, P1_Response response)
    {
        NetWorkHandler.GetDispatch().Dispatch<P1_Response, P1_Request>(NetWorkHandler.GetDispatchKey(msgId), response, request);
    }

    P1_Response GetResponseData()
    {
        var res = new P1_Response();
        res.AuthToken = "asggfssfdgfg";
        res.AuthKey = "88886666";
        res.Result = new PBResult(){
            Code = 0
        };

        return res;
    }
}
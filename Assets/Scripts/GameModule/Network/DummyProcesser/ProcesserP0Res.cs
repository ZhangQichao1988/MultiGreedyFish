using NetWorkModule.Dummy;
using Google.Protobuf;
using UnityEngine;

/// <summary>
/// 注册 
/// </summary>
public class ProcesserP0Res : BaseDummyProcesser<P0_Request, P0_Response>
{
    public override P0_Response ProcessRequest(int msgId, P0_Request pbData)
    {
        var response = GetResponseData();
        PlayerPrefs.SetString(NetworkConst.AUTH_KEY, response.AuthToken);
        PlayerPrefs.Save();
        
        return response;
    }

    P0_Response GetResponseData()
    {
        var res = new P0_Response();
        res.AuthToken = "asggfssfdgfg";
        res.AuthKey = "88886666";

        return res;
    }
}
using NetWorkModule.Dummy;
using Google.Protobuf;
using UnityEngine;

/// <summary>
/// 三方登录
/// </summary>
public class ProcesserP1Res : IDummyResponseProcesser
{
    public IMessage ProcessRequest(int resId, IMessage pbData)
    {
        var request = pbData as P1_Request;
        var response = GetResponseData();
        PlayerPrefs.SetString(NetworkConst.AUTH_KEY, response.AuthToken);
        PlayerPrefs.Save();

        return response;
    } 

    public void DispatchRes(int resId, IMessage request, IMessage response)
    {
        NetWorkHandler.GetDispatch().Dispatch<P1_Response, P1_Request>(NetWorkHandler.GetDispatchKey(resId), response as P1_Response, request as P1_Request);
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
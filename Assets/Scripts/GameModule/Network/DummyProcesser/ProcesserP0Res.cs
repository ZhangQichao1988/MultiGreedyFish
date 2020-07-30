using NetWorkModule.Dummy;
using Google.Protobuf;
using UnityEngine;

/// <summary>
/// 注册 
/// </summary>
public class ProcesserP0Res : IDummyResponseProcesser
{
    public IMessage ProcessRequest(int resId, IMessage pbData)
    {
        var request = pbData as P0_Request;
        var response = GetResponseData();
        PlayerPrefs.SetString(NetworkConst.AUTH_KEY, response.AuthToken);
        PlayerPrefs.Save();
        
        return response;
    } 

    public void DispatchRes(int resId, IMessage request, IMessage response)
    {
        NetWorkHandler.GetDispatch().Dispatch<P0_Response>(NetWorkHandler.GetDispatchKey(resId), response as P0_Response);
    }

    P0_Response GetResponseData()
    {
        var res = new P0_Response();
        res.AuthToken = "asggfssfdgfg";
        res.AuthKey = "88886666";

        return res;
    }
}
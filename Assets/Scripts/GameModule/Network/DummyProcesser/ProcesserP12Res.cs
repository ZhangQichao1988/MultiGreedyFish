using NetWorkModule.Dummy;
using Google.Protobuf;
using UnityEngine;
using pbc = global::Google.Protobuf.Collections;
using UnityEngine.UIElements;

/// <summary>
/// 修改昵称
/// </summary>
public class ProcesserP12Res : BaseDummyProcesser<P12_Request, P12_Response>
{
    public override P12_Response ProcessRequest(int msgId, P12_Request pbData)
    {
        var response = GetResponseData();

        return response;
        
    }

    P12_Response GetResponseData()
    {
        var res = new P12_Response();
        res.Result = new PBResult() { Code = 0 };
        res.ResultCode = PurchasedResponseKind.ProcessedSuccessfully;
        return res;
    }


    public override void DispatchRes(int msgId, P12_Request request, P12_Response response)
    {
        NetWorkHandler.GetDispatch().Dispatch<P12_Response, ShopItemVo>(NetWorkHandler.GetDispatchKey(msgId), response, GetCachedData() as ShopItemVo);
    }
}
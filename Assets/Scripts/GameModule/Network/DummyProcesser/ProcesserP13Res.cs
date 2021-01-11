using NetWorkModule.Dummy;
using Google.Protobuf;
using UnityEngine;
using pbc = global::Google.Protobuf.Collections;
using UnityEngine.UIElements;

/// <summary>
/// 修改昵称
/// </summary>
public class ProcesserP13Res : BaseDummyProcesser<P13_Request, P13_Response>
{
    public override P13_Response ProcessRequest(int msgId, P13_Request pbData)
    {
        var response = GetResponseData();

        return response;
        
    }

    P13_Response GetResponseData()
    {
        var res = new P13_Response();
        res.Result = new PBResult() { Code = 0 };
        res.ResultCode = PurchasedResponseKind.ProcessedSuccessfully;
        for (int i = 0; i < 10; i++)
        {
            var content = new ProductContent(){ ContentId = 1 + Random.Range(0, 2), Amount = i + 1 };
            res.Content.Add(content);
        }
        return res;
    }


    public override void DispatchRes(int msgId, P13_Request request, P13_Response response)
    {
        NetWorkHandler.GetDispatch().Dispatch<P13_Response, string>(NetWorkHandler.GetDispatchKey(msgId), response, GetCachedData().ToString());
    }
}
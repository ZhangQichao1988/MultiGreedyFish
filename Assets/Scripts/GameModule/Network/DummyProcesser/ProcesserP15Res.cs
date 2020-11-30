using NetWorkModule.Dummy;
using Google.Protobuf;
using UnityEngine;
using pbc = global::Google.Protobuf.Collections;
using UnityEngine.UIElements;

/// <summary>
/// 修改昵称
/// </summary>
public class ProcesserP15Res : BaseDummyProcesser<P15_Request, P15_Response>
{
    public override P15_Response ProcessRequest(int msgId, P15_Request pbData)
    {
        var response = GetResponseData();

        return response;
        
    }

    P15_Response GetResponseData()
    {
        var res = new P15_Response();
        res.Result = new PBResult() { Code = 0 };
        res.ResultCode = PurchasedResponseKind.ProcessedSuccessfully;
        for (int i = 0; i < 10; i++)
        {
            var content = new ProductContent(){ ContentId = 1 + Random.Range(0, 3), Amount = i + 1 };
            res.Content.Add(content);
        }
        return res;
    }


    public override void DispatchRes(int msgId, P15_Request request, P15_Response response)
    {
        NetWorkHandler.GetDispatch().Dispatch<P15_Response, string>(NetWorkHandler.GetDispatchKey(msgId), response, GetCachedData().ToString());
    }
}
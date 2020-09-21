using NetWorkModule.Dummy;
using Google.Protobuf;
using UnityEngine;
using pbc = global::Google.Protobuf.Collections;
using UnityEngine.UIElements;

/// <summary>
/// 修改昵称
/// </summary>
public class ProcesserP11Res : BaseDummyProcesser<P11_Request, P11_Response>
{
    public override P11_Response ProcessRequest(int msgId, P11_Request pbData)
    {
        var response = GetResponseData();

        return response;
        
    }

    P11_Response GetResponseData()
    {
        var res = new P11_Response();
        res.Result = new PBResult() { Code = 0 };
        for (int i = 0; i < 10; i++)
        {
            var content = new ProductContent(){ ContentId = 1 + Random.Range(0, 3), Amount = i + 1 };
            res.Content.Add(content);
        }

        res.IsTreasure = true;
        for (int i = 0; i < 3; i++)
        {
            var content = new ProductContent(){ ContentId = 1 + Random.Range(0, 3), Amount = i + 1 };
            res.TreaContent.Add(content);
        }
        return res;
    }


    public override void DispatchRes(int msgId, P11_Request request, P11_Response response)
    {
        NetWorkHandler.GetDispatch().Dispatch<P11_Response, P11_Request>(NetWorkHandler.GetDispatchKey(msgId), response, request);
    }
}
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
        var content = new ProductContent() { ContentId = 4, Amount = 1 };
        res.Content.Add(new ProductContent() { ContentId = 20, Amount = 100 });
        res.Content.Add(new ProductContent() { ContentId = 60, Amount = 50 });

        //res.Content.Add(content);

        //res.IsTreasure = true;
        //res.TreaContent.Add(new ProductContent() { ContentId = 60, Amount = 100 });
        //res.TreaContent.Add(new ProductContent() { ContentId = 100, Amount = 5 });
        //res.TreaContent.Add(new ProductContent() { ContentId = 600, Amount = 10 });
        //res.TreaContent.Add(new ProductContent() { ContentId = 700, Amount = 7 });
        return res;
    }


    public override void DispatchRes(int msgId, P11_Request request, P11_Response response)
    {
        NetWorkHandler.GetDispatch().Dispatch<P11_Response, P11_Request, ShopItemVo>(NetWorkHandler.GetDispatchKey(msgId), response, request, GetCachedData() as ShopItemVo);
    }
}
using NetWorkModule.Dummy;
using Google.Protobuf;
using UnityEngine;
using pbc = global::Google.Protobuf.Collections;
using UnityEngine.UIElements;

/// <summary>
/// 修改昵称
/// </summary>
public class ProcesserP9Res : BaseDummyProcesser<P9_Request, P9_Response>
{
    public override P9_Response ProcessRequest(int msgId, P9_Request pbData)
    {
        var response = GetResponseData();

        return response;
        
    }

    P9_Response GetResponseData()
    {
        var res = new P9_Response();
        res.Result = new PBResult() { Code = 0 };
        return res;
    }
}
using NetWorkModule.Dummy;
using Google.Protobuf;
using UnityEngine;
using pbc = global::Google.Protobuf.Collections;
using UnityEngine.UIElements;

/// <summary>
/// 获取玩家信息
/// </summary>
public class ProcesserP6Res : BaseDummyProcesser<P6_Request, P6_Response>
{
    public override P6_Response ProcessRequest(int msgId, P6_Request pbData)
    {
        var response = GetResponseData();

        return response;
        
    }

    P6_Response GetResponseData()
    {
        var res = new P6_Response();
        res.Result = new PBResult() { Code = 0 };

        return res;
    }
}
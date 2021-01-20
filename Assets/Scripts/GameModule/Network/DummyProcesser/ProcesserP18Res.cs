using NetWorkModule.Dummy;
using Google.Protobuf;
using UnityEngine;
using pbc = global::Google.Protobuf.Collections;
using UnityEngine.UIElements;

/// <summary>
/// 鱼升级
/// </summary>
public class ProcesserP18Res : BaseDummyProcesser<NullMessage, P18_Response>
{
    public override P18_Response ProcessRequest(int msgId, NullMessage pbData)
    {
        var response = GetResponseData();

        return response;
        
    }

    P18_Response GetResponseData()
    {
        var res = new P18_Response();
        res.Result = new PBResult() { Code = 0 };
        return res;
    }
}
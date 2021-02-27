using NetWorkModule.Dummy;
using Google.Protobuf;
using UnityEngine;
using pbc = global::Google.Protobuf.Collections;
using UnityEngine.UIElements;

/// <summary>
/// 鱼升级
/// </summary>
public class ProcesserP23Res : BaseDummyProcesser<NullMessage, P23_Response>
{
    public override P23_Response ProcessRequest(int msgId, NullMessage pbData)
    {
        var response = GetResponseData();

        return response;
        
    }

    P23_Response GetResponseData()
    {
        var res = new P23_Response();
        res.Result = new PBResult() { Code = 0 };
        return res;
    }
}
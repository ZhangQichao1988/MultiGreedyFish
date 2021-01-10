using NetWorkModule.Dummy;
using Google.Protobuf;
using UnityEngine;
using pbc = global::Google.Protobuf.Collections;
using UnityEngine.UIElements;

/// <summary>
/// 获取已获得段位奖励ID
/// </summary>
public class ProcesserP16Res : BaseDummyProcesser<NullMessage, P16_Response>
{
    public override P16_Response ProcessRequest(int msgId, NullMessage pbData)
    {
        var response = GetResponseData();

        return response;
        
    }

    P16_Response GetResponseData()
    {
        var res = new P16_Response();
        res.Result = new PBResult() { Code = 0 };
        //res.GettedBonusIds.Add();
        return res;
    }

}
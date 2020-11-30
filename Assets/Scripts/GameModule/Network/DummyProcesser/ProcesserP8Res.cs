using NetWorkModule.Dummy;
using Google.Protobuf;
using UnityEngine;
using pbc = global::Google.Protobuf.Collections;
using UnityEngine.UIElements;

/// <summary>
/// 获取战役奖励
/// </summary>
public class ProcesserP8Res : BaseDummyProcesser<P8_Request, P8_Response>
{
    public override P8_Response ProcessRequest(int msgId, P8_Request pbData)
    {
        var response = GetResponseData();

        return response;
        
    }

    P8_Response GetResponseData()
    {
        var res = new P8_Response();
        res.Result = new PBResult() { Code = 0 };
        res.RewardMoney = 999;
        return res;
    }
}
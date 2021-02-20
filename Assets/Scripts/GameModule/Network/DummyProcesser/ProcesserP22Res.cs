using NetWorkModule.Dummy;
using Google.Protobuf;
using UnityEngine;
using pbc = global::Google.Protobuf.Collections;
using UnityEngine.UIElements;

/// <summary>
/// 获取已获得段位奖励ID
/// </summary>
public class ProcesserP22Res : BaseDummyProcesser<NullMessage, P22_Response>
{
    public override P22_Response ProcessRequest(int msgId, NullMessage pbData)
    {
        var response = GetResponseData();

        return response;
        
    }

    P22_Response GetResponseData()
    {
        var res = new P22_Response();
        res.Result = new PBResult() { Code = 0 };
        res.AdvertRewardRemainingCnt = 4;
        return res;
    }

}
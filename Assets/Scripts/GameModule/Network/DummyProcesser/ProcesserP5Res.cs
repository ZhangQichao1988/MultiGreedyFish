using NetWorkModule.Dummy;
using Google.Protobuf;
using UnityEngine;
using pbc = global::Google.Protobuf.Collections;
using UnityEngine.UIElements;

/// <summary>
/// 获取玩家信息
/// </summary>
public class ProcesserP5Res : BaseDummyProcesser<P5_Request, P5_Response>
{
    public override P5_Response ProcessRequest(int msgId, P5_Request pbData)
    {
        var response = GetResponseData();

        return response;
    }

    P5_Response GetResponseData()
    {
        var res = new P5_Response();
        res.Result = new PBResult() { Code = 0 };
        res.GainRankLevel = 20;
        //获得的金币
        res.GainGold = 10;
        res.GainRankLevelupBonusGold = 20;

        return res;
    }
}
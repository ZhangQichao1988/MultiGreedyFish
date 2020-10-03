using NetWorkModule.Dummy;
using Google.Protobuf;
using UnityEngine;
using pbc = global::Google.Protobuf.Collections;
using UnityEngine.UIElements;

/// <summary>
/// 获取玩家信息
/// </summary>
public class ProcesserP14Res : BaseDummyProcesser<NullMessage, P14_Response>
{
    public override P14_Response ProcessRequest(int msgId, NullMessage pbData)
    {
        var response = GetResponseData();

        return response;
    }

    P14_Response GetResponseData()
    {
        var res = new P14_Response();
        res.Level = 1;
        res.CurrGold = 50;
        res.CurrTime = 1601719931;
        res.NextAt = 1601719941;
        res.FullAt = 1601720181;
        return res;
    }
}
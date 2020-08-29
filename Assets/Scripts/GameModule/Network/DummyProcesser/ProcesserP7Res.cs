using NetWorkModule.Dummy;
using Google.Protobuf;
using UnityEngine;
using pbc = global::Google.Protobuf.Collections;
using UnityEngine.UIElements;

/// <summary>
/// 鱼升级
/// </summary>
public class ProcesserP7Res : BaseDummyProcesser<P7_Request, P7_Response>
{
    private P7_Request request;
    public override P7_Response ProcessRequest(int msgId, P7_Request pbData)
    {
        request = pbData;
        var response = GetResponseData();

        return response;
        
    }

    P7_Response GetResponseData()
    {
        var res = new P7_Response();
        res.Result = new PBResult() { Code = 0 };
        res.FishInfo = new PBPlayerFishLevelInfo(){
            FishId = request.FishId,
            FishLevel = 2,
            FishChip = 2,
            RankLevel = 1
        };
        return res;
    }
}
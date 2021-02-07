using NetWorkModule.Dummy;
using Google.Protobuf;
using UnityEngine;
using pbc = global::Google.Protobuf.Collections;
using UnityEngine.UIElements;

/// <summary>
/// 获取已获得段位奖励ID
/// </summary>
public class ProcesserP21Res : BaseDummyProcesser<NullMessage, P21_Response>
{
    public override P21_Response ProcessRequest(int msgId, NullMessage pbData)
    {
        var response = GetResponseData();

        return response;
        
    }

    P21_Response GetResponseData()
    {
        var res = new P21_Response();
        res.Result = new PBResult() { Code = 0 };
        //res.GettedBonusIds.Add();
        var content = new ProductContent() { ContentId = 4, Amount = 1 };
        res.Content.Add(content);

        res.IsTreasure = true;
        for (int i = 0; i < 3; i++)
        {
            content = new ProductContent() { ContentId = 1 + Random.Range(0, 2), Amount = i + 1 };
            res.TreaContent.Add(content);
        }
        res.NewMission = new PBMission() { MissionId = 1, ActionId = 1, IsComplete = false, CurrTrigger = 50, Trigger = 100, Type = MissionType.MissionDaily, Reward = "[{\"id\":2, \"amount\":200},{\"id\":100, \"amount\":15}]" };

        return res;
    }

}
using NetWorkModule.Dummy;
using Google.Protobuf;
using UnityEngine;
using pbc = global::Google.Protobuf.Collections;
using UnityEngine.UIElements;

/// <summary>
/// 获取战斗信息
/// </summary>
public class ProcesserP20Res : BaseDummyProcesser<NullMessage, P20_Response>
{
    public override P20_Response ProcessRequest(int msgId, NullMessage pbData)
    {
        var response = GetResponseData();
        return response;
        
    }

    P20_Response GetResponseData()
    {
        var res = new P20_Response();
        res.Result = new PBResult() { Code = 0 };
        res.MissionList.Add(new PBMission() { MissionId = 1, ActionId = 1, IsComplete = false, CurrTrigger = 30, Trigger = 50, Type = MissionType.MissionDaily, Reward = "[{\"id\":2, \"amount\":100},{\"id\":100, \"amount\":5}]" });
        res.MissionList.Add(new PBMission() { MissionId = 2, ActionId = 2, IsComplete = false, CurrTrigger = 30, Trigger = 100, Type = MissionType.MissionWeekly, Reward = "[{\"id\":2, \"amount\":100}]" });
        res.MissionList.Add(new PBMission() { MissionId = 3, ActionId = 3, IsComplete = true, CurrTrigger = 30, Trigger = 80, Type = MissionType.MissionAchievement, Reward = "[{\"id\":2, \"amount\":100}]" });
        res.MissionList.Add(new PBMission() { MissionId = 4, ActionId = 4, IsComplete = false, CurrTrigger = 30, Trigger = 50, Type = MissionType.MissionAchievement, Reward = "[{\"id\":2, \"amount\":100}]" });
        return res;
    }
}
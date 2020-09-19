using NetWorkModule.Dummy;
using Google.Protobuf;
using UnityEngine;
using pbc = global::Google.Protobuf.Collections;
using UnityEngine.UIElements;

/// <summary>
/// 获取战斗信息
/// </summary>
public class ProcesserP4Res : BaseDummyProcesser<NullMessage, P4_Response>
{
    public override P4_Response ProcessRequest(int msgId, NullMessage pbData)
    {
        var response = GetResponseData();
        return response;
        
    }

    P4_Response GetResponseData()
    {
        var res = new P4_Response();
        res.Result = new PBResult() { Code = 0 };

        res.AryEnemyDataInfo.Add(new PBEnemyDataInfo() { FishId = 0, FishLevel = 1, FishCountMin = 5, FishCountMax = 100 });
        res.AryEnemyDataInfo.Add(new PBEnemyDataInfo() { FishId = 4, FishLevel = 3, FishCountMin = 2, FishCountMax = 40 });
        res.AryEnemyDataInfo.Add(new PBEnemyDataInfo() { FishId = 5, FishLevel = 5, FishCountMin = 1, FishCountMax = 5 });

        res.AryRobotDataInfo.Add(new PBRobotDataInfo() { FishId = 1, AiId = 1, Level = 1 });
        res.AryRobotDataInfo.Add(new PBRobotDataInfo() { FishId = 1, AiId = 1, Level = 1 });
        res.AryRobotDataInfo.Add(new PBRobotDataInfo() { FishId = 1, AiId = 1, Level = 1 });
        res.AryRobotDataInfo.Add(new PBRobotDataInfo() { FishId = 1, AiId = 0, Level = 1 });
        res.AryRobotDataInfo.Add(new PBRobotDataInfo() { FishId = 1, AiId = 0, Level = 1 });
        res.AryRobotDataInfo.Add(new PBRobotDataInfo() { FishId = 1, AiId = 0, Level = 1 });
        res.AryRobotDataInfo.Add(new PBRobotDataInfo() { FishId = 1, AiId = 0, Level = 1 });
        res.AryRobotDataInfo.Add(new PBRobotDataInfo() { FishId = 1, AiId = 2, Level = 1 });
        res.AryRobotDataInfo.Add(new PBRobotDataInfo() { FishId = 1, AiId = 2, Level = 1 });

        res.BattleId = "11111";

        //res.StageInfo.AryRobotDataInfo.Add(new PBRobotDataInfo() { FishId = 2, AiId = 3 });
        return res;
    }
}
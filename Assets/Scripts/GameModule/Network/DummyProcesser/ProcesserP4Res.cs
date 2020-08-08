using NetWorkModule.Dummy;
using Google.Protobuf;
using UnityEngine;
using pbc = global::Google.Protobuf.Collections;
using UnityEngine.UIElements;

/// <summary>
/// 获取玩家信息
/// </summary>
public class ProcesserP4Res : IDummyResponseProcesser
{
    public IMessage ProcessRequest(int msgId, IMessage pbData)
    {
        var response = GetResponseData();

        return response;
        
    } 

    public void DispatchRes(int msgId, IMessage request, IMessage response)
    {
        NetWorkHandler.GetDispatch().Dispatch<P4_Response>(NetWorkHandler.GetDispatchKey(msgId), response as P4_Response);
    }

    P4_Response GetResponseData()
    {
        var res = new P4_Response();
        res.Result = new PBResult() { Code = 0 };

        res.StageInfo = new PBStageInfo();
        res.StageInfo.AryEnemyDataInfo.Add(new PBEnemyDataInfo() { FishId = 0, FishCountMin = 5, FishCountMax = 100 });
        res.StageInfo.AryRobotDataInfo.Add(new PBRobotDataInfo() { FishId = 1, AiId = 1 });
        res.StageInfo.AryRobotDataInfo.Add(new PBRobotDataInfo() { FishId = 1, AiId = 1 });
        res.StageInfo.AryRobotDataInfo.Add(new PBRobotDataInfo() { FishId = 1, AiId = 1 });
        res.StageInfo.AryRobotDataInfo.Add(new PBRobotDataInfo() { FishId = 1, AiId = 0 });
        res.StageInfo.AryRobotDataInfo.Add(new PBRobotDataInfo() { FishId = 1, AiId = 0 });
        res.StageInfo.AryRobotDataInfo.Add(new PBRobotDataInfo() { FishId = 1, AiId = 0 });
        res.StageInfo.AryRobotDataInfo.Add(new PBRobotDataInfo() { FishId = 1, AiId = 0 });
        res.StageInfo.AryRobotDataInfo.Add(new PBRobotDataInfo() { FishId = 1, AiId = 2 });
        res.StageInfo.AryRobotDataInfo.Add(new PBRobotDataInfo() { FishId = 1, AiId = 2 });

        //res.StageInfo.AryRobotDataInfo.Add(new PBRobotDataInfo() { FishId = 2, AiId = 3 });
        return res;
    }
}
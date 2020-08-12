using NetWorkModule.Dummy;
using Google.Protobuf;
using UnityEngine;

/// <summary>
/// 获取玩家信息
/// </summary>
public class ProcesserP3Res : BaseDummyProcesser<NullMessage, P3_Response>
{
    public override P3_Response ProcessRequest(int msgId, NullMessage pbData)
    {
        var response = GetResponseData();

        return response;
        
    }

    P3_Response GetResponseData()
    {
        var res = new P3_Response();
        res.Player = new PBPlayer(){
            PlayerId = 9999999L,
            FightFish = 3,
            Gold = 10,
            Diamond = 10,
            Nickname = "Dummy",
            Power = 100,
            PowerAt = System.DateTime.Now.Ticks + 3600L * 1000L
        };
        res.Result = new PBResult(){
            Code = 0
        };
        res.Player.AryPlayerFishInfo.Add(new PBPlayerFishLevelInfo() { FishId = 0, FishChip = 10, FishLevel = 2, RankLevel = 3 });
        res.Player.AryPlayerFishInfo.Add(new PBPlayerFishLevelInfo() { FishId = 1, FishChip = 5, FishLevel = 1, RankLevel = 6 });
        res.Player.AryPlayerFishInfo.Add(new PBPlayerFishLevelInfo() { FishId = 3, FishChip = 5, FishLevel = 1, RankLevel = 6 });

        return res;
    }
}
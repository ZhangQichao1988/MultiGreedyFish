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
        res.Player = new PBPlayer() {
            PlayerId = 9999999L,
            FaceIconId = 1,
            FightFish = 8,
            Gold = 10,
            Diamond = 10,
            Nickname = "孙小杰",
            Power = 100,
            PowerAt = System.DateTime.Now.Ticks + 3600L * 1000L
        };
        res.Result = new PBResult(){
            Code = 0
        };
        res.Player.AryPlayerFishInfo.Add(new PBPlayerFishLevelInfo() { FishId = 1, FishChip = 5, FishLevel = 1, RankLevel = 115 });
        res.Player.AryPlayerFishInfo.Add(new PBPlayerFishLevelInfo() { FishId = 3, FishChip = 15, FishLevel = 1, RankLevel = 6 });
        res.Player.AryPlayerFishInfo.Add(new PBPlayerFishLevelInfo() { FishId = 6, FishChip = 5, FishLevel = 1, RankLevel = 2 });
        res.Player.AryPlayerFishInfo.Add(new PBPlayerFishLevelInfo() { FishId = 7, FishChip = 5, FishLevel = 1, RankLevel = 0 });
        res.Player.AryPlayerFishInfo.Add(new PBPlayerFishLevelInfo() { FishId = 8, FishChip = 5, FishLevel = 1, RankLevel = 0 });
        res.Player.AryPlayerFishInfo.Add(new PBPlayerFishLevelInfo() { FishId = 9, FishChip = 15, FishLevel = 0, RankLevel = 0 });
        res.Player.AryPlayerFishInfo.Add(new PBPlayerFishLevelInfo() { FishId = 10, FishChip = 5, FishLevel = 0, RankLevel = 0 });
        res.Player.AryPlayerFishInfo.Add(new PBPlayerFishLevelInfo() { FishId = 11, FishChip = 5, FishLevel = 0, RankLevel = 0 });

        return res;
    }
}
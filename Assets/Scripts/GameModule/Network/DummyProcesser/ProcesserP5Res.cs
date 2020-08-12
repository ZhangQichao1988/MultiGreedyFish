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
        res.Player = new PBPlayer()
        {
            PlayerId = 9999999L,
            FightFish = 1,
            Gold = 100,
            Diamond = 999999,
            Nickname = "Dummy",
            Power = 100,
            PowerAt = System.DateTime.Now.Ticks + 3600L * 1000L
        };
        res.Player.AryPlayerFishInfo.Add(new PBPlayerFishLevelInfo() { FishId = 0, FishChip = 10, FishLevel = 2, RankLevel = 3 });
        res.Player.AryPlayerFishInfo.Add(new PBPlayerFishLevelInfo() { FishId = 1, FishChip = 5, FishLevel = 1, RankLevel = 10 });

        //获得的金币
        res.GainGold = 10;

        return res;
    }
}
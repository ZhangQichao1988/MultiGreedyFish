using NetWorkModule.Dummy;
using Google.Protobuf;
using UnityEngine;
using pbc = global::Google.Protobuf.Collections;
using UnityEngine.UIElements;

/// <summary>
/// 获取玩家信息
/// </summary>
public class ProcesserP5Res : IDummyResponseProcesser
{
    public IMessage ProcessRequest(int msgId, IMessage pbData)
    {
        var response = GetResponseData();

        return response;
        
    } 

    public void DispatchRes(int msgId, IMessage request, IMessage response)
    {
        NetWorkHandler.GetDispatch().Dispatch<P5_Response>(NetWorkHandler.GetDispatchKey(msgId), response as P5_Response);
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
            Rank = 2,
            Power = 100,
            PowerAt = System.DateTime.Now.Ticks + 3600L * 1000L
        };
        res.Player.AryPlayerFishInfo.Add(new PBPlayerFishLevelInfo() { FishId = 0, FishChip = 10, FishLevel = 2, RankLevel = 3 });
        res.Player.AryPlayerFishInfo.Add(new PBPlayerFishLevelInfo() { FishId = 1, FishChip = 5, FishLevel = 1, RankLevel = 10 });

        return res;
    }
}
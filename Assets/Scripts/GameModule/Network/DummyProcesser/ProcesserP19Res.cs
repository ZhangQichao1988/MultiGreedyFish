using NetWorkModule.Dummy;
using Google.Protobuf;
using UnityEngine;
using pbc = global::Google.Protobuf.Collections;
using UnityEngine.UIElements;

/// <summary>
/// 获取玩家信息
/// </summary>
public class ProcesserP19Res : BaseDummyProcesser<P19_Request, P19_Response>
{
    public override P19_Response ProcessRequest(int msgId, P19_Request pbData)
    {
        var response = GetResponseData();

        return response;
    }

    P19_Response GetResponseData()
    {
        var res = new P19_Response();
        res.Result = new PBResult() { Code = 0 };
        res.RankBatch = 1;
        res.Rank = 4;
        res.RankRate = 100;
        res.RankPlayerList.Add(new RankPlayer() { Nickname = "草雉牙牙1", PlayerId = 334978252, Score = 5 });
        res.RankPlayerList.Add(new RankPlayer() { Nickname = "草雉牙牙2", PlayerId = 334978252, Score = 5 });
        res.RankPlayerList.Add(new RankPlayer() { Nickname = "草雉牙牙3", PlayerId = 334978252, Score = 5 });
        res.RankPlayerList.Add(new RankPlayer() { Nickname = "草雉牙牙4", PlayerId = 334978252, Score = 5 });
        res.RankPlayerList.Add(new RankPlayer() { Nickname = "草雉牙牙1", PlayerId = 334978252, Score = 5 });
        res.RankPlayerList.Add(new RankPlayer() { Nickname = "草雉牙牙1", PlayerId = 334978252, Score = 5 });
        res.RankPlayerList.Add(new RankPlayer() { Nickname = "草雉牙牙6", PlayerId = 334978252, Score = 5 });
        res.RankPlayerList.Add(new RankPlayer() { Nickname = "草雉牙牙1", PlayerId = 334978252, Score = 5 });
        res.RankPlayerList.Add(new RankPlayer() { Nickname = "草雉牙牙1", PlayerId = 334978252, Score = 5 });
        res.RankPlayerList.Add(new RankPlayer() { Nickname = "草雉牙牙1", PlayerId = 334978252, Score = 5 });
        res.RankPlayerList.Add(new RankPlayer() { Nickname = "草雉牙牙1", PlayerId = 334978252, Score = 5 });
        res.RankPlayerList.Add(new RankPlayer() { Nickname = "草雉牙牙1", PlayerId = 334978252, Score = 5 });
        res.RankPlayerList.Add(new RankPlayer() { Nickname = "草雉牙牙1", PlayerId = 334978252, Score = 5 });
        res.RankPlayerList.Add(new RankPlayer() { Nickname = "草雉牙牙1", PlayerId = 334978252, Score = 5 });
        res.RankPlayerList.Add(new RankPlayer() { Nickname = "草雉牙牙1", PlayerId = 334978252, Score = 5 });
        res.RankPlayerList.Add(new RankPlayer() { Nickname = "草雉牙牙1", PlayerId = 334978252, Score = 5 });
        res.RankPlayerList.Add(new RankPlayer() { Nickname = "草雉牙牙1", PlayerId = 334978252, Score = 5 });
        res.RankPlayerList.Add(new RankPlayer() { Nickname = "草雉牙牙1", PlayerId = 334978252, Score = 5 });
        res.RankPlayerList.Add(new RankPlayer() { Nickname = "草雉牙牙65", PlayerId = 334978252, Score = 5 });
        res.RankPlayerList.Add(new RankPlayer() { Nickname = "草雉牙牙43", PlayerId = 334978252, Score = 5 });
        return res;
    }
}
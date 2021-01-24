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
        var rankReward = new PBRankReward();
        rankReward.Rank = 8;
        rankReward.RankRate = 50.48f;
        rankReward.RankBatch = 2;
        rankReward.Content.Add(new ProductContent() { ContentId = 20, Amount = 50 });
        rankReward.Content.Add(new ProductContent() { ContentId = 100, Amount = 50 });
        rankReward.Content.Add(new ProductContent() { ContentId = 200, Amount = 50 });
        rankReward.Content.Add(new ProductContent() { ContentId = 300, Amount = 50 });
        rankReward.Content.Add(new ProductContent() { ContentId = 400, Amount = 50 });
        res.RankReward = rankReward;
        res.RankBatch = 1;
        res.Rank = 10;
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
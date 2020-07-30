using NetWorkModule.Dummy;
using Google.Protobuf;
using UnityEngine;

/// <summary>
/// 获取玩家信息
/// </summary>
public class ProcesserP3Res : IDummyResponseProcesser
{
    public IMessage ProcessRequest(int resId, IMessage pbData)
    {
        var response = GetResponseData();

        return response;
        
    } 

    public void DispatchRes(int resId, IMessage request, IMessage response)
    {
        NetWorkHandler.GetDispatch().Dispatch<P3_Response>(NetWorkHandler.GetDispatchKey(resId), response as P3_Response);
    }

    P3_Response GetResponseData()
    {
        var res = new P3_Response();
        res.Player = new PBPlayer(){
            PlayerId = 9999999L,
            FightFish = 1,
            Gold = 999999,
            Diamond = 999999,
            Nickname = "Dummy",
            Rank = 2,
            Power = 100,
            PowerAt = System.DateTime.Now.Ticks + 3600L * 1000L
        };
        res.Result = new PBResult(){
            Code = 0
        };

        return res;
    }
}
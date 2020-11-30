using System.Linq;
using UnityEngine;


public class StageModel : BaseModel<StageModel>
{
    public P5_Response resultResponse;
    public string battleId = "";     
    public int battleRanking;   // 战斗排名
    public PBEnemyDataInfo[] aryEnemyDataInfo;
    public PBRobotDataInfo[] aryRobotDataInfo;
    public StageModel() : base()
    {

    }

    public void SetStartBattleRes(P4_Response realResponse)
    {
        aryEnemyDataInfo = realResponse.AryEnemyDataInfo.ToArray<PBEnemyDataInfo>();
        battleId = realResponse.BattleId;
        aryRobotDataInfo = realResponse.AryRobotDataInfo.ToArray<PBRobotDataInfo>();
    }

}
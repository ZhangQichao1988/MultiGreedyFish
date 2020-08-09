using System.Linq;
using UnityEngine;


public class StageModel : BaseModel<StageModel>
{
    public string BattleId;
    public PBEnemyDataInfo[] aryEnemyDataInfo;
    public PBRobotDataInfo[] aryRobotDataInfo;
    public StageModel() : base()
    {

    }

    public void SetStartBattleRes(P4_Response realResponse)
    {
        aryEnemyDataInfo = realResponse.AryEnemyDataInfo.ToArray<PBEnemyDataInfo>();
        BattleId = realResponse.BattleId;
        aryRobotDataInfo = realResponse.AryRobotDataInfo.ToArray<PBRobotDataInfo>();
    }

}
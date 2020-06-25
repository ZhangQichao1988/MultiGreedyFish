using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRobot : PlayerBase
{


    protected override bool showLifeGauge { get { return false; } }

    public override FishType fishType { get { return FishType.PlayerRobot; } }

	public override void Init(Data data)
	{
		base.Init(data);
    }
	
    void CalcMoveAction()
    {
        // 追踪附近比自己小的离最近的鱼
        List<FishBase> listFish = ManagerGroup.GetInstance().fishManager.GetEnemiesInRange( this, transform.position, GameConst.RobotFindFishRange );
        if (listFish.Count > 0)
        {
            listFish.Sort((a, b) => { return (int)(Vector3.Distance(a.transform.position, transform.position) - Vector3.Distance(b.transform.position, transform.position)); });
            FishBase target = listFish[0];
            Dir = target.transform.position - transform.position;
            Dir.Normalize();
        }
        else
        {
            EnemyIdle();
        }
        
    }
    public override void CustomUpdate()
    {
        CalcMoveAction();

        base.CustomUpdate();
    }
}

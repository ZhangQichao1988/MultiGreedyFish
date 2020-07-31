using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSkillSuck : FishSkillBase
{
	public override bool Skill()
	{
		var listFish = BattleManagerGroup.GetInstance().fishManager.GetEnemiesInRange(playerBase, playerBase.transform.position, BattleConst.RobotVision);
		Vector3 playerToTargetDir;
		float dot;
		float distance;
		for (int i = 0; i < listFish.Count; ++i)
		{
			playerToTargetDir = listFish[i].transform.position - playerBase.transform.position;
			distance = playerToTargetDir.magnitude;
			playerToTargetDir = Vector3.Normalize(playerToTargetDir);
			dot = Vector3.Dot(playerBase.transModel.transform.forward, playerToTargetDir);
			if (dot > 0.5f && distance <= listParam[0])
			{
				listFish[i].AddBuff(playerBase, 1);

			}
		}

		return true;
	}
	public override void CbAttack()
	{
		currentGauge += listParam[1];
	}
}

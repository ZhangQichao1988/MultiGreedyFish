using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSkillHeadLife : FishSkillBase
{
	public override bool Skill()
	{
		if (playerBase.lifeRate >= 1f)
		{ return false; }
		int healLife = (int)((float)playerBase.lifeMax * listParam[0]);
		playerBase.life += System.Math.Min(healLife, playerBase.lifeMax - playerBase.life);
		

		return true;
	}
	public override void CbAttack()
	{
		currentGauge += listParam[1];
	}
}

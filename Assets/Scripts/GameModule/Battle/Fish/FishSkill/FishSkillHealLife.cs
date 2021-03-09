using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSkillHealLife : FishSkillBase
{
	public override bool Skill()
	{
		if (playerBase.lifeRate >= 1f)
		{ return false; }
		playerBase.AddBuff(playerBase, (int)listParam[1]);

		return true;
	}

	public override void CalcAI()
	{
		if (currentGauge < 1) { return; }
		int healLife = (int)((float)playerBase.lifeMax * listParam[1]);

		// 若有受伤buff，就开启膨胀技能
		if (playerBase.lifeRate < 0.5f)
		{
			RunSkill();
		}
	}
}

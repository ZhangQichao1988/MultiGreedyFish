using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSkillHealLife : FishSkillBase
{
	public override bool Skill()
	{
		if (playerBase.lifeRate >= 1f)
		{ return false; }
		int healLife = (int)((float)playerBase.lifeMax * listParam[1]);
		playerBase.Heal(System.Math.Min(healLife, playerBase.lifeMax - playerBase.life));
		if (playerBase.isBecameInvisible) { BattleEffectManager.CreateEffect(0, playerBase.transform); }

		return true;
	}
	public override void CbAttack()
	{
		currentGauge += listParam[0];
	}
	public override void CalcAI()
	{
		if (currentGauge < 1) { return; }
		int healLife = (int)((float)playerBase.lifeMax * listParam[1]);

		// 若有受伤buff，就开启膨胀技能
		if (playerBase.lifeMax - playerBase.life > healLife)
		{
			RunSkill();
		}
	}
}

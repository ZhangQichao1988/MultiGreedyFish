using GooglePlayGames.BasicApi.Multiplayer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSkillStealth : FishSkillBase
{

	public override bool Skill()
	{
		// 无敌buff
		BuffBase buff = playerBase.AddBuff(playerBase, (int)listParam[1]);

		return true;
	}
    public override void CbAttack()
	{
		currentGauge += listParam[0];
	}
    public override void CalcAI()
    {
		if (currentGauge < 1) { return; }
		RunSkill();
	}
}

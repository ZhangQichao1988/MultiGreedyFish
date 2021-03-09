using GooglePlayGames.BasicApi.Multiplayer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSkillInvincible : FishSkillBase
{

	public override bool Skill()
	{
		// 无敌buff
		BuffBase buff = playerBase.AddBuff(playerBase, (int)listParam[2]);
		if (buff != null) { buff.remainingTime = listParam[1]; }

		return true;
	}

    public override void CalcAI()
    {
		if (currentGauge < 1) { return; }
		// 若有受伤buff，就发动技能
		if (playerBase.ContainsBuff(0))
		{
			RunSkill();
		}
    }
}

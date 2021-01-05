using GooglePlayGames.BasicApi.Multiplayer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 高压电场
/// </summary>
public class FishSkillVoltage : FishSkillBase
{
	float remainingTime;
	int step = 0;
	int buffId;

	Effect effect;
	public override bool Skill()
	{
		if (remainingTime > 0f) { return false; }
		remainingTime = listParam[1];
		buffId = (int)listParam[2];
		step = 1;

		SoundManager.PlaySE(16, playerBase.audioSource);
		var effectId = BattleEffectManager.CreateEffect(8, playerBase.transModel);
		effect = EffectManager.GetEffect(effectId);

		return true;
	}
    public override void Update()
    {
		if (step  == 0) { return; }
		
		switch (step)
		{
			case 1:
				// 伤害判定
				var listFish = BattleManagerGroup.GetInstance().fishManager.GetEnemiesInRange(playerBase, playerBase.transform.position, BattleConst.instance.RobotVision);
				for (int i = 0; i < listFish.Count; ++i)
				{
					if (listFish[i].fishType == FishBase.FishType.Enemy) { continue; }
					if(playerBase.colliderBody.bounds.Intersects(listFish[i].colliderBody.bounds))
					{
						if (!listFish[i].ContainsBuffType(BuffBase.BuffType.Shield))
						{
							if (!listFish[i].ContainsBuff(buffId))
							{
								var buff = listFish[i].AddBuff(playerBase, buffId);
								BattleEffectManager.CreateEffect(5, listFish[i].transModel, listFish[i].transform.localScale.x);
							}
						}
					}
				}
				remainingTime -= Time.deltaTime;
				if (remainingTime <= 0f)
				{
					effect.Destroy();
					step = 0;
				}
				break;
		}
		
	}
    public override void CbAttack()
	{
		currentGauge += listParam[0];
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

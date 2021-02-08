using GooglePlayGames.BasicApi.Multiplayer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSkillShield : FishSkillBase
{
	float remainingTime;
	int step = 0;
	BuffBase buff;

	// 吸收次数
	int defenseCnt;

	Effect effect;
	public override bool Skill()
	{
		remainingTime = listParam[1];
		defenseCnt = (int)listParam[2];
		step = 1;

		SoundManager.PlaySE(14, playerBase.audioSource);
		var effectId = BattleEffectManager.CreateEffect(7, playerBase.transModel);
		effect = EffectManager.GetEffect(effectId);
		// 无敌护盾buff
		buff = playerBase.AddBuff(playerBase, 4);
		if (buff != null) { buff.remainingTime = listParam[1]; }

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
						// 被敌人弹开
						if(playerBase.AddBuff(listFish[i], 2) != null)
                        {
							if (playerBase.fishType == FishBase.FishType.Player)
							{
								PlayerModel.Instance.MissionActionTriggerAdd(18, 1);
							}
							if (--defenseCnt <= 0)
							{
								effect.Destroy();
								step = 0;
								buff.Destory();
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

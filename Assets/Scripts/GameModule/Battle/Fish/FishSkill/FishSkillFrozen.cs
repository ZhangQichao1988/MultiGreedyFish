using GooglePlayGames.BasicApi.Multiplayer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSkillFrozen : FishSkillBase
{
	Animator animatorPlayer;

	public override void Init(PlayerBase playerBase, FishSkillDataInfo baseData)
	{
		base.Init(playerBase, baseData);
		animatorPlayer = playerBase.transModel.GetComponent<Animator>();
	}

	public override bool Skill()
	{
		animatorPlayer.SetTrigger("Skill");
		BattleEffectManager.CreateEffect(baseData.effectId, playerBase.transModel);

		// 范围内敌人冰冻
		int cnt = 0;
		var listFish = BattleManagerGroup.GetInstance().fishManager.GetEnemiesInRange(playerBase, playerBase.transform.position, listParam[1]);
		for (int i = 0; i < listFish.Count; ++i)
		{
			if (!listFish[i].ContainsBuffType(BuffBase.BuffType.Shield) &&
				!listFish[i].ContainsBuffType(BuffBase.BuffType.ShieldGold))
			{
				listFish[i].AddBuff(playerBase, (int)listParam[2]);
			}
		}
		if (playerBase.fishType == FishBase.FishType.Player)
		{
			PlayerModel.Instance.MissionActionTriggerAdd(24, cnt);
		}

		return true;
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

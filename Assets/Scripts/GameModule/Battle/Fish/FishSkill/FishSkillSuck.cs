using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSkillSuck : FishSkillBase
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

		var listFish = BattleManagerGroup.GetInstance().fishManager.GetEnemiesInRange(playerBase, playerBase.transform.position, BattleConst.instance.RobotVision);
		Vector3 playerToTargetDir;
		float dot;
		float distance;
		for (int i = 0; i < listFish.Count; ++i)
		{
			playerToTargetDir = listFish[i].transform.position - playerBase.transform.position;
			distance = playerToTargetDir.magnitude;
			playerToTargetDir = Vector3.Normalize(playerToTargetDir);
			dot = Vector3.Dot(playerBase.transModel.transform.forward, playerToTargetDir);
			if (dot > 0.5f && distance <= listParam[1])
			{
				listFish[i].AddBuff(playerBase, 1);

			}
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
		FishBase targetFish = ((PlayerRobotBase)playerBase).targetFish;
		if (targetFish.fishType == FishBase.FishType.Player)
		{
			Vector3 playerToTargetDir = targetFish.transform.position - playerBase.transform.position;
			float distance = playerToTargetDir.magnitude;
			if (distance <= listParam[1])
			{
				RunSkill();
			}
		}
	}

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSkillSwelling : FishSkillBase
{
	static readonly float SCALE_SPD = 300f;
	float remainingTime;
	float process = 0f;
	int step = 0;
	SkinnedMeshRenderer skinnedMeshRenderer;
	public override void Init(PlayerBase playerBase, FishSkillDataInfo baseData)
	{
		base.Init(playerBase, baseData);
		var transBody = playerBase.transModel.Find("body");
		Debug.Assert(transBody != null, "FishSkillSwelling.Init()_1");
		skinnedMeshRenderer = transBody.GetComponent<SkinnedMeshRenderer>();
	}
	public override bool Skill()
	{
		remainingTime = listParam[1];
		process = 0f;
		step = 1;

		// 减速buff
		BuffBase buff = playerBase.AddBuff(playerBase, 3);
		if (buff != null) { buff.remainingTime = listParam[1]; }

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
				Vector3 playerToTargetDir;
				float distance;
				for (int i = 0; i < listFish.Count; ++i)
				{
					playerToTargetDir = listFish[i].transform.position - playerBase.transform.position;
					distance = playerToTargetDir.magnitude;
					if (playerToTargetDir.magnitude <= 2f)
					{
						// 弹开buff
						listFish[i].AddBuff(playerBase, 2);
						// 伤害
						if(!listFish[i].ContainsBuff(0))
                        {	// 判定是否受伤中
							listFish[i].Damage( (int)(playerBase.data.atk * listParam[2]), null );
							
						}
						
					}
				}

				if (process < 100) 
				{
					process += Time.deltaTime * SCALE_SPD;
					process = Mathf.Min(100, process);
					skinnedMeshRenderer.SetBlendShapeWeight(0, process);
				}
				remainingTime -= Time.deltaTime;
				if (remainingTime <= 0f)
				{
					step = 2;
				}
				break;
			case 2:
				process -= Time.deltaTime * SCALE_SPD;
				if (process <= 0f)
				{
					process = 0f;
					step = 0;
				}
				skinnedMeshRenderer.SetBlendShapeWeight(0, process);
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
		// 若有受伤buff，就开启膨胀技能
		if (playerBase.ContainsBuff(0))
		{
			RunSkill();
		}
    }
}

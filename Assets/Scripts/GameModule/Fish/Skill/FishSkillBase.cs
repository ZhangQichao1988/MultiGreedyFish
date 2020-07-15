using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSkillBase
{
	protected PlayerBase playerBase;
	protected FishSkillData.FishSkillBaseData baseData;
	public float currentGauge { get; protected set; }

	static public FishSkillBase SetFishSkill(PlayerBase playerBase, int id)
	{
		
		var baseData = FishSkillData.dicFishSkillBaseData[id];
		FishSkillBase fishSkill = null;
		switch (baseData.skillType)
		{
			case FishSkillData.SkillType.HealLife:
				fishSkill = new FishSkillHeadLife();
				break;
			default:
				Debug.LogError("FishSkillBase.SetFishSkill()_1");
				break;
		}
		fishSkill.Init(playerBase, baseData);

		return fishSkill;
	}
	public void Init(PlayerBase playerBase, FishSkillData.FishSkillBaseData baseData)
	{
		this.playerBase = playerBase;
		this.baseData = baseData;

		currentGauge = 0f;
	}

	public void RunSkill()
	{
		if (currentGauge < 1f)
		{ return; }
		if (Skill())
		{
			currentGauge = 0f;
		}
	}
	public virtual bool Skill()
	{
		Debug.LogError("FishSkillBase.Skill()_1");
		return false;
	}
	public virtual void CbAttack()
	{
	}

	public virtual void CbDamage()
	{
	}
}

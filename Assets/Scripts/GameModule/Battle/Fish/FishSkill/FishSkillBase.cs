using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FishSkillBase
{
	protected PlayerBase playerBase;
	public FishSkillDataInfo baseData;
	protected float[] listParam;
	public float currentGauge { get; protected set; }

	static public FishSkillBase SetFishSkill(PlayerBase playerBase, int id)
	{
		
		var baseData = FishSkillDataTableProxy.Instance.GetDataById(id);
		if (baseData == null) { return new FishSkillBase(); }

		System.Type type = Type.GetType(baseData.skillType);
		FishSkillBase fishSkill = Activator.CreateInstance(type) as FishSkillBase;
		fishSkill.Init(playerBase, baseData);

		return fishSkill;
	}
	public virtual void Init(PlayerBase playerBase, FishSkillDataInfo baseData)
	{
		this.playerBase = playerBase;
		this.baseData = baseData;
		listParam = Wrapper.GetParamFromString(baseData.aryParam);
		
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
	public virtual void Update()
	{ 
	}
}

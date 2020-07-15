using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSkillData
{
	public enum SkillType
	{ 
		HealLife,

	};
	public struct FishSkillBaseData
	{
		public SkillType skillType;
		public float[] aryParam;

		public FishSkillBaseData(SkillType skillType, float[] aryParam)
		{
			this.skillType = skillType;
			this.aryParam = aryParam;
		}
	};
	static readonly public Dictionary<int, FishSkillBaseData> dicFishSkillBaseData = new Dictionary<int, FishSkillBaseData>()
	{
		// 玩家鱼
		{1,  new FishSkillBaseData( SkillType.HealLife, new float[]{ 0.5f,/*恢复血量百分比*/1f,/*吃一条鱼加能量值的比例*/ }) },
	};
}

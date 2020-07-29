using Microsoft.SqlServer.Server;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSkillData
{
	public enum SkillType
	{ 
		HealLife,
		Suck,

	};
	public struct FishSkillBaseData
	{
		public SkillType skillType;
		public int effectId;
		public float[] aryParam;

		public FishSkillBaseData(SkillType skillType, int effectId, float[] aryParam)
		{
			this.skillType = skillType;
			this.effectId = effectId;
			this.aryParam = aryParam;
		}
	};
	static readonly Dictionary<int, FishSkillBaseData> dicFishSkillBaseData = new Dictionary<int, FishSkillBaseData>()
	{
		// 玩家鱼
		{1,  new FishSkillBaseData( SkillType.HealLife, 0, new float[]{ 0.5f,/*恢复血量百分比*/0.1f,/*吃一条鱼加能量值的比例*/ }) },
		// 大嘴鱼
		{2,  new FishSkillBaseData( SkillType.Suck, -1/*未实装*/, new float[]{ 9f,/*吞吸距离的平方*/1f,/*吃一条鱼加能量值的比例*/ }) },
	};
	static public FishSkillBaseData GetFishSkillBaseData(int id)
	{
		Debug.Assert(dicFishSkillBaseData.ContainsKey(id), string.Format("Skill Id:{0} is not found.", id.ToString()));
		return dicFishSkillBaseData[id];
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffData
{
	public enum BuffType
	{
		SpeedUp,
		BeSucked,
	};
	public struct BuffBaseData
	{
		public BuffType buffType;
		public float[] aryParam;
		public BuffBaseData(BuffType buffType, float[] aryParam)
		{
			this.buffType = buffType;
			this.aryParam = aryParam;
		}
	}
	static readonly Dictionary<int, BuffBaseData> dicBuffBaseData = new Dictionary<int, BuffBaseData>()
		{
			{ 0, new BuffBaseData(BuffType.SpeedUp, new float[]{ 1f/*持续时间*/, 2f/*初始速度倍率*/ }) },
			{ 1, new BuffBaseData(BuffType.BeSucked, new float[]{ 1f/*持续时间*/, 2f/*吸引速度*/ }) },
		};

	static public BuffBase SetBuff(FishBase Initiator, int id, FishBase fish)
	{
		Debug.Assert(dicBuffBaseData.ContainsKey(id), "BuffData.GetBuff()_1");
		BuffBaseData bbd = dicBuffBaseData[id];
		switch (bbd.buffType)
		{
			case BuffType.SpeedUp:
				return new BuffSpeedUp(Initiator, fish, bbd.aryParam);
			case BuffType.BeSucked:
				return new BuffBeSucked(Initiator, fish, bbd.aryParam);
		}
		Debug.LogError("BuffData.GetBuff()_2");
		return null;
	}
}

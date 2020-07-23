using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffData
{
	public enum BuffType
	{
		SpeedUp,
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
			{ 0, new BuffBaseData(BuffType.SpeedUp, new float[]{ 0.5f/*持续时间*/, 2f/*初始速度倍率*/ }) }
		};

	static public BuffBase SetBuff(int id, FishBase fish)
	{
		Debug.Assert(dicBuffBaseData.ContainsKey(id), "BuffData.GetBuff()_1");
		BuffBaseData bbd = dicBuffBaseData[id];
		switch (bbd.buffType)
		{
			case BuffType.SpeedUp:
				return new BuffSpeedUp(fish, bbd.aryParam);
		}
		Debug.LogError("BuffData.GetBuff()_2");
		return null;
	}
}

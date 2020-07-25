using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectData
{
	public struct EffectBaseData
	{
		public string prefabPath;
		public float duration;

		public EffectBaseData(string prefabPath, float duration)
		{
			this.prefabPath = prefabPath;
			this.duration = duration;
		}

	};
	static readonly Dictionary<int, EffectBaseData> dicEffectData = new Dictionary<int, EffectBaseData>()
	{
		{ 0, new EffectBaseData("fx_buff_Return_blood", 2f) },
	};

	static public EffectBaseData GetEffectData(int id)
	{
		return dicEffectData[id];
	}
}

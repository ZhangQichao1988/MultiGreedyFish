using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishData
{
	public struct FishBaseData
	{
		public string prefabPath;
		public int atk;
		public int life;
		public float moveSpeed;

		public FishBaseData(string prefabPath, int atk, int life, float moveSpeed)
		{
			this.prefabPath = prefabPath;
			this.atk = atk;
			this.life = life;
			this.moveSpeed = moveSpeed;
		}
	};
	static readonly public Dictionary<int, FishBaseData> listFishBaseData = new Dictionary<int, FishBaseData>()
	{
		// 杂鱼
		{ 0, new FishBaseData( "FishNpc_01", 0, 20, 0.4f) },
		// 玩家鱼
		{10,  new FishBaseData( "FishPlayer_01", 20, 100, 0.6f) },
	};
}

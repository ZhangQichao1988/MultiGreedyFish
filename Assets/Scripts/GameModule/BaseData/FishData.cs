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
		public int skillId;

		public FishBaseData(string prefabPath, int atk, int life, float moveSpeed, int skillId)
		{
			this.prefabPath = prefabPath;
			this.atk = atk;
			this.life = life;
			this.moveSpeed = moveSpeed;
			this.skillId = skillId;
		}
	};
	static readonly Dictionary<int, FishBaseData> dicFishBaseData = new Dictionary<int, FishBaseData>()
	{
		// 杂鱼
		{ 0, new FishBaseData( "FishNpc_01", 0, 20, 0.4f, -1) },
		// 玩家鱼
		//{1,  new FishBaseData( "FishPlayer_01", 20, 100, 0.6f, 1) },
		{1,  new FishBaseData( "FishPlayer_01", 20, 100, 0.6f, 2) },
		// 鲨鱼
		{2,  new FishBaseData( "FishNpc_02", 20, 1000, 0.4f, -1) },
	};

	static public FishDataInfo GetFishBaseData(int id)
	{
		return FishDataTableProxy.Instance.GetDataById(id);
	}
}

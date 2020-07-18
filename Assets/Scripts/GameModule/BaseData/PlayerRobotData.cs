using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRobotData
{
	public enum PlayerRobotType
	{
		PlayerRobotBase,
		PlayerRobotStayAquatic,
	}
	public struct PlayerRobotDataBaseData
	{
		public int fishId;
		public string name;
		public int aiId;


		public PlayerRobotDataBaseData(int fishId, string name, PlayerRobotType playerRobotType, int aiId)
		{
			this.fishId = fishId;
			this.name = name;
			this.aiId = aiId;
		}
	};

	public struct PlayerRobotAiBaseData
	{
		public int id;
		public PlayerRobotType playerRobotType;
		public float[] aryParam;

		public PlayerRobotAiBaseData(int id, PlayerRobotType playerRobotType, float[] aryParam)
		{
			this.id = id;
			this.playerRobotType = playerRobotType;
			this.aryParam = aryParam;
		}
	};

	static readonly public List<PlayerRobotDataBaseData> baseDatas = new List<PlayerRobotDataBaseData>()
	{
		//{new PlayerRobotDataBaseData(1, "阿超", PlayerRobotType.PlayerRobotBase, 2) },
		//{new PlayerRobotDataBaseData(1, "Ferya", PlayerRobotType.PlayerRobotBase, 2) },
		//{new PlayerRobotDataBaseData(1, "HUSKY", PlayerRobotType.PlayerRobotBase, 2) },
		//{new PlayerRobotDataBaseData(1, "DIAMOND DRAGON", PlayerRobotType.PlayerRobotBase, 2) },
		//{new PlayerRobotDataBaseData(1, "Mr.L  - Jiayin", PlayerRobotType.PlayerRobotBase, 2) },
		//{new PlayerRobotDataBaseData(1, "Asura", PlayerRobotType.PlayerRobotBase, 2) },
		//{new PlayerRobotDataBaseData(1, "睡在梦里，醒在梦境", PlayerRobotType.PlayerRobotBase, 2) },
		//{new PlayerRobotDataBaseData(1, "Rebecca", PlayerRobotType.PlayerRobotBase, 2) },
		//{new PlayerRobotDataBaseData(1, "༄༠་Yོiིnྀgོ་༠࿐", PlayerRobotType.PlayerRobotBase, 2) },
	};

	static readonly public List<PlayerRobotAiBaseData> aiBaseDatas = new List<PlayerRobotAiBaseData>()
	{
		// 普通型
		{
			new PlayerRobotAiBaseData(0, PlayerRobotType.PlayerRobotBase, new float[]{ 
			0.7f,/*血量低于这个比例吃杂鱼，高于则吃玩家*/
			0.2f,/*血量低于这个比例躲到草丛里回血*/
			})
		},
		// 勇敢型
		{
			new PlayerRobotAiBaseData(1, PlayerRobotType.PlayerRobotBase, new float[]{
			0.2f,/*血量低于这个比例吃杂鱼，高于则吃玩家*/
			0.2f,/*血量低于这个比例躲到草丛里回血*/
			})
		},
		// 发育型
		{
			new PlayerRobotAiBaseData(2, PlayerRobotType.PlayerRobotBase, new float[]{
			0.95f,/*血量低于这个比例吃杂鱼，高于则吃玩家*/
			0.9f,/*血量低于这个比例躲到草丛里回血*/
			})
		},
		//// 潜行者
		//{
		//	new PlayerRobotAiBaseData(2, PlayerRobotType.PlayerRobotStayAquatic, new float[]{
		//	0.95f,/*血量低于这个比例吃杂鱼，高于则吃玩家*/
		//	0.4f,/*血量低于这个比例躲到草丛里回血*/
		//	1.5f,/*scale低于这个数吃杂鱼，高于则躲水草*/
		//	})
		//},
	};

	static public PlayerRobotAiBaseData GetPlayerRobotAiBaseData(int id)
	{
		foreach (PlayerRobotAiBaseData playerRobotAiBaseData in aiBaseDatas)
		{
			if (playerRobotAiBaseData.id == id)
			{
				return playerRobotAiBaseData;
			}
		}
		Debug.LogError("PlayerRobotData.GetPlayerRobotAiBaseData()_1");
		return new PlayerRobotAiBaseData();
	}
}

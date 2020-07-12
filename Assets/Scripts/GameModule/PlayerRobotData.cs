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
		{new PlayerRobotDataBaseData(10, "阿超", PlayerRobotType.PlayerRobotBase, 0) },
		{new PlayerRobotDataBaseData(10, "Ferya", PlayerRobotType.PlayerRobotBase, 0) },
		{new PlayerRobotDataBaseData(10, "HUSKY", PlayerRobotType.PlayerRobotBase, 0) },
		{new PlayerRobotDataBaseData(10, "DIAMOND DRAGON", PlayerRobotType.PlayerRobotBase, 0) },
		{new PlayerRobotDataBaseData(10, "Mr.L  - Jiayin", PlayerRobotType.PlayerRobotBase, 0) },
		{new PlayerRobotDataBaseData(10, "Asura", PlayerRobotType.PlayerRobotBase, 0) },
		{new PlayerRobotDataBaseData(10, "睡在梦里，醒在梦境", PlayerRobotType.PlayerRobotBase, 0) },
		{new PlayerRobotDataBaseData(10, "Rebecca", PlayerRobotType.PlayerRobotBase, 0) },
		{new PlayerRobotDataBaseData(10, "༄༠་Yོiིnྀgོ་༠࿐", PlayerRobotType.PlayerRobotBase, 0) },
	};

	static readonly public List<PlayerRobotAiBaseData> aiBaseDatas = new List<PlayerRobotAiBaseData>()
	{
		{
			new PlayerRobotAiBaseData(0, PlayerRobotType.PlayerRobotBase, new float[]{ 
			0.8f,/*不吃杂鱼，吃玩家鱼血量*/
			0.2f,/*躲到草丛里回血的血量*/
			})
		},
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

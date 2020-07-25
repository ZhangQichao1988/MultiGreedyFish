using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRobotData
{
	public enum PlayerRobotType
	{
		PlayerRobotBase,
		PlayerRobotStayAquatic,
		Shark,
	}
	public struct PlayerRobotDataBaseData
	{
		public int fishId;
		public string name;
		public int aiId;


		public PlayerRobotDataBaseData(int fishId, string name, int aiId)
		{
			this.fishId = fishId;
			this.name = name;
			this.aiId = aiId;
		}
	};

	public struct PlayerRobotAiBaseData
	{
		public PlayerRobotType playerRobotType;
		public float[] aryParam;

		public PlayerRobotAiBaseData(PlayerRobotType playerRobotType, float[] aryParam)
		{
			this.playerRobotType = playerRobotType;
			this.aryParam = aryParam;
		}
	};

	static readonly public List<PlayerRobotDataBaseData> baseDatas = new List<PlayerRobotDataBaseData>()
	{
		{new PlayerRobotDataBaseData(1, "阿超", 1) },
		{new PlayerRobotDataBaseData(1, "Ferya", 1) },
		{new PlayerRobotDataBaseData(1, "HUSKY", 1) },
		{new PlayerRobotDataBaseData(1, "DIAMOND DRAGON", 0) },
		{new PlayerRobotDataBaseData(1, "Mr.L  - Jiayin", 0) },
		{new PlayerRobotDataBaseData(1, "Asura", 0) },
		{new PlayerRobotDataBaseData(1, "睡在梦里，醒在梦境", 0) },
		{new PlayerRobotDataBaseData(1, "Rebecca", 2) },
		{new PlayerRobotDataBaseData(1, "༄༠་Yོiིnྀgོ་༠࿐", 2) },

		{new PlayerRobotDataBaseData(2, "Boss", 3) },

	};

	static readonly Dictionary<int, PlayerRobotAiBaseData> dicAiBaseData = new Dictionary<int, PlayerRobotAiBaseData>()
	{
		// 普通型
		{
			0,
			new PlayerRobotAiBaseData(PlayerRobotType.PlayerRobotBase, new float[]{ 
			0.5f,/*血量低于这个比例吃杂鱼，高于则吃玩家*/
			0.2f,/*血量低于这个比例躲到草丛里回血*/
			})
		},
		// 勇敢型
		{
			1,
			new PlayerRobotAiBaseData( PlayerRobotType.PlayerRobotBase, new float[]{
			0.2f,/*血量低于这个比例吃杂鱼，高于则吃玩家*/
			0.2f,/*血量低于这个比例躲到草丛里回血*/
			})
		},
		// 发育型
		{
			2, 
			new PlayerRobotAiBaseData(PlayerRobotType.PlayerRobotBase, new float[]{
			0.95f,/*血量低于这个比例吃杂鱼，高于则吃玩家*/
			0.9f,/*血量低于这个比例躲到草丛里回血*/
			})
		},
		// Boss
		{
			3,
			new PlayerRobotAiBaseData( PlayerRobotType.Shark, new float[]{
			0f,/*血量低于这个比例吃杂鱼，高于则吃玩家*/
			0f,/*血量低于这个比例躲到草丛里回血*/
			})
		},
	};

	static public PlayerRobotAiBaseData GetPlayerRobotAiBaseData(int id)
	{
		return dicAiBaseData[id];
	}
}

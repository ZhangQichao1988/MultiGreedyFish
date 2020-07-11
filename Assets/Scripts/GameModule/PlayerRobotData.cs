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
		public PlayerRobotType playerRobotType;

		public PlayerRobotDataBaseData(int fishId, string name, PlayerRobotType playerRobotType)
		{
			this.fishId = fishId;
			this.name = name;
			this.playerRobotType = playerRobotType;
		}
	};
	static readonly public List<PlayerRobotDataBaseData> baseDatas = new List<PlayerRobotDataBaseData>()
	{
		{new PlayerRobotDataBaseData(10, "阿超", PlayerRobotType.PlayerRobotStayAquatic) },
		{new PlayerRobotDataBaseData(10, "Ferya", PlayerRobotType.PlayerRobotStayAquatic) },
		{new PlayerRobotDataBaseData(10, "HUSKY", PlayerRobotType.PlayerRobotStayAquatic) },
		{new PlayerRobotDataBaseData(10, "DIAMOND DRAGON", PlayerRobotType.PlayerRobotStayAquatic) },
		{new PlayerRobotDataBaseData(10, "Mr.L  - Jiayin", PlayerRobotType.PlayerRobotStayAquatic) },
		{new PlayerRobotDataBaseData(10, "Asura", PlayerRobotType.PlayerRobotStayAquatic) },
		{new PlayerRobotDataBaseData(10, "睡在梦里，醒在梦境", PlayerRobotType.PlayerRobotStayAquatic) },
		{new PlayerRobotDataBaseData(10, "Rebecca", PlayerRobotType.PlayerRobotStayAquatic) },
		{new PlayerRobotDataBaseData(10, "༄༠་Yོiིnྀgོ་༠࿐", PlayerRobotType.PlayerRobotStayAquatic) },
	};
}

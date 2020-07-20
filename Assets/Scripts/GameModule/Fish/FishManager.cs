using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
	static public float FlipFrequency = 2f;
	static public float MoveSpeed = 0.2f;
	public List<FishBase> listFish = new List<FishBase>();


	public PlayerBase CreatePlayer()
	{
		GameObject go = Wrapper.CreateEmptyGameObject(transform, "Player");
		PlayerBase player = go.AddComponent<PlayerBase>();
		listFish.Add(player);
		player.Init(1, LanguageData.GetText("PlayerName"));
		return player;
	}

	public void Clean()
	{
		for (int i = 0; i < listFish.Count; ++i)
		{
			Destroy(listFish[i].gameObject);
		}
		listFish.Clear();
	}
		public void CreateEnemy()
	{
		
		GameObject goEnemy;
		PlayerRobotBase prb = null;
		PlayerRobotData.PlayerRobotAiBaseData playerRobotAiBaseData;
		// 机器人
		for (int i = 0; i < PlayerRobotData.baseDatas.Count; ++i)
		{
			goEnemy = Wrapper.CreateEmptyGameObject(transform);
			playerRobotAiBaseData = PlayerRobotData.GetPlayerRobotAiBaseData(PlayerRobotData.baseDatas[i].aiId);
			switch (playerRobotAiBaseData.playerRobotType)
			{
				case PlayerRobotData.PlayerRobotType.PlayerRobotBase:
					prb = goEnemy.AddComponent<PlayerRobotBase>();
					break;
				case PlayerRobotData.PlayerRobotType.PlayerRobotStayAquatic:
					prb = goEnemy.AddComponent<PlayerRobotStayAquatic>();
					break;
				default:
					Debug.LogError("FishManager.CreateEnemy()_1");
					break;
			}
			prb.Init(PlayerRobotData.baseDatas[i].fishId, PlayerRobotData.baseDatas[i].name);
			prb.SetAI(playerRobotAiBaseData);
			listFish.Add(prb);
		}

		FishBase fb = null;
		// 杂鱼
		for (int i = 0; i < GameConst.EnemyNumMax; ++i)
		{
			goEnemy = Wrapper.CreateEmptyGameObject(transform);
			fb = goEnemy.AddComponent<EnemyBase>();
			fb.Init(0, "");
			listFish.Add(fb);
		}

	}
	public void CustomUpdate()
	{
		List<EnemyBase> bornWaittingEnemies = new List<EnemyBase>();
		int aliveEnemyNum = 0;
		for( int i = 0; i < listFish.Count; ++i )
		{
			listFish[i].CustomUpdate();

			// 活着的鱼数量统计
			if (listFish[i].actionStep == FishBase.ActionType.BornWaitting)
			{
				bornWaittingEnemies.Add(listFish[i] as EnemyBase);
			}
			else
			{ ++aliveEnemyNum; }
		}

		// 能活着的杂鱼数量计算
		int enemyNum = (int)Mathf.Lerp(GameConst.EnemyNumMin, GameConst.EnemyNumMax, ManagerGroup.GetInstance().poisonRing.GetPoisonRange() / GameConst.PoisonRingRadiusMax);

		// 当活着的鱼比能活着的鱼数量少的时候，复活鱼
		if (enemyNum > aliveEnemyNum)
		{
			for (int i = 0; i < bornWaittingEnemies.Count && enemyNum > aliveEnemyNum + i; ++i)
			{
				bornWaittingEnemies[i].Born();
			}
		}
	}

	public void EatCheck(PlayerBase player, BoxCollider atkCollider)
	{
		FishBase fb;
		List<FishBase> listFish = ManagerGroup.GetInstance().fishManager.GetEnemiesInRange(player, player.transform.position, GameConst.RobotVision);
		for ( int i = listFish.Count -1; i >= 0; --i )
		{
			fb = listFish[i];
			if (player == fb) { continue; }
			if (fb.EatCheck(atkCollider))
			{
				player.Atk(fb);
				continue;
			}
		}

	}


	public List<FishBase> GetEnemiesInRange(FishBase me, Vector3 pos, Vector2 range)
	{
		List<FishBase> enemies = new List<FishBase>();
		for (int i = 0; i < listFish.Count; ++i)
		{
			if (me == listFish[i]) { continue; }
			if (listFish[i].actionStep == FishBase.ActionType.Die ||
				listFish[i].actionStep == FishBase.ActionType.Born ||
				listFish[i].actionStep == FishBase.ActionType.BornWaitting) { continue; }

			if (pos.x + range.x > listFish[i].transform.position.x &&
				pos.x - range.x < listFish[i].transform.position.x &&
				pos.z + range.y > listFish[i].transform.position.z &&
				pos.z - range.y < listFish[i].transform.position.z)
			{
				enemies.Add(listFish[i]);
			}
		}
		return enemies;
	}

	public List<FishBase> GetAlivePlayer()
	{
		List<FishBase> listPlayer = new List<FishBase>();
		for (int i = 0; i < listFish.Count; ++i)
		{
			if (listFish[i].fishType != FishBase.FishType.Enemy && listFish[i].actionStep != FishBase.ActionType.Die)
			{
				listPlayer.Add(listFish[i]);
			}
		}
		return listPlayer;
	}

}

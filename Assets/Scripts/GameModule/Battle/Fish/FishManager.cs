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
		player.Init(1, LanguageDataTableProxy.GetText(0));
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
		RobotAiDataInfo playerRobotAiBaseData;
		RobotDataInfo playerRobotBaseData;
		// 机器人
		int robotCount = RobotDataTableProxy.Instance.GetRobotCount();
		for (int i = 0; i < robotCount; ++i)
		{
			playerRobotBaseData = RobotDataTableProxy.Instance.GetDataById(i);
				goEnemy = Wrapper.CreateEmptyGameObject(transform);
			playerRobotAiBaseData = RobotAiDataTableProxy.Instance.GetDataById(playerRobotBaseData.aiId);
			prb = (PlayerRobotBase)goEnemy.AddComponent(System.Type.GetType(playerRobotAiBaseData.aiType));
			prb.Init(playerRobotBaseData.fishId, playerRobotBaseData.name);
			prb.SetAI(playerRobotAiBaseData);
			listFish.Add(prb);
		}

		FishBase fb = null;
		// 杂鱼
		for (int i = 0; i < BattleConst.EnemyNumMax; ++i)
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
		int enemyNum = (int)Mathf.Lerp(BattleConst.EnemyNumMin, BattleConst.EnemyNumMax, BattleManagerGroup.GetInstance().poisonRing.GetPoisonRange() / BattleConst.PoisonRingRadiusMax);

		// 当活着的鱼比能活着的鱼数量少的时候，复活鱼
		if (enemyNum > aliveEnemyNum)
		{
			for (int i = 0; i < bornWaittingEnemies.Count && enemyNum > aliveEnemyNum + i; ++i)
			{
				bornWaittingEnemies[i].Born();
			}
		}
	}

	void EatCheck(PlayerBase player, BoxCollider atkCollider, List<FishBase> listFish)
	{
		FishBase fb;
		for (int i = listFish.Count - 1; i >= 0; --i)
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
	public void EatEnemyCheck(PlayerBase player, BoxCollider atkCollider)
	{
		
		List<FishBase> listFish = GetEnemiesInRange(player, player.transform.position, BattleConst.RobotVision);
		EatCheck(player, atkCollider, listFish);
	}

	public void EatPlayerCheck(PlayerBase player, BoxCollider atkCollider)
	{
		List<FishBase> listFish = GetAlivePlayerInRange(player, player.transform.position, BattleConst.RobotVision);
		EatCheck(player, atkCollider, listFish);
	}


	public List<FishBase> GetAlivePlayerInRange(FishBase me, Vector3 pos, Vector2 range)
	{
		List<FishBase> listFish = GetEnemiesInRange(me, pos, range);
		for (int i = listFish.Count - 1; i >= 0; --i)
		{
			if (listFish[i].fishType == FishBase.FishType.PlayerRobot || listFish[i].fishType == FishBase.FishType.Player)
			{
				continue;
			}
			listFish.RemoveAt(i);
		}
		return listFish;
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
			if ((	listFish[i].fishType == FishBase.FishType.PlayerRobot || listFish[i].fishType == FishBase.FishType.Player)
					 && listFish[i].actionStep != FishBase.ActionType.Die)
			{
				listPlayer.Add(listFish[i]);
			}
		}
		return listPlayer;
	}
	public List<FishBase> GetAlivePlayerSort(Vector3 pos)
	{
		List<FishBase> listPlayer = GetAlivePlayer();
		listPlayer.Sort((a,b)=> { return (int)(Vector3.Distance(pos, a.transform.position) - Vector3.Distance(pos, b.transform.position)); });
		return listPlayer;
	}

}
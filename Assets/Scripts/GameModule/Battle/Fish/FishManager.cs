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
		player.Init(1, PlayerModel.Instance.player.Nickname);
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
		PBRobotDataInfo pBRobotDataInfo;
		// 机器人
		var aryRobotDataInfo = StageModel.Instance.aryRobotDataInfo;
		int robotCount = aryRobotDataInfo.Length;
		var listName = RobotNameDataTableProxy.Instance.GetAllRobotNames();
		for (int i = 0; i < robotCount; ++i)
		{
			pBRobotDataInfo = aryRobotDataInfo[i];
			goEnemy = Wrapper.CreateEmptyGameObject(transform);
			playerRobotAiBaseData = RobotAiDataTableProxy.Instance.GetDataById(pBRobotDataInfo.AiId);
			prb = (PlayerRobotBase)goEnemy.AddComponent(System.Type.GetType(playerRobotAiBaseData.aiType));
			prb.Init(pBRobotDataInfo.FishId, listName[i]);
			prb.SetAI(playerRobotAiBaseData);
			listFish.Add(prb);
		}

		// Boss
		goEnemy = Wrapper.CreateEmptyGameObject(transform);
		playerRobotAiBaseData = RobotAiDataTableProxy.Instance.GetDataById(3);
		prb = (PlayerRobotBase)goEnemy.AddComponent(System.Type.GetType(playerRobotAiBaseData.aiType));
		prb.Init(2, "BOSS");
		prb.SetAI(playerRobotAiBaseData);
		listFish.Add(prb);

		List<FishBase> listEnemy = new List<FishBase>();
		FishBase fb = null;
		PBEnemyDataInfo enemyGroup;
		// 杂鱼

		var aryEnemyDataInfo = StageModel.Instance.aryEnemyDataInfo;
		int EnemyNumMax = aryEnemyDataInfo.Length;
		for (int i = 0; i < EnemyNumMax; ++i)
		{
			enemyGroup = aryEnemyDataInfo[i];
			for (int j = 0; j < enemyGroup.FishCountMax; ++j)
			{
				goEnemy = Wrapper.CreateEmptyGameObject(transform);
				fb = goEnemy.AddComponent<EnemyBase>();
				fb.Init(enemyGroup.FishId, "");
				listEnemy.Add(fb);
			}
		}
		// 打乱敌人列表
		listEnemy.Sort((a,b)=> { return Wrapper.GetRandom(-1, 1); });
		listFish.AddRange(listEnemy);
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
		int enemyNum = GetEnemyCount();

		// 当活着的鱼比能活着的鱼数量少的时候，复活鱼
		if (enemyNum > aliveEnemyNum)
		{
			for (int i = 0; i < bornWaittingEnemies.Count && enemyNum > aliveEnemyNum; ++i)
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

	int GetEnemyCount()
	{
		int enemyMax = 0;
		float rate = BattleManagerGroup.GetInstance().poisonRing.GetPoisonRange() / BattleConst.PoisonRingRadiusMax;

		var aryEnemyDataInfo = StageModel.Instance.aryEnemyDataInfo;
		for (int i = 0; i < aryEnemyDataInfo.Length; ++i)
		{
			enemyMax += (int)Mathf.Lerp(aryEnemyDataInfo[i].FishCountMin, aryEnemyDataInfo[i].FishCountMax, rate);
		}
		return enemyMax;
	}
}

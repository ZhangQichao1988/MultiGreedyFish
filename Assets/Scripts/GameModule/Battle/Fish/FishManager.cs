using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
	public List<FishBase> listFish = new List<FishBase>();

    private void Awake()
    {
		FishBase.uidCnt = (uint)Wrapper.GetRandom(0,9);
	}
    public PlayerBase CreatePlayer()
	{
		FishRankLevelDataInfo currentRankData, nextRankData;
		FishRankLevelDataTableProxy.Instance.GetFishRankLevelData(PlayerModel.Instance.GetCurrentPlayerFishLevelInfo().RankLevel, out currentRankData, out nextRankData);

		GameObject go = Wrapper.CreateEmptyGameObject(transform, "Player");
		PlayerBase player = go.AddComponent<PlayerBase>();
		listFish.Add(player);
		var playerData = PlayerModel.Instance.player;
		player.Init(playerData.FightFish, playerData.Nickname, PlayerModel.Instance.GetCurrentPlayerFishLevelInfo().FishLevel, currentRankData.rankIcon);
		player.gameObject.AddComponent<AudioListener>();
		//GameObjectUtil.InstantiatePrefab(ResourceManager.LoadSync<GameObject>("SoundManager").Asset, player.gameObject);

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

		FishRankLevelDataInfo currentRankData, nextRankData;
		FishRankLevelDataTableProxy.Instance.GetFishRankLevelData(PlayerModel.Instance.GetCurrentPlayerFishLevelInfo().RankLevel, out currentRankData, out nextRankData);


		for (int i = 0; i < robotCount; ++i)
		{
			pBRobotDataInfo = aryRobotDataInfo[i];
			goEnemy = Wrapper.CreateEmptyGameObject(transform);
			playerRobotAiBaseData = RobotAiDataTableProxy.Instance.GetDataById(pBRobotDataInfo.AiId);
			prb = (PlayerRobotBase)goEnemy.AddComponent(System.Type.GetType(playerRobotAiBaseData.aiType));

			int rankId = Mathf.Clamp( currentRankData.ID + Wrapper.GetRandom(-2, 2), 1, 20);
			prb.Init(pBRobotDataInfo.FishId, listName[i], pBRobotDataInfo.Level, FishRankLevelDataTableProxy.Instance.GetDataById(rankId).rankIcon);
			prb.SetRobot(playerRobotAiBaseData, pBRobotDataInfo.Growth/100f);
			listFish.Add(prb);
		}

		// Boss
		goEnemy = Wrapper.CreateEmptyGameObject(transform);
		playerRobotAiBaseData = RobotAiDataTableProxy.Instance.GetDataById(3);
		prb = (PlayerRobotBase)goEnemy.AddComponent(System.Type.GetType(playerRobotAiBaseData.aiType));
		prb.Init(2, "BOSS", 1);
		prb.SetRobot(playerRobotAiBaseData, 0f);
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
				switch (enemyGroup.FishId)
				{
					case 0: fb = goEnemy.AddComponent<EnemyBase>(); break;  // 宝宝鱼
					case 4: fb = goEnemy.AddComponent<EnemyJellyfish>(); break;  // 水母
					case 5: fb = goEnemy.AddComponent<EnemyTortoise>(); break;  // 金龟
				}
				fb.Init(enemyGroup.FishId, "", enemyGroup.FishLevel);
				listEnemy.Add(fb);
			}
		}

		// 打乱敌人列表
		listEnemy = Wrapper.RandomSortList<FishBase>(listEnemy);
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
			else if(listFish[i].fishType == FishBase.FishType.Enemy)
			{ ++aliveEnemyNum; }
		}

		bornWaittingEnemies = Wrapper.RandomSortList<EnemyBase>(bornWaittingEnemies);
		
		// 能活着的杂鱼数量计算
		int enemyNum = GetEnemyCount();

		// 当活着的鱼比能活着的鱼数量少的时候，复活鱼
		if (enemyNum > aliveEnemyNum)
		{
			for (int i = 0; i < bornWaittingEnemies.Count && enemyNum > aliveEnemyNum; ++i, ++aliveEnemyNum)
			{
				bornWaittingEnemies[i].Born();
			}
		}
	}

	public void EatCheck(PlayerBase player, BoxCollider atkCollider, List<FishBase> listFish)
	{
		FishBase fb;
		for (int i = listFish.Count - 1; i >= 0; --i)
		{
			fb = listFish[i];
			if (player == fb) { continue; }
			if (player.fishType == FishBase.FishType.Boss && fb.fishType == FishBase.FishType.Enemy) { continue; }
			if (fb.EatCheck(player, atkCollider))
			{
				player.Atk(fb);
				continue;
			}
		}
	}
	public List<FishBase> EatEnemyCheck(PlayerBase player, BoxCollider atkCollider)
	{
		
		List<FishBase> listFish = GetEnemiesInRange(player, player.transform.position, BattleConst.instance.RobotVision);
		EatCheck(player, atkCollider, listFish);
		return listFish;
	}

	public List<FishBase> GetEnemiesInRange(FishBase me, Vector3 pos, Vector2 range)
	{
		List<FishBase> enemies = new List<FishBase>();
		Vector3 enemyPos;
		for (int i = 0; i < listFish.Count; ++i)
		{
			if (me == listFish[i]) { continue; }
			if (listFish[i].actionStep == FishBase.ActionType.Die ||
				listFish[i].actionStep == FishBase.ActionType.Born ||
				listFish[i].actionStep == FishBase.ActionType.BornWaitting){ continue; }
				//listFish[i].fishType == FishBase.FishType.Boss

			enemyPos = listFish[i].transform.position;
			if (pos.x + range.x > enemyPos.x &&
				pos.x - range.x < enemyPos.x &&
				pos.z + range.y > enemyPos.z &&
				pos.z - range.y < enemyPos.z)
			{
				enemies.Add(listFish[i]);
			}
		}
		return enemies;
	}
	public List<FishBase> GetEnemiesInRange(FishBase me, Vector3 pos, float range)
	{
		List<FishBase> enemies = new List<FishBase>();
		Vector3 enemyPos;
		for (int i = 0; i < listFish.Count; ++i)
		{
			if (me == listFish[i]) { continue; }
			if (listFish[i].actionStep == FishBase.ActionType.Die ||
				listFish[i].actionStep == FishBase.ActionType.Born ||
				listFish[i].actionStep == FishBase.ActionType.BornWaitting) { continue; }
			//listFish[i].fishType == FishBase.FishType.Boss

			enemyPos = listFish[i].transform.position;
			if ( (me.transform.position- enemyPos).magnitude <= range)
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
		float rate = BattleManagerGroup.GetInstance().poisonRing.GetPoisonRange() / BattleConst.instance.PoisonRingRadiusMax;

		var aryEnemyDataInfo = StageModel.Instance.aryEnemyDataInfo;
		for (int i = 0; i < aryEnemyDataInfo.Length; ++i)
		{
			enemyMax += (int)Mathf.Lerp(aryEnemyDataInfo[i].FishCountMin, aryEnemyDataInfo[i].FishCountMax, rate);
		}
		return enemyMax;
	}

}

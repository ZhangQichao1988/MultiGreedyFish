using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
	static public float FlipFrequency = 2f;
	static public float MoveSpeed = 0.2f;
	public List<FishBase> listFish = new List<FishBase>();

	private void Awake()
	{
		
	}

	public PlayerBase CreatePlayer()
	{
		GameObject go = Wrapper.CreateEmptyGameObject(transform, "Player");
		PlayerBase player = go.AddComponent<PlayerBase>();
		listFish.Add(player);
		player.Init(new FishBase.Data(0, GameConst.PlayerName, 100, 10, 0.6f));
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
		FishBase fb;
		// 机器人
		for (int i = 0; i < GameConst.RobotNum; ++i)
		{
			goEnemy = Wrapper.CreateEmptyGameObject(transform);
			fb = goEnemy.AddComponent<PlayerRobot>();
			fb.Init(new FishBase.Data(0, GameConst.RobotName[i], 100, 10, 0.6f));
			listFish.Add(fb);
		}

		// 杂鱼
		for (int i = 0; i < GameConst.EnemyNum; ++i)
		{
			goEnemy = Wrapper.CreateEmptyGameObject(transform);
			fb = goEnemy.AddComponent<EnemyBase>();
			fb.Init(new FishBase.Data( 1, "", 10, 0, 0.4f));
			listFish.Add(fb);
		}

	}

	private void Update()
	{
		for( int i = 0; i < listFish.Count; ++i )
		{
			listFish[i].CustomUpdate();
		}
	}

	public void EatCheck(PlayerBase player, BoxCollider atkCollider)
	{
		FishBase fb;
		List<FishBase> listFish = ManagerGroup.GetInstance().fishManager.GetEnemiesInRange(player, player.transform.position, GameConst.RobotFindFishRange);
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
				listFish[i].actionStep == FishBase.ActionType.Born) { continue; }

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

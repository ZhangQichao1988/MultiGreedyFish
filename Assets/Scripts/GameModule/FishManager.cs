using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
	static public float FlipFrequency = 2f;
	static public float MoveSpeed = 0.2f;
	List<FishBase> listFish = new List<FishBase>();

	private void Awake()
	{
		
	}

	public PlayerBase CreatePlayer()
	{
		GameObject go = Wrapper.CreateEmptyGameObject(transform, "Player");
		PlayerBase player = go.AddComponent<PlayerBase>();
		listFish.Add(player);
		player.Init(new FishBase.Data(0, 1, 2));
		return player;
	}

		public void CreateEnemy()
	{
		
		GameObject goEnemy;
		FishBase fb;
		// 杂鱼
		for (int i = 0; i < GameConst.EnemyNum; ++i)
		{
			goEnemy = Wrapper.CreateEmptyGameObject(transform);
			fb = goEnemy.AddComponent<EnemyBase>();
			fb.Init(new FishBase.Data( 1, 1, 1));
			listFish.Add(fb);
		}

		// 机器人
		for (int i = 0; i < GameConst.RobotNum; ++i)
		{
			goEnemy = Wrapper.CreateEmptyGameObject( transform);
			fb = goEnemy.AddComponent<PlayerRobot>();
			fb.Init(new FishBase.Data(0, 100, 2));
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

	public void EatCheck(PlayerBase player, Vector3 mouthPos, float range)
	{
		FishBase fb;
		int eatNum = 0;
		for ( int i = listFish.Count -1; i >= 0; --i )
		{
			fb = listFish[i];
			if (fb.EatCheck(mouthPos, range))
			{
				fb.Die();
				listFish.Remove(fb);
				++eatNum;
				continue;
			}
		}
		player.Eat(eatNum);

	}

	public List<FishBase> GetEnemiesInRange(FishBase me, Vector3 pos, Vector2 range)
	{
		List<FishBase> enemies = new List<FishBase>();
		for (int i = 0; i < listFish.Count; ++i)
		{
			if (me == listFish[i]) { continue; }
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

}

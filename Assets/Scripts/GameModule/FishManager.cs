using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
	public int enemyNum = 50;
	public int robotNum = 9;
	static public float FlipFrequency = 2f;
	static public float MoveSpeed = 0.2f;
	int uidCnt = 0;
	List<FishBase> listFish = new List<FishBase>();

	private void Awake()
	{
		
	}

	public PlayerBase CreatePlayer()
	{
		GameObject go = Wrapper.CreateEmptyGameObject(transform, "Player");
		PlayerBase player = go.AddComponent<PlayerBase>();
		player.Init(new FishBase.Data(0, 1, 2));
		return player;
	}

		public void CreateEnemy()
	{
		
		GameObject goEnemy;
		FishBase fb;
		// 杂鱼
		for (int i = 0; i < enemyNum; ++i)
		{
			goEnemy = Wrapper.CreateEmptyGameObject(transform, "Enemy_" + uidCnt );
			fb = goEnemy.AddComponent<EnemyBase>();
			fb.Init(new FishBase.Data( 1, 1, 1));
			listFish.Add(fb);
		}

		// 机器人
		for (int i = 0; i < robotNum; ++i)
		{
			goEnemy = Wrapper.CreateEmptyGameObject( transform, "Robot_" + uidCnt);
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

}

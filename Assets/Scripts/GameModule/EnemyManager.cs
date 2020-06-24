using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
	public int enemyNum = 50;
	public int robotNum = 9;
	static public float FlipFrequency = 2f;
	static public float MoveSpeed = 0.2f;
	int uidCnt = 0;
	List<FishBase> listEnemy = new List<FishBase>();

	private void Awake()
	{
		
	}
	public void Init(ManagerGroup managerGroup)
	{
		
		GameObject goEnemy;
		FishBase fb;
		// 杂鱼
		for (int i = 0; i < enemyNum; ++i)
		{
			goEnemy = Wrapper.CreateGameObject(new GameObject(), transform, "Enemy_" + uidCnt );
			fb = goEnemy.AddComponent<EnemyBase>();
			fb.Init(new FishBase.Data( 1, 1, 1));
			listEnemy.Add(fb);
		}

		// 机器人
		for (int i = 0; i < robotNum; ++i)
		{
			goEnemy = Wrapper.CreateGameObject(new GameObject(), transform, "Robot_" + uidCnt);
			fb = goEnemy.AddComponent<PlayerRobot>();
			fb.Init(new FishBase.Data(0, 100, 2));
			listEnemy.Add(fb);
		}

	}

	private void Update()
	{
		for( int i = 0; i < listEnemy.Count; ++i )
		{
			listEnemy[i].CustomUpdate();
		}
	}

	public void EatCheck(PlayerBase player, Vector3 mouthPos, float range)
	{
		FishBase fb;
		int eatNum = 0;
		for ( int i = listEnemy.Count -1; i >= 0; --i )
		{
			fb = listEnemy[i];
			if (fb.EatCheck(mouthPos, range))
			{
				fb.Die();
				listEnemy.Remove(fb);
				++eatNum;
				continue;
			}
		}
		player.Eat(eatNum);

	}

}

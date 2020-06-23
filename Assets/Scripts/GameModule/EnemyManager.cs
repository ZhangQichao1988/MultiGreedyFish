using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
	public int enemyNum = 50;
	static public float FlipFrequency = 2f;
	static public float MoveSpeed = 0.2f;
	int uidCnt = 0;
	List<EnemyBase> listEnemy = new List<EnemyBase>();

	private void Awake()
	{
		
	}
	public void Init(ManagerGroup managerGroup)
	{
		
		GameObject goEnemy;
		for (int i = 0; i < enemyNum; ++i)
		{
			goEnemy = Wrapper.CreateGameObject(new GameObject(), transform, "Enemy_" + uidCnt );
			EnemyBase eb = goEnemy.AddComponent<EnemyBase>();
			eb.Init(new FishBase.Data( 1, 1, 1));
			listEnemy.Add(eb);
		}
		
	}

	private void Update()
	{
		foreach ( EnemyBase eb in listEnemy )
		{
			eb.CustomUpdate();
		}
	}

	public void EatCheck(PlayerBase player, Vector3 mouthPos, float range)
	{
		EnemyBase eb;
		int eatNum = 0;
		for ( int i = listEnemy.Count -1; i >= 0; --i )
		{
			eb = listEnemy[i];
			if (eb.EatCheck(mouthPos, range))
			{
				eb.Die();
				listEnemy.Remove(eb);
				++eatNum;
				continue;
			}
		}
		player.Eat(eatNum);

	}

}

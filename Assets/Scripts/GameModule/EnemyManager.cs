using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
	public int enemyNum = 50;
	public float FlipFrequency = 2f;
	public float MoveSpeed = 0.2f;
	static readonly string enemyPath = "ArtResources/Models/Prefabs/fish/yellow_fish";
	int uidCnt = 0;
	List<EnemyBase> listEnemy = new List<EnemyBase>();

	private void Awake()
	{
		
	}
	public void Init(ManagerGroup managerGroup)
	{
		Object enemyObj = Resources.Load(enemyPath);
		GameObject goEnemy;
		for (int i = 0; i < enemyNum; ++i)
		{
			goEnemy = Wrapper.AssetLoad(enemyObj, transform) as GameObject;
			EnemyBase eb = goEnemy.AddComponent<EnemyBase>();
			eb.Init(this, uidCnt++);
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
}

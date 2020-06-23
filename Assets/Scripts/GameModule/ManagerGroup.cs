using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerGroup : MonoBehaviour
{
    static ManagerGroup instance = null;
    public EnemyManager enemyManager = null;

	private void Awake()
	{
        instance = this;
        enemyManager.Init(this);
    }
    static public ManagerGroup GetInstance()
    { return instance; }
}

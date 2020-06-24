using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerGroup : MonoBehaviour
{
    static ManagerGroup instance = null;
    public FishManager fishManager = null;

	private void Awake()
	{
        instance = this;
        fishManager.CreateEnemy();
    }
    static public ManagerGroup GetInstance()
    { return instance; }
}

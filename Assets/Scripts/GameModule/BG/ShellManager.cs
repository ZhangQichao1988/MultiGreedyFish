using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellManager : MonoBehaviour
{
	public List<ShellControl> listShell = new List<ShellControl>();

	private void Awake()
	{
		Transform transTmp = null;
		for (int i = 0; i < transform.childCount; ++i)
		{
			transTmp = transform.GetChild(i);
			if (transTmp.gameObject.activeSelf)
			{
				listShell.Add(transTmp.GetComponent<ShellControl>());
			}
		}
	}

	public void EatPearl(PlayerBase fish)
	{
		for (int i = 0; i < listShell.Count; ++i)
		{
			float distance = Vector3.Distance(listShell[i].transform.position, fish.transform.position);
			if (distance <= GameConst.EatPearlRange)
			{
				// 根据开合状态来判定吃了珍珠还是被夹
				if(listShell[i].PearlEatenCheck())
				{
					fish.life += GameConst.PearlRecoverLife;
					return;
				}
			}
		}
	}

}

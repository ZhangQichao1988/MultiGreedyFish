using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AquaticManager : MonoBehaviour
{
	public List<Transform> listTransAquatic = new List<Transform>();

	private void Awake()
	{
		Transform transTmp = null;
		for (int i = 0; i < transform.childCount; ++i)
		{
			transTmp = transform.GetChild(i);
			if (transTmp.gameObject.activeSelf)
			{
				listTransAquatic.Add(transTmp);
			}
		}
	}

	public bool IsInAquatic(FishBase fish)
	{
		for (int i = 0; i < listTransAquatic.Count; ++i)
		{
			float distance = Vector3.Distance(listTransAquatic[i].position, fish.transform.position);
			if (distance <= GameConst.AquaticRange)
			{
				return true;
			}
		}
		return false;
	}
}

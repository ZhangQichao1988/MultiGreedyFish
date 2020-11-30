using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PoisonRing : MonoBehaviour
{
	//Material material = null;
	List<MaterialPropertyBlock> listMaterialBlock = new List<MaterialPropertyBlock>();
	MeshRenderer[] aryMeshRenderer = null;

	float poisonRange;
	float beforePoisonRange;

	public void Init()
	{
		ApplyRange(BattleConst.instance.PoisonRingRadiusMax);
	}

	private void Awake()
	{
		poisonRange = BattleConst.instance.PoisonRingRadiusMax;
		beforePoisonRange = BattleConst.instance.PoisonRingRadiusMax;
		listMaterialBlock.Clear();
		aryMeshRenderer = GetComponentsInChildren<MeshRenderer>();
		MaterialPropertyBlock materialProperty;
		foreach (MeshRenderer mr in aryMeshRenderer)
		{
			materialProperty = new MaterialPropertyBlock();
			mr.GetPropertyBlock(materialProperty);
			listMaterialBlock.Add(materialProperty);
		}
	}
	public void CustomUpdate()
	{
		poisonRange -= Time.deltaTime * ConfigTableProxy.Instance.GetDataByKey("PoisonRingScaleSpeed");
		if (beforePoisonRange - poisonRange > 0.03f)
		{
			ApplyRange( poisonRange);
		}
	}

	void ApplyRange(float value)
	{
		poisonRange = Math.Max(value, BattleConst.instance.PoisonRingRadiusMin);
		beforePoisonRange = poisonRange;
		for (int i =0; i < listMaterialBlock.Count; ++i)
		{
			listMaterialBlock[i].SetFloat("_SafeAreaRange", poisonRange);
			listMaterialBlock[i].SetFloat("_SafeGradAreaRange", poisonRange - 3f);
			aryMeshRenderer[i].SetPropertyBlock(listMaterialBlock[i]);
		}
	}

	public float GetPoisonRange()
	{
		return poisonRange;
	}
}

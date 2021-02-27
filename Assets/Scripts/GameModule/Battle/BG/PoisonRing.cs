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
	float poisonRangePow;
	float beforePoisonRange;
	float safeRudius;
	float safeRudiusPow;

	public void Init()
	{
		ApplyRange(BattleConst.instance.PoisonRingRadiusMax);
	}

	private void Awake()
	{
		poisonRange = BattleConst.instance.PoisonRingRadiusMax;
		poisonRangePow = Mathf.Pow(poisonRange, 2);
		beforePoisonRange = BattleConst.instance.PoisonRingRadiusMax;
		ApplyPublicPoisonRange();

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
		poisonRangePow = Mathf.Pow(poisonRange, 2);

		if (beforePoisonRange - poisonRange > 0.03f)
		{
			ApplyRange( poisonRange);
		}
		
	}
	void ApplyPublicPoisonRange()
	{
		safeRudius = Mathf.Min(poisonRange, BattleConst.instance.BgBound);
		safeRudiusPow = Mathf.Pow(safeRudius, 2);
	}
	void ApplyRange(float value)
	{
		poisonRange = Math.Max(value, BattleConst.instance.PoisonRingRadiusMin);
		beforePoisonRange = poisonRange;
		ApplyPublicPoisonRange();
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
	public float GetPoisonRangePow()
	{
		return poisonRangePow;
	}
	public float GetSafeRudius()
	{
		return safeRudius;
	}
	public float GetSafeRudiusPow()
	{
		return safeRudiusPow;
	}
}

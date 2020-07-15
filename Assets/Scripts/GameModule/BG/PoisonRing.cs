using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonRing : MonoBehaviour
{
	//Material material = null;
	MaterialPropertyBlock materialBlock = null;
	MeshRenderer meshRenderer = null;

	float poisonRange = GameConst.PoisonRingRadiusMax;
	float beforePoisonRange = GameConst.PoisonRingRadiusMax;

	public void Init()
	{
		ApplyRange(GameConst.PoisonRingRadiusMax);
	}

	private void Awake()
	{
		materialBlock = new MaterialPropertyBlock();
		meshRenderer = GetComponent<MeshRenderer>();
		meshRenderer.GetPropertyBlock(materialBlock);
	}
	public void Update()
	{
		poisonRange -= Time.deltaTime * GameConst.PoisonRingScaleSpeed;
		if (beforePoisonRange - poisonRange > 0.03f)
		{
			ApplyRange( poisonRange);
		}
	}

	void ApplyRange(float value)
	{
		poisonRange = Math.Max(value, GameConst.PoisonRingRadiusMin);
		beforePoisonRange = poisonRange;
		materialBlock.SetFloat("_SafeAreaRange", poisonRange);
		materialBlock.SetFloat("_SafeGradAreaRange", poisonRange - 3f);
		meshRenderer.SetPropertyBlock(materialBlock);
	}

	public float GetPoisonRange()
	{
		return poisonRange;
	}
}

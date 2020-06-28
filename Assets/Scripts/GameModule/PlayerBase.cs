using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerBase : FishBase
{
	static readonly string boneNameMouth = "head_end";
	public BoxCollider colliderMouth = null;

	protected override bool showLifeGauge { get { return true; } }

	public override FishType fishType { get { return FishType.Player; } }

	protected override void Awake()
	{
		base.Awake();
	}
	public override void Init(Data data)
	{
		base.Init(data);

		// 嘴巴位置获得
		 GameObject go= GameObjectUtil.FindGameObjectByName(boneNameMouth, gameObject);
		Debug.Assert(go, "transMouth is not found.");
		colliderMouth = go.GetComponent<BoxCollider>();
		Debug.Assert(colliderMouth, "colliderMouth is not found.");
	}

	public void TouchUp(BaseEventData data)
	{
		curDir = Dir;
		Dir = Vector3.zero;
		moveDir = Vector3.zero;
	}

	public void TouchDown(BaseEventData data)
	{
		Dir = Vector3.zero;
	}

	public void TouchDrag(BaseEventData data, Vector3 pos, float maxLength)
	{
		Dir = pos / maxLength;
		Dir.z = Dir.y;
		Dir.y = 0;
		if (Dir.x != 0 || Dir.z != 0)
		{
			Dir = (0.9f * Dir.sqrMagnitude + 0.101f) * Dir.normalized;
			if (curDir.sqrMagnitude < 0.001f)
				curDir = transform.forward;
		}
	}

	public override void CustomUpdate()
	{
		switch (actionStep)
		{
			case ActionType.Idle:
				Idle();
				break;
			case ActionType.Eatting:
				Eatting();
				break;
		}

	}

	public void Eatting()
	{
		if (!animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("eat"))
		{
			data.moveSpeed = originalData.moveSpeed;
			actionStep = ActionType.Idle;
		}
		
		base.CustomUpdate();
	}
	public void Idle()
	{
		base.CustomUpdate();

		// 吞噬
		ManagerGroup.GetInstance().fishManager.EatCheck(this, colliderMouth);
	}



	public void Atk(FishBase fish)
	{
		animator.SetTrigger("Eat");
		actionStep = ActionType.Eatting;
		data.moveSpeed = 0f;

		fish.life -= (int)((float)data.atk * transform.localScale.x);
		if (fish.data.life <= 0)
		{
			fish.Die();
			Eat(fish);
		}
	}
	public void Eat(FishBase fish)
	{
		life += (int)(fish.lifeMax * GameConst.HealLifeFromEatRate);

		if (fishType == FishType.Player)
		{
			if (ManagerGroup.GetInstance().fishManager.GetAlivePlayer().Count <= 1)
			{
				ManagerGroup.GetInstance().GotoResult(1);
			}
		}
		
	}

	public override void Die()
	{
		base.Die();
		if (fishType == FishType.Player)
		{
			ManagerGroup.GetInstance().GotoResult(ManagerGroup.GetInstance().fishManager.GetAlivePlayer().Count + 1);
		}
	}


}

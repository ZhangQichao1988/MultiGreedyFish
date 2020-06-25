using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerBase : FishBase
{
	string boneNameMouth = "head_end";
	Transform transMouth = null;

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
		transMouth = GameObjectUtil.FindGameObjectByName(boneNameMouth, gameObject).transform;
		Debug.Assert(transMouth, "transMouth is not found.");

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
		base.CustomUpdate();

		// 吞噬
		ManagerGroup.GetInstance().fishManager.EatCheck(this, transMouth.position, 0.3f * data.size);
	}



	public void Atk(FishBase fish)
	{
		fish.dmgCoolTime = GameConst.EnemyDmgCoolTime;
		fish.life -= data.atk;
		if (fish.data.life <= 0)
		{
			fish.Die();
			Eat();
		}
	}
		public void Eat()
	{
		data.size += GameConst.PlayerSizeUpRate;
		data.size = Math.Min(GameConst.FishMaxScale, data.size);
		ApplySize();
	}

	private void ApplySize()
	{
		transform.localScale = Vector3.one * data.size;
	}

}

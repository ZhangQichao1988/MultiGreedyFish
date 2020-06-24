using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerBase : FishBase
{
	static readonly string lifeGaugePath = "Prefabs/PlayerLifeGauge";
	string boneNameMouth = "head_end";
	Transform transMouth = null;
	Slider lifeGauge = null;

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

		// 生命条
		Object obj = Resources.Load(lifeGaugePath);
		GameObject go = Wrapper.CreateGameObject(obj, transform) as GameObject;
		lifeGauge = go.GetComponentInChildren<Slider>();
		Debug.Assert(lifeGauge, "lifeGauge is not found.");
		lifeGauge.maxValue = data.life;
		lifeGauge.value = data.life;
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
		Dir.z = 0;
		//Dir.y = 0;
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
		ManagerGroup.GetInstance().enemyManager.EatCheck(this, transMouth.position, 0.3f * data.size);
	}

	public void Eat(int num)
	{
		data.size += GameConst.PlayerSizeUpRate * num;
		ApplySize();
	}

	private void ApplySize()
	{
		transform.localScale = Vector3.one * data.size;
	}

}

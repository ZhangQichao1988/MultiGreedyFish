using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerBase : FishBase
{
	static readonly string boneNameMouth = "eye";


	public BoxCollider colliderMouth = null;
	public GameObject goNamepalte = null;

	protected override bool showLifeGauge { get { return true; } }

	public override FishType fishType { get { return FishType.Player; } }

	public FishSkillBase fishSkill { get; private set; }


	protected override void Awake()
	{
		base.Awake();
	}
	public override void Init(int fishId, string playerName)
	{
		base.Init(fishId, playerName);

		fishSkill = FishSkillBase.SetFishSkill(this, fishBaseData.skillId);

		// 嘴巴位置获得
		 GameObject go= GameObjectUtil.FindGameObjectByName(boneNameMouth, gameObject);
		Debug.Assert(go, "transMouth is not found.");
		colliderMouth = go.GetComponent<BoxCollider>();
		Debug.Assert(colliderMouth, "colliderMouth is not found.");

		CreateNameplate(data.name);
	}
	public override bool Damage(int dmg, Transform hitmanTrans)
	{
		bool ret = base.Damage(dmg, hitmanTrans);
		if (ret)
		{
			animator.SetTrigger("Damage");
		}
		return ret;
	}
	protected void CreateNameplate(string playerName)
	{
		// 生命条
		string prefabPath = fishType == FishType.Player ? AssetPathConst.playerNameplatePrefabPath : AssetPathConst.robotNameplatePrefabPath;
		GameObject go = ResourceManager.LoadSync(prefabPath, typeof(GameObject)).Asset as GameObject;
		goNamepalte = GameObjectUtil.InstantiatePrefab(go, gameObject, false);
		//UnityEngine.Object obj = Resources.Load(prefabPath);
		//goNamepalte = Wrapper.CreateGameObject(obj, transform) as GameObject;
		Text textName = goNamepalte.GetComponentInChildren<Text>();
		textName.text = playerName;

	}

	protected override float SetAlpha(float alpha)
	{
		alpha = base.SetAlpha(alpha);
		return alpha;
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

	protected override void MoveUpdate()
	{
		switch (actionStep)
		{
			case ActionType.Born:
				Born();
				break;
			case ActionType.Idle:
				Idle();
				break;
			case ActionType.Eatting:
				Eatting();
				break;
			case ActionType.Die:
				DieWait();
				break;
		}

	}

    public override void CustomUpdate()
    {
		fishSkill.Update();
		base.CustomUpdate();
    }

    void DieWait()
	{
		remainingTime -= Time.deltaTime;
		if (remainingTime <= 0)
		{
			BattleManagerGroup.GetInstance().fishManager.listFish.Remove(this);
			Destroy(gameObject);
		}
		else
		{
			float progress = remainingTime / BattleConst.instance.EatFishTime;
			transform.localScale = Vector3.one * Mathf.Lerp(0, localScaleBackup, progress);

			if (eatFishTrans != null)
			{ transform.position = Vector3.Lerp(eatFishTrans.position, transform.position, progress); }
		}
	}
	void Born()
	{
		actionStep = ActionType.Idle;
	}

	public void Eatting()
	{
		remainingTime -= Time.deltaTime;
		if (remainingTime <= 0)
		{
			data.moveSpeed = originalData.moveSpeed;
			actionStep = ActionType.Idle;
		}
		
		base.MoveUpdate();
	}
	public virtual void Idle()
	{
		base.MoveUpdate();

		// 吞噬
		BattleManagerGroup.GetInstance().fishManager.EatEnemyCheck(this, colliderMouth);

		// 吃珍珠判定
		EatPearlCheck();
	}

	protected override Vector3 GetBornPosition()
	{
		return Quaternion.AngleAxis(data.uid * 36f, Vector3.up) * Vector3.right * (GetSafeRudius() - 5f);
	}

	protected void EatPearlCheck()
	{
		BattleManagerGroup.GetInstance().shellManager.EatPearl(this);
	}
	public void Atk(FishBase fish)
	{
		animator.SetTrigger("Attack");
		remainingTime = BattleConst.instance.AttackHardTime;
		actionStep = ActionType.Eatting;
		//data.moveSpeed = 0f;
		canStealthRemainingTime = BattleConst.instance.CanStealthTimeFromDmg;
		fishSkill.CbAttack();
		fish.Damage( (int)((float)data.atk * transform.localScale.x), colliderMouth.transform);
		if (fish.life <= 0)
		{
			Eat(fish);
		}

	}
	public virtual void Eat(FishBase fish)
	{
		Heal((int)(fish.lifeMax * BattleConst.instance.HealLifeFromEatRate));
		//fish.Die(colliderMouth.transform);
		animator.SetTrigger("Eat");
	}

	public override void Die( Transform eatFishTrans )
	{
		actionStep = ActionType.Die;
		localScaleBackup = transform.localScale.x;
		remainingTime = BattleConst.instance.EatFishTime;
		this.eatFishTrans = eatFishTrans;
	}


}

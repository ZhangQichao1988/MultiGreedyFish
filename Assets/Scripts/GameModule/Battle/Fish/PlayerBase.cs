using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerBase : FishBase
{
	static readonly string boneNameMouth = "eye";

	// 当前等级排名
	public int battleLevelRanking = 0;

	public BoxCollider colliderMouth = null;
	public GameObject goNamepalte = null;

	protected override bool showLifeGauge { get { return true; } }
	protected virtual bool showNameplate { get { return true; } }


	public override FishType fishType { get { return FishType.Player; } }

	public FishSkillBase fishSkill { get; private set; }


	protected List<FishBase> fishBasesInRange;
	protected Vector3 closestAquatic;

	protected override void Awake()
	{
		base.Awake();
	}
	public override void Init(int fishId, string playerName, float level, string rankIcon = "")
	{
		base.Init(fishId, playerName, level, rankIcon);

		fishSkill = FishSkillBase.SetFishSkill(this, fishBaseData.skillId);

		// 嘴巴位置获得
		 GameObject go= GameObjectUtil.FindGameObjectByName(boneNameMouth, gameObject);
		Debug.Assert(go, "transMouth is not found.");
		colliderMouth = go.GetComponent<BoxCollider>();
		Debug.Assert(colliderMouth, "colliderMouth is not found.");
		if (showNameplate)
		{
			CreateNameplate(data.name, level, rankIcon);
		}
		fishBasesInRange = BattleManagerGroup.GetInstance().fishManager.GetEnemiesInRange(this, transform.position, BattleConst.instance.RobotVision);
	}
	public override bool Damage(int dmg, Transform hitmanTrans)
	{
		bool ret = base.Damage(dmg, hitmanTrans);
		if (ret)
		{
			if (fishType == FishType.Player)
			{
				StartCoroutine(Vibrate(VibrationMng.VibrationType.Normal));
			}
			animator.SetTrigger("Damage");
		}
		return ret;
	}
	protected void CreateNameplate(string playerName, float level, string rankIcon)
	{
		// 生命条
		GameObject go = ResourceManager.LoadSync(AssetPathConst.playerNameplatePrefabPath, typeof(GameObject)).Asset as GameObject;
		goNamepalte = GameObjectUtil.InstantiatePrefab(go, gameObject, false);
		//UnityEngine.Object obj = Resources.Load(prefabPath);
		//goNamepalte = Wrapper.CreateGameObject(obj, transform) as GameObject;
		var textName = goNamepalte.transform.Find("TextName").GetComponent<Text>();
		textName.text = playerName;
		if (fishType == FishType.PlayerRobot)
		{
			textName.color = Color.red;
		}

		goNamepalte.transform.Find("TextLevel").GetComponent<Text>().text = "Lv" + level;

		var spAsset = ResourceManager.LoadSync<Sprite>(Path.Combine(AssetPathConst.texCommonPath, rankIcon));
		goNamepalte.transform.Find("ImageRank").GetComponent<Image>().sprite = spAsset.Asset;
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
	protected override void MoveInit()
	{
		if ((actionWaitCnt + uid) % 3 != 0) { return; }
		fishBasesInRange = BattleManagerGroup.GetInstance().fishManager.GetEnemiesInRange(this, transform.position, BattleConst.instance.RobotVision);
	}
	protected override void MoveUpdate()
	{
		switch (actionStep)
		{
			case ActionType.Born:
				Born();
				break;
			case ActionType.Idle:
				if (isFrozen) { return; }
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
		BattleManagerGroup.GetInstance().fishManager.EatCheck(this, colliderMouth, fishBasesInRange);

		// 吃珍珠判定
		EatPearlCheck();
	}

	protected override Vector3 GetBornPosition()
	{
		return Quaternion.AngleAxis(uid * 36f, Vector3.up) * Vector3.right * (BattleConst.instance.BgBound - 5f);
	}

	protected void EatPearlCheck()
	{
		BattleManagerGroup.GetInstance().shellManager.EatPearl(this);
	}
	IEnumerator Vibrate(VibrationMng.VibrationType vibrationType)
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		yield return new WaitForSeconds(0.15f);
#endif
		VibrationMng.ShortVibration(vibrationType);
		yield return null;

	}
	public void Atk(FishBase fish)
	{
		animator.SetTrigger("Attack");
		//remainingTime = BattleConst.instance.AttackHardTime;
		//actionStep = ActionType.Eatting;
		//canStealthRemainingTime = BattleConst.instance.CanStealthTimeFromDmg;
		fishSkill.CbAttack();
		fish.Damage(data.atk, colliderMouth.transform);
		if (fish.life <= 0)
		{
			if(fishType == FishType.Player)
			{   // 任务相关
				int actionId = 0;
				switch (fish.originalData.fishId)
				{
					case 0: actionId = 1; break;
					case 1: actionId = 30; break;
					case 2: actionId = 11; break;
					case 3: actionId = 25; break;
					case 4: actionId = 8; break;
					case 5: actionId = 7; break;
					case 6: actionId = 31; break;
					case 7: actionId = 32; break;
					case 8: actionId = 26; break;
					case 9: actionId = 27; break;
					case 10: actionId = 29; break;
					case 11: actionId = 28; break;
				}
				PlayerModel.Instance.MissionActionTriggerAdd(actionId, 1);
				if (fish.fishType == FishType.PlayerRobot)
				{
					if (data.isShield)
					{
						PlayerModel.Instance.MissionActionTriggerAdd(22, 1);
					}
					if (isStealth)
					{
						PlayerModel.Instance.MissionActionTriggerAdd(23, 1);
					}
				}
			}
			
			Eat(fish.battleLevel);
		}

	}
	public virtual void Eat(float fishLevel)
	{
		
		//Heal((int)(fish.lifeMax * BattleConst.instance.HealLifeFromEatRate));
		this.battleLevel += fishLevel * ConfigTableProxy.Instance.GetDataById(8).floatValue;
		//fishLevel += fish.fishLevel * 0.1f;
		int _life = FishLevelUpDataTableProxy.Instance.GetFishHp(fishBaseData, this.fishLevel, this.battleLevel);
		int _atk = FishLevelUpDataTableProxy.Instance.GetFishAtk(fishBaseData, this.fishLevel, this.battleLevel);
		int lifeMax = data.lifeMax;
		data.lifeMax = _life;
		life += _life - lifeMax;
		data.atk = _atk;

		ApplySize();
		this.originalData.atk = data.atk;
		animator.SetTrigger("Eat");
		if (isBecameInvisible)
		{
			VibrationMng.VibrationType vibrationType = VibrationMng.VibrationType.Short;
			if (fishLevel > 5)
			{
				BattleEffectManager.CreateEffect(4, lifeGauge.dmgExpLocation.transform);
				vibrationType = VibrationMng.VibrationType.Normal;
			}
			else if (fishLevel > 1)
			{
				BattleEffectManager.CreateEffect(3, lifeGauge.dmgExpLocation.transform);
			}
			else
			{
				BattleEffectManager.CreateEffect(2, lifeGauge.dmgExpLocation.transform);
			}
			if (fishType == FishType.Player)
			{
				StartCoroutine(Vibrate(vibrationType));
			}
		}
        
    }

	public override void Die( Transform eatFishTrans )
	{
		actionStep = ActionType.Die;
		localScaleBackup = transform.localScale.x;
		remainingTime = BattleConst.instance.EatFishTime;
		this.eatFishTrans = eatFishTrans;

		// 战斗结果检测
		BattleManagerGroup.GetInstance().inGameUIPanel.CheckBattleEnd();
	}
	protected override void AquaticCheck()
	{
		if (!isBecameInvisible) { return; }

		canStealthRemainingTime = Math.Max(0f, canStealthRemainingTime - Time.deltaTime);
		Vector3 myPos = transform.position;
		if ((actionWaitCnt + uid) % 3 != 0) 
		{
			List<Transform> listTransAquatic = new List<Transform>(BattleManagerGroup.GetInstance().aquaticManager.listTransAquatic);
			float minDistance = float.MaxValue;
			float distance;
			closestAquatic = Vector3.zero;
			Vector3 tmpPos;
			for (int i = 0; i < listTransAquatic.Count; ++i)
			{
				tmpPos = listTransAquatic[i].position;
				if (new Vector2(tmpPos.x, tmpPos.z).sqrMagnitude > BattleManagerGroup.GetInstance().poisonRing.GetPoisonRangePow())
				{ continue; }
				distance = Vector3.Distance(myPos, tmpPos);
				if (distance < minDistance)
				{
					minDistance = distance;
					closestAquatic = tmpPos;
				}
			}
		}

		closestAquatic = new Vector3(closestAquatic.x, 0f, closestAquatic.z);

		// 在水草里恢复血量
		if (closestAquatic != Vector3.zero && 
			Vector3.Distance(closestAquatic, myPos) <= BattleConst.instance.AquaticRange && 
			canStealthRemainingTime <= 0f)
		{
			if (!beforeInAquatic)
			{
				inAquaticHealCnt = 0;
			}
			beforeInAquatic = true;
			inAquaticTime += Time.deltaTime;
		}
		else
		{

			inAquaticTime = 0;
			beforeInAquatic = false;
		}

		if (beforeInAquatic && inAquaticTime >= inAquaticHealCnt * BattleConst.instance.AquaticHealCoolTime)
		{
			inAquaticHealCnt++;
			int healLife = (int)(BattleConst.instance.AquaticHeal * (float)lifeMax);
			healLife = Mathf.Min(lifeMax - life, healLife);
			Heal(healLife);
			if (fishType == FishType.Player)
			{
				PlayerModel.Instance.MissionActionTriggerAdd(6, healLife);
			}
		}

		// 在水草里透明
		float stealthAlpha = 1f;
		switch (fishType)
		{
			case FishType.Player:
				stealthAlpha = 0.3f;
				break;
			case FishType.PlayerRobot:
				stealthAlpha = 0f;
				break;
		}
		SetAlpha(beforeInAquatic || isStealth ? stealthAlpha : 1f);
	}

}

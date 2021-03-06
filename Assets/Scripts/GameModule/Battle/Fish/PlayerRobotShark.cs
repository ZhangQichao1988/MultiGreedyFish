using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRobotShark : PlayerRobotBase
{
	public override FishType fishType { get { return FishType.Boss; } }
	protected override bool showLifeGauge { get { return true; } }
	protected override bool showNameplate { get { return false; } }

	float growingUpRemainingTime;
	public override void Init(int fishId, string playerName, float level, string rankIcon = "")
	{
		base.Init(fishId, playerName, level);
		battleLevel = ConfigTableProxy.Instance.GetDataById(36).floatValue;
		if (TutorialControl.IsStep(TutorialControl.Step.GotoTutorialBattle))
		{
			life = (int)(lifeMax * ConfigTableProxy.Instance.GetDataById(40).floatValue);
		}
	}
	protected override Vector3 GetBornPosition()
	{
		return Vector3.zero;
	}
	protected override void ApplySize()
	{
		transform.localScale = Vector3.one * (0.5f + 2.5f * lifeRate);
	}
	public override bool Damage(int dmg, Transform hitmanTrans, AttackerType attackerType)
	{
		// 为了在击杀鲨鱼教学模式以外无法击杀鲨鱼
		if (!BattleManagerGroup.GetInstance().IsTutorialStep(BattleManagerGroup.TutorialStep.None) && !BattleManagerGroup.GetInstance().IsTutorialStep(BattleManagerGroup.TutorialStep.KillSharkMissionChecking))
		{
			if (data.life - dmg <= 0)
			{
				dmg = data.life - 1;
			}
		}
        bool ret = base.Damage(dmg, hitmanTrans, attackerType);
        if (ret)
        {
            canStealthRemainingTime = BattleConst.instance.CanStealthTimeFromDmg;
            animator.SetTrigger("Damage");
        }
        return ret;
    }

	// 不会透明
	protected override float SetAlpha(float alpha)
	{
		return 1f;
	}
	protected override void MoveInit()
	{
		if ((actionWaitCnt + uid) % thinkingTime != 0) { return; }
		fishBasesInRange = BattleManagerGroup.GetInstance().fishManager.GetAlivePlayer();
	}

	protected override void CalcMoveAction()
	{
		// 追踪最近的玩家
		var listFish = new List<FishBase>(fishBasesInRange);

		// 判定是否有白名单的鱼
		//bool hasWhiteFish = false;
		float nearestDis = float.MaxValue;
		float tmp;
		// 把新发现的，隐身的鱼排除
		for (int i = listFish.Count - 1; i >= 0; --i)
		{
			if (listFish[i] == null)
			{
				listFish.RemoveAt(i);
				continue;
			}
			// 排除隐身的鱼
			if (listFish[i].isStealth || listFish[i].isShield)
			{
				listFish.RemoveAt(i);
				continue;
			}

			// 新发现的鱼
			if (listFindedFish != null && !listFindedFish.Contains(listFish[i]))
			{
				if (listFish[i].beforeInAquatic)
				{
					listFish.RemoveAt(i);
					continue;
				}
			}
			tmp = listFish[i].transform.position.sqrMagnitude;
			if (nearestDis > tmp)
			{
				nearestDis = tmp;
				targetFish = listFish[i];
			}
		}
		if (targetFish != null)
		{
			targetPos = targetFish.transform.position;
		}

		listFindedFish = listFish;
	}

	public override void SetRobot(RobotAiDataInfo aiData, float growth)
	{
		this.growth = growth;
		float[] aryParam = Wrapper.GetParamFromString(aiData.aryParam);
		targetCntTimeLimit = aryParam[0];
		targetCntTime = targetCntTimeLimit;

	}
	public override void Atk(FishBase fish)
	{
		base.Atk(fish);
	}
	public override void CustomUpdate()
	{
		base.CustomUpdate();

		// 不在屏幕内的时候，定期成长
		if (!isBecameInvisible)
		{
			growingUpRemainingTime -= Time.deltaTime;
			if (growingUpRemainingTime <= 0f)
			{
				growingUpRemainingTime = 5f;
				int atkMax = 0;
				var listPlayer = BattleManagerGroup.GetInstance().fishManager.GetAlivePlayer();
				foreach (var note in listPlayer)
				{
					if (note.data.atk > atkMax)
					{
						atkMax = note.data.atk;
					}
				}
				data.atk = (int)(atkMax * ConfigTableProxy.Instance.GetDataById(39).floatValue);
			}
		}

	}
	public override void Eat(float fishLevel)
	{
		//fish.Die(colliderMouth.transform);
		//base.Eat(fishLevel);
		animator.SetTrigger("Eat");
		BattleEffectManager.CreateEffect(4, lifeGauge.dmgExpLocation.transform);
	}
}

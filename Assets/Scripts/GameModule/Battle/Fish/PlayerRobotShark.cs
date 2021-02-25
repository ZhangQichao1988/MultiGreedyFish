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
	}
	protected override Vector3 GetBornPosition()
	{
		return Vector3.zero;
	}
	protected override void ApplySize()
	{
		transform.localScale = Vector3.one * 3;
	}
	public override bool Damage(int dmg, Transform hitmanTrans)
	{
        bool ret = base.Damage(dmg, hitmanTrans);
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

	protected override void CalcMoveAction()
	{
		// 过一段时间改变一下方向
		//changeVectorRemainingTime -= Time.deltaTime;

		// 追踪最近的玩家
		//List<FishBase> listFish = BattleManagerGroup.GetInstance().fishManager.GetAlivePlayerSort(transform.position);
		var listFish = new List<FishBase>(fishBasesInRange);

		// 判定是否有白名单的鱼
		//bool hasWhiteFish = false;

		// 把新发现的，隐身的鱼排除
		for (int i = listFish.Count - 1; i >= 0; --i)
		{
			//// 剔除白名单的鱼
			//if (whiteFish == listFish[i] &&
			//	 (listFish[i].transform.position - transform.position).magnitude <= ConfigTableProxy.Instance.GetDataById(34).floatValue)
			//{
			//	hasWhiteFish = true;
			//	listFish.RemoveAt(i);
			//	continue;
			//}
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
				}
			}
		}

		// 视野范围没有白名单的鱼的话
		//if (!hasWhiteFish)
		//{
		//	whiteFish = null;
		//}

		listFindedFish = listFish;
		if (listFish.Count > 0 && (listFish[0].transform.position-transform.position).magnitude <= ConfigTableProxy.Instance.GetDataById(34).floatValue)
		{
			FishBase target;
			//if (targetFish == listFish[0])
			//{
			//	targetCntTime -= Time.deltaTime;
			//	if (targetCntTime <= 0 && listFish.Count > 1)
			//	{
			//		whiteFish = listFish[0];
			//		target = listFish[1];
			//	}
			//	else
			//	{
			//		target = listFish[0];
			//	}
			//}
			//else
			{
				target = listFish[0];
				targetCntTime = targetCntTimeLimit;
			}
			targetFish = target;
			MoveToTarget(targetFish.transform.position);
		}
		else
		{
			EnemyIdle();
		}

	}

	public override void SetRobot(RobotAiDataInfo aiData, float growth)
	{
		this.growth = growth;
		float[] aryParam = Wrapper.GetParamFromString(aiData.aryParam);
		targetCntTimeLimit = aryParam[0];
		targetCntTime = targetCntTimeLimit;

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
				data.atk = atkMax / 2;
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

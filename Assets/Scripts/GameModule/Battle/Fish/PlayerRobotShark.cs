using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRobotShark : PlayerRobotBase
{
	public override FishType fishType { get { return FishType.Boss; } }
	protected override bool showLifeGauge { get { return true; } }

	protected override Vector3 GetBornPosition()
	{
		return Vector3.zero;
	}
	protected override void ApplySize()
	{
		transform.localScale = Vector3.one * 3;
	}
	public override void Damge()
	{
		canStealthRemainingTime = BattleConst.CanStealthTimeFromDmg;
		animator.SetTrigger("Damage");
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
		List<FishBase> listFish = BattleManagerGroup.GetInstance().fishManager.GetAlivePlayerSort(transform.position);

		// 把新发现的，隐身的鱼排除
		for (int i = listFish.Count - 1; i >= 0; --i)
		{
			// 新发现的鱼
			if (listFindedFish != null && !listFindedFish.Contains(listFish[i]))
			{
				if (listFish[i].beforeInAquatic)
				{
					listFish.RemoveAt(i);
				}
			}
		}
		listFindedFish = listFish;
		if (listFish.Count > 0)
		{
			FishBase target = listFish[0];
			MoveToTarget(target.transform.position);
		}
		else
		{
			EnemyIdle();
		}

	}


	public override void Eat(FishBase fish)
	{
		fish.Die(colliderMouth.transform);
		animator.SetTrigger("Eat");
	}
}

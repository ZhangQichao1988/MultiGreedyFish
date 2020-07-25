using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRobotShark : PlayerRobotBase
{
    public override FishType fishType { get { return FishType.Boss; } }

	protected override Vector3 GetBornPosition()
	{
		return Vector3.zero;
	}
	protected override void ApplySize()
	{
		//transform.localScale = Vector3.one;
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
}

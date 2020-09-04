using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleConst
{
	static BattleConst _instance;
	public static BattleConst instance 
	{ 
		get 
		{
			if (_instance == null)
			{
				_instance = new BattleConst();
			}
			return _instance;
		}
	}

	BattleConst()
	{
		PoisonRingScaleSpeed = ConfigTableProxy.Instance.GetDataByKey("PoisonRingScaleSpeed");
		PoisonRingRadiusMin = ConfigTableProxy.Instance.GetDataByKey("PoisonRingRadiusMin");
		PoisonRingRadiusMax = ConfigTableProxy.Instance.GetDataByKey("PoisonRingRadiusMax");
		PoisonRingDmg = (int)ConfigTableProxy.Instance.GetDataByKey("PoisonRingDmg");
		PoisonRingDmgCoolTime = ConfigTableProxy.Instance.GetDataByKey("PoisonRingDmgCoolTime");
		BgBound = ConfigTableProxy.Instance.GetDataByKey("BgBound");
		AquaticHeal = (int)ConfigTableProxy.Instance.GetDataByKey("AquaticHeal");
		AquaticHealCoolTime = ConfigTableProxy.Instance.GetDataByKey("AquaticHealCoolTime");
		PlayerSizeUpRate = ConfigTableProxy.Instance.GetDataByKey("PlayerSizeUpRate");
		FishMaxScale = ConfigTableProxy.Instance.GetDataByKey("FishMaxScale");
		HealLifeFromEatRate = ConfigTableProxy.Instance.GetDataByKey("HealLifeFromEatRate");
		EatFishTime = ConfigTableProxy.Instance.GetDataByKey("EatFishTime");
		AttackHardTime = ConfigTableProxy.Instance.GetDataByKey("AttackHardTime");
		RobotVision = new Vector2(ConfigTableProxy.Instance.GetDataByKey("RobotVisionX"), ConfigTableProxy.Instance.GetDataByKey("RobotVisionY"));
		RobotVisionRange = ConfigTableProxy.Instance.GetDataByKey("RobotVisionRange");
		EnemyResurrectionRemainingTime = ConfigTableProxy.Instance.GetDataByKey("EnemyResurrectionRemainingTime");
		AquaticRange = ConfigTableProxy.Instance.GetDataByKey("AquaticRange");
		CanStealthTimeFromDmg = ConfigTableProxy.Instance.GetDataByKey("CanStealthTimeFromDmg");
		EatPearlRange = ConfigTableProxy.Instance.GetDataByKey("EatPearlRange");
		OpenShellRemainingTime = ConfigTableProxy.Instance.GetDataByKey("OpenShellRemainingTime");
		ShellOpenningTime = ConfigTableProxy.Instance.GetDataByKey("ShellOpenningTime");
		ShellCloseDmg = (int)ConfigTableProxy.Instance.GetDataByKey("ShellCloseDmg");
		ShellPearlResetRate = ConfigTableProxy.Instance.GetDataByKey("ShellPearlResetRate");
	}
	public readonly bool FreeMode = false;																											// 只剩自己也不会胜利


	public float PoisonRingScaleSpeed = 0.6f;                                                                                         // 毒圈缩小速度
	public float PoisonRingRadiusMin = 5f;																								// 毒圈的最小半径
	public float PoisonRingRadiusMax = 120f;																	                    // 毒圈的最大半径
	public int PoisonRingDmg = 10;																										// 毒圈伤害量
	public float PoisonRingDmgCoolTime = 1f;																	                     // 毒圈伤害间隙

	public float BgBound;		// 鱼可游动范围

	public int AquaticHeal = 10;																	// 水草恢复量
	public float AquaticHealCoolTime = 1f;                                                 // 水草恢复血间隙


	public float PlayerSizeUpRate = 0.3f;                                                         // 吃一条鱼变大倍率
	public float FishMaxScale = 3f;                                                                 // 鱼最大体积倍率
	public float HealLifeFromEatRate = 1f;                                                    // 吃一条鱼可恢复对方体力上线的比例
	public float EatFishTime = 0.2f;                                                                        // 鱼被吃掉的时候缩小的时间
	public float AttackHardTime = 0.35f;															// 攻击鱼之后的硬直时间

	// 机器人相关
	public Vector2 RobotVision;                                 // 机器人视野（矩形）
	public float RobotVisionRange = 557f;					                                    // 机器人视野半径平方（圆型）

	public float EnemyResurrectionRemainingTime = 3f;                               // 杂鱼死亡后的复活间隙

	public float AquaticRange = 2f;                                                                 // 水草的范围
	public float CanStealthTimeFromDmg = 2f;                                                // 受伤之后多久不能隐身&恢复生命

	public float EatPearlRange = 3f;                                                                 // 吃珍珠的判定半径
	public float OpenShellRemainingTime = 5f;												// 打开贝壳的时间间隔
	public float ShellOpenningTime = 2f;                                                        // 贝壳开着的时间
	public int ShellCloseDmg = 100;                                                                  // 贝壳关闭伤害
	public float ShellPearlResetRate = 0.2f;                                                       // 贝壳重置珍珠概率



}

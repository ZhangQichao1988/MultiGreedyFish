using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleConst
{
	static readonly public bool FreeMode = false;																											// 只剩自己也不会胜利
	static readonly public int EnemyNumMax = 100;                                                                                                                   // 杂鱼最多数量
	static readonly public int EnemyNumMin = 5;																														// 杂鱼最少数量


	static readonly public float PoisonRingScaleSpeed = 0.6f;                                                                                         // 毒圈缩小速度
	static readonly public float PoisonRingRadiusMin = 5f;																								// 毒圈的最小半径
	static readonly public float PoisonRingRadiusMax = 120f;																	                    // 毒圈的最大半径
	static readonly public int PoisonRingDmg = 10;																										// 毒圈伤害量
	static readonly public float PoisonRingDmgCoolTime = 1f;																	                     // 毒圈伤害间隙

	static readonly public Vector2 BgBound = new Vector2(85f, 85f);		// 鱼可游动范围

	static readonly public int AquaticHeal = 10;																	// 水草恢复量
	static readonly public float AquaticHealCoolTime = 1f;                                                 // 水草恢复血间隙


	static readonly public float PlayerSizeUpRate = 0.3f;                                                         // 吃一条鱼变大倍率
	static readonly public float FishMaxScale = 3f;                                                                 // 鱼最大体积倍率
	static readonly public float HealLifeFromEatRate = 1f;                                                    // 吃一条鱼可恢复对方体力上线的比例
	static readonly public float EatFishTime = 0.2f;                                                                        // 鱼被吃掉的时候缩小的时间
	static readonly public float AttackHardTime = 0.35f;															// 攻击鱼之后的硬直时间

	// 机器人相关
	static readonly public Vector2 RobotVision = new Vector2(20, 10);										// 机器人视野

	static readonly public float EnemyResurrectionRemainingTime = 3f;                               // 杂鱼死亡后的复活间隙

	static readonly public float AquaticRange = 2f;                                                                 // 水草的范围
	static readonly public float CanStealthTimeFromDmg = 2f;                                                // 受伤之后多久不能隐身&恢复生命

	static readonly public float EatPearlRange = 3f;                                                                 // 吃珍珠的判定半径
	static readonly public float OpenShellRemainingTime = 5f;												// 打开贝壳的时间间隔
	static readonly public float ShellOpenningTime = 2f;                                                        // 贝壳开着的时间
	static readonly public int PearlRecoverLife = 100;                                                               // 珍珠恢复血量
	static readonly public int ShellCloseDmg = 100;                                                                  // 贝壳关闭伤害
	static readonly public float ShellPearlResetRate = 0.2f;                                                       // 贝壳重置珍珠概率


}

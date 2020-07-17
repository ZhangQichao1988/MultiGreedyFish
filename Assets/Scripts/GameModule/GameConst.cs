using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConst
{
	static readonly public bool FreeMode = false;																											// 只剩自己也不会胜利
	static readonly public int EnemyNumMax = 100;                                                                                                                   // 杂鱼最多数量
	static readonly public int EnemyNumMin = 5;																														// 杂鱼最少数量


	static readonly public float PoisonRingScaleSpeed = 0.6f;                                                                                         // 毒圈缩小速度
	static readonly public float PoisonRingRadiusMin = 5f;																								// 毒圈的最小半径
	static readonly public float PoisonRingRadiusMax = 100f;																	                    // 毒圈的最大半径
	static readonly public int PoisonRingDmg = 10;																										// 毒圈伤害量
	static readonly public float PoisonRingDmgCoolTime = 1f;																	                     // 毒圈伤害间隙

	static readonly public Vector2 BgBound = new Vector2(PoisonRingRadiusMax-4f, PoisonRingRadiusMax - 4f);		// 鱼可游动范围

	static readonly public int AquaticHeal = 10;																	// 水草恢复量
	static readonly public float AquaticHealCoolTime = 1f;                                                 // 水草恢复血间隙


	static readonly public float PlayerSizeUpRate = 0.3f;                                                         // 吃一条鱼变大倍率
	static readonly public float FishMaxScale = 3f;                                                                 // 鱼最大体积倍率
	static readonly public float HealLifeFromEatRate = 1f;                                                    // 吃一条鱼可恢复对方体力上线的比例
	static readonly public float EatFishTime = 0.2f;																		// 鱼被吃掉的时候缩小的时间

	// 机器人相关
	static readonly public Vector2 RobotFindFishRange = new Vector2(10, 20);                    // 机器人发现目标范围

	static readonly public float EnemyResurrectionRemainingTime = 3f;                               // 杂鱼死亡后的复活间隙

	static readonly public float AquaticRange = 2f;                                                                 // 水草的范围
	static readonly public float CanStealthTimeFromDmg = 2f;												// 受伤之后多久不能隐身&恢复生命









	static readonly public string PlayerName = "江小鱼";                                                         // 玩家名
	

	static readonly public string ResultText = "第{0}名";                                                         // 结算文字
	static readonly public string LanguageAddLifeMax = "+{0}Max";										// 增加血量上限的显示文字
	static readonly public string LanguageAddLife = "+{0}";													// 增加血量上限的显示文字
	static readonly public string LanguageDamage = "-{0}";													// 增加血量上限的显示文字
}

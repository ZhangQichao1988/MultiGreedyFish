using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConst
{
	static readonly public float JoyDefaultPosY = -350f;															// 默认遥感纵坐标位置

	static readonly public int EnemyNum = 50;                                                                       // 杂鱼数量
	static readonly public int RobotNum = 9;                                                                            // 机器人数量

	static readonly public Vector2 BgBound = new Vector2(48, 48);                                       // 鱼可游动范围
	static readonly public float PoisonRingScaleSpeed = 0.1f;                                             // 毒圈缩小速度
	static readonly public float PoisonRingRadiusMin = 5f;														// 毒圈的最小半径
	static readonly public float PoisonRingRadiusMax = 50f;                                                 // 毒圈的最大半径
	static readonly public int PoisonRingDmg = 10;															// 毒圈伤害量
	static readonly public float PoisonRingDmgCoolTime = 1f;                                                 // 毒圈伤害间隙

	static readonly public float PlayerSizeUpRate = 0.3f;                                                         // 吃一条鱼变大倍率
	static readonly public float FishMaxScale = 3f;                                                                 // 鱼最大体积倍率
	static readonly public float HealLifeFromEatRate = 1f;                                                    // 吃一条鱼可恢复对方体力上线的比例
	static readonly public float EatFishTime = 0.2f;																		// 鱼被吃掉的时候缩小的时间

	// 机器人相关
	static readonly public float RobotFollowBigFishLifeRate = 0.8f;										// 当体力大于多少比例，会去追踪大型鱼
	static readonly public Vector2 RobotFindFishRange = new Vector2(10, 20);                    // 机器人发现目标范围

	static readonly public float EnemyResurrectionRemainingTime = 3f;                               // 杂鱼死亡后的复活间隙
	//static readonly public float EnemyResurrectionY = 30f;                                                      // 杂鱼复活后的出生Y坐标

	static readonly public string ResultText = "第{0}名";															// 结算文字


}

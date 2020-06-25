using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConst
{
	static readonly public int EnemyNum = 50;                                                                       // 杂鱼数量
	static readonly public int RobotNum = 9;																			// 机器人数量
	static readonly public Vector2 bgBound = new Vector2(48, 48);                                       // 鱼可游动范围

	static readonly public float PlayerSizeUpRate = 0.01f;                                                         // 吃一条鱼变大倍率
	static readonly public float FishMaxScale = 3f;                                                                 // 鱼最大体积倍率

	static readonly public Vector2 RobotFindFishRange = new Vector2(10, 20);                    // 机器人发现目标范围
	static readonly public float EnemyResurrectionRemainingTime = 3f;                               // 杂鱼死亡后的复活间隙
	static readonly public float EnemyResurrectionY = 30f;                                                      // 杂鱼复活后的出生Y坐标

	static readonly public float EnemyDmgCoolTime = 0.5f;													// 受伤之后的无敌时间

}

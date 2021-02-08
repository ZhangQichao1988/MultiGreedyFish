using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffHeal : BuffBase
{
    int healCnt = 0;
    public override BuffType buffType { get { return BuffType.Heal; } }

    public BuffHeal(FishBase Initiator, FishBase fish, FishBuffDataInfo baseData) : base(Initiator, fish, baseData)
    {
    }
    public override void ApplyStatus()
    {
        int nowCnt = (int)(aryParam[0] - remainingTime);
        if (nowCnt >= healCnt)
        {
            ++healCnt;
            int healLife = (int)(aryParam[1] * fish.data.lifeMax);
            fish.Heal(healLife);
            if (fish.fishType == FishBase.FishType.Player)
            {
                PlayerModel.Instance.MissionActionTriggerAdd(17, healLife);
            }
            if (fish.isBecameInvisible) { BattleEffectManager.CreateEffect(0, fish.transModel); }
        }

    }

}

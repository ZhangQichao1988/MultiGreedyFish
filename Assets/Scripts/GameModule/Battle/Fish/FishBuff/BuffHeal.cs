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
            fish.Heal((int)(aryParam[1] * fish.data.lifeMax));
            if (fish.isBecameInvisible) { BattleEffectManager.CreateEffect(0, fish.transModel); }
        }

    }

}

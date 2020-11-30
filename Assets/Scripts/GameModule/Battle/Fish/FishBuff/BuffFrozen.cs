using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FishBase;

public class BuffFrozen : BuffParalysis
{
    Effect effect;
    public BuffFrozen(FishBase Initiator, FishBase fish, FishBuffDataInfo baseData) : base(Initiator, fish, baseData)
    {
        int effectId = BattleEffectManager.CreateEffect(10, fish.transModel);
        effect = EffectManager.GetEffect(effectId);
    }

    public override void Destory()
    {
        base.Destory();
        effect.Destroy();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FishBase;

public class BuffStealth : BuffShield
{
    public override BuffType buffType { get { return BuffType.Stealth; } }

    public BuffStealth(FishBase Initiator, FishBase fish, FishBuffDataInfo baseData) : base(Initiator, fish, baseData)
    {
    }
    public override void ApplyStatus()
    {
        fish.isStealth = true;
    }
}

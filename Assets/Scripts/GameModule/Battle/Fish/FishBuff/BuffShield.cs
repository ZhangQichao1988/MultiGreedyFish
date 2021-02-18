using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FishBase;

public class BuffShield : BuffBase
{
    public override BuffType buffType { get { return BuffType.Shield; } }

    public BuffShield(FishBase Initiator, FishBase fish, FishBuffDataInfo baseData) : base(Initiator, fish, baseData)
    {
    }
    public override void ApplyStatus()
    {
        fish.isShield = true;
    }

}

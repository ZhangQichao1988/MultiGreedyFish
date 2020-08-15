using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffSpeedEqual : BuffBase
{
    public override BuffType buffType { get { return BuffType.ChangeSpeed; } }

    public BuffSpeedEqual(FishBase Initiator, FishBase fish, FishBuffDataInfo baseData) : base(Initiator, fish, baseData)
    {
    }
    public override void ApplyStatus()
    {
        fish.data.moveSpeed = aryParam[1];
    }

}

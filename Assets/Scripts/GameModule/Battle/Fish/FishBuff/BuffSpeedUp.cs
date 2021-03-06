using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffSpeedUp : BuffBase
{
    public override BuffType buffType { get { return BuffType.ChangeSpeed; } }

    public BuffSpeedUp(FishBase Initiator, FishBase fish, FishBuffDataInfo baseData) : base(Initiator, fish, baseData)
    {
    }
    public override void ApplyStatus()
    {
        fish.data.moveSpeed = Mathf.Lerp(fish.originalData.moveSpeed * aryParam[1], fish.originalData.moveSpeed, remainingTime / aryParam[0]);
    }

}

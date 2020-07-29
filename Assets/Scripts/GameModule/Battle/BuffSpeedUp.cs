using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffSpeedUp : BuffBase
{
    public BuffSpeedUp(FishBase Initiator, FishBase fish, float[] aryParam) : base(Initiator, fish, aryParam)
    {
    }
    public override void ApplyStatus()
    {
        fish.data.moveSpeed = Mathf.Lerp(fish.originalData.moveSpeed * aryParam[1], fish.originalData.moveSpeed, remainingTime / aryParam[0]);
    }

}

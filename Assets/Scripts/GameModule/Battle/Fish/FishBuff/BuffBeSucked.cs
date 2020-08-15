using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffBeSucked : BuffBase
{
    public override BuffType buffType { get { return BuffType.ChangePostion; } }

    public BuffBeSucked(FishBase Initiator, FishBase fish, FishBuffDataInfo baseData) : base(Initiator, fish, baseData)
    {
    }
    public override void ApplyStatus()
    {
        fish.transform.position -= (fish.transform.position - Initiator.transform.position) * aryParam[1] * Time.deltaTime;
        //fish.data.moveSpeed = Mathf.Lerp(fish.originalData.moveSpeed * aryParam[1], fish.originalData.moveSpeed, remainingTime / aryParam[0]);
    }

}

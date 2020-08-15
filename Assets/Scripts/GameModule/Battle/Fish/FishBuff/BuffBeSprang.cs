using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffBeSprang : BuffBase
{
    public override BuffType buffType { get { return BuffType.ChangePostion; } }

    public BuffBeSprang(FishBase Initiator, FishBase fish, FishBuffDataInfo baseData) : base(Initiator, fish, baseData)
    {
    }
    public override void ApplyStatus()
    {
        fish.transform.position += (fish.transform.position - Initiator.transform.position) * aryParam[1] * Time.deltaTime * remainingTime;
    }

}

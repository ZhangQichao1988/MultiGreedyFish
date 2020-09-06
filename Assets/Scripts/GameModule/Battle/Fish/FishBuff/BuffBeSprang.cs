using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffBeSprang : BuffBase
{
    public override BuffType buffType { get { return BuffType.ChangePostion; } }
    Vector3 pos;
    public BuffBeSprang(FishBase Initiator, FishBase fish, FishBuffDataInfo baseData) : base(Initiator, fish, baseData)
    {
        pos = Initiator.transform.position;
    }
    public override void ApplyStatus()
    {
        fish.transform.position += (fish.transform.position - pos).normalized * aryParam[1] * Time.deltaTime * remainingTime;
    }

}

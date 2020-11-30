using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FishBase;

public class BuffParalysis : BuffBase
{
    public override BuffType buffType { get { return BuffType.ChangeSpeed; } }

    public BuffParalysis(FishBase Initiator, FishBase fish, FishBuffDataInfo baseData) : base(Initiator, fish, baseData)
    {
    }
    public override void ApplyStatus()
    {
        fish.data.moveSpeed = 0f;
    }

}

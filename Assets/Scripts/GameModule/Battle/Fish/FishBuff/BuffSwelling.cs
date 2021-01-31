using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FishBase;

public class BuffSwelling : BuffShield
{
    public override BuffType buffType { get { return BuffType.Swelling; } }

    public BuffSwelling(FishBase Initiator, FishBase fish, FishBuffDataInfo baseData) : base(Initiator, fish, baseData)
    {
    }


}

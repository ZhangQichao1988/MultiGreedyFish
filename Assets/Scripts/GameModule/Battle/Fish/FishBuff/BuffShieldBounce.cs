using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FishBase;

public class BuffShieldBounce : BuffShield
{
    public override BuffType buffType { get { return BuffType.Shield; } }

    Renderer renderer;
    public BuffShieldBounce(FishBase Initiator, FishBase fish, FishBuffDataInfo baseData) : base(Initiator, fish, baseData)
    {
        renderer = fish.colliderBody.GetComponent<Renderer>();
        renderer.material.EnableKeyword("_METAL_REF_ON");
    }

    public override void Destory()
    {
        base.Destory();
        renderer.material.DisableKeyword("_METAL_REF_ON");
    }
}

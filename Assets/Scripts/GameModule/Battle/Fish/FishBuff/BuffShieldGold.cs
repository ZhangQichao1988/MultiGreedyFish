using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FishBase;

public class BuffShieldGold : BuffShield
{
    AudioSource audioSource;
    public override BuffType buffType { get { return BuffType.ShieldGold; } }

    Renderer renderer;
    public BuffShieldGold(FishBase Initiator, FishBase fish, FishBuffDataInfo baseData) : base(Initiator, fish, baseData)
    {
        if (fish.fishType == FishType.Player) 
        {
            audioSource = SoundManager.PlaySE(17);
        }
        renderer = fish.colliderBody.GetComponent<Renderer>();
        renderer.material.EnableKeyword("_METAL_REF_ON"); 
    }

    public override void Destory()
    {
        base.Destory();
        if (audioSource)
        {
            audioSource.Stop();
        }
        renderer.material.DisableKeyword("_METAL_REF_ON");
    }
}

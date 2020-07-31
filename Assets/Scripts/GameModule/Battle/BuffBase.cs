using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffBase
{
    public float[] aryParam;
    public float remainingTime { get; private set; }
    public FishBase Initiator { get; private set; }
    public FishBase fish { get; private set; }

    public BuffBase(FishBase Initiator, FishBase fish, float[] aryParam)
    {
        this.Initiator = Initiator;
        this.fish = fish;
        this.aryParam = aryParam;
        remainingTime = aryParam[0];
    }
    public virtual void Update()
    {
        remainingTime -= Time.deltaTime;
        ApplyStatus();
    }

    public virtual bool IsDestory() 
    {
        return remainingTime <= 0f;
    }

    public virtual void Destory()
    { 
        
    }

    public virtual void ApplyStatus()
    {
        Debug.LogError("BuffBase.ApplyStatus()_1");
    }
}

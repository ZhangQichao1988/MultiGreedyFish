using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuffBase
{
    public enum BuffType
    { 
        None,
        ChangePostion,             // 位移
        ChangeSpeed,                // 变速
        Shield,                             // 护盾
    };
    public abstract BuffType buffType { get; }
    public FishBuffDataInfo baseData;
    public float[] aryParam;
    public float remainingTime { get; set; }
    public FishBase Initiator { get; private set; }
    public FishBase fish { get; private set; }

    public BuffBase(FishBase Initiator, FishBase fish, FishBuffDataInfo baseData)
    {
        this.baseData = baseData;
        this.Initiator = Initiator;
        this.fish = fish;
        float[] aryParam = Wrapper.GetParamFromString(baseData.aryParam);
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

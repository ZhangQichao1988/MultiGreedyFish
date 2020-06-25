using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : FishBase
{
    protected float remainingTime = 0f;
    protected bool isCastShadow = true;
    public override FishType fishType { get { return FishType.Enemy; } }

    public override void Init(Data data)
    {
        base.Init(data);
    }

    // Update is called once per frame
    public override void CustomUpdate()
    {
        

        switch (actionStep)
        {
            case ActionType.Born:
                Born();
                break;
            case ActionType.Idle:
                EnemyIdle();
                base.CustomUpdate();
                break;
            case ActionType.Die:
                DieWait();
                break;
        }
        
        
    }


    protected void DieWait()
    {
        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0)
        {
            actionStep = ActionType.Born;
            transModel.gameObject.SetActive(true);
            transform.position = GetBornPosition() + Vector3.up * GameConst.EnemyResurrectionY;
            Dir = new Vector3( Wrapper.GetRandom(-0.5f, 0.5f), -0.2f, Wrapper.GetRandom(-0.5f, 0.5f) );
        }
    }

    protected void Born()
    {
        base.CustomUpdate();

        if (!isCastShadow && transform.position.y <= 1f)
        {
            SetCastShadowMode(true);
        }
        if (transform.position.y <= 0)
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            Dir.y = 0;
            curDir.y = 0;
            moveDir.y = 0;
            actionStep = ActionType.Idle;
        }
    }

    public override void Die()
    {
        SetCastShadowMode(false);
        transModel.gameObject.SetActive(false);
        actionStep = ActionType.Die;
        remainingTime = GameConst.EnemyResurrectionRemainingTime;
    }


    void SetCastShadowMode(bool isEnable)
    {
        isCastShadow = isEnable;
        SkinnedMeshRenderer[] meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer mr in meshRenderers)
        {
            mr.shadowCastingMode = isEnable? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }

}

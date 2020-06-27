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

        CreateBlisterParticle();
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
            particleBlister.Play();
            transModel.gameObject.SetActive(true);
            transform.position = GetBornPosition() + Vector3.up * GameConst.EnemyResurrectionY;
        }
    }

    protected void EnemyIdle()
    {
        // 过一段时间改变一下方向
        changeVectorRemainingTime -= Time.deltaTime;

        // 更改方向的倒计时还有
        if (changeVectorRemainingTime > 0)
        {
            // 撞墙前改变方向
            hitWallCoolTime -= Time.deltaTime;
            if (hitWallCoolTime < 0)
            {
                Vector3 pos = transform.position;
                pos.x = Mathf.Clamp(transform.position.x, -GameConst.bgBound.x + 5, GameConst.bgBound.x - 5);
                pos.z = Mathf.Clamp(transform.position.z, -GameConst.bgBound.y + 5, GameConst.bgBound.y - 5);
                if (transform.position != pos)
                {
                    Dir = -Dir;
                    hitWallCoolTime = hitWallCoolTimeMax;
                }
            }

            // 让它不走直线
            else if (Wrapper.GetRandom(0f, 1f) > 0.95f)
            {
                Dir += new Vector3(Wrapper.GetRandom(-0.1f, 0.1f), 0, Wrapper.GetRandom(-0.1f, 0.1f));
                Dir.Normalize();
            }

            return;
        }

        // 改变方向
        Vector2 moveVec = new Vector2(Wrapper.GetRandom(-1f, 1f), Wrapper.GetRandom(-1f, 1f));
        moveVec.Normalize();
        Dir = new Vector3(moveVec.x, 0, moveVec.y);

        // 设置下次更改方向的剩余时间
        changeVectorRemainingTime = Wrapper.GetRandom(1, 7f);
    }

    protected void Born()
    {
        EnemyIdle();
        Dir.y = -0.2f;

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
            changeVectorRemainingTime = 0f;
            particleBlister.Stop();
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

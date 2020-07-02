using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : FishBase
{
    protected bool isCastShadow = true;
    public override FishType fishType { get { return FishType.Enemy; } }

    public override void Init(Data data)
    {
        base.Init(data);

        // 一开始不要一起出生
        remainingTime = Wrapper.GetRandom(0f, 5f);
        //CreateBlisterParticle();
    }

    // Update is called once per frame
    protected override void MoveUpdate()
    {
        

        switch (actionStep)
        {
            case ActionType.Born:
                Born();
                break;
            case ActionType.Idle:
                Idle();
                break;
            case ActionType.Die:
                DieWait();
                break;
        }
        
    }

    protected override Vector3 GetBornPosition()
    {
        return Quaternion.AngleAxis(Wrapper.GetRandom(0f, 360f), Vector3.up) * Vector3.right * Wrapper.GetRandom(0f, ManagerGroup.GetInstance().poisonRing.GetPoisonRange() - 5f);
    }
    protected void DieWait()
    {
        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0)
        {
            actionStep = ActionType.Born;
            if(particleBlister!= null) particleBlister.Play();
            //transModel.gameObject.SetActive(true);
            remainingTime = 3f;
            SetAlpha(0f);
            transform.position = GetBornPosition();
            transform.localScale = Vector3.one * localScaleBackup;
        }
        else
        {
            float progress = remainingTime / GameConst.EatFishTime;
            transform.localScale = Vector3.one * Mathf.Lerp(0, localScaleBackup, progress);

            if (eatFishTrans != null)
            { transform.position = Vector3.Lerp(eatFishTrans.position, transform.position, progress); }
        }
    }


    protected void Idle()
    {
        // 过一段时间改变一下方向
        changeVectorRemainingTime -= Time.deltaTime;

        // 更改方向的倒计时还有
        if (changeVectorRemainingTime > 0 || hitWallCoolTime > 0)
        {
            // 撞墙前改变方向
            hitWallCoolTime -= Time.deltaTime;
            if (hitWallCoolTime < 0)
            {
                if (transform.position.sqrMagnitude >= Math.Pow(ManagerGroup.GetInstance().poisonRing.GetPoisonRange(), 2) - 5)
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

        }
        else
        {

            // 改变方向
            Vector2 moveVec = new Vector2(Wrapper.GetRandom(-1f, 1f), Wrapper.GetRandom(-1f, 1f));
            moveVec.Normalize();
            Dir = new Vector3(moveVec.x, 0, moveVec.y);

            // 设置下次更改方向的剩余时间
            changeVectorRemainingTime = Wrapper.GetRandom(1f, 7f);
        }

        base.MoveUpdate();
    }

    protected void Born()
    {
        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            actionStep = ActionType.Idle;
        }
        SetAlpha((1-remainingTime));


        Idle();
        base.MoveUpdate();
    }

    public override void Die(Transform eatFishTrans)
    {
        //SetCastShadowMode(false);
        localScaleBackup = transform.localScale.x;
        this.eatFishTrans = eatFishTrans;
        //transModel.gameObject.SetActive(false);
        actionStep = ActionType.Die;
        remainingTime = GameConst.EatFishTime;
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

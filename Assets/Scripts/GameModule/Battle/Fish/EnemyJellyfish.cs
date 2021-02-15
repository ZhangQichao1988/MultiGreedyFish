using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJellyfish : EnemyBase
{
    enum Status : int 
    {
        DarkCloudInit = 0,
        DarkCloud,
        Cloud,
    }
    static readonly float DarkBrightness = 0.5f;
    ParticleSystem goCloud;
    Vector3 startPos, endPos;

    int idleStep = 0;

    float changeStatusRemainingTime = 0f;
    Status statusChangeStep = Status.Cloud;

    public override void Init(int fishId, string playerName, float level, string rankIcon = "")
    {
        base.Init(fishId, playerName, level);

        goCloud = transModel.gameObject.GetComponentInChildren<ParticleSystem>();
        goCloud.gameObject.SetActive(false);
        //goDarkCloud = GameObjectUtil.FindGameObjectByName("cloud_01", transModel.gameObject);
        Dir = Quaternion.AngleAxis(Wrapper.GetRandom(0, 360), Vector3.up) * Vector3.right;

        // 为了不要一起动
        changeStatusRemainingTime = Wrapper.GetRandom(0f, 5f);
        statusChangeStep = Status.Cloud;

    }

    protected override void Idle()
    {
        if (!isBecameInvisible) { return; }

        StatusUpdate();
        Move();
    }
    protected void StatusUpdate()
    {
        switch (statusChangeStep)
        {
            case Status.DarkCloudInit: //  乌云状态初期化
                goCloud.gameObject.SetActive(true);
                //goDarkCloud.SetActive(false);
                SetBrightness(0.5f);
                statusChangeStep = Status.DarkCloud;
                changeStatusRemainingTime = ConfigTableProxy.Instance.GetDataById(32).floatValue;
                break;
            case Status.DarkCloud: // 乌云状态阶段
                changeStatusRemainingTime -= Time.deltaTime;
                if (changeStatusRemainingTime < DarkBrightness)
                {
                    SetBrightness(1 - changeStatusRemainingTime);
                }
                if (changeStatusRemainingTime < 0)
                {
                    // 云状态初始化
                    goCloud.gameObject.SetActive(false);
                    //goDarkCloud.SetActive(true);
                    SetBrightness(1f);
                    changeStatusRemainingTime = ConfigTableProxy.Instance.GetDataById(33).floatValue;
                    statusChangeStep = Status.Cloud;
                }
                break;
            case Status.Cloud: // 云状态阶段
                changeStatusRemainingTime -= Time.deltaTime;
                if (changeStatusRemainingTime < DarkBrightness)
                {
                    SetBrightness(0.5f + changeStatusRemainingTime);
                }
                    
                if (changeStatusRemainingTime < 0)
                {
                    statusChangeStep = Status.DarkCloudInit;
                }
                break;
        }
    }
    protected void Move()
    {

        switch (idleStep)
        {
            case 0: // 停留
                changeVectorRemainingTime -= Time.deltaTime;
                transform.position += Dir * Time.deltaTime;

                if (changeVectorRemainingTime < 0)
                {
                    animator.SetTrigger("Jump");
                    SoundManager.PlaySE(8, audioSource);
                    if (transform.position.sqrMagnitude >= BattleManagerGroup.GetInstance().poisonRing.GetSafeRudiusPow() - 5)
                    {
                        Dir = -Dir;
                    }
                    startPos = transform.position;
                    endPos = transform.position + Dir * ConfigTableProxy.Instance.GetDataById(31).floatValue;
                    idleStep = 1;
                }

                break;
            case 1: // 跳
                //transform.position += Dir * Time.deltaTime * 8f;

                AnimatorStateInfo stateinfo = animator.GetCurrentAnimatorStateInfo(0);


                if (!stateinfo.IsName("Base Layer.jump"))
                {
                    idleStep = 0;
                    changeVectorRemainingTime = ConfigTableProxy.Instance.GetDataById(30).floatValue;
                }
                else
                {
                    float rate = Mathf.Sin(stateinfo.normalizedTime * 0.785f);
                    transform.position = Vector3.Lerp(startPos, endPos, rate);
                }
                break;
        }
    }

    public override void Die(Transform eatFishTrans)
    {
        base.Die(eatFishTrans);
        statusChangeStep = 0;
        //SetBrightness(1f);
        goCloud.gameObject.SetActive(false);
    }

    public override bool EatCheck(PlayerBase player, BoxCollider atkCollider)
    {
        bool isHit = base.EatCheck(player, atkCollider);
        if (isHit)
        {
            if (goCloud.gameObject.activeSelf && 
                !player.ContainsBuffType(BuffBase.BuffType.Shield) &&
                !player.ContainsBuffType(BuffBase.BuffType.ShieldGold))
            {
                if (!player.ContainsBuff(6))
                {
                    var buff = player.AddBuff(this, 6);
                    int effectId = BattleEffectManager.CreateEffect(5, player.transModel, player.transform.localScale.x);
                }
                return false;
            }
            else
            {
                return true;
            }
        }
        return false;
    }
    public bool isDark { get { return statusChangeStep == Status.DarkCloud; } }
}

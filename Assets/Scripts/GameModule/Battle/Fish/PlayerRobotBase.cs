using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRobotBase : PlayerBase
{
    //protected float aiParamRobotFollowBigFishLifeRate = 0f;
    protected float aiParamRobotGotoAquaticLifeRate = 0f;
    protected float aiParamRobotGotoHealLifeRate = 0f;
    protected bool isGotoAquatic = false;
    protected float growth = 0f;

    protected List<FishBase> listFindedFish;

    // 鱼白名单，不会攻击的鱼
    protected FishBase whiteFish;
    // 目标鱼
    public FishBase targetFish;
    // 锁定同一目标鱼时间，用来规避穷追不舍
    protected float targetCntTime;
    protected float targetCntTimeLimit;
    protected override bool showLifeGauge { get { return true; } }
    public override FishType fishType { get { return FishType.PlayerRobot; } }

    public virtual void SetRobot(RobotAiDataInfo aiData, float growth)
    {
        this.growth = growth;
        float[] aryParam = Wrapper.GetParamFromString(aiData.aryParam);
        targetCntTimeLimit = aryParam[0];
        aiParamRobotGotoAquaticLifeRate = aryParam[1];
        aiParamRobotGotoHealLifeRate = aryParam[2];
        targetCntTime = targetCntTimeLimit;
    }

    protected virtual void Attack()
    {
        // 过一段时间改变一下方向
        changeVectorRemainingTime -= Time.deltaTime;

        // 搜索附近的鱼
        var listFish = new List<FishBase>(fishBasesInRange);
        // 判定是否有白名单的鱼
        bool hasWhiteFish = false;
        Vector3 myPos = transform.position;
        // 是否在视野外？
        bool isEyeOver = BattleConst.instance.RobotVisionRange + 5f < Vector3.SqrMagnitude(BattleManagerGroup.GetInstance().cameraFollow.targetPlayerPos - myPos);
        
        for (int i = listFish.Count - 1; i >= 0; --i)
        {
            // 如果玩家不在视野范围，就不攻击玩家和机器人
            if (isEyeOver && listFish[i].fishType == FishType.PlayerRobot)
            {
                listFish.RemoveAt(i);
                continue;
            }
            // 剔除白名单的鱼
            if (whiteFish == listFish[i])
            {
                hasWhiteFish = true;
                listFish.RemoveAt(i);
                continue;
            }
            // 排除隐身的鱼
            if (listFish[i].isStealth) 
            {
                listFish.RemoveAt(i);
                continue;
            }
            // 排除鲨鱼
            if (listFish[i].fishType == FishType.Boss)
            {
                listFish.RemoveAt(i);
                continue;
            }
            // 剔除毒圈的敌人
            if (listFish[i].transform.position.sqrMagnitude > BattleManagerGroup.GetInstance().poisonRing.GetPoisonRangePow())
            {
                listFish.RemoveAt(i);
                continue;
            }
            // 如果不是无敌状态，剔除乌云状态水母
            if (!ContainsBuffType(BuffBase.BuffType.Shield) &&
                !ContainsBuffType(BuffBase.BuffType.ShieldGold))
            {
                if (listFish[i].originalData.fishId == 4 && ((EnemyJellyfish)listFish[i]).isDark)
                {
                    listFish.RemoveAt(i);
                    continue;
                }
            }
            
            // 新发现的鱼
            if (listFindedFish != null && !listFindedFish.Contains(listFish[i]))
            {
                if ( listFish[i].beforeInAquatic)
                {
                    listFish.RemoveAt(i);
                    continue;
                }
            }
        }

        // 视野范围没有白名单的鱼的话
        if (!hasWhiteFish)
        {
            whiteFish = null;
        }

        // 按距离升序排序
        listFish.Sort((a, b) => { return (int)(Vector3.Distance(a.transform.position, myPos) - Vector3.Distance(b.transform.position, myPos)); });
        listFindedFish = listFish;
        if (listFish.Count > 0)
        {
            // 当体力较多时，追踪大鱼
            if (lifeRate > aiParamRobotGotoAquaticLifeRate ||
                ContainsBuffType(BuffBase.BuffType.Shield) ||
                ContainsBuffType(BuffBase.BuffType.ShieldGold))
            {
                listFish.Sort((a, b) => { return b.lifeMax - a.lifeMax; });
            }
            else
            {
                listFish.Sort((a, b) => { return a.lifeMax - b.lifeMax; });
            }
            FishBase target;
            if (targetFish == listFish[0])
            {
                targetCntTime -= Time.deltaTime;
                if (targetCntTime <= 0 && listFish.Count > 1)
                {
                    whiteFish = listFish[0];
                    target = listFish[1];
                }
                else 
                {
                    target = listFish[0];
                }
            }
            else
            {
                target = listFish[0];
                targetCntTime = targetCntTimeLimit;
            }
            targetFish = target;
            MoveToTarget(targetFish.transform.position);
            
        }
        else
        {
            EnemyIdle();
        }
    }
    public override void CustomUpdate()
    {
        fishSkill.CalcAI();
        base.CustomUpdate();
    }
    
    protected bool GotoAquatic()
    {
        
        if (beforeInAquatic)
        {
            data.moveSpeed = 0.01f;
        }

        MoveToTarget(closestAquatic);
        return true;
    }
    public override bool Heal(int value)
    {
        bool ret = base.Heal(value);
        if (life == lifeMax) { isGotoAquatic = false; }
        return ret;
    }
    protected virtual void CalcMoveAction()
    {
        
        if ((actionWaitCnt + uid) % 3 != 0) { return; }
        // 当血量过低时，去找水草回血
        if (lifeRate < aiParamRobotGotoHealLifeRate || isGotoAquatic)
        {
            Vector3 myPos = transform.position;
            var fishs = BattleManagerGroup.GetInstance().fishManager.GetAlivePlayerSort(myPos);
            if (fishs.Count > 1 && Vector3.SqrMagnitude(fishs[1].transform.position - myPos) > ConfigTableProxy.Instance.GetDataById(35).floatValue)
            {   // 附近没有其他玩家鱼的话躲草丛
                isGotoAquatic = GotoAquatic();
                if (isGotoAquatic) { return; }
            }
        }

        var shell = BattleManagerGroup.GetInstance().shellManager.GetPearlWithRange(transform.position, BattleConst.instance.RobotVision);
        if (shell)
        {   // 吃珍珠
            MoveToTarget(new Vector3(shell.transform.position.x, 0f, shell.transform.position.z));
        }
        else
        {   // 吃鱼
            Attack();
        }

    }


    protected void MoveToTarget(Vector3 targetPos)
    {
        Dir = targetPos - transform.position;
        Dir.Normalize();

        if (changeVectorRemainingTime <= 0)
        {
            changeVectorRemainingTime = Wrapper.GetRandom(1f, 2f);
            Dir = Quaternion.AngleAxis(Wrapper.GetRandom(-20f, 20f), Vector3.up) * Dir;
        }
    }

    protected void EnemyIdle()
    {       
        // 更改方向的倒计时还有
        if (changeVectorRemainingTime > 0)
        {
            // 撞墙前改变方向
            hitWallCoolTime -= Time.deltaTime;
            if (hitWallCoolTime < 0)
            {
                if (transform.position.sqrMagnitude >= BattleManagerGroup.GetInstance().poisonRing.GetSafeRudiusPow() - 5)
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
        changeVectorRemainingTime = Wrapper.GetRandom(1f, 3f);
    }

    protected override void MoveUpdate()
    {
        CalcMoveAction();

        base.MoveUpdate();
    }

    protected override float SetAlpha(float alpha)
    {
        alpha = base.SetAlpha(alpha);
        GameObjectUtil.SetActive(goNamepalte, alpha > 0.8);
        return alpha;
    }

    public override void Eat(float fishLevel)
    {
        fishLevel *= growth;
        base.Eat(fishLevel);
    }
}

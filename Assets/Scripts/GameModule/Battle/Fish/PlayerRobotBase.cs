using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRobotBase : PlayerBase
{
    protected float aiParamRobotGotoAquaticLifeRate = 0f;
    protected float aiParamRobotGotoHealLifeRate = 0f;
    protected bool isGotoAquatic = false;
    protected bool isIdle = false;
    protected float growth = 0f;

    protected List<FishBase> listFindedFish;

    // 鱼白名单，不会攻击的鱼
    protected FishBase whiteFish;
    // 目标鱼
    public FishBase targetFish;
    // 目标点
    protected Vector3 targetPos;
    // 锁定同一目标鱼时间，用来规避穷追不舍
    protected float targetCntTime;
    protected float targetCntTimeLimit;
    protected override bool showLifeGauge { get { return true; } }
    public override FishType fishType { get { return FishType.PlayerRobot; } }

    protected override int thinkingTime { get { return 15; } }

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
        isIdle = false;
        // 搜索附近的鱼
        var listFish = new List<FishBase>(fishBasesInRange);
        // 判定是否有白名单的鱼
        bool hasWhiteFish = false;
        Vector3 myPos = transform.position;
        FishBase target = null;

        // 是否在视野外？
        bool isEyeOver = BattleConst.instance.RobotVisionRange + 5f < Vector3.SqrMagnitude(BattleManagerGroup.GetInstance().cameraFollow.targetPlayerPos - myPos);

        List<FishBase> listTargets = new List<FishBase>();

        // 如果视野里有金龟，就优先吃金龟
        listFish.ForEach((a)=> { if (a.data.fishId == 5) 
            {
                listTargets.Add(a);
            } });

        // 没有金龟的话就找最近的鱼
        if (listTargets.Count == 0)
        {
            for (int i = listFish.Count - 1; i >= 0; --i)
            {
                if (listFish[i] == null)
                {
                    listFish.RemoveAt(i);
                    continue;
                }
                // 如果玩家不在视野范围，就不攻击玩家和机器人
                if (isEyeOver && (listFish[i].fishType == FishType.PlayerRobot || listFish[i].fishType == FishType.Boss))
                {
                    listFish.RemoveAt(i);
                    continue;
                }
                // 当鲨鱼血量低于一定量时，优先攻击
                if (listFish[i].fishType == FishType.Boss && listFish[i].life <= BattleConst.instance.SharkLifeRateFirstAtk)
                {
                    target = listFish[i];
                    break;
                }
                // 剔除白名单的鱼
                if (whiteFish == listFish[i])
                {
                    hasWhiteFish = true;
                    listFish.RemoveAt(i);
                    continue;
                }
                // 排除隐身，无敌的鱼
                if (listFish[i].isStealth || listFish[i].isShield)
                {
                    listFish.RemoveAt(i);
                    continue;
                }
                
                // 剔除毒圈的敌人
                if (listFish[i].transform.position.sqrMagnitude > BattleManagerGroup.GetInstance().poisonRing.GetPoisonRangePow() - 5)
                {
                    listFish.RemoveAt(i);
                    continue;
                }
                

                // 新发现的鱼
                if (listFindedFish != null && !listFindedFish.Contains(listFish[i]))
                {
                    if (listFish[i].beforeInAquatic)
                    {
                        listFish.RemoveAt(i);
                        continue;
                    }
                }

                // 如果不是无敌状态
                if (!isShield)
                {
                    // 排除鲨鱼
                    if (listFish[i].fishType == FishType.Boss)
                    {
                        listFish.RemoveAt(i);
                        continue;
                    }
                    // 剔除乌云状态水母
                    if (listFish[i].originalData.fishId == 4 && ((EnemyJellyfish)listFish[i]).isDark)
                    {
                        listFish.RemoveAt(i);
                        continue;
                    }
                    // 剔除比自己血多的玩家
                    if ((listFish[i].fishType == FishType.Player || listFish[i].fishType == FishType.PlayerRobot) && data.life < listFish[i].life)
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
        }
        else
        {
            listFish = listTargets;
        }

        // 当有优先攻击目标时
        if (target != null)
        {
            targetFish = target;
            targetPos = target.transform.position;
        }
        else if (listFish.Count > 0)
        {
            // 按距离升序排序
            listFish.Sort((a, b) => { return (int)(Vector3.Distance(a.transform.position, myPos) - Vector3.Distance(b.transform.position, myPos)); });
            listFindedFish = listFish;

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
            targetPos = targetFish.transform.position;

        }
        else
        {
            isIdle = true;
            EnemyIdle();
        }
    }
    public override void Atk(FishBase fish)
    {
        base.Atk(fish);
        if (fish == targetFish)
        {   //如果咬到了就继续追
            targetCntTime = targetCntTimeLimit;
        }
    }
    public override void CustomUpdate()
    {
        fishSkill.CalcAI();
        base.CustomUpdate();
    }
    
    protected void GotoAquatic()
    {
        targetPos = closestAquatic;
    }
    public override bool Heal(int value)
    {
        bool ret = base.Heal(value);
        if (life == lifeMax) { isGotoAquatic = false; }
        return ret;
    }

    protected virtual void CalcMoveAction()
    {
        
        if ((actionWaitCnt + uid) % thinkingTime != 0) { return; }
        // 当血量过低时，去找水草回血
        isGotoAquatic = false;
        if (lifeRate < aiParamRobotGotoHealLifeRate || isGotoAquatic)
        {
            Vector3 myPos = transform.position;
            float eyeRange = ConfigTableProxy.Instance.GetDataById(35).floatValue;
            if (BattleManagerGroup.GetInstance().fishManager.boss != null && Vector3.SqrMagnitude(BattleManagerGroup.GetInstance().fishManager.boss.transform.position - myPos) > eyeRange)
            {
                var fishs = BattleManagerGroup.GetInstance().fishManager.GetAlivePlayerSort(myPos);
                if (fishs.Count > 1 && Vector3.SqrMagnitude(fishs[1].transform.position - myPos) > eyeRange)
                {   // 附近没有其他玩家鱼的话躲草丛
                    GotoAquatic();
                    isGotoAquatic = true;
                    return;
                }
            }
            
        }

        var shell = BattleManagerGroup.GetInstance().shellManager.GetPearlWithRange(transform.position, BattleConst.instance.RobotVision);
        if (shell)
        {   // 吃珍珠
            targetPos = new Vector3(shell.transform.position.x, 0f, shell.transform.position.z);
        }
        else
        {   // 吃鱼
            Attack();
        }

    }


    protected void MoveToTarget(Vector3 targetPos)
    {
        Vector3 tmpDir = targetPos - transform.position;
        tmpDir.Normalize();
        Dir = Vector3.Lerp(Dir, tmpDir, 0.5f);

        //if (changeVectorRemainingTime <= 0)
        //{
        //    changeVectorRemainingTime = Wrapper.GetRandom(1f, 2f);
        //    Dir = Quaternion.AngleAxis(Wrapper.GetRandom(-20f, 20f), Vector3.up) * Dir;
        //}
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
            return;
        }

        // 改变方向
        Vector2 moveVec = new Vector2(Wrapper.GetRandom(-1f, 1f), Wrapper.GetRandom(-1f, 1f));
        moveVec.Normalize();
        Dir = new Vector3(moveVec.x, 0, moveVec.y);

        // 设置下次更改方向的剩余时间
        changeVectorRemainingTime = Wrapper.GetRandom(1f, 3f);
    }
    protected override void MoveInit()
    {
        if (beforeInAquatic && isGotoAquatic)
        {
            data.moveSpeed = 0f;
        }
        base.MoveInit();
    }

    protected override void MoveUpdate()
    {
        CalcMoveAction();
        if (!isIdle || isGotoAquatic)
        {
            MoveToTarget(targetPos);
        }

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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRobotBase : PlayerBase
{
    protected float aiParamRobotFollowBigFishLifeRate = 0f;

    protected List<FishBase> listFindedFish;

    protected override bool showLifeGauge { get { return true; } }
    public override FishType fishType { get { return FishType.PlayerRobot; } }
    

    public virtual void SetAI(RobotAiDataInfo aiData)
    {
        float[] aryParam = Wrapper.GetParamFromString(aiData.aryParam);
        aiParamRobotFollowBigFishLifeRate = aryParam[0];
    }

    protected virtual void Attack()
    {
        // 过一段时间改变一下方向
        changeVectorRemainingTime -= Time.deltaTime;

        // 追踪附近比自己小的离最近的鱼
        List<FishBase> listFish = BattleManagerGroup.GetInstance().fishManager.GetEnemiesInRange(this, transform.position, BattleConst.instance.RobotVision);

        // 把新发现的，隐身的鱼排除
        for (int i = listFish.Count - 1; i >= 0; --i)
        {
            // 新发现的鱼
            if (listFindedFish != null && !listFindedFish.Contains(listFish[i]))
            {
                if (listFish[i].beforeInAquatic)
                {
                    listFish.RemoveAt(i);
                    continue;
                }
            }
            // 肿胀的河豚鱼剔除
            //if (listFish[i].ContainsBuff(4))
            //{
            //    listFish.RemoveAt(i);
            //    continue;
            //}
        }
        // 按距离升序排序
        listFish.Sort((a, b) => { return (int)(Vector3.Distance(a.transform.position, transform.position) - Vector3.Distance(b.transform.position, transform.position)); });
        listFindedFish = listFish;
        if (listFish.Count > 0)
        {
            // 当体力较多时，追踪大鱼
            if (lifeRate > aiParamRobotFollowBigFishLifeRate)
            {
                listFish.Sort((a, b) => { return b.lifeMax - a.lifeMax; });
            }
            else
            {
                listFish.Sort((a, b) => { return a.lifeMax - b.lifeMax; });
            }

            FishBase target = listFish[0];
            MoveToTarget(target.transform.position);

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
    protected void GotoAquatic()
    {
		foreach (FishBase fb in listFindedFish)
		{
			if (fb.fishType == FishType.Player || fb.fishType == FishType.PlayerRobot)
			{
                return;
			}

		}
        List<Transform> listTransAquatic = BattleManagerGroup.GetInstance().aquaticManager.listTransAquatic;
        for (int i = listTransAquatic.Count - 1; i >= 0 ; --i)
        {
            if (listTransAquatic[i].position.sqrMagnitude > Mathf.Pow( GetSafeRudius(), 2))
            {
                listTransAquatic.RemoveAt(i);
            }
        }
        if (listTransAquatic.Count <= 0) { return; }
        listTransAquatic.Sort((a, b) => { return (int)(Vector3.Distance(transform.position, a.position) - Vector3.Distance(transform.position, b.position)); });
        MoveToTarget(new Vector3( listTransAquatic[0].transform.position.x, 0f, listTransAquatic[0].transform.position.z));
    }

    protected virtual void CalcMoveAction()
    {
        // 躲草丛
        //if (lifeRate > aiParamRobotGotoAquaticLifeRate && !isGotoAquatic)
        //{   // 吃鱼模式
        //    Attack();
        //}
        //else if (lifeRate >= 1f)
        //{
        //    isGotoAquatic = false;
        //}
        //else
        //{
        //    //  恢复模式
        //    GotoAquatic();
        //}

        var shell = BattleManagerGroup.GetInstance().shellManager.GetPearlWithRange(transform.position, BattleConst.instance.RobotVision);
        if (shell)
        {   // 吃珍珠
            MoveToTarget(new Vector3(shell.transform.position.x, 0f, shell.transform.position.z));
        }
        else
        {
            // 吃鱼
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
                if (transform.position.sqrMagnitude >= Math.Pow(GetSafeRudius(), 2) - 5)
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
        goNamepalte.SetActive(alpha > 0.8);
        return alpha;
    }

	
}

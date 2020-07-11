using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRobotBase : PlayerBase
{
    protected List<FishBase> listFindedFish;

    protected override bool showLifeGauge { get { return false; } }

    public override FishType fishType { get { return FishType.PlayerRobot; } }

	
    protected virtual void CalcMoveAction()
    {
        // 过一段时间改变一下方向
        changeVectorRemainingTime -= Time.deltaTime;

        // 追踪附近比自己小的离最近的鱼
        List<FishBase> listFish = ManagerGroup.GetInstance().fishManager.GetEnemiesInRange( this, transform.position, GameConst.RobotFindFishRange );

        // 把新发现的，隐身的鱼排除
        for (int i = listFish.Count - 1; i >= 0 ; --i)
        {
            // 新发现的鱼
            if (listFindedFish!= null && !listFindedFish.Contains(listFish[i]))
            {
                if (listFish[i].isStealth)
                {
                    listFish.RemoveAt(i);
                }
            }
        }
        listFindedFish = listFish;
        if (listFish.Count > 0)
        {
            // 按距离升序排序
            listFish.Sort((a, b) => { return (int)(Vector3.Distance(a.transform.position, transform.position) - Vector3.Distance(b.transform.position, transform.position)); });

            // 当体力较多时，追踪大鱼
            if (lifeRate > GameConst.RobotFollowBigFishLifeRate)
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
}

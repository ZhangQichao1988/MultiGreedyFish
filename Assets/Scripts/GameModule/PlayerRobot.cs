﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRobot : PlayerBase
{

    float changeVectorRemainingTime = 0f;

    static readonly float hitWallCoolTimeMax = 1f;
    float hitWallCoolTime = 0f;

    public override FishType fishType { get { return FishType.PlayerRobot; } }

	public override void Init(Data data)
	{
		base.Init(data);
    }
	void Idle()
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
                pos.z = Mathf.Clamp(transform.position.y, -GameConst.bgBound.y + 5, GameConst.bgBound.y - 5);
                if (transform.position != pos)
                {
                    Dir = -Dir;
                    hitWallCoolTime = hitWallCoolTimeMax;
                }
            }

            // 让它不走直线
            else if (Wrapper.GetRandom(0, 1) > 0.95f)
            {
                Dir += new Vector3(Wrapper.GetRandom(-0.1f, 0.1f), 0, Wrapper.GetRandom(-0.1f, 0.1f));
                Dir.Normalize();
            }

            return;
        }

        // 改变方向
        Vector2 moveVec = new Vector2(Wrapper.GetRandom(-1, 1), Wrapper.GetRandom(-1, 1));
        moveVec.Normalize();
        Dir = new Vector3(moveVec.x, 0, moveVec.y);

        // 设置下次更改方向的剩余时间
        changeVectorRemainingTime = Wrapper.GetRandom(1, 3);
    }
    void CalcMoveAction()
    {
        // 追踪附近比自己小的离最近的鱼
        List<FishBase> listFish = ManagerGroup.GetInstance().fishManager.GetEnemiesInRange( this, transform.position, GameConst.RobotFindFishRange );
        if (listFish.Count > 0)
        {
            listFish.Sort((a, b) => { return (int)(Vector3.Distance(a.transform.position, transform.position) - Vector3.Distance(b.transform.position, transform.position)); });
            FishBase target = listFish[0];
            Dir = target.transform.position - transform.position;
            Dir.Normalize();
        }
        else
        {
            Idle();
        }
        

        


    }
    public override void CustomUpdate()
    {
        CalcMoveAction();

        base.CustomUpdate();
    }
}

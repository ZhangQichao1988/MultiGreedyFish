using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRobot : PlayerBase
{

    float changeVectorRemainingTime = 0f;

    static readonly float hitWallCoolTimeMax = 1f;
    float hitWallCoolTime = 0f;

    public override FishType fishType { get { return FishType.PlayerRobot; } }

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
                pos.x = Mathf.Clamp(transform.position.x, -GameConst.bound.x + 5, GameConst.bound.x - 5);
                pos.y = Mathf.Clamp(transform.position.y, -GameConst.bound.y + 5, GameConst.bound.y - 5);
                if (transform.position != pos)
                {
                    Dir = -Dir;
                    hitWallCoolTime = hitWallCoolTimeMax;
                }
            }

            // 让它不走直线
            else if (Wrapper.GetRandom(0, 1) > 0.95f)
            {
                Dir += new Vector3(Wrapper.GetRandom(-0.1f, 0.1f), Wrapper.GetRandom(-0.1f, 0.1f), 0);
                Dir.Normalize();
            }

            return;
        }

        // 改变方向
        Vector2 moveVec = new Vector2(Wrapper.GetRandom(-1, 1), Wrapper.GetRandom(-1, 1));
        moveVec.Normalize();
        Dir = new Vector3(moveVec.x, moveVec.y, 0);

        // 设置下次更改方向的剩余时间
        changeVectorRemainingTime = Wrapper.GetRandom(1, 3);
    }
    void CalcMoveAction()
    {
        // 侦察附近是否有杂鱼
        //if(  )
        Idle();


    }
    public override void CustomUpdate()
    {
        CalcMoveAction();

        base.CustomUpdate();
    }
}

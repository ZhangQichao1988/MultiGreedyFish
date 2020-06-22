using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : FishBase
{
    Vector3 moveDir;
    EnemyManager enemyManager = null;
    int uid = 0;


	public void Init(EnemyManager enemyManager, int uid)
    {
        this.enemyManager = enemyManager;
        this.uid = uid;
        transform.position = new Vector3( Wrapper.GetRandom(-GameConst.bound.x, GameConst.bound.x), Wrapper.GetRandom(-GameConst.bound.y, GameConst.bound.y));
    }

    // Update is called once per frame
    public override void CustomUpdate()
    {
        float moveVec = (float)Math.Sin((Time.time + uid) * enemyManager.FlipFrequency) * enemyManager.MoveSpeed;
        Dir = new Vector3(moveVec, 0, 0);
       // transform.rotation = Quaternion.LookRotation(moveDir);
        //float moveSpeed = Math.Abs(moveVec) * enemyManager.MoveSpeed;
        //SetMoveDir(moveDir);
        //FishBase.Transformation(transform, animator,ref moveDir, curDir, Dir);
        //animator.SetFloat("Speed", moveSpeed);
        base.CustomUpdate();
    }
}

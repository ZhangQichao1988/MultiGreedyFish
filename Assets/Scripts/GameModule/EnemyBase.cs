using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : FishBase
{
	public override void Init(Data data)
    {
        base.Init(data);
        transform.position = new Vector3( Wrapper.GetRandom(-GameConst.bound.x, GameConst.bound.x), Wrapper.GetRandom(-GameConst.bound.y, GameConst.bound.y));
    }

    // Update is called once per frame
    public override void CustomUpdate()
    {
        float moveVec = (float)Math.Sin((Time.time + data.uid) * EnemyManager.FlipFrequency) * EnemyManager.MoveSpeed;
        Dir = new Vector3(moveVec, 0, 0);

        base.CustomUpdate();
    }

    public bool EatCheck(Vector3 mouthPos, float range)
    {
        return Vector3.Distance(transform.position, mouthPos) < range;
    }

    public void Die()
    {
        Destroy(this.gameObject);
    }
}

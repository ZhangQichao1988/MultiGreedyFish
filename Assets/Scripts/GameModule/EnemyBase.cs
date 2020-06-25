using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : FishBase
{
    public override FishType fishType { get { return FishType.Enemy; } }

    public override void Init(Data data)
    {
        base.Init(data);
    }

    // Update is called once per frame
    public override void CustomUpdate()
    {
        float moveVec = (float)Math.Sin((Time.time + data.uid) * FishManager.FlipFrequency) * FishManager.MoveSpeed;
        Dir = new Vector3(moveVec, 0, 0);

        base.CustomUpdate();
    }

    
}

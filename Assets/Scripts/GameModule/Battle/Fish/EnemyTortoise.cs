using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTortoise : EnemyBase
{
    public override bool EatCheck(PlayerBase player, BoxCollider atkCollider)
    {
        bool isHit = base.EatCheck(player, atkCollider);
        if (isHit)
        {
            player.AddBuff(this, 7);
        }
        return isHit;
    }
}

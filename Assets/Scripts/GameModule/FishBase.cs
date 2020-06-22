using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBase : MonoBehaviour
{

    protected Animator animator = null;

    protected Vector3 Dir;
    protected Vector3 curDir;
    protected Vector3 moveDir;
    //Vector3 pos;
    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetMoveDir(Vector3 moveDir)
    {
        Dir = moveDir;
    }
    public virtual void CustomUpdate()
    {
        Vector3 pos = transform.position;
        if (Dir.sqrMagnitude > 0)
        {
            float angle = Vector3.Angle(curDir.normalized, Dir.normalized);
            curDir = Vector3.Slerp(curDir.normalized, Dir.normalized, 520 / angle * Time.deltaTime);
            moveDir = Vector3.Lerp(moveDir, Dir, 360 / angle * Time.deltaTime);
            pos += moveDir * 10 * Time.deltaTime;
            pos = ObstacleGrid.ObstacleClamp(pos, moveDir);
            animator.SetFloat("Speed", Mathf.Clamp(moveDir.magnitude, 0.101f, 1f));
        }
        else
        {
            curDir = Vector3.Lerp(curDir, Dir, 5 * Time.deltaTime);
            if (curDir.sqrMagnitude > 0.001f)
            {
                pos += curDir * 10 * Time.deltaTime;
                pos = ObstacleGrid.ObstacleClamp(pos, curDir);
            }
            animator.SetFloat("Speed", curDir.magnitude);
        }
        if (curDir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(curDir);

        //Debug.Log("curDir:" + curDir);
        // 界限限制
        if (pos.x < 0) { pos.x = Math.Max(pos.x, -GameConst.bound.x); }
        else if (pos.x > 0) { pos.x = Math.Min(pos.x, GameConst.bound.x); }
        if (pos.y < 0) { pos.y = Math.Max(pos.y, -GameConst.bound.y); }
        else if (pos.y > 0) { pos.y = Math.Min(pos.y, GameConst.bound.y); }

        transform.position = pos;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBase : MonoBehaviour
{
    static readonly string fishPrefabRootPath = "ArtResources/Models/Prefabs/fish/";
    Dictionary<int, string> fishData = new Dictionary<int, string>()
    {
        { 0, "clown_fish" },
        { 1, "yellow_fish" },
    };

    public enum FishType
    { 
        None,
        Player,
        PlayerRobot,
        Enemy,
    }
    public struct Data
    {
        public int uid;
        public int fishId;
        public int life;
        public float size;

        public Data(int fishId, int life, float size)
        {
            uid = -1;
            this.fishId = fishId;
            this.life = life;
            this.size = size;
        }
    }
    static protected int uidCnt = 0;

    protected Data data;
    protected Animator animator = null;
    protected Transform transModel = null;
   // protected MeshFilter meshFilter = null;

    protected Vector3 Dir;
    protected Vector3 curDir;
    protected Vector3 moveDir;

    public virtual FishType fishType { get { return FishType.None; } }


    //Vector3 pos;
    protected virtual void Awake()
    {
    }


    public virtual void Init(Data data)
    {
        data.uid = uidCnt++;
        this.data = data;
        UnityEngine.Object obj = Resources.Load(fishPrefabRootPath + fishData[data.fishId]);
        GameObject go = Wrapper.CreateGameObject(obj, transform) as GameObject;
        transModel = go.transform;
        transform.localScale = Vector3.one * data.size;
        transform.position = new Vector3(Wrapper.GetRandom(-GameConst.bgBound.x, GameConst.bgBound.x),
                                                                0,
                                                                Wrapper.GetRandom(-GameConst.bgBound.y, GameConst.bgBound.y));

        //meshFilter = go.GetComponent<MeshFilter>();
        animator = transModel.GetComponent<Animator>();

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
            transModel.rotation = Quaternion.LookRotation(curDir);

        //Debug.Log("curDir:" + curDir);
        // 界限限制
        pos.x = Mathf.Clamp(pos.x, -GameConst.bgBound.x, GameConst.bgBound.x);
        pos.z = Mathf.Clamp(pos.z, -GameConst.bgBound.y, GameConst.bgBound.y);

        transform.position = pos;
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

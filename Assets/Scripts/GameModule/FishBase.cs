using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public enum ActionType
    { 
        None,
        Born,
        Idle,
        Runaway,
        Die,
    }

    public struct Data
    {
        public int uid;
        public int fishId;
        public int life;
        public int lifeMax;
        public float size;
        public int atk;

        public Data(int fishId, int life, float size, int atk)
        {
            uid = -1;
            this.fishId = fishId;
            this.life = life;
            this.lifeMax = life;
            this.size = size;
            this.atk = atk;
        }
    }
    static protected int uidCnt = 0;

    public Data data;
    protected Animator animator = null;
    protected Transform transModel = null;
    public ActionType actionStep;

    // idle相关
    float changeVectorRemainingTime = 0f;
    static readonly float hitWallCoolTimeMax = 1f;
    float hitWallCoolTime = 0f;

    // 伤害相关
    public float dmgCoolTime = 0;

    protected Vector3 Dir;
    protected Vector3 curDir;
    protected Vector3 moveDir;

    static readonly string lifeGaugePath = "Prefabs/PlayerLifeGauge";
    protected Slider lifeGauge = null;

    protected virtual bool showLifeGauge { get { return false; } }
    public virtual FishType fishType { get { return FishType.None; } }

    public int life
    {
        get { return data.life; }
        set
        {
            data.life = value;
            if (lifeGauge != null) { lifeGauge.value = value; }
        }
    }

    //Vector3 pos;
    protected virtual void Awake()
    {
    }


    public virtual void Init(Data data)
    {
        actionStep = ActionType.Idle;
        data.uid = uidCnt++;
        this.data = data;
        transform.name = fishType.ToString() + this.data.uid;
        UnityEngine.Object obj = Resources.Load(fishPrefabRootPath + fishData[data.fishId]);
        GameObject go = Wrapper.CreateGameObject(obj, transform) as GameObject;
        transModel = go.transform;
        transform.localScale = Vector3.one * data.size;
        transform.position = GetBornPosition();

        //meshFilter = go.GetComponent<MeshFilter>();
        animator = transModel.GetComponent<Animator>();

        if (showLifeGauge)
        {
            CreateLifeGuage();
        }
    }

    protected Vector3 GetBornPosition()
    {
        return new Vector3(Wrapper.GetRandom(-GameConst.bgBound.x, GameConst.bgBound.x), 0, Wrapper.GetRandom(-GameConst.bgBound.y, GameConst.bgBound.y));
    }

    public void SetMoveDir(Vector3 moveDir)
    {
        Dir = moveDir;
    }

    protected void MoveUpdate()
    {
        Vector3 pos = transform.position;

        // 速度计算
        float spd = Mathf.Sqrt(data.size);
        Vector3 dir = Dir * spd;

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
    public virtual void CustomUpdate()
    {
        if (dmgCoolTime > 0) { dmgCoolTime -= Time.deltaTime; }

        MoveUpdate();
    }

    public bool EatCheck(Vector3 mouthPos, float range)
    {
        return dmgCoolTime <= 0 &&
        actionStep != ActionType.Born &&
        actionStep != ActionType.Die &&
        actionStep != ActionType.None &&
        Vector3.Distance(transform.position, mouthPos) < range;
    }

    public virtual void Die()
    {
        ManagerGroup.GetInstance().fishManager.listFish.Remove( this );
        Destroy( gameObject );
    }

    protected virtual void EnemyIdle()
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

    protected void CreateLifeGuage()
    { 
        // 生命条
		UnityEngine.Object obj = Resources.Load(lifeGaugePath);
        GameObject go = Wrapper.CreateGameObject(obj, transform) as GameObject;
        lifeGauge = go.GetComponentInChildren<Slider>();
        Debug.Assert(lifeGauge, "lifeGauge is not found.");
        lifeGauge.maxValue = data.lifeMax;
        lifeGauge.value = data.life;
    }
}

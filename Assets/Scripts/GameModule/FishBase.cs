﻿using System;
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
        Eatting,
        Die,
    }

    public struct Data
    {
        public int uid;
        public int fishId;
        public int life;
        public int lifeMax;
        public int atk;
        public float moveSpeed;

        public Data(int fishId, int life, int atk, float moveSpeed)
        {
            uid = -1;
            this.fishId = fishId;
            this.life = life;
            this.lifeMax = life;
            this.atk = atk;
            this.moveSpeed = moveSpeed;
        }
    }
    static protected int uidCnt = 0;

    public Data data;
    public Data originalData;
    protected Animator animator = null;
    protected Transform transModel = null;
    protected BoxCollider colliderBody;
    public ActionType actionStep;

    // idle相关
    protected float changeVectorRemainingTime = 0f;
    protected static readonly float hitWallCoolTimeMax = 1f;
    protected float hitWallCoolTime = 0f;

    protected Vector3 Dir;
    protected Vector3 curDir;
    protected Vector3 moveDir;

    static readonly string lifeGaugePath = "Prefabs/PlayerLifeGauge";
    static readonly string blisterParticlePath = "ArtResources/Particles/Prefabs/blister";
    
    public Slider lifeGauge = null;
    public Text lifeText = null;

    // 水泡粒子
    protected ParticleSystem particleBlister;
    protected virtual bool showLifeGauge { get { return false; } }
    public virtual FishType fishType { get { return FishType.None; } }

    public int life
    {
        get { return data.life; }
        set
        {
            if (value >= lifeMax)
            {
                lifeMax = value;
            }
            data.life = value;
            if (lifeGauge != null) { lifeGauge.value = data.life; }
            UpdateLifeText();
        }
    }

    public int lifeMax
    {
        get { return data.lifeMax; }
        set
        {
            data.lifeMax = value;
            if (lifeGauge != null) { lifeGauge.maxValue = data.lifeMax; }
            UpdateLifeText();
            ApplySize();
        }
    }

    public float lifeRate 
    {
        get 
        {
            return (float)life / (float)lifeMax;
        }
    }
    void UpdateLifeText()
    {
        if (lifeText != null)
        {
            lifeText.text = string.Format("{0}/{1}", life, lifeMax);
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
        this.originalData = data;
        transform.name = fishType.ToString() + this.data.uid;
        UnityEngine.Object obj = Resources.Load(fishPrefabRootPath + fishData[data.fishId]);
        GameObject go = Wrapper.CreateGameObject(obj, transform) as GameObject;
        transModel = go.transform;
        transform.position = GetBornPosition();

        colliderBody = transModel.gameObject.GetComponent<BoxCollider>();
        

        animator = transModel.GetComponent<Animator>();

        if (showLifeGauge)
        {
            CreateLifeGuage();
        }
        life = data.life;
        lifeMax = data.lifeMax;
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
        float spd = Mathf.Sqrt( transform.localScale.x );
        Vector3 dir = Dir / spd * data.moveSpeed;

        if (dir.sqrMagnitude > 0)
        {
            float angle = Vector3.Angle(curDir.normalized, dir.normalized);
            curDir = Vector3.Slerp(curDir.normalized, dir.normalized, 520 / angle * Time.deltaTime);
            moveDir = Vector3.Lerp(moveDir, dir, 360 / angle * Time.deltaTime);
            pos += moveDir * 10 * Time.deltaTime;
            pos = ObstacleGrid.ObstacleClamp(pos, moveDir);
            animator.SetFloat("Speed", Mathf.Clamp(moveDir.magnitude, 0.101f, 1f));
        }
        else
        {
            curDir = Vector3.Lerp(curDir, dir, 5 * Time.deltaTime);
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
        MoveUpdate();
    }

    public bool EatCheck(BoxCollider atkCollider)
    {
        if (actionStep == ActionType.Born ||
            actionStep == ActionType.Die ||
            actionStep == ActionType.None)
        {
            return false;
        }

        //float dis = BoundsBody.SqrDistance(mouthPos);
        //range = (float)Math.Pow(range, 2);
        
        return colliderBody.bounds.Intersects(atkCollider.bounds);
    }

    public virtual void Die()
    {
        ManagerGroup.GetInstance().fishManager.listFish.Remove( this );
        Destroy( gameObject );
    }

    protected void CreateLifeGuage()
    { 
        // 生命条
		UnityEngine.Object obj = Resources.Load(lifeGaugePath);
        GameObject go = Wrapper.CreateGameObject(obj, transform) as GameObject;
        lifeGauge = go.GetComponentInChildren<Slider>();
        Debug.Assert(lifeGauge, "lifeGauge is not found.");
        //lifeGauge.maxValue = data.lifeMax;
        //lifeGauge.value = data.life;

        lifeText = go.GetComponentInChildren<Text>();
        Debug.Assert(lifeText, "lifeText is not found.");
    }

    // 水泡
    protected void CreateBlisterParticle()
    {
        
        UnityEngine.Object obj = Resources.Load(blisterParticlePath);
        GameObject go = Wrapper.CreateGameObject(obj, transform) as GameObject;
        particleBlister = go.GetComponent<ParticleSystem>();
        particleBlister.Stop();
    }

    private void ApplySize()
    {
        float size = ((float)lifeMax - (float)originalData.lifeMax) / (float)originalData.lifeMax;
        size = 1 + (float)Math.Sqrt(size) * GameConst.PlayerSizeUpRate;
        size = Math.Min(GameConst.FishMaxScale, size);
        transform.localScale = new Vector3(size, size, size);
    }
}

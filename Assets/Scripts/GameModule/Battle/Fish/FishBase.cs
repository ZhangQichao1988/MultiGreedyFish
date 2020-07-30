using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishBase : MonoBehaviour
{


    public enum FishType
    { 
        None,
        Player,
        PlayerRobot,
        Boss,
        Enemy,
    }

    public enum ActionType
    { 
        None,
        Born,
        BornWaitting,
        Idle,
        Runaway,
        Eatting,
        Die,
    }

    public struct Data
    {
        public int uid;
        public int fishId;
        public string name;
        public int life;
        public int lifeMax;
        public int atk;
        public float moveSpeed;

        public Data(int fishId, string name, int life, int atk, float moveSpeed)
        {
            uid = -1;
            this.fishId = fishId;
            this.name = name;
            this.life = life;
            this.lifeMax = life;
            this.atk = atk;
            this.moveSpeed = moveSpeed;
        }
    }
    static protected int uidCnt = 0;

    protected FishDataInfo fishBaseData;
    public Data data;
    public Data originalData;
    protected Animator animator = null;
    public Transform transModel = null;
    protected Renderer[] renderers = null;
    protected BoxCollider colliderBody;
    public ActionType actionStep;

    protected List<BuffBase> listBuff = new List<BuffBase>();

    // idle相关
    protected float changeVectorRemainingTime = 0f;
    protected static readonly float hitWallCoolTimeMax = 1f;
    protected float hitWallCoolTime = 0f;

    protected float canStealthRemainingTime = 0f;
    protected bool beforeInPoisonRing = false;
    protected float inPoisonRingTime = 0f;
    protected int inPoisonRingDmgCnt = 0;

    public bool beforeInAquatic = false;
    protected float inAquaticTime = 0f;
    protected int inAquaticHealCnt = 0;


    protected Vector3 Dir;
    protected Vector3 curDir;
    protected Vector3 moveDir;

    protected float remainingTime = 0f;

    // 被吃掉鱼的Trans
    protected Transform eatFishTrans = null;
    // 用来记录被吃掉时候的缩放值
    protected float localScaleBackup = 0f;

     


    static readonly string blisterParticlePath = "ArtResources/Particles/Prefabs/blister";
    
    public LifeGauge lifeGauge = null;
    public Text lifeText = null;

    // 水泡粒子
    protected ParticleSystem particleBlister;
    protected virtual bool showLifeGauge { get { return false; } }
    public virtual FishType fishType { get { return FishType.None; } }

    public virtual int life
    {
        get { return data.life; }
        set
        {
            if (value > lifeMax)
            {
                lifeMax = value;
            }
            if (value < data.life)
            {
                Damge();
            }
            else if (value > data.life)
            {
                Heal();
            }
            data.life = value;
            if (lifeGauge != null) { lifeGauge.SetValue(data.life, data.lifeMax); }
        }
    }

    public int lifeMax
    {
        get { return data.lifeMax; }
        set
        {
            data.lifeMax = value;
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

    // 是否隐身
    public bool isStealth { get; set; }

    //Vector3 pos;
    protected virtual void Awake()
    {
    }


    public virtual void Init(int fishId, string playerName)
    {
        foreach (BuffBase bb in listBuff)
        { bb.Destory(); }
        listBuff.Clear();

        actionStep = ActionType.Born;
        fishBaseData = FishData.GetFishBaseData(fishId);
        this.data = new Data(fishId, playerName, fishBaseData.life, fishBaseData.atk, fishBaseData.moveSpeed);
        data.uid = uidCnt++;
        this.originalData = data;
        transform.name = fishType.ToString() + this.data.uid;
        GameObject go = ResourceManager.LoadSync(AssetPathConst.fishPrefabRootPath + fishBaseData.prefabPath, typeof(GameObject)).Asset as GameObject;
        go = GameObjectUtil.InstantiatePrefab(go, gameObject, false);
        transModel = go.transform;
        renderers = transModel.GetComponentsInChildren<Renderer>();
        transform.position = GetBornPosition();

        // 身体判定范围取得
        go = GameObjectUtil.FindGameObjectByName("body", gameObject);
        Debug.Assert(go, "body is not found.");
        colliderBody = go.GetComponent<BoxCollider>();
        Debug.Assert(colliderBody, "colliderBody is not found.");        

        animator = transModel.GetComponent<Animator>();

        if (showLifeGauge)
        {
            CreateLifeGuage();
            lifeGauge.slider.maxValue = data.lifeMax;
            lifeGauge.slider.value = data.life;
        }
        
        lifeMax = data.lifeMax;
        life = data.life;
    }

    protected virtual Vector3 GetBornPosition()
    {
        Debug.LogError("GetBornPosition()");
       return Vector3.zero;
    }

    public void SetMoveDir(Vector3 moveDir)
    {
        Dir = moveDir;
    }

    protected virtual void MoveUpdate()
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
        pos.x = Mathf.Clamp(pos.x, -BattleConst.BgBound.x, BattleConst.BgBound.x);
        pos.z = Mathf.Clamp(pos.z, -BattleConst.BgBound.y, BattleConst.BgBound.y);

        transform.position = pos;
    }
    public virtual void CustomUpdate()
    {
        BuffUpdate();
        AquaticCheck();
        PoisonRingCheck();
        MoveUpdate();
    }
    void BuffUpdate()
    { 
        data.moveSpeed = originalData.moveSpeed;

        for(int i = listBuff.Count - 1; i >= 0; --i)
        {
            listBuff[i].Update();
            if (listBuff[i].IsDestory())
            {
                listBuff[i].Destory();
                listBuff.RemoveAt(i);
            }
        }
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

    public virtual void Die(Transform eatFishTrans)
    {
        
    }

    protected void CreateLifeGuage()
    {
        // 生命条
        //UnityEngine.Object obj = Resources.Load(AssetPathConst.lifeGaugePath);
        GameObject go = ResourceManager.LoadSync(AssetPathConst.lifeGaugePath, typeof(GameObject)).Asset as GameObject;
        go = GameObjectUtil.InstantiatePrefab(go,  gameObject, false);
        //GameObject go = Wrapper.CreateGameObject(obj, transform) as GameObject;
        lifeGauge = go.GetComponentInChildren<LifeGauge>();
        Debug.Assert(lifeGauge, "lifeGauge is not found.");

    }

    protected virtual void ApplySize()
    {
        float size = ((float)lifeMax - (float)originalData.lifeMax) / (float)originalData.lifeMax;
        size = 1 + (float)Math.Sqrt(size) * BattleConst.PlayerSizeUpRate;
        size = Math.Min(BattleConst.FishMaxScale, size);
        transform.localScale = new Vector3(size, size, size);
    }
    protected float GetSafeRudius()
    {
        return Mathf.Min(BattleManagerGroup.GetInstance().poisonRing.GetPoisonRange(), BattleConst.BgBound.x);
    }

    protected virtual float SetAlpha(float alpha)
    {
        transModel.gameObject.SetActive(alpha > 0);
        if (lifeGauge) { lifeGauge.gameObject.SetActive(alpha > 0); }
        alpha = Mathf.Clamp(alpha, 0f, 1f);
        //SetCastShadowMode(alpha > 0.8f);
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        foreach (Renderer renderer in renderers)
        {
            renderer.GetPropertyBlock( mpb );
            mpb.SetFloat("_Alpha", alpha);
            renderer.SetPropertyBlock(mpb);
        }
        return alpha; 
    }

    void SetCastShadowMode(bool isEnable)
    {
        Renderer[] meshRenderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer mr in meshRenderers)
        {
            mr.shadowCastingMode = isEnable ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }

    protected void AquaticCheck()
    {
        canStealthRemainingTime = Math.Max(0f, canStealthRemainingTime - Time.deltaTime);
        // 在水草里恢复血量
        if (BattleManagerGroup.GetInstance().aquaticManager.IsInAquatic(this) && canStealthRemainingTime <= 0f)
        {
            if (!beforeInAquatic)
            {
                inAquaticHealCnt = 0;
            }
            beforeInAquatic = true;
            inAquaticTime += Time.deltaTime;
        }
        else
        {

            inAquaticTime = 0;
            beforeInAquatic = false;
        }

        if (beforeInAquatic && inAquaticTime >= inAquaticHealCnt * BattleConst.AquaticHealCoolTime)
        {
            int healLife = BattleConst.AquaticHeal * inAquaticHealCnt++;
            healLife = Math.Min(lifeMax - life, healLife);
            life += healLife;
        }

        // 在水草里透明
        float stealthAlpha = fishType == FishType.Player ? 0.3f : 0f;
        SetAlpha(beforeInAquatic ? stealthAlpha : 1f);
    }

    // 毒圈判定
    protected void PoisonRingCheck()
    {
        if (actionStep == ActionType.Die) { return; }
        if (transform.position.sqrMagnitude >= Math.Pow(BattleManagerGroup.GetInstance().poisonRing.GetPoisonRange(), 2))
        {
            if (!beforeInPoisonRing)
            {
                inPoisonRingDmgCnt = 0;
            }
            beforeInPoisonRing = true;
            inPoisonRingTime += Time.deltaTime;
        }
        else
        {

            inPoisonRingTime = 0;
            beforeInPoisonRing = false;
        }

        if (beforeInPoisonRing && inPoisonRingTime >= inPoisonRingDmgCnt * BattleConst.PoisonRingDmgCoolTime)
        {
            life -= BattleConst.PoisonRingDmg * inPoisonRingDmgCnt++;
            if (data.life <= 0)
            {
                Die(null);
            }
        }
    }

    public virtual void Damge()
    {
        canStealthRemainingTime = BattleConst.CanStealthTimeFromDmg;
        AddBuff(this, 0);
    }

    public virtual void Heal()
    {
        BattleEffectManager.CreateEffect(0, transform);
    }

    public void AddBuff(FishBase Initiator, int buffId)
    {
        listBuff.Add(BuffData.SetBuff(Initiator, buffId, this));
    }
}

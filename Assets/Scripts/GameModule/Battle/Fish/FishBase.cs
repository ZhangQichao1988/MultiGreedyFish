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
        public int fishId;
        public string name;
        public int life;
        public int lifeMax;
        public int atk;
        public float moveSpeed;
        public bool isShield;           // 护盾，不受伤害

        public Data(int fishId, string name, int life, int atk, float moveSpeed)
        {
            this.fishId = fishId;
            this.name = name;
            this.life = life;
            this.lifeMax = life;
            this.atk = atk;
            this.moveSpeed = moveSpeed;
            this.isShield = false;
        }
    }
    static protected int uidCnt = 0;

    protected int uid = -1;
    protected uint actionWaitCnt = 0;
    public float fishLevel;
    protected FishDataInfo fishBaseData;
    public Data data;
    public Data originalData;
    protected Animator animator = null;
    public Transform transModel = null;
    protected Renderer[] renderers = null;
    public BoxCollider colliderBody;
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

    protected float dmgTime = 0f;

    protected Vector3 Dir;
    protected Vector3 curDir;
    protected Vector3 moveDir;

    protected float remainingTime = 0f;

    // 被吃掉鱼的Trans
    protected Transform eatFishTrans = null;
    // 用来记录被吃掉时候的缩放值
    protected float localScaleBackup = 0f;
    // 是否在屏幕内
    public bool isBecameInvisible = true;
    //private uint updateCnt = 0;


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
            data.life = value;
            if (lifeGauge != null) { lifeGauge.SetValue(data.life, data.lifeMax); }
        }
    }
    public virtual bool Heal(int value)
    {
        if (value <= 0)
        {
            return false;
        }
        int _life = Mathf.Clamp(life + value, 0, lifeMax );
        life = _life;
        if (lifeGauge) lifeGauge.ShowNumber(new LifeGauge.NumberData(LifeGauge.NumberType.Life, value));
        return true;
    }
    public virtual bool Damage(int dmg, Transform hitmanTrans)
    {
        if (dmgTime > 0) { return false; }
        if (data.isShield) { return false; }
        life -= dmg;
        if(lifeGauge) lifeGauge.ShowNumber(new LifeGauge.NumberData(LifeGauge.NumberType.Damage, dmg));
        if (life <= 0)
        {
            Die(hitmanTrans);
        }
        dmgTime = 0.5f;
        canStealthRemainingTime = BattleConst.instance.CanStealthTimeFromDmg;
        AddBuff(this, 0);
        return true;
    }
    public int lifeMax
    {
        get { return data.lifeMax; }
        set
        {
            data.lifeMax = value;
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


    public virtual void Init(int fishId, string playerName, float level)
    {
        foreach (BuffBase bb in listBuff)
        { bb.Destory(); }
        listBuff.Clear();

        fishLevel = level;
        ApplySize();
        actionStep = ActionType.Born;
        fishBaseData = FishDataTableProxy.Instance.GetDataById(fishId);
        int life = FishLevelUpDataTableProxy.Instance.GetFishHp(fishBaseData, fishLevel);
        int atk = FishLevelUpDataTableProxy.Instance.GetFishAtk(fishBaseData, fishLevel);
        this.data = new Data(fishId, playerName, life, atk, fishBaseData.moveSpeed);
        uid = uidCnt++;
        this.originalData = data;
        transform.name = fishType.ToString() + uid;
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
        
        this.lifeMax = data.lifeMax;
        this.life = data.life;
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
        pos.x = Mathf.Clamp(pos.x, -BattleConst.instance.BgBound, BattleConst.instance.BgBound);
        pos.z = Mathf.Clamp(pos.z, -BattleConst.instance.BgBound, BattleConst.instance.BgBound);

        transform.position = pos;
    }
    public virtual void CustomUpdate()
    {
        if (actionWaitCnt++ >= uint.MaxValue) { actionWaitCnt = 0; }
#if !UNITY_EDITOR   // 性能优化相关，编辑器模式方便debug所以关闭
        // 计算离相机目标的距离太远的话就不显示（优化）
        if((actionWaitCnt + uid) % 3 == 1)
        {
            if (isBecameInvisible)
            {
                if (BattleConst.instance.RobotVisionRange + 5f < Vector3.SqrMagnitude(BattleManagerGroup.GetInstance().cameraFollow.targetPlayerPos - transform.position))
                {
                    foreach (var note in renderers)
                    { if (note.enabled) note.enabled = false; }
                    if(lifeGauge) GameObjectUtil.SetActive(lifeGauge.gameObject, false);
                    isBecameInvisible = false;
                }

            }
            else
            {
                if (BattleConst.instance.RobotVisionRange - 5f > Vector3.SqrMagnitude(BattleManagerGroup.GetInstance().cameraFollow.targetPlayerPos - transform.position))
                {
                    foreach (var note in renderers)
                    { if (!note.enabled) note.enabled = true; }
                    if (lifeGauge) GameObjectUtil.SetActive(lifeGauge.gameObject, true);
                    isBecameInvisible = true;
                }
            }
        }
#endif
        if (dmgTime > 0f) { dmgTime -= Time.deltaTime; }
        BuffUpdate();
        AquaticCheck();
        PoisonRingCheck();
        MoveUpdate();
    }

    void BuffUpdate()
    { 
        data.moveSpeed = originalData.moveSpeed;
        data.isShield = originalData.isShield;

        for (int i = listBuff.Count - 1; i >= 0; --i)
        {
            listBuff[i].Update();
            if (listBuff[i].IsDestory())
            {
                listBuff[i].Destory();
                listBuff.RemoveAt(i);
            }
        }
    }
    public virtual bool EatCheck(PlayerBase player, BoxCollider atkCollider)
    {
        if (actionStep == ActionType.Born ||
            actionStep == ActionType.BornWaitting ||
            actionStep == ActionType.Die ||
            actionStep == ActionType.None)
        {
            return false;
        }
        // 判断是否刚被攻击
        if (ContainsBuff(0) || ContainsBuff(7)) { return false; }
        //float dis = BoundsBody.SqrDistance(mouthPos);
        //range = (float)Math.Pow(range, 2);
        if (transModel.gameObject.activeSelf)
        {
            return colliderBody.bounds.Intersects(atkCollider.bounds);
        }
        return false;
        //else
        //{
        //    return Vector3.Distance(colliderBody.transform.position, atkCollider.transform.position) < 3f;
        //}
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
        float size = 1 + (fishLevel-1) / 5;
        size = Mathf.Min(3f, size);
        transform.localScale = new Vector3(size, size, size);
    }
    protected float GetSafeRudius()
    {
        return Mathf.Min(BattleManagerGroup.GetInstance().poisonRing.GetPoisonRange(), BattleConst.instance.BgBound);
    }

    protected virtual float SetAlpha(float alpha)
    {
        GameObjectUtil.SetActive(transModel.gameObject, alpha > 0);
        if (lifeGauge && isBecameInvisible)
        {
            GameObjectUtil.SetActive(lifeGauge.gameObject, alpha > 0);
        }
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
    /// <summary>
    /// 辉度设置
    /// </summary>
    /// <param name="alpha"></param>
    /// <returns></returns>
    protected virtual void SetBrightness(float brightness)
    {
        brightness = Mathf.Clamp(brightness, 0f, 1f);
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        foreach (Renderer renderer in renderers)
        {
            renderer.GetPropertyBlock(mpb);
            mpb.SetColor("_MulColor", Color.white * brightness);
            renderer.SetPropertyBlock(mpb);
        }
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
        if (!isBecameInvisible) { return; }
        
        canStealthRemainingTime = Math.Max(0f, canStealthRemainingTime - Time.deltaTime);
        if ((actionWaitCnt + uid) % 3 != 0) { return; }
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

        if (beforeInAquatic && inAquaticTime >= inAquaticHealCnt * BattleConst.instance.AquaticHealCoolTime)
        {
            inAquaticHealCnt++;
            int healLife = (int)(BattleConst.instance.AquaticHeal * (float)lifeMax);
            healLife = Mathf.Min(lifeMax - life, healLife);
            Heal(healLife);
        }

        // 在水草里透明
        float stealthAlpha = 1f;
        switch (fishType)
        {
            case FishType.Player:
                stealthAlpha = 0.3f;
                break;
            case FishType.PlayerRobot:
                stealthAlpha = 0f;
                break;
        }
        SetAlpha(beforeInAquatic ? stealthAlpha : 1f);
    }

    // 毒圈判定
    protected void PoisonRingCheck()
    {
        if (actionStep == ActionType.Die ||
            actionStep == ActionType.Born ||
            actionStep == ActionType.BornWaitting) { return; }
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

        if (beforeInPoisonRing && inPoisonRingTime >= inPoisonRingDmgCnt * BattleConst.instance.PoisonRingDmgCoolTime)
        {
            Damage( BattleConst.instance.PoisonRingDmg * inPoisonRingDmgCnt++, null );
        }
    }

    public void RemoteBuff(int buffId)
    {
        BuffBase note;
        for (int i = listBuff.Count - 1; i >= 0; --i)
        {
            note = listBuff[i];
            if (note.baseData.ID == buffId)
            {
                listBuff.RemoveAt(i);
                return;
            }

        }
    }
    public BuffBase AddBuff(FishBase Initiator, int buffId)
    {
        
        BuffBase note;
        for (int i = listBuff.Count - 1; i >= 0; --i)
        {
            note = listBuff[i];
            if (note.Initiator == Initiator && note.baseData.ID == buffId)
            {
                return null;
            }
            
        }

        BuffBase buff = FishBuffDataTableProxy.Instance.SetBuff(Initiator, buffId, this);
        for (int i = listBuff.Count - 1; i >= 0; --i)
        {
            note = listBuff[i];
            
            // Buff类型一样覆盖元Buff
            if (buff.buffType == note.buffType)
            {
                listBuff.RemoveAt(i);
            }
        }
            
        listBuff.Add(buff);
        return buff;
    }

    public bool ContainsBuff(int buffId)
    {
        foreach (var note in listBuff)
        {
            if (note.baseData.ID == buffId) { return true; }
        }
        return false;
    }
    public bool ContainsBuffType(BuffBase.BuffType buffType)
    {
        foreach (var note in listBuff)
        {
            if (note.buffType == buffType) { return true; }
        }
        return false;
    }
}

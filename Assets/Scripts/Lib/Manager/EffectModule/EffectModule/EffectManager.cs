using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 创建销毁特效，维护特效生命周期
/// </summary>
public class EffectManager : MonoBehaviour
{
    private static EffectManager Instance { get; set; }

    /// <summary>
    /// 最大缓存数量
    /// </summary>
    private const int MAX_CACHE = 16;

    /// <summary>
    /// 活动特效
    /// </summary>
    private Dictionary<int, Effect> _activeEffects;

    /// <summary>
    /// 缓存特效
    /// </summary>
    private Dictionary<int, Queue<Effect>> _cachedEffects;

    /// <summary>
    /// 生命到期需要消灭的特效id
    /// </summary>
    private List<int> _effectsToDie;

    /// <summary>
    /// 缓存特效的挂载节点
    /// </summary>
    private GameObject _cacheNode;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        _activeEffects = new Dictionary<int, Effect>();
        _cachedEffects = new Dictionary<int, Queue<Effect>>();
        _effectsToDie = new List<int>();

        _cacheNode = new GameObject("EffectCacheNode");
        _cacheNode.transform.SetParent(transform);
    }

    void OnDestroy()
    {
        Clear();

        if(Instance != null)
        {
            Instance = null;
        }
    }

    /// <summary>
    /// 更新存活特效的生命时间
    /// </summary>
    void LateUpdate()
    {
        Dictionary<int, Effect>.Enumerator enumerator = _activeEffects.GetEnumerator();

        while (enumerator.MoveNext())
        {
            Effect effect = enumerator.Current.Value;

            if(effect != null)
            {
                effect.Update();

                if (!effect.IsAlive() || !effect.IsExist())
                {
                    _effectsToDie.Add(effect.uid);
                }
            }
        }

        for(int i = 0; i < _effectsToDie.Count; i++)
        {
            int uid = _effectsToDie[i];

            Free(uid);
        }
        _effectsToDie.Clear();
    }

    /// <summary>
    /// 预缓存特效
    /// </summary>
    public static void Prewarm(AssetRef<GameObject> effectPrefab, int count)
    {
        if(Instance != null)
        {
            Instance.PrewarmInternal(effectPrefab, count);
        }
    }

    public void PrewarmInternal(AssetRef<GameObject> effectPrefab, int count)
    {
        for(int i = 0; i < count; i++)
        {
            Effect effect = Create(effectPrefab.Asset);
            Deactive(effect);
            PutIntoCache(effect);
        }
    }

    /// <summary>
    /// 重置，清除所有存活特效和缓存特效
    /// </summary>
    public static void Clear()
    {
        if(Instance != null)
        {
            Instance.ClearInternal();
        }
    }

    public void ClearInternal()
    {
        foreach (Effect effect in _activeEffects.Values)
        {
            Destroy(effect);
        }
        _activeEffects.Clear();

        foreach (Queue<Effect> effectQueue in _cachedEffects.Values)
        {
            foreach (Effect effect in effectQueue)
            {
                Destroy(effect);
            }
        }
        _cachedEffects.Clear();

        _effectsToDie.Clear();
    }

    /// <summary>
    /// 清除指定特效缓存
    /// </summary>
    /// <param name="assetRef"></param>
    public static void Clear(AssetRef assetRef)
    {
        if (Instance != null)
        {
            if(assetRef.Asset != null)
            {
                Instance.ClearInternal(assetRef.Asset.GetInstanceID());
            }
        }
    }

    /// <summary>
    /// 清除指定特效缓存
    /// </summary>
    /// <param name="prefab"></param>
    public static void Clear(GameObject prefab)
    {
        if (Instance != null)
        {
            Instance.ClearInternal(prefab.GetInstanceID());
        }
    }

    public void ClearInternal(int instanceID)
    {
        Queue<Effect> effectQueue;

        if(_cachedEffects.TryGetValue(instanceID, out effectQueue))
        {
            foreach (Effect effect in effectQueue)
            {
                Destroy(effect);
            }
        }
        _cachedEffects.Remove(instanceID);
    }

    /// <summary>
    /// 生产特效
    /// </summary>
    /// <param name="effectPrefab">特效资源prefab</param>
    /// <param name="parent">父节点</param>
    /// <param name="duration">持续时间,-1表示无限</param>
    /// <returns></returns>
    public static int Alloc(AssetRef<GameObject> effectPrefab, Transform parent, float duration)
    {
        if(Instance != null)
        {
            return Instance.AllocInternal(effectPrefab.Asset, parent, duration);
        }
        return -1;
    }

    public static int Alloc(GameObject effectPrefab, Transform parent, float duration)
    {
        if (Instance != null)
        {
            return Instance.AllocInternal(effectPrefab, parent, duration);
        }
        return -1;
    }

    public static int Alloc(AssetRef<GameObject> effectPrefab, float duration)
    {
        if (Instance != null)
        {
            return Instance.AllocInternal(effectPrefab.Asset, Vector3.zero, Quaternion.identity, 1.0f, duration);
        }
        return -1;
    }

    public static int Alloc(GameObject effectPrefab, float duration)
    {
        if (Instance != null)
        {
            return Instance.AllocInternal(effectPrefab, Vector3.zero, Quaternion.identity, 1.0f, duration);
        }
        return -1;
    }

    public static int Alloc(AssetRef<GameObject> effectPrefab, Vector3 position, float duration)
    {
        if (Instance != null)
        {
            return Instance.AllocInternal(effectPrefab.Asset, position, Quaternion.identity, 1.0f, duration);
        }
        return -1;
    }

    public static int Alloc(GameObject effectPrefab, Vector3 position, float duration)
    {
        if (Instance != null)
        {
            return Instance.AllocInternal(effectPrefab, position, Quaternion.identity, 1.0f, duration);
        }
        return -1;
    }

    public static int Alloc(AssetRef<GameObject> effectPrefab, Vector3 position, float scale, float duration)
    {
        if (Instance != null)
        {
            return Instance.AllocInternal(effectPrefab.Asset, position, Quaternion.identity, scale, duration);
        }
        return -1;
    }


    public static int Alloc(GameObject effectPrefab, Vector3 position, float scale, float duration)
    {
        if (Instance != null)
        {
            return Instance.AllocInternal(effectPrefab, position, Quaternion.identity, scale, duration);
        }
        return -1;
    }

    public static int Alloc(AssetRef<GameObject> effectPrefab, Vector3 position, Quaternion rotation, float duration)
    {
        if (Instance != null)
        {
            return Instance.AllocInternal(effectPrefab.Asset, position, rotation, 1.0f, duration);
        }
        return -1;
    }


    public static int Alloc(GameObject effectPrefab, Vector3 position, Quaternion rotation, float duration)
    {
        if (Instance != null)
        {
            return Instance.AllocInternal(effectPrefab, position, rotation, 1.0f, duration);
        }
        return -1;
    }

    public static int Alloc(AssetRef<GameObject> effectPrefab, Vector3 position, Quaternion rotation, float scale, float duration)
    {
        if (Instance != null)
        {
            return Instance.AllocInternal(effectPrefab.Asset, position, rotation, scale, duration);
        }
        return -1;
    }


    public static int Alloc(GameObject effectPrefab, Vector3 position, Quaternion rotation, float scale, float duration)
    {
        if (Instance != null)
        {
            return Instance.AllocInternal(effectPrefab, position, rotation, scale, duration);
        }
        return -1;
    }

    private int AllocInternal(GameObject effectPrefab, Transform parent, float duration)
    {
        Effect effect = GetFromCache(effectPrefab.GetInstanceID());
        if (effect == null)
        {
            effect = Create(effectPrefab);
        }
        Active(effect, parent, duration);

        return effect.uid;
    }

    private int AllocInternal(GameObject effectPrefab, Vector3 position, Quaternion rotation, float scale, float duration)
    {
        Effect effect = GetFromCache(effectPrefab.GetInstanceID());
        if (effect == null)
        {
            effect = Create(effectPrefab);
        }
        Active(effect, position, rotation, scale, duration);

        return effect.uid;
    }

    public static void Attach(int handle, Transform parent)
    {
        if (Instance != null)
        {
            Instance.AttachInternal(handle, parent);
        }
    }

    public static void Attach(int handle, Transform parent, Vector3 position, Quaternion rotation, float scale)
    {
        if (Instance != null)
        {
            Instance.AttachInternal(handle, parent, position, rotation, scale);
        }
    }

    private void AttachInternal(int handle, Transform parent)
    {
        if (_activeEffects.TryGetValue(handle, out Effect effect))
        {
            effect.Attach(parent);
        }
    }

    private void AttachInternal(int handle, Transform parent, Vector3 position, Quaternion rotation, float scale)
    {
        if (_activeEffects.TryGetValue(handle, out Effect effect))
        {
            effect.Attach(parent, position, rotation, scale);
        }
    }

    /// <summary>
    /// 立刻消灭特效
    /// </summary>
    /// <param name="handle"></param>
    public static void Free(int handle)
    {
        if(Instance != null)
        {
            Instance.FreeInternal(handle);
        }
    }

    public void FreeInternal(int handle)
    {
        Effect effect;

        if (_activeEffects.TryGetValue(handle, out effect))
        {
            Deactive(effect);

            if(!PutIntoCache(effect))
            {
                Destroy(effect);
            }
        }
    }

    /// <summary>
    /// 先停止放射，延迟后再消灭特效
    /// </summary>
    /// <param name="handle"></param>
    /// <param name="delayTime"></param>
    public static void DelayFree(int handle, float delayTime = 0.0f)
    {
        if(Instance != null)
        {
            Instance.StartCoroutine(Instance.DelayFreeInternal(handle, delayTime));
        }
    }

    private IEnumerator DelayFreeInternal(int handle, float delayTime)
    {
        Effect effect;

        if (_activeEffects.TryGetValue(handle, out effect))
        {
            StopEmit(effect);

            if(delayTime > 0.0f)
            {
                yield return new WaitForSeconds(delayTime);
            }
            else
            {
                yield return new WaitUntil(() => !effect.IsAnyParticleSystemAlive());
            }
            Deactive(effect);

            if (!PutIntoCache(effect))
            {
                Destroy(effect);
            }
        }
    }

    private Effect GetFromCache(int instanceID)
    {
        Queue<Effect> queueEffects;

        if(_cachedEffects.TryGetValue(instanceID, out queueEffects))
        {
            while(queueEffects.Count > 0)
            {
                Effect effect = queueEffects.Dequeue();

                if(effect.IsExist())
                {
                    return effect;
                }
            }
        }
        return null;
    }

    private bool PutIntoCache(Effect effect)
    {
        if(effect.IsExist())
        {
            Queue<Effect> queueEffects;

            if (!_cachedEffects.TryGetValue(effect.resid, out queueEffects))
            {
                queueEffects = new Queue<Effect>(MAX_CACHE);

                _cachedEffects.Add(effect.resid, queueEffects);
            }

            if (queueEffects.Count < MAX_CACHE)
            {
                queueEffects.Enqueue(effect);

                return true;
            }
        }
        return false;
    }

    private Effect Create(GameObject effectPrefab)
    {
        Effect effect = new Effect(effectPrefab);

        return effect;
    }

    private void Destroy(Effect effect)
    {
        effect.Destroy();
    }

    private void Active(Effect effect, Vector3 position, Quaternion rotation, float scale, float duration)
    {
        if (effect != null)
        {
            effect.SetActive(true);
            effect.Attach(null, position, rotation, scale);
            effect.OnActive(duration);

            _activeEffects.Add(effect.uid, effect);
        }
    }

    private void Active(Effect effect, Transform parent, float duration)
    {
        if(effect != null)
        {
            effect.SetActive(true);
            effect.Attach(parent);
            effect.OnActive(duration);

            _activeEffects.Add(effect.uid, effect);
        }
    }

    private void Deactive(Effect effect)
    {
        if(effect != null)
        {
            effect.SetActive(false);
            effect.Attach(_cacheNode.transform);
            effect.OnDeactive();

            _activeEffects.Remove(effect.uid);
        }		
    }

    private void StopEmit(Effect effect)
    {
        effect.Attach(null, true);

        if(effect.ptcs != null)
        {
            for(int i = 0; i < effect.ptcs.Count; i++)
            {
                effect.ptcs[i].Stop();
            }
        }

        if(effect.trdr != null)
        {
            for (int i = 0; i < effect.trdr.Count; i++)
            {
                effect.trdr[i].emitting = false;
            }
        }
    }
}

using System.Collections.Generic;
using UnityEngine;


public class AssetsCacher
{
    private Dictionary<RequestInfo, AssetRef> m_CachedAssets;

#if UNITY_EDITOR
    /// <summary>
    /// 用于编辑器模式下，显示资源加载情况
    /// </summary>
    public static List<AssetRef> AssetRefs
    {
        get;
        private set;
    }
#endif
    public AssetsCacher()
    {
        m_CachedAssets = new Dictionary<RequestInfo, AssetRef>();
#if UNITY_EDITOR
        AssetRefs = new List<AssetRef>();
#endif
    }

    public bool IsCached(RequestInfo info)
    {
        if(info != null)
        {
            return m_CachedAssets.ContainsKey(info);
        }
        return false;
    }

    public void PutIntoCache(RequestInfo info, Object asset)
    {
        if(!m_CachedAssets.ContainsKey(info))
        {
            AssetRef assetRef = System.Activator.CreateInstance(typeof(AssetRef<>).MakeGenericType(info.type), info, asset) as AssetRef;

            m_CachedAssets.Add(info, assetRef);
#if UNITY_EDITOR
            AssetRefs.Add(assetRef);
#endif
        }
    }

    public AssetRef GetFromCache(RequestInfo info)
    {
        if(m_CachedAssets.TryGetValue(info, out AssetRef assetRef))
        {
            assetRef.IncRef();

            return assetRef;
        }
        return null;
    }

    public bool RemoveFromCache(AssetRef assetRef)
    {
        if(assetRef != null)
        {
            if (assetRef.DecRef() <= 0)
            {
                m_CachedAssets.Remove(assetRef.Info);
#if UNITY_EDITOR
                if (assetRef.RefCount < 0)
                {
                    Debug.LogError(assetRef.Info.path + " is already unloaded");
                }
                AssetRefs.Remove(assetRef);
#endif
                assetRef.Dispose();

                return true;
            }
        }
        return false;
    }

    public Dictionary<RequestInfo, AssetRef> GetAllCachedAssets()
    {
        return m_CachedAssets;
    }

    public void Clear()
    {
        m_CachedAssets.Clear();
#if UNITY_EDITOR
        AssetRefs.Clear();
#endif
    }

    public void Update()
    {

    }
}
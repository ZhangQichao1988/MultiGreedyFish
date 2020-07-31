using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseScene
{
    Dictionary<int, Dictionary<int ,string>> cachedDic = new Dictionary<int, Dictionary<int, string>>();
    public Dictionary<string, UnityEngine.Object> cachedObject = new Dictionary<string, UnityEngine.Object>();

    public class SceneData
    {
        public string Resource;
        public Type ResType;
    }
    protected List<SceneData> m_sceneData = new List<SceneData>();

    private int resBlock;
    public virtual void Update()
    {

    }

    public virtual void Init(System.Object parms)
    {

    }

    void OnLoaded(int block, int resHandle, AssetRef obj)
    {
        string resPath = cachedDic[block][resHandle];
        Debug.Assert(obj != null, resPath + " is not found.");
        cachedObject[resPath] = obj.Asset;
    }

    public virtual void Cache(int block)
    {
        resBlock = block;
        if (!cachedDic.ContainsKey(resBlock))
        {
            cachedDic[block] = new Dictionary<int, string>();
        }
        foreach (var item in m_sceneData)
        {
            int resHandle = ResourceManager.Request(item.Resource, item.ResType, OnLoaded);
            cachedDic[block][resHandle] = item.Resource;
        }
    }

    public virtual void Create()
    {

    }

    public virtual void Destory()
    {
        cachedDic = null;
        cachedObject = null;
    }

}

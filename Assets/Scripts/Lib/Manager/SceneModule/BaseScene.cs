using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseScene
{
    Dictionary<int, Dictionary<int ,string>> cachedDic = new Dictionary<int, Dictionary<int, string>>();
    public Dictionary<string, UnityEngine.Object> cachedObject = new Dictionary<string, UnityEngine.Object>();

    protected Dictionary<string, GameObject> dicUI;

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
        dicUI = new Dictionary<string, GameObject>();
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
        dicUI = null;
    }

    private void CreateUI(string uiName)
    {
        string uiPath = Path.Combine(AssetPathConst.uiRootPath, uiName);
        var mainGo = cachedObject[uiPath] as GameObject;
        mainGo = GameObjectUtil.InstantiatePrefab(mainGo, null);
        dicUI.Add(uiName, mainGo);
    }
    public void GotoSceneUI(string uiName)
    {
        // 隐藏所有UI
        foreach (var note in dicUI.Values)
        {
            note.SetActive(false);
        }

        if (dicUI.ContainsKey(uiName))
        {
            dicUI[uiName].SetActive(true);
        }
        else
        {
            CreateUI(uiName);
        }
    }

}

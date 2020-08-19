using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseScene
{
    private string currentScene;
    private List<string> sceneHistory;

    Dictionary<int, Dictionary<int ,string>> cachedDic = new Dictionary<int, Dictionary<int, string>>();
    public Dictionary<string, UnityEngine.Object> cachedObject = new Dictionary<string, UnityEngine.Object>();

    protected Dictionary<string, UIBase> dicUI;

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
        Resources.UnloadUnusedAssets();
        sceneHistory = new List<string>();
        // Home相关UI预载
        dicUI = new Dictionary<string, UIBase>();
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

    private UIBase CreateUI(string uiName)
    {
        string uiPath = Path.Combine(AssetPathConst.uiRootPath, uiName);
        var mainGo = cachedObject[uiPath] as GameObject;
        mainGo = GameObjectUtil.InstantiatePrefab(mainGo, null);
        var uiBase = mainGo.GetComponent<UIBase>();
        uiBase.Init();
        dicUI.Add(uiName, uiBase);
        return uiBase;
    }
    public virtual UIBase GotoSceneUI(string uiName, bool saveHistory = true)
    {
        if (saveHistory) { sceneHistory.Add(currentScene); }

        currentScene = uiName;
        // 隐藏所有UI
        foreach (var note in dicUI.Values)
        {
            note.gameObject.SetActive(false);
        }

        if (dicUI.ContainsKey(uiName))
        {
            dicUI[uiName].gameObject.SetActive(true);
            dicUI[uiName].Init();
            return dicUI[uiName];
        }
        else
        {
            return CreateUI(uiName);
        }
    }

    public void BackPrescene()
    {
        GotoSceneUI(sceneHistory[sceneHistory.Count - 1], false);
        sceneHistory.RemoveAt(sceneHistory.Count - 1);
    }
}

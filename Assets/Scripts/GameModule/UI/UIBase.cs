using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIBase : MonoBehaviour
{

    protected GameObject root;
    public Action onClose = null;
    protected virtual string uiName { get;set; }

    static Dictionary<string, UIBase> dicUi = new Dictionary<string, UIBase>();
    public enum UILayers
    {
        DEFAULT,
        POPUP,
        RESOURCE,
        OVERLAY,
        LOADING
    }

    public static GameObject Open(string path, UILayers layer = UILayers.DEFAULT, System.Object parms = null)
    {
        var rootObj = BlUIManager.GetLayerNode(layer.ToString());
	
        AssetRef<GameObject> objRef = ResourceManager.LoadSync<GameObject>(path);
        GameObject go = GameObject.Instantiate(objRef.Asset, rootObj.transform);
        UIBase uIBase = go.GetComponent<UIBase>();
        uIBase.uiName = path + "|" + go.GetHashCode();
        uIBase.Init();
        uIBase.OnEnter(parms);
        dicUi.Add(uIBase.uiName, uIBase);
        return go;
    }

    public static T Open<T>(string path, UILayers layer = UILayers.DEFAULT, System.Object parms = null)
    {
        GameObject go = Open(path, layer, parms);
        return go.GetComponent<T>();
    }

    public static void Close(string uiName)
    {
        if (!dicUi.ContainsKey(uiName)) { return; }
        dicUi[uiName].Close();
    }

    protected virtual void Awake()
    {
        root = gameObject;
        uiName = uiName == null ? "ui" + gameObject.GetHashCode().ToString() : uiName;
    }

    void Start()
    {

    }
    
    void OnDestroy()
    {
        OnUnRegisterEvent();
    }

    protected virtual void OnRegisterEvent()
    {

    }

    protected virtual void OnUnRegisterEvent()
    {
        
    }
    public virtual void Show()
    {
        if (root != null)
        {
            root.SetActive(true);
        }
    }

    public virtual void Hide()
    {
        if (root != null)
        {
            root.SetActive(false);
        }
    }

    public virtual void Close()
    {
        if (onClose != null) { onClose(); }
        dicUi.Remove(uiName);
        Destroy(root);
    }

    /// <summary>
    /// 初始化调用一次
    /// </summary>
    public virtual void Init()
	{
        OnRegisterEvent();
	}

    /// <summary>
    /// 每次进来都会调用
    /// </summary>
    /// <param name="obj"></param>
    public virtual void OnEnter(System.Object obj)
	{
	 
	}
	
    protected virtual IEnumerator DelayClose(float time = 1.5f)
    {
        yield return new WaitForSeconds(time);
        Close();
    }
}
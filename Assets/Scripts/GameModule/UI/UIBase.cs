using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIBase : MonoBehaviour
{

    protected GameObject root;
    protected virtual string uiName { get;set; }

    static Dictionary<string, UIBase> dicUi = new Dictionary<string, UIBase>();
    public enum UILayers
    {
        DEFAULT,
        POPUP,
        OVERLAY,
        LOADING
    }

    public static GameObject Open(string path, UILayers layer = UILayers.DEFAULT)
    {
        var rootObj = BlUIManager.GetLayerNode(layer.ToString());
	
        AssetRef<GameObject> objRef = ResourceManager.LoadSync<GameObject>(path);
        GameObject go = GameObject.Instantiate(objRef.Asset, rootObj.transform);
        UIBase uIBase = go.GetComponent<UIBase>();
        uIBase.uiName = path + "|" + go.GetHashCode();
        dicUi.Add(uIBase.uiName, uIBase);
        return go;
    }

    public static T Open<T>(string path, UILayers layer = UILayers.DEFAULT)
    {
        GameObject go = Open(path, layer);
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
        dicUi.Remove(uiName);
        Destroy(root);
    }
    public virtual void Init()
	{
	
	}
	
    protected virtual IEnumerator DelayClose(float time = 1.5f)
    {
        yield return new WaitForSeconds(time);
        Close();
    }
}
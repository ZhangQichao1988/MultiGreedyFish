using UnityEngine;
using System;
using System.Collections.Generic;

public class UIBase : MonoBehaviour
{

    protected GameObject root;
    protected virtual string uiName { get { return ""; } }

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

        void Awake()
    {
        root = gameObject;
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
}
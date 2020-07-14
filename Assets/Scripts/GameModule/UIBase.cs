using UnityEngine;
using System;

public class UIBase : MonoBehaviour
{
    protected GameObject root;

    public enum UILayers
    {
        DEFAULT,
        POPUP,
        OVERLAY,
        LOADING
    }

    public static void Open(string path, UILayers layer = UILayers.DEFAULT)
    {
        var rootObj = BlUIManager.GetLayerNode(layer.ToString());
	
        AssetRef<GameObject> objRef = ResourceManager.LoadSync<GameObject>(path);
        GameObject go = GameObject.Instantiate(objRef.Asset, rootObj.transform);
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
}
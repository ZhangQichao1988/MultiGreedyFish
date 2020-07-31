using UnityEngine;
using UnityEngine.UI;

public class UIPanel
{
    private GameObject parentObject;
    private System.Object parmsInfo;
    protected GameObject UIRootObject;

    private GameObject cachedPrefab;
    private string rootPath;
    public UIPanel(string path)
    {
        rootPath = path;
    }

    public virtual void Create(GameObject go, System.Object parms)
    {
        parentObject = go;
        parmsInfo = parms;
        this.PreLoad();
        cachedPrefab = BlSceneManager.GetCachedPrefab(rootPath);
        if (cachedPrefab == null)
        {
            cachedPrefab = ResourceManager.LoadSync(rootPath, typeof(GameObject)).Asset as GameObject;
        }
        
        UIRootObject = GameObjectUtil.InstantiatePrefab(cachedPrefab, parentObject);
        this.OnEnter();
        this.OnRegisterEvent();
    }

    public void Close()
    {
        GameObject.Destroy(UIRootObject);
    }

    public void Show()
    {
        UIRootObject.SetActive(true);
    }
    public void Hide()
    {
        UIRootObject.SetActive(false);
    }

    protected void AddListener(GameObject go)
    {
        
    }

    protected void RemoveListener(GameObject go)
    {
        
    }

    protected void AddClickListener(GameObject go)
    {
        
    }

    protected void RemoveClickListener(GameObject go)
    {
        
    }

    protected virtual void OnEnter()
    {
        
    }

    protected virtual void PreLoad()
    {

    }

    protected virtual void OnRegisterEvent()
    {

    }

    protected virtual void OnUnRegisterEvent()
    {
        
    }
}
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基表代理 提供对外数据接口
/// </summary>
/// <typeparam name="T">基表基础类型集合</typeparam>
/// <typeparam name="V">基表基础数据类</typeparam>
/// <typeparam name="U">子类实现单例</typeparam>
public class BaseDataTableProxy<T, V, U> : IDataTableProxy where T : BaseDataTable<V> where U : IDataTableProxy, new() where V : IQueryById
{
    //基表内容
    protected List<V> content;

    bool hasCached;

    AssetRef<TextAsset> assetRef;
    private string tableName;

    public string TableName
    {
        get
        {
            return tableName;
        }
    }

    public BaseDataTableProxy(string tbName)
    {
        tableName = tbName;
        Cached();
    }

    public void Cached()
    {
        if (!hasCached)
        {
            assetRef = ResourceManager.LoadSync<TextAsset>(tableName);
            T entity = JsonUtility.FromJson<T>(assetRef.Asset.text);
            content = entity.Items;
            hasCached = true;
        }
    }

    public List<V> GetAll()
    {
        return content;
    }

    public V GetDataById(int id)
    {
        return content.Find(t=>t.ID == id);
    }

    public void Destory()
    {
        ResourceManager.Unload(assetRef);
        assetRef = null;
        hasCached = false;
        content = null;
    }


    private static U _instance;

    public static U Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new U();
                BaseDataTableProxyMgr.Add(_instance.TableName, _instance);
            }
            return _instance;
        }
    }
}

public static class BaseDataTableProxyMgr
{
    private static Dictionary<string, IDataTableProxy> proxyDic = new Dictionary<string, IDataTableProxy>();

    internal static void Add(string key, IDataTableProxy value)
    {
        proxyDic.Add(key, value);
    }

    public static void Destory(string name = null)
    {
        if (name == null)
        {
            foreach (var proxy in proxyDic.Values)
            {
                proxy.Destory();
            }
            proxyDic.Clear();
        }
        else
        {
            if (proxyDic.ContainsKey(name))
            {
                proxyDic[name].Destory();
                proxyDic.Remove(name);
            }
        }
    }

    
}
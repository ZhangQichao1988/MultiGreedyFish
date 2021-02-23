using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

/// <summary>
/// 基表代理 提供对外数据接口
/// </summary>
/// <typeparam name="T">基表基础类型集合</typeparam>
/// <typeparam name="V">基表基础数据类</typeparam>
/// <typeparam name="U">子类实现单例</typeparam>
public class BaseDataTableProxy<T, V, U> : IDataTableProxy where T : BaseDataTable<V> where U : IDataTableProxy, new() where V : IQueryById
{
    //基表内容
    protected Dictionary<int, V> content;

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

    public void ResetCache()
    {
        hasCached = false;
        Cached();
    }

    public virtual void Cached()
    {
        if (!hasCached)
        {
            T entity = null;
            if (AppConst.ServerType == ESeverType.OFFLINE)
            {
                assetRef = ResourceManager.LoadSync<TextAsset>(tableName);
                entity = JsonUtility.FromJson<T>(assetRef.Asset.text);
            }
            else
            {
                string jsonText = ReadFromLocalStorage(tableName);
                if (jsonText == null)
                {
                    Debug.LogError("Master Data is not exist");
                    return;
                }
                entity = JsonUtility.FromJson<T>(jsonText);
            }

            List<V> contentList = entity.Items;
            content = new Dictionary<int, V>();
            foreach (var item in contentList)
            {
                if (!content.ContainsKey(item.ID))
                {
                    content.Add(item.ID, item);
                }
            }
            hasCached = true;
        }
    }

    /// <summary>
    /// 读基表并解密
    /// </summary>
    /// <param name="tableName"></param>
    /// <returns></returns>
    private string ReadFromLocalStorage(string tableName)
    {
        string localFileName = Path.Combine( AppConst.MasterSavedPath, tableName.Replace("JsonData/", "") + ".json.enc");
        if (!File.Exists(localFileName))
        {
            return null;
        }
        byte[] fileBytes = File.ReadAllBytes(localFileName);
        ZipHelper.DesMasterFile(fileBytes);
        return System.Text.Encoding.UTF8.GetString(fileBytes);
    }

    public List<V> GetAll()
    {
        return content.Values.ToList();
    }

    public V GetDataById(int id)
    {
        return (content != null && content.ContainsKey(id)) ? content[id] : null;
    }

    public void Destory()
    {
        if (assetRef != null)
        {
            ResourceManager.Unload(assetRef);
            assetRef = null;
        }
        _instance = default(U);
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

    public static void ResetCache()
    {
        foreach (var proxy in proxyDic.Values)
        {
            proxy.ResetCache();
        }
    }

    
}
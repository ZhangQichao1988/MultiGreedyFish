using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基表代理 提供对外数据接口
/// </summary>
/// <typeparam name="T">基表基础类型集合</typeparam>
/// <typeparam name="V">基表基础数据类</typeparam>
/// <typeparam name="U">子类实现单例</typeparam>
public class BaseDataTableProxy<T, V, U> where T : BaseDataTable<V> where U : class, new()
{
    //基表内容
    protected List<V> content;

    bool hasCached;
    protected string tableName;

    public void Cached()
    {
        if (!hasCached)
        {
            var asset = Resources.Load<TextAsset>(tableName);
            T entity = JsonUtility.FromJson<T>(asset.text);
            content = entity.Items;
            hasCached = true;
            Debug.Log(asset.text);
        }
        Debug.LogWarning(content.Count);
    }


    private static U _instance;

    public static U Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new U();
            }
            return _instance;
        }
    }
}
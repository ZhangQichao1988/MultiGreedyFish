using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 基础model
/// </summary>
/// <typeparam name="T"></typeparam>
public class BaseModel<T> where T : class, new()
{

    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
            }
            return _instance;
        }
    }

    private Dictionary<string, Action<System.Object>> dispatchDic;
    public BaseModel()
    {
        dispatchDic = new Dictionary<string, Action<System.Object>>();
    }

    public void Dispose()
    {
        dispatchDic = null;
        _instance = null;
    }

    public void AddListener(string evt, Action<Object> callback)
    {
        if (!dispatchDic.ContainsKey(evt))
        {
            dispatchDic.Add(evt, null);
        }
        dispatchDic[evt] = dispatchDic[evt] + callback;
    }

    public void RemoveListener(string evt, Action<Object> callback)
    {
        if (dispatchDic.ContainsKey(evt))
        {
            dispatchDic[evt] = dispatchDic[evt] - callback;
        }
        if (dispatchDic[evt] == null)
        {
            dispatchDic.Remove(evt);
        }
    }

    public void RemoveAllListener(string evt)
    {
        dispatchDic.Remove(evt);
    }


    public void RemoveAll()
    {
        dispatchDic.Clear();
    }

    protected void Dispatch(string evt, System.Object obj = null)
    {
        if (dispatchDic.ContainsKey(evt))
        {
            if (dispatchDic[evt] != null)
            {
                dispatchDic[evt](obj);
            }
        }
    }
}
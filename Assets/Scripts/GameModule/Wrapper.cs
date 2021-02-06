using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wrapper
{
    static public GameObject CreateGameObject(string assetPath, GameObject parent, string name = null)
    {
        GameObject go = ResourceManager.LoadSync(assetPath, typeof(GameObject)).Asset as GameObject;
        go = GameObjectUtil.InstantiatePrefab(go, parent, false);
        go.name = name;
        return go;
    }

    static public GameObject CreateEmptyGameObject(Transform parent, string name = null)
    {
        GameObject go = new GameObject(name);
        go.transform.parent = parent;
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = Vector3.zero;
        return go;
    }
        

    static public float GetRandom(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    static public int GetRandom(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    public static float[] GetParamFromString(string param)
    {
        List<float> listParam = new List<float>();
        string[] aryStr = param.Split(',');
        foreach (string str in aryStr)
        {
            listParam.Add(float.Parse(str));
        }
        return listParam.ToArray();
    }

    public static List<T> RandomSortList<T>(List<T> list)
    {
        List<T> cache = new List<T>();
        int currentIndex;
        while (list.Count > 0)
        {
            currentIndex = GetRandom(0, list.Count);
            cache.Add(list[currentIndex]);
            list.RemoveAt(currentIndex);
        }
        for (int i = 0; i < cache.Count; i++)
        {
            list.Add(cache[i]);
        }
        return list;
    }
    public static DateTime ConvertIntDatetime(double utc)

    {

        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));

        startTime = startTime.AddSeconds(utc);

        //startTime = startTime.AddHours(8);//转化为北京时间(北京时间=UTC时间+8小时 )            

        return startTime;

    }
}

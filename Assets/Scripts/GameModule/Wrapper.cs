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
}

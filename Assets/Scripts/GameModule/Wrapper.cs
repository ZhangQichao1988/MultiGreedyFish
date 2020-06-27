using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wrapper
{
    static public GameObject CreateGameObject(Object original, Transform parent, string name = null)
    {
        GameObject go = ResourceManager.Instantiate(original, parent) as GameObject;
        if (name != null)
        {
            go.name = name;
        }
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

    static public float GetRandom(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }
}

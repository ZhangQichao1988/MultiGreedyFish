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

    static public float GetRandom(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }
}

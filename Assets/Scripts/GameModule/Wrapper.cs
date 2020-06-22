using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wrapper
{
    static public Object AssetLoad(Object original, Transform parent)
    {
        return ResourceManager.Instantiate(original, parent);
    }

    static public float GetRandom(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }
}

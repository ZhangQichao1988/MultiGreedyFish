using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class SerializableDictionary<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver
{
    [SerializeField]
    private K[] keys;

    [SerializeField]
    private V[] values;

    public void OnAfterDeserialize()
    {
        Clear();

        var c = keys.Length;
        for (int i = 0; i < c; i++)
        {
            this[keys[i]] = values[i];
        }
        keys = null;
        values = null;
    }

    public void OnBeforeSerialize()
    {
        keys = new K[Count];
        values = new V[Count];
        int i = 0;
        using (Enumerator e = GetEnumerator())
        {
            while (e.MoveNext())
            {
                var kvp = e.Current;
                keys[i] = kvp.Key;
                values[i] = kvp.Value;
                i++;
            }
        }         
    }
}
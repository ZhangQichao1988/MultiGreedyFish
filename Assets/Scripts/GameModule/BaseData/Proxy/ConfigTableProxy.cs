using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// 基表代理
/// 每张基表对应一个proxy
/// </summary>
public class ConfigTableProxy : BaseDataTableProxy<ConfigTable, ConfigInfo, ConfigTableProxy>
{
    protected Dictionary<string, float> keyContent;
    public ConfigTableProxy() : base("JsonData/Config") { }

    public override void Cached()
    {
        base.Cached();
        keyContent = new Dictionary<string, float>();
        foreach (var note in content.Values)
        {
            Debug.Assert(!keyContent.ContainsKey(note.key), "ConfigTableProxy.Cached()_1"); 
            keyContent.Add(note.key, note.value);
        }
    }

    public float GetDataByKey(string key)
    {
        Debug.Assert(keyContent.ContainsKey(key), "ConfigTableProxy.GetDataByKey()_1");
        return keyContent[key];
    }
}
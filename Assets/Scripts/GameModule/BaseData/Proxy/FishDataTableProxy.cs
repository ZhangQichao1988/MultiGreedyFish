using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// 基表代理
/// 每张基表对应一个proxy
/// </summary>
public class FishDataTableProxy : BaseDataTableProxy<FishDataTable, FishDataInfo, FishDataTableProxy>
{
    public FishDataTableProxy() : base("JsonData/FishData"){}

    public FishDataInfo GetDataById(int id)
    {
        return content.Find(t=>t.ID == id);
    }
}
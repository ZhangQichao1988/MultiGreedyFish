using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// 基表代理
/// 每张基表对应一个proxy
/// </summary>
public class FishSkillDataTableProxy : BaseDataTableProxy<FishSkillDataTable, FishSkillDataInfo, FishSkillDataTableProxy>
{

    public FishSkillDataTableProxy() : base("JsonData/FishSkillData") {}


}
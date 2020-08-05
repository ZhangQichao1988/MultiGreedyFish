using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;

/// <summary>
/// 基表代理
/// 每张基表对应一个proxy
/// </summary>
public class RobotAiDataTableProxy : BaseDataTableProxy<RobotAiDataTable, RobotAiDataInfo, RobotAiDataTableProxy>
{

    public RobotAiDataTableProxy() : base("JsonData/RobotAiData") {}

}
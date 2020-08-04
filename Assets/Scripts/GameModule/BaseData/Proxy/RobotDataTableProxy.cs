using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// 基表代理
/// 每张基表对应一个proxy
/// </summary>
public class RobotDataTableProxy : BaseDataTableProxy<RobotDataTable, RobotDataInfo, RobotDataTableProxy>
{
    public RobotDataTableProxy() : base("JsonData/sc_RobotData") {}

	public int GetRobotCount()
	{
		return GetAll().Count;
	}

	public int[] GetAllRobotFishIds()
	{
		List<int> retList = new List<int>();
		
		foreach (var prdbd in GetAll())
		{
			retList.Add(prdbd.fishId);
		}
		return retList.ToArray();
	}
}
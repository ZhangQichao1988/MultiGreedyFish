using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 基表代理
/// 每张基表对应一个proxy
/// </summary>
public class RobotNameDataTableProxy : BaseDataTableProxy<RobotNameDataTable, RobotNameDataInfo, RobotNameDataTableProxy>
{
    public RobotNameDataTableProxy() : base("JsonData/RobotNameData") {}

	public int GetRobotCount()
	{
		return GetAll().Count;
	}

	public string[] GetAllRobotNames()
	{
		var listName = GetAll();
		listName.Sort((a, b)=> { return Wrapper.GetRandom(-1, 1); });
		List<string> ret = new List<string>();
		for (int i = 0; i < 9; ++i)
		{
			ret.Add(listName[i].name);
		}
		return ret.ToArray();
	}
}
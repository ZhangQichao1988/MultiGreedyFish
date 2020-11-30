using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 基表代理
/// 每张基表对应一个proxy
/// </summary>
public class FishBuffDataTableProxy : BaseDataTableProxy<FishBuffDataTable, FishBuffDataInfo, FishBuffDataTableProxy>
{

    public FishBuffDataTableProxy() : base("JsonData/FishBuffData") {}

	public BuffBase SetBuff(FishBase Initiator, int id, FishBase fish)
	{
		FishBuffDataInfo bbd = GetDataById(id);
		System.Type type = Type.GetType(bbd.buffType);
		BuffBase bb = Activator.CreateInstance(type, new object[] { Initiator, fish, bbd }) as BuffBase;
		return bb;
	}


}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



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
		float[] aryParam = Wrapper.GetParamFromString(bbd.aryParam);
		switch (bbd.buffType)
		{
			case "SpeedUp":
				return new BuffSpeedUp(Initiator, fish, aryParam);
			case "BeSucked":
				return new BuffBeSucked(Initiator, fish, aryParam);
		}
		Debug.LogError("BuffData.GetBuff()_2");
		return null;
	}


}
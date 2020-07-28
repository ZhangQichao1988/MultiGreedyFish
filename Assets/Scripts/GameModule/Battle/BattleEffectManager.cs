using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BattleEffectManager
{
	static public void CreateEffect(int id, Transform parent)
	{
		EffectData.EffectBaseData effectBaseData = EffectData.GetEffectData(id);
		GameObject go = ResourceManager.LoadSync(Path.Combine(AssetPathConst.effectRootPath, effectBaseData.prefabPath), typeof(GameObject)).Asset as GameObject;
		EffectManager.Alloc(go, parent, effectBaseData.duration);
	}
}

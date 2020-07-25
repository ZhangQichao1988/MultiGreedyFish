using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BattleEffectManager
{
	static readonly string prefabRootPath = "ArtResources/Effect/Prefab/FX";
	static public void CreateEffect(int id, Transform parent)
	{
		EffectData.EffectBaseData effectBaseData = EffectData.GetEffectData(id);
		UnityEngine.Object obj = Resources.Load(Path.Combine(prefabRootPath, effectBaseData.prefabPath));
		//GameObject go = Wrapper.CreateGameObject(obj, parent) as GameObject;
		EffectManager.Alloc(obj as GameObject, parent, effectBaseData.duration);
	}
}

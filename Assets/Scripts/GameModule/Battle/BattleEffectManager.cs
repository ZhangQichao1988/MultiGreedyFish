using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BattleEffectManager
{
	static BattleEffectManager _instance = null;
	Dictionary<int, AssetRef<GameObject>> dicEffect;
	static public void Init()
	{
		if (_instance == null) { _instance = new BattleEffectManager(); }
		_instance.dicEffect = new Dictionary<int, AssetRef<GameObject>>();
		var listEff = EffectDataTableProxy.Instance.GetAll();
		AssetRef<GameObject> _effect;
		foreach (var note in listEff)
		{
			_effect = ResourceManager.LoadSync<GameObject>(Path.Combine(AssetPathConst.effectRootPath, note.prefabPath));
			EffectManager.Prewarm(_effect, note.cacheCount);
			_instance.dicEffect[note.ID] = _effect;
		}
	}

	static public int CreateEffect(int id, Transform parent)
	{
		var effectBaseData = EffectDataTableProxy.Instance.GetDataById(id);
		GameObject go = ResourceManager.LoadSync(Path.Combine(AssetPathConst.effectRootPath, effectBaseData.prefabPath), typeof(GameObject)).Asset as GameObject;
		return EffectManager.Alloc(_instance.dicEffect[id], parent, effectBaseData.duration);
	}
	static public int CreateEffect(int id, Vector3 position, float scale)
	{
		var effectBaseData = EffectDataTableProxy.Instance.GetDataById(id);
		GameObject go = ResourceManager.LoadSync(Path.Combine(AssetPathConst.effectRootPath, effectBaseData.prefabPath), typeof(GameObject)).Asset as GameObject;
		return EffectManager.Alloc(_instance.dicEffect[id], position, scale, effectBaseData.duration);
	}
	static public void Destroy()
	{
		_instance.dicEffect.Clear();
		EffectManager.Clear();
		_instance = null;
	}
}

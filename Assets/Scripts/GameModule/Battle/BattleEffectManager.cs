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

	static public int CreateEffect(int id, Transform parent, float scale = float.NaN)
	{
		//PlaySE(_instance.dicEffect[id].Asset);
		
		var effectBaseData = EffectDataTableProxy.Instance.GetDataById(id);
		int effectId = EffectManager.Alloc(_instance.dicEffect[id], parent, effectBaseData.duration);
		if (!float.IsNaN(scale))
		{
			var effect = EffectManager.GetEffect(effectId);
			var particles = effect.effectObject.GetComponentsInChildren<ParticleSystem>();
			foreach (var particle in particles)
			{
				particle.transform.localScale = Vector3.one * scale;
			}

		}
		return effectId;
	}
	static public int CreateEffect(int id, Vector3 position, float scale)
	{
		//PlaySE(_instance.dicEffect[id].Asset);

		var effectBaseData = EffectDataTableProxy.Instance.GetDataById(id);
		return EffectManager.Alloc(_instance.dicEffect[id], position, scale, effectBaseData.duration);
	}
	static private void PlaySE(GameObject goEffect)
	{
		AudioSource audioSource = goEffect.GetComponent<AudioSource>();
		if (audioSource == null)
		{
			audioSource = goEffect.AddComponent<AudioSource>();
		}
		audioSource.clip = ResourceManager.LoadSync<AudioClip>(string.Format(AssetPathConst.soundRootPath, 0)).Asset;
	}
	static public void Destroy()
	{
		_instance.dicEffect.Clear();
		EffectManager.Clear();
		_instance = null;
	}
}

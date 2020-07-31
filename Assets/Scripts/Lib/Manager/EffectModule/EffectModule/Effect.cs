using System.Collections.Generic;
using UnityEngine;


public class Effect
{
	/// <summary>
	/// 唯一ID
	/// </summary>
	public int uid;

	/// <summary>
	/// 资源ID
	/// </summary>
	public int resid;

	/// <summary>
	/// 特效对象
	/// </summary>
	public GameObject effectObject;

	/// <summary>
	/// 持续时间,-1为无限
	/// </summary>
	public float duration;

	/// <summary>
	/// 特效生命时间
	/// </summary>
	public float life;

	public List<TrailRenderer> trdr = new List<TrailRenderer>();
	public List<ParticleSystem> ptcs = new List<ParticleSystem>();
	public List<Animation> anims = new List<Animation>();
	public List<Animator> animtors = new List<Animator>();

	public Effect(GameObject effectPrefab)
	{
		resid = effectPrefab.GetInstanceID();
		effectObject = Object.Instantiate(effectPrefab);
		uid = effectObject.GetInstanceID();

		ptcs.AddRange(effectObject.GetComponentsInChildren<ParticleSystem>());
		anims.AddRange(effectObject.GetComponentsInChildren<Animation>());
		animtors.AddRange(effectObject.GetComponentsInChildren<Animator>());
	}

	public void Update()
	{
		if (duration > 0)
		{
			life -= Time.deltaTime;
		}
	}

	public void Destroy()
	{
		ptcs.Clear();
		anims.Clear();
		animtors.Clear();

		if (effectObject != null)
		{
			Object.Destroy(effectObject);
		}
	}

	public bool IsAlive()
	{
		return duration < 0.0f || life > 0.0f;
	}

	public bool IsLoop()
	{
		return duration < 0.0f;
	}

	public bool IsExist()
	{
		return effectObject != null;
	}

	public bool IsAnyParticleSystemAlive()
	{
		for (int i = 0; i < ptcs.Count; i++)
		{
			ParticleSystem ps = ptcs[i];

			if (ps != null)
			{
				if(ps.IsAlive())
				{
					return true;
				}
			}
		}
		return false;
	}

	public void SetActive(bool active)
	{
		if(effectObject != null)
		{
			effectObject.SetActive(active);
		}
	}

	public void OnActive(float lifeDuration)
	{
		duration = lifeDuration;
		life = lifeDuration;

		for (int i = 0; i < ptcs.Count; i++)
		{
			ParticleSystem ps = ptcs[i];

			if (ps != null)
			{
				ps.Play();
			}
		}

		for (int i = 0; i < trdr.Count; i++)
		{
			TrailRenderer tr = trdr[i];

			if (tr != null)
			{
				tr.emitting = true;
			}
		}

		for (int i = 0; i < anims.Count; i++)
		{
			Animation an = anims[i];
			if (an != null)
			{
				an.Play(PlayMode.StopAll);
			}
		}

		for (int i = 0; i < animtors.Count; i++)
		{
			Animator an = animtors[i];
			if (an != null)
			{
				an.Play(an.GetCurrentAnimatorStateInfo(0).shortNameHash, 0);
			}
		}
	}

	public void Attach(Transform parent, bool worldPositionStays = false)
	{
		if (effectObject != null)
		{
			effectObject.transform.SetParent(parent, worldPositionStays);

			if(!worldPositionStays)
			{
				effectObject.transform.localPosition = Vector3.zero;
				effectObject.transform.localRotation = Quaternion.identity;
				effectObject.transform.localScale = Vector3.one;
			}
		}
	}

	public void Attach(Transform parent, Vector3 position, Quaternion rotation, float scale)
	{
		if(effectObject != null)
		{
			effectObject.transform.SetParent(parent);
			effectObject.transform.position = position;
			effectObject.transform.rotation = rotation;
			effectObject.transform.localScale = Vector3.one * scale;
		}
	}

	public void OnDeactive()
	{

	}
}

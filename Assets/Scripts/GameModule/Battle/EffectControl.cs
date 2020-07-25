using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectControl : MonoBehaviour
{
	int particleNum = 0;
	int particleFinishedNum = 0;
	private void Awake()
	{
		var aryParticle = GetComponentsInChildren<ParticleSystem>().ToArray();

		for (int i = 0; i < aryParticle.Length; ++i)
		{
			ParticleSystem.MainModule mainModule = aryParticle[i].main;
			if (!mainModule.loop) { ++particleNum; }
			mainModule.stopAction = ParticleSystemStopAction.Destroy;
		}
	}
}

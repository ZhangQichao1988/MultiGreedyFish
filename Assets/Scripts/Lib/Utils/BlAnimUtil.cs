using UnityEngine;

public static class BlAnimUtil
{
	public static AnimationState GetAnimState(Animation anim, string clip_name)
	{
		return anim[clip_name];
	}

	public static void SetAnimTime(Animation anim, string clip_name, float time)
	{
		anim[clip_name].time = time;
	}

	public static void SetAnimSpeed(Animation anim, string clip_name, float speed)
	{
		anim[clip_name].speed = speed;
	}
}
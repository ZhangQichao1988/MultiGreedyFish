using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class BlTypeUtil
{
	public static bool IsTypeOf(object obj, string typeName)
	{
		System.Type t = System.Type.GetType(typeName);
		return obj != null && obj.GetType().Equals(t);
	}

	public static System.Type GetType(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return null;
		}
		System.Type t = System.Type.GetType(s + ",Assembly-CSharp");

		if (t == null)
		{
			t = System.Type.GetType(string.Format("UnityEngine.{0},UnityEngine", s));
		}

		if (t == null)
		{
			t = System.Type.GetType(string.Format("UnityEngine.UI.{0},UnityEngine.UI", s));
		}

		if (t == null)
		{
			t = System.Type.GetType(string.Format("UnityEngine.Timeline.{0},UnityEngine.Timeline", s));
		}

		if (t == null)
		{
			Debug.LogError(string.Format("OzTypeUtil.GetType({0}) is NULL !", s));
		}
		return t;

	}
}

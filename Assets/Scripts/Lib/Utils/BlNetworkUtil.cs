using UnityEngine;

public class BlNetworkUtil
{
	public static bool IsWifi
	{
		get
		{
			return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
		}
	}

	public static bool NetAvailable
	{
		get
		{
			return Application.internetReachability != NetworkReachability.NotReachable;
		}
	}

	public static string GetServiceSlugValue(string host)
	{
		string result = "";
		if (host.LastIndexOf("/") >= 0)
		{
			result = host.Substring(host.LastIndexOf("/") + 1);
		}
		return result;
	}
}

using System;

public static class BlDateTimeUtil
{
	public static DateTime S_TimeSice1970;
	public static bool S_TimeHasSet = false;

	public static double GetCurrentTime()
	{
		if (!S_TimeHasSet)
		{
			S_TimeSice1970 = new DateTime(1970, 1, 1);
			S_TimeSice1970 = S_TimeSice1970.ToLocalTime();
			S_TimeHasSet = true;
		}
		TimeSpan ts = DateTime.Now - S_TimeSice1970;
		return ts.TotalSeconds;
	}

	public static bool IsSameDay(long millisec1, long millisec2)
	{
		DateTime dt1 = new DateTime(millisec1 * TimeSpan.TicksPerMillisecond);
		DateTime dt2 = new DateTime(millisec2 * TimeSpan.TicksPerMillisecond);

		dt1 = dt1.ToLocalTime();
		dt2 = dt2.ToLocalTime();

		return dt1.Year == dt2.Year && dt1.Month == dt2.Month && dt1.Day == dt2.Day;
	}

	public static string GetCurrentDay(long millisec)
	{
		string str = "";
		DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
		DateTime dt = startTime.AddMilliseconds(millisec);
		string lt = dt.ToString("T");
		// DateTime dt = new DateTime((S_TimeSice1970.Millisecond+millisec) * TimeSpan.TicksPerMillisecond);
		// str = dt.Year + "-" + dt.Month + "-" + dt.Day + " " + dt.Hour + ":" + dt.Minute;
		str = dt.Year + "-" + dt.Month + "-" + dt.Day + " " + lt;
		return str;
	}
}

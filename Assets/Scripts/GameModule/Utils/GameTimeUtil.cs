using UnityEngine;
using System;

public class GameTimeUtil
{
    const long MillsPerSecond = 1000;
    static long LocalStartTime = 0; //毫秒
    static long ServerTime = 0;     //毫秒
    

    /// <summary>
    /// 获取服务器时间
    /// </summary>
    /// <returns></returns>
    public static long GetCurServerTime()
    {
        long localTimeElapse = (long)(Time.realtimeSinceStartup * MillsPerSecond) - LocalStartTime;
        return ServerTime + localTimeElapse;
    }


    /// <summary>
    /// 设置服务器时间
    /// </summary>
    /// <param name="serverTime"></param>
    /// <param name="offsetTime"></param>
    public static void SetServerTime(long serverTime)
    {
        ServerTime = serverTime;
        LocalStartTime = (long)Time.realtimeSinceStartup * MillsPerSecond;
    }

    /// <summary>
    /// 本地时间
    /// </summary>
    /// <returns></returns>
    public static long GetCurTimeMillis(){

        return (long)BlDateTimeUtil.GetCurrentTime() * MillsPerSecond;
    }
}
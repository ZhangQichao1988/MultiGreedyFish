using UnityEngine;

public class ClockRecorder
{
    static float currentTime;
    public static void StartRecord()
    {
        currentTime = Time.realtimeSinceStartup;
    }

    public static void GetRecord()
    {
        Debug.LogFormat( "Used Time : {0}", Time.realtimeSinceStartup - currentTime);
        currentTime = Time.realtimeSinceStartup;
    }
}
using UnityEngine;


namespace NetWorkModule
{
    /// <summary>
    /// 時刻管理クラス
    /// </summary>
    public static class Clock
    {
        /// <summary>
        /// サーバとの差分
        /// </summary>
        static double? delta;

        /// <summary>
        /// MilliTimestampの下限
        /// </summary>
        static long? lowerLimit;

        /// <summary>
        /// 推定時刻 (unixtime ミリ秒)
        /// DMHttpApi専用。DMではTimestampを使うこと
        /// </summary>
        public static long? MilliTimestamp
        {
            get
            {
                if (!delta.HasValue || !lowerLimit.HasValue)
                {
                    return null;
                }

                return lowerLimit = System.Math.Max(lowerLimit.Value, (long)((Time.realtimeSinceStartup + delta.Value) * 1000));
            }
        }

        /// <summary>
        /// 推定時刻 (unixtime 秒)
        /// </summary>
        public static long? Timestamp => MilliTimestamp / 1000;

        /// <summary>
        /// 時差を取得する
        /// </summary>
        /// <value>The time difference.</value>
        public static int TimeDifference
        {
            get
            {
                // サマータイムによって時差が変わらないように、常に一定の日付で時差計算を行う
                var localTime = new System.DateTime(2017, 1, 1, 0, 0, 0, System.DateTimeKind.Local);
                var localOffset = new System.DateTimeOffset(localTime).Offset;
                return (localOffset.Hours * 60 * 60) + (localOffset.Minutes * 60) + localOffset.Seconds;
            }
        }

    #if !LLAS_NODEBUG
        /// <summary>
        /// 現在時刻での時差を取得する
        /// </summary>
        /// <value>The time difference.</value>
        public static int LocalTimeDifference
        {
            get
            {
                var localOffset = System.DateTimeOffset.Now.Offset;
                return (localOffset.Hours * 60 * 60) + (localOffset.Minutes * 60) + localOffset.Seconds;
            }
        }
    #endif

        /// <summary>
        /// 受信時に時刻を設定する
        /// </summary>
        /// <param name="responseTimestamp">サーバから受け取った時刻 (ミリ秒)</param>
        /// <param name="sentTime">送信時のTime.realtimeSinceStartup</param>
        public static void SetServerTime(long responseTimestamp, float sentTime)
        {
            Debug.LogWarningFormat("Server TS is {0}, SendTime is {1}", responseTimestamp, sentTime);
            var recvTime = Time.realtimeSinceStartup;
            lowerLimit = (lowerLimit.HasValue ? System.Math.Max(lowerLimit.Value, responseTimestamp) : responseTimestamp) + 1;
            if (delta.HasValue)
            {
                var t = (responseTimestamp * 1e-3) - delta.Value;
                // sentTime <= t <= recvTimeのときは何もしない
                if (t < sentTime)
                {
                    delta = (responseTimestamp * 1e-3) - sentTime;
                }
                else if (t > recvTime)
                {
                    delta = (responseTimestamp * 1e-3) - recvTime;
                }
            }
            else
            {
                delta = (responseTimestamp * 1e-3) - recvTime;
            }
        }

        /// <summary>
        /// サーバのuser_status.last_timestampが未来に行ってしまう場合に備えてlowerLimitを設定する
        /// </summary>
        /// <param name="lastTimestamp">user_status.last_timestamp</param>
        public static void SetLastTimestamp(long lastTimestamp)
        {
            lowerLimit = (lowerLimit.HasValue ? System.Math.Max(lowerLimit.Value, lastTimestamp) : lastTimestamp) + 1;
        }

        /// <summary>
        /// StaticCleaner用
        /// </summary>
        public static void Reset()
        {
            delta = null;
            lowerLimit = null;
        }
    }
}
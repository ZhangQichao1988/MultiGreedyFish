using System;
using UnityEngine;

namespace TimerModule
{
    class ScaleTimer : Timer
    {
        float _delayTime;
        float _endTime = 0;

        public ScaleTimer(int id, float delayTime, Action<System.Object> func, System.Object obj, int repeatCount) : base(id, func, obj, repeatCount)
        {
            _delayTime = delayTime;

            Reset();
        }

        public override bool IsTimeUp()
        {
            return Time.time > _endTime;
        }

        public override void Reset()
        {
            _endTime = Time.time + _delayTime;
        }
    }
}

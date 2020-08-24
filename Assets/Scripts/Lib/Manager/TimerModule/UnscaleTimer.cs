using UnityEngine;
using System;

namespace TimerModule
{
    class UnscaleTimer : Timer
    {
        float _delayTime;
        float _endTime = 0;

        public UnscaleTimer(int id, float delayTime, Action<System.Object> func, System.Object obj, int repeatCount) : base(id, func, obj, repeatCount)
        {
            _delayTime = delayTime;
            Reset();
        }

        public override bool IsTimeUp()
        {
            return Time.unscaledTime > _endTime;
        }

        public override void Reset()
        {
            _endTime = Time.unscaledTime + _delayTime;
        }
    }

}

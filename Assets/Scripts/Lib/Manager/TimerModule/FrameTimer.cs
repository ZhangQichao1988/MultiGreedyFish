using UnityEngine;
using System;

namespace TimerModule
{
    class FrameTimer : Timer
    {
        int _delayFrame;
        int _endFrame = 0;

        public FrameTimer(int id, int delayFrame, Action<System.Object> func, System.Object obj, int repeatCount) : base(id, func, obj, repeatCount)
        {
            _delayFrame = delayFrame;
            Reset();
        }

        public override bool IsTimeUp()
        {
            return Time.frameCount >= _endFrame;
        }

        public override void Reset()
        {
            _endFrame = Time.frameCount + _delayFrame;
        }
    }

}

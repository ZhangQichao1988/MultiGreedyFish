using System;

namespace TimerModule
{
    abstract class Timer : IDisposable
    {
        public Timer(int id, Action<Object> func, Object obj, int repeatCount)
        {
            _id = id;
            _valid = true;

            _Function = func;
            _object = obj;

            _repeatCount = repeatCount;
        }

        public abstract bool IsTimeUp();

        public abstract void Reset();

        public void Execute()
        {
            if (_repeatCount > 0)
            {
                _repeatCount--;
            }

            if (_Function != null)
            {
                if (_object != null)
                {
                    _Function.Invoke(_object);
                }
                else
                {
                    _Function.Invoke(null);
                }
            }
        }

        public void Kill()
        {
            _valid = false;
        }

        public void Dispose()
        {
            _Function = null;
            _object = null;
        }

        public int ID
        {
            get
            {
                return _id;
            }

        }
        int _id;

        public bool Repeat
        {
            get
            {
                return _repeatCount > 0 || _repeatCount == -1;
            }
        }
        protected int _repeatCount;

        public bool Valid
        {
            get
            {
                return _valid;
            }
        }
        protected bool _valid;

        protected Action<Object> _Function;
        protected Object _object;
    }

}

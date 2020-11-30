using System.Collections.Generic;
using System;
using UnityEngine;

namespace TimerModule
{
    enum eTimerType
    {
        ScaleTime = 0,
        UnscaleTime = 1,
        RealTime = 2,
        FrameCount = 3,
    }

    /// <summary>
    /// 计时器
    /// </summary>
    public class TimerManager : MonoBehaviour
    {
        private static TimerManager Instance { get; set; }

        private LinkedList<Timer> m_LinkedTimers;
        private Dictionary<int, Timer> m_DictTimers;

        private int m_TimerID;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            m_TimerID = 0;

            m_LinkedTimers = new LinkedList<Timer>();
            m_DictTimers = new Dictionary<int, Timer>(1024);
        }

        private void OnDestroy()
        {
            m_LinkedTimers.Clear();
            m_DictTimers.Clear();

            m_TimerID = 0;

            if (Instance != null)
            {
                Instance = null;
            }
        }

        private void Update()
        {
            LinkedListNode<Timer> currentNode = m_LinkedTimers.First;

            while (currentNode != null)
            {
                Timer timer = currentNode.Value;

                LinkedListNode<Timer> next = currentNode.Next;

                if (timer.Valid)
                {
                    if (timer.IsTimeUp())
                    {
                        timer.Execute();

                        if (timer.Repeat)
                        {
                            timer.Reset();
                        }
                        else
                        {
                            m_LinkedTimers.Remove(currentNode);

                            m_DictTimers.Remove(timer.ID);

                            timer.Dispose();
                        }
                    }
                }
                else
                {
                    m_LinkedTimers.Remove(currentNode);

                    m_DictTimers.Remove(timer.ID);

                    timer.Dispose();
                }
                currentNode = next;
            }
        }

        /// <summary>
        /// 添加计时器
        /// </summary>
        /// <param name="timerType">计时器类型: eTimerType.ScaleTime, eTimerType.UnscaleTime, eTimerType.RealTime, eTimerType.FrameCount</param>
        /// <param name="delayTime">延迟时间或帧数</param>
        /// <param name="func">lua回调函数</param>
        /// <param name="obj">self</param>
        /// <param name="repeatCount">重复次数，默认0不重复</param>
        /// <returns>计时器ID</returns>
        private int AddTimerInternal(int timerType, float delayTime, Action<System.Object> func, System.Object obj, int repeatCount = 0)
        {
            m_TimerID++;

            Timer timer;

            switch ((eTimerType)timerType)
            {
                case eTimerType.ScaleTime:
                    timer = new ScaleTimer(m_TimerID, delayTime, func, obj, repeatCount);
                    break;
                case eTimerType.UnscaleTime:
                    timer = new UnscaleTimer(m_TimerID, delayTime, func, obj, repeatCount);
                    break;
                case eTimerType.RealTime:
                    timer = new RealTimer(m_TimerID, delayTime, func, obj, repeatCount);
                    break;
                case eTimerType.FrameCount:
                    timer = new FrameTimer(m_TimerID, (int)delayTime, func, obj, repeatCount);
                    break;
                default:
                    m_TimerID--;
                    return -1;
            }
            m_LinkedTimers.AddLast(timer);
            m_DictTimers.Add(m_TimerID, timer);

            return m_TimerID;
        }

        private void RemoveTimerInternal(int timerID)
        {
            if (m_DictTimers.TryGetValue(timerID, out Timer timer))
            {
                timer.Kill();
            }
        }

        public static int AddTimer(int timerType, float delayTime, Action<System.Object> func, System.Object obj, int repeatCount = 0)
        {
            if (Instance != null)
            {
                return Instance.AddTimerInternal(timerType, delayTime, func, obj, repeatCount);
            }
            return -1;
        }

        public static void RemoveTimer(int timerID)
        {
            if (Instance != null)
            {
                Instance.RemoveTimerInternal(timerID);
            }
        }
    }
}
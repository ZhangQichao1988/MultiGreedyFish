using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 时间收发器
/// </summary>
public class EventDispatch 
{
    public delegate void Callback();
    public delegate void Callback<T>(T arg1);
    public delegate void Callback<T, U>(T arg1, U arg2);
    public delegate void Callback<T, U, V>(T arg1, U arg2, V arg3);
    public delegate void Callback<T, U, V, X>(T arg1, U arg2, V arg3, X arg4);
    public delegate void Callback<T, U, V, X, Y>(T arg1, U arg2, V arg3, X arg4, Y arg5);

    public Dictionary<string, Delegate> mEventTable = new Dictionary<string, Delegate>();

    public void PrstringEventTable()
    {
        foreach (KeyValuePair<string, Delegate> pair in mEventTable)
        {
            Debug.Log("\t\t\t" + pair.Key + "\t\t" + pair.Value);
        }

        Debug.Log("\n");
    }

    void OnListenerAdding(string eventType, Delegate listenerBeingAdded)
    {
        if (!mEventTable.ContainsKey(eventType))
        {
            mEventTable.Add(eventType, null);
        }
        Delegate d = mEventTable[eventType];
        if (d != null && d.GetType() != listenerBeingAdded.GetType())
        {
            throw new ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
        }
    }

    bool OnListenerRemoving(string eventType, Delegate listenerBeingRemoved)
    {
        if (mEventTable.ContainsKey(eventType))
        {
            Delegate d = mEventTable[eventType];
            if (d == null)
            {
                //throw new ListenerException(string.Format("Attempting to remove listener with for event type \"{0}\" but current listener is null.", eventType));
                return false;
            }
            else if (d.GetType() != listenerBeingRemoved.GetType())
            {
                //throw new ListenerException(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
                return false;
            }
        }
        else
        {
            //throw new ListenerException(string.Format("Attempting to remove listener for type \"{0}\" but Messenger doesn't know about this event type.", eventType));
            return false;
        }
        return true;
    }

    void OnListenerRemoved(string eventType)
    {
        if (mEventTable[eventType] == null)
        {
            mEventTable.Remove(eventType);
        }
    }

    void OnDispatch(string eventType)
    {

    }

    DispatchException CreateDispatchSignatureException(string eventType)
    {
        return new DispatchException(string.Format("Broadcasting message \"{0}\" but listeners have a different signature than the broadcaster.", eventType));
    }

    public class DispatchException : Exception
    {
        public DispatchException(string msg)
        : base(msg)
        {
        }
    }

    public class ListenerException : Exception
    {
        public ListenerException(string msg)
        : base(msg)
        {
        }
    }

    //No parameters
    public void AddListener(string eventType, Callback handler)
    {
        OnListenerAdding(eventType, handler);
        mEventTable[eventType] = (Callback)mEventTable[eventType] + handler;
    }

    //Single parameter
    public void AddListener<T>(string eventType, Callback<T> handler)
    {
        OnListenerAdding(eventType, handler);
        mEventTable[eventType] = (Callback<T>)mEventTable[eventType] + handler;
    }
    //Two parameters
    public void AddListener<T, U>(string eventType, Callback<T, U> handler)
    {
        OnListenerAdding(eventType, handler);
        mEventTable[eventType] = (Callback<T, U>)mEventTable[eventType] + handler;
    }

    //Three parameters
    public void AddListener<T, U, V>(string eventType, Callback<T, U, V> handler)
    {
        OnListenerAdding(eventType, handler);
        mEventTable[eventType] = (Callback<T, U, V>)mEventTable[eventType] + handler;
    }

    //Four parameters
    public void AddListener<T, U, V, X>(string eventType, Callback<T, U, V, X> handler)
    {
        OnListenerAdding(eventType, handler);
        mEventTable[eventType] = (Callback<T, U, V, X>)mEventTable[eventType] + handler;
    }
    //five parameters
    public void AddListener<T, U, V, X,Y>(string eventType, Callback<T, U, V, X,Y> handler)
    {
        OnListenerAdding(eventType, handler);
        mEventTable[eventType] = (Callback<T, U, V, X,Y>)mEventTable[eventType] + handler;
    }


    public void RemoveListener(string eventType)
    {
        if (mEventTable.ContainsKey(eventType))
        {
            mEventTable[eventType] = null;
            OnListenerRemoved(eventType);
        }
    }

    //No parameters
    public void RemoveListener(string eventType, Callback handler)
    {
        if (OnListenerRemoving(eventType, handler))
        {
            mEventTable[eventType] = (Callback)mEventTable[eventType] - handler;
            OnListenerRemoved(eventType);
        }
    }
    //Single parameter
    public void RemoveListener<T>(string eventType, Callback<T> handler)
    {
        if (OnListenerRemoving(eventType, handler))
        {
            mEventTable[eventType] = (Callback<T>)mEventTable[eventType] - handler;
            OnListenerRemoved(eventType);
        }
    }

    //Two parameters
    public void RemoveListener<T, U>(string eventType, Callback<T, U> handler)
    {
        if (OnListenerRemoving(eventType, handler))
        {
            OnListenerRemoving(eventType, handler);
            mEventTable[eventType] = (Callback<T, U>)mEventTable[eventType] - handler;
            OnListenerRemoved(eventType);
        }
    }

    //Three parameters
    public void RemoveListener<T, U, V>(string eventType, Callback<T, U, V> handler)
    {
        if (OnListenerRemoving(eventType, handler))
        {
            OnListenerRemoving(eventType, handler);
            mEventTable[eventType] = (Callback<T, U, V>)mEventTable[eventType] - handler;
            OnListenerRemoved(eventType);
        }
    }

    //Four parameters
    public void RemoveListener<T, U, V, X>(string eventType, Callback<T, U, V, X> handler)
    {
        if (OnListenerRemoving(eventType, handler))
        {
            OnListenerRemoving(eventType, handler);
            mEventTable[eventType] = (Callback<T, U, V, X>)mEventTable[eventType] - handler;
            OnListenerRemoved(eventType);
        }
    }
    //Five parameters
    public void RemoveListener<T, U, V, X,Y>(string eventType, Callback<T, U, V, X,Y> handler)
    {
        if (OnListenerRemoving(eventType, handler))
        {
            OnListenerRemoving(eventType, handler);
            mEventTable[eventType] = (Callback<T, U, V, X, Y>)mEventTable[eventType] - handler;
            OnListenerRemoved(eventType);
        }
    }

    public void RemoveAll()
    {
        mEventTable.Clear();
    }

    //No parameters
    public void Dispatch(string eventType)
    {
        OnDispatch(eventType);
        Delegate d;
        if (mEventTable.TryGetValue(eventType, out d))
        {
            Callback callback = d as Callback;

            if (callback != null)
            {
                callback();
            }
            else
            {
                throw CreateDispatchSignatureException(eventType);
            }
        }
    }
    
    //Single parameter
    public void Dispatch<T>(string eventType, T arg1)
    {
        OnDispatch(eventType);
        Delegate d;
        if (mEventTable.TryGetValue(eventType, out d))
        {
            Callback<T> callback = d as Callback<T>;

            if (callback != null)
            {
                callback(arg1);
            }
            else
            {
                throw CreateDispatchSignatureException(eventType);
            }
        }
    }

    //Two parameters
    public void Dispatch<T, U>(string eventType, T arg1, U arg2)
    {
        OnDispatch(eventType);
        Delegate d;
        if (mEventTable.TryGetValue(eventType, out d))
        {
            Callback<T, U> callback = d as Callback<T, U>;

            if (callback != null)
            {
                callback(arg1, arg2);
            }
            else
            {
                throw CreateDispatchSignatureException(eventType);
            }
        }
    }

    //Three parameters
    public void Dispatch<T, U, V>(string eventType, T arg1, U arg2, V arg3)
    {
        OnDispatch(eventType);
        Delegate d;
        if (mEventTable.TryGetValue(eventType, out d))
        {
            Callback<T, U, V> callback = d as Callback<T, U, V>;

            if (callback != null)
            {
                callback(arg1, arg2, arg3);
            }
            else
            {
                throw CreateDispatchSignatureException(eventType);
            }
        }
    }

    //Four parameters
    public void Dispatch<T, U, V, X>(string eventType, T arg1, U arg2, V arg3, X arg4)
    {
        OnDispatch(eventType);
        Delegate d;
        if (mEventTable.TryGetValue(eventType, out d))
        {
            Callback<T, U, V, X> callback = d as Callback<T, U, V, X>;

            if (callback != null)
            {
                callback(arg1, arg2, arg3, arg4);
            }
            else
            {
                throw CreateDispatchSignatureException(eventType);
            }
        }
    }
    //Five
    public void Dispatch<T, U, V, X,Y>(string eventType, T arg1, U arg2, V arg3, X arg4,Y arg5)
    {
        OnDispatch(eventType);
        Delegate d;
        if (mEventTable.TryGetValue(eventType, out d))
        {
            Callback<T, U, V, X,Y> callback = d as Callback<T, U, V, X,Y>;

            if (callback != null)
            {
                callback(arg1, arg2, arg3, arg4,arg5);
            }
            else
            {
                throw CreateDispatchSignatureException(eventType);
            }
        }
    }
}
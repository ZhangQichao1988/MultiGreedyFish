using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventTrigger : MonoBehaviour
{
    [SerializeField]
    protected List<UIEvent> triggers = new List<UIEvent>();

    protected void Trigger(BaseEventData eventData)
    {
        if (triggers != null)
        {
            for (int i = 0; i < triggers.Count; ++i)
            {
                UIEvent triggerEvent = triggers[i];

                if (triggerEvent != null)
                {
                    triggerEvent.Invoke(eventData);
                }
            }
        }
    }

    public void Add(UIEvent triggerEvent)
    {
        if(triggers != null)
        {
            triggers.Add(triggerEvent);
        }
    }

    public bool Remove(UIEvent triggerEvent)
    {
        if (triggers != null)
        {
            return triggers.Remove(triggerEvent);
        }
        return false;
    }

    public int RemoveAll(Predicate<UIEvent> match)
    {
        if (triggers != null)
        {
            return triggers.RemoveAll(match);
        }
        return 0;
    }
}
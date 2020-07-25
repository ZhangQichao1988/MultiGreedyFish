using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerLongPressTrigger : EventTrigger, IPointerDownHandler, IPointerUpHandler
{
    /// <summary>
    /// 长按多久后开始
    /// </summary>
    public float StartAfter = 0.5f;//1.0f;

    Coroutine _coroutine;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        _coroutine = StartCoroutine(OnPointerLongPress(eventData));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
        int clickCount = eventData.clickCount;
        eventData.clickCount = 0;
        Trigger(eventData);
        eventData.clickCount = clickCount;
    }

    public virtual IEnumerator OnPointerLongPress(PointerEventData eventData)
    {
        yield return new WaitForSeconds(StartAfter);

        int clickCount = eventData.clickCount;
        eventData.clickCount = 1;
        Trigger(eventData);
        eventData.clickCount = clickCount;
    }
}
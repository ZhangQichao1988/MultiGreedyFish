using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class JoyStick : ScrollRect
{
    private float mRadius;
    public System.Action<RectTransform> JoystickMoveHandle;
    public System.Action<RectTransform> JoystickEndHandle;
    
    protected override void Start()
    {
        mRadius = this.GetComponent<RectTransform>().sizeDelta.x * 0.5f;
        this.content.gameObject.SetActive(false);
    }
    
    public override void OnDrag(PointerEventData eventData)
    {  
        base.OnDrag(eventData);
        this.content.gameObject.SetActive(true);
        //虚拟摇杆移动
        var contentPostion = this.content.anchoredPosition;        
        if (contentPostion.magnitude > mRadius)
        {
            contentPostion = contentPostion.normalized * mRadius;
            SetContentAnchoredPosition(contentPostion);
        }
        //旋转
        if (content.anchoredPosition.y != 0)
        {
            content.eulerAngles = new Vector3(0, 0, Vector3.Angle(Vector3.right, content.anchoredPosition) * content.anchoredPosition.y / Mathf.Abs(content.anchoredPosition.y) - 90);
        }
    }
    private void FixedUpdate()
    {
        if (this.content.gameObject.activeInHierarchy)
        {
            if (JoystickMoveHandle != null)
            {
                JoystickMoveHandle(this.content);
            }
        }
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        this.content.gameObject.SetActive(false);
        if (JoystickEndHandle != null)
        {
            JoystickEndHandle(this.content);
        }
    }
}
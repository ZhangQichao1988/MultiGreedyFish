using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using UnityEngine.EventSystems;
using System;

public class DropdownPlus : Dropdown
{
    public UnityAction onClickFrame;
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        //onClickFrame();
    }
    public override void OnCancel(BaseEventData eventData)
    {
        base.OnCancel(eventData);
        onClickFrame();
    }
    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);
        onClickFrame();
    }
    protected override GameObject CreateBlocker(Canvas rootCanvas)
    {
        GameObject go = base.CreateBlocker(rootCanvas);
        var btn = go.GetComponent<Button>();
        btn.onClick.AddListener(onClickFrame);
        return go;
    }
}

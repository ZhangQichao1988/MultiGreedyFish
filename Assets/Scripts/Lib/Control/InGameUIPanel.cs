using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class InGameUIPanel : MonoBehaviour
{
    

    public Transform TouchTF;
    public Transform TouchStickTF;
    public CameraFollow cameraFollow;
    private PlayerBase Player;
    private RectTransform SelfRectTF;
    private float MaxLength = 120;
    // Start is called before the first frame update
    void Start()
    {
        SelfRectTF = GetComponent<RectTransform>();
        TouchTF.gameObject.SetActive(false);

        //GameObject go = Wrapper.CreateGameObject(new GameObject(), transform, "Player");
        Player = ManagerGroup.GetInstance().fishManager.CreatePlayer();
        cameraFollow.Target = Player.transform;
    }



    public void TouchDown(BaseEventData data)
    {
        TouchTF.gameObject.SetActive(true);
        TouchTF.localPosition = GetUIPos(((PointerEventData)data).position);
        TouchStickTF.localPosition = Vector3.zero;
        if (Player != null)
        {
            Player.TouchDown(data);
        }
    }
    public void TouchUp(BaseEventData data)
    {
        TouchTF.gameObject.SetActive(false);
        if (Player != null)
        {
            Player.TouchUp(data);
        }
        //curDir = Dir;
        //Dir = Vector3.zero;
        //moveDir = Vector3.zero;
    }
    public void TouchDrag(BaseEventData data)
    {
        Vector2 pos = GetUIPos(((PointerEventData)data).position) - (Vector2)TouchTF.localPosition;
        float length = pos.magnitude;
        if (length > MaxLength)
        {
            pos = pos * MaxLength / length;
        }
        TouchStickTF.localPosition = pos;
        if (Player != null) 
        {
            Player.TouchDrag(data, pos, MaxLength);
        }
    }

    public Vector2 GetUIPos(Vector2 pos)
    {
        pos.x = (pos.x / Screen.width - 0.5f) * SelfRectTF.rect.width;
        pos.y = (pos.y / Screen.height - 0.5f) * SelfRectTF.rect.height;
        return pos;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class InGameUIPanel : MonoBehaviour
{
    public Transform TouchTF;
    public Transform TouchStickTF;
    public Transform Character;
    public Animator ChAnimator;
    private RectTransform SelfRectTF;
    private float MaxLength = 60;
    // Start is called before the first frame update
    void Start()
    {
        SelfRectTF = GetComponent<RectTransform>();
        TouchTF.gameObject.SetActive(false);
    }

    // Update is called once per frame


    public void TouchDown(BaseEventData data)
    {
        TouchTF.gameObject.SetActive(true);
        TouchTF.localPosition = GetUIPos(((PointerEventData)data).position);
        TouchStickTF.localPosition = Vector3.zero;
        Dir = Vector3.zero;
    }
    public void TouchUp(BaseEventData data)
    {
        TouchTF.gameObject.SetActive(false);
        curDir = Dir;
        Dir = Vector3.zero;
        moveDir = Vector3.zero;
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
        Dir = pos / MaxLength;
        Dir.z = 0;
        //Dir.y = 0;
        if (Dir.x != 0 || Dir.z != 0)
        {
            Dir = (0.9f * Dir.sqrMagnitude + 0.101f) * Dir.normalized;
            if (curDir.sqrMagnitude < 0.001f)
                curDir = Character.forward;
        }
    }

    public Vector2 GetUIPos(Vector2 pos)
    {
        pos.x = (pos.x / Screen.width - 0.5f) * SelfRectTF.rect.width;
        pos.y = (pos.y / Screen.height - 0.5f) * SelfRectTF.rect.height;
        return pos;
    }
    Vector3 Dir;

    Vector3 curDir;
    Vector3 moveDir;
    Vector3 pos;
    public void Update()
    {
        pos = Character.position;
        if (Dir.sqrMagnitude > 0)
        {
            float angle = Vector3.Angle(curDir.normalized, Dir.normalized);
            curDir = Vector3.Slerp(curDir.normalized, Dir.normalized, 520 / angle * Time.deltaTime);
            moveDir = Vector3.Lerp(moveDir, Dir, 360 / angle * Time.deltaTime);
            pos += moveDir * 10 * Time.deltaTime;
            pos = ObstacleGrid.ObstacleClamp(pos, moveDir);
            ChAnimator.SetFloat("Speed", Mathf.Clamp(moveDir.magnitude, 0.101f, 1f));
        }
        else
        {
            curDir = Vector3.Lerp(curDir, Dir, 5 * Time.deltaTime);
            if (curDir.sqrMagnitude > 0.001f)
            {
                pos += curDir * 10 * Time.deltaTime;
                pos = ObstacleGrid.ObstacleClamp(pos, curDir);
            }
            ChAnimator.SetFloat("Speed", curDir.magnitude);
        }
        if (curDir.sqrMagnitude > 0.001f)
            Character.rotation = Quaternion.LookRotation(curDir);
        Character.position = pos;
    }
}

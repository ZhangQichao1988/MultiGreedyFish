using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class InGameUIPanel : MonoBehaviour
{
    public float curveWorldStrength = 0f;

    public RectTransform TouchTF;
    public Transform TouchStickTF;
    public CameraFollow cameraFollow;
    public Image skillGuage;
    public Button skillBtn;
    private PlayerBase Player;
    private RectTransform SelfRectTF;
    private float MaxLength = 120;
    // Start is called before the first frame update
    void Start()
    {
        SelfRectTF = GetComponent<RectTransform>();
        TouchTF.anchoredPosition = new Vector2(200f, 200f);
        TouchStickTF.transform.localPosition = Vector3.zero;
        //TouchTF.gameObject.SetActive(false);
    }

	public void Init()
	{
        Player = ManagerGroup.GetInstance().fishManager.CreatePlayer();
        cameraFollow.Target = Player.transform;
    }

	public void TouchDown(BaseEventData data)
    {
        //TouchTF.gameObject.SetActive(true);
        TouchTF.position = ((PointerEventData)data).position;
        TouchStickTF.localPosition = Vector3.zero;
        if (Player != null)
        {
            Player.TouchDown(data);
        }
    }
    public void TouchUp(BaseEventData data)
    {
        TouchTF.anchoredPosition = new Vector2(200f, 200f);
        TouchStickTF.transform.localPosition = Vector3.zero;
        //TouchTF.gameObject.SetActive(false);
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
        Vector2 pos = ((PointerEventData)data).position - (Vector2)TouchTF.position;
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

	public void Update()
	{
        if (!Player) { return; }
        // TODO:不要弯曲的话可以删除
        Shader.SetGlobalVector("_WorldSpacePlayerPos", Player.transform.position);
        Shader.SetGlobalFloat("_CurveWorldStrength", curveWorldStrength);

        if (Player.actionStep == FishBase.ActionType.Die)
        {
            ManagerGroup.GetInstance().GotoResult(ManagerGroup.GetInstance().fishManager.GetAlivePlayer().Count + 1);
            var listPlayer = ManagerGroup.GetInstance().fishManager.GetAlivePlayerSort(cameraFollow.Target.position);
            cameraFollow.SetTarget(listPlayer[0].transform);
        }
        else if (ManagerGroup.GetInstance().fishManager.GetAlivePlayer().Count <= 1 && !GameConst.FreeMode)
        {
            ManagerGroup.GetInstance().GotoResult(1);
        }
        else if (ManagerGroup.GetInstance().fishManager.GetAlivePlayer().Count == 2)
        {
            ManagerGroup.GetInstance().SetPlayPoint();
        }

        skillGuage.fillAmount = Player.fishSkill.currentGauge;
        skillBtn.interactable = Player.fishSkill.currentGauge >= 1f;
    }
    public void PlayerRunSkill()
    {
        Player.fishSkill.RunSkill();
    }
}

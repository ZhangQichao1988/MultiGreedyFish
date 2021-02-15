using System;
using System.IO;
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
    public Image skillGauge;

    static readonly float miniMapPointSize = 5f;
    public RectTransform transMiniMap, transPlayerPoint;
    private float miniMapSize;
    //public GameObject goSkillBtnDisable;
    //public GameObject goSkillBtnEnable;

    public int alivePlayerNum;
    private Animator animator;
    private PlayerBase Player;
    private RectTransform SelfRectTF;
    private float MaxLength = 60;
    // Start is called before the first frame update
    void Start()
    {
        SelfRectTF = GetComponent<RectTransform>();
        TouchTF.anchoredPosition = new Vector2(200f, 200f);
        TouchStickTF.transform.localPosition = Vector3.zero;
        animator = GetComponent<Animator>();
        animator.SetBool("skill_enable", false);

        miniMapSize = transMiniMap.sizeDelta.x / 2 - miniMapPointSize;
    }

    public void Init()
	{
        Player = BattleManagerGroup.GetInstance().fishManager.CreatePlayer();
        cameraFollow.Target = Player.transform;
        ApplyMiniMap();
    }
    private void ApplyMiniMap()
    {
        Vector3 myPos = Player.transform.position;
        transPlayerPoint.anchoredPosition = new Vector2(myPos.x / BattleConst.instance.BgBound * miniMapSize,
                                                                                    myPos.z / BattleConst.instance.BgBound * miniMapSize);
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
        Vector2 posStick = pos;
        float length = pos.magnitude;
        if (length > MaxLength)
        {
            pos = pos * MaxLength / length;
        }

        // 虚位
        if (length > MaxLength + 15)
        {
            posStick = posStick * (MaxLength+15) / length;
        }
        TouchStickTF.localPosition = posStick;

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
#if CONSOLE_ENABLE
    public void KillEnemy()
    {
        var listPlayer = BattleManagerGroup.GetInstance().fishManager.GetAlivePlayer();
        if (listPlayer.Count > 1)
        {
            foreach (var note in listPlayer)
            {
                if (note.fishType == FishBase.FishType.PlayerRobot)
                {
                    if (note.Damage(999999, null))
                    {
                        break;
                    }
                }
            }
        }
    }
    public void KillSelf()
    {
        Player.Damage(999999, null);
    }
#endif

    public void Update()
	{
#if UNITY_EDITOR && CONSOLE_ENABLE
        // 自杀
        if (Input.GetKey(KeyCode.D))
        {
            KillSelf();
        }
        // 随机杀死一个机器人
        if (Input.GetKeyDown(KeyCode.E))
        {
            KillEnemy();
        }
#endif

        if (!Player) { return; }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.A))
        {
            Player.fishSkill.currentGauge = 1f;
            PlayerRunSkill();
        }

#endif

        skillGauge.fillAmount = Player.fishSkill.currentGauge;
        animator.SetBool("skill_enable", Player.fishSkill.currentGauge >= 1f);

        ApplyMiniMap();
    }
    public void PlayerRunSkill()
    {
        Player.fishSkill.RunSkill();
    }

    public void CheckBattleEnd()
    {
        alivePlayerNum = BattleManagerGroup.GetInstance().fishManager.GetAlivePlayer().Count;
        if (alivePlayerNum == 1)
        {
            BattleManagerGroup.GetInstance().BattleEnd();
        }

        if (Player.actionStep == FishBase.ActionType.Die)
        {
            BattleManagerGroup.GetInstance().GotoResult(alivePlayerNum + 1);
            var listPlayer = BattleManagerGroup.GetInstance().fishManager.GetAlivePlayerSort(cameraFollow.Target.position);
            if (listPlayer[0].transform != cameraFollow.Target)
            {
                cameraFollow.SetTarget(listPlayer[0].transform);
            }
        }
        else if (alivePlayerNum <= 1 && !BattleConst.instance.FreeMode)
        {
            BattleManagerGroup.GetInstance().GotoResult(1);
        }
        else if (alivePlayerNum == 2)
        {
            BattleManagerGroup.GetInstance().SetPlayPoint();
        }

    }

}

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
        Player = BattleManagerGroup.GetInstance().fishManager.CreatePlayer();
        Image image = skillBtn.GetComponent<Image>();
        string path = string.Format(AssetPathConst.skillIconPath, Player.fishSkill.baseData.skillType);
        var asset = ResourceManager.LoadSync( path, typeof(Sprite));
        image.sprite = asset.Asset as Sprite;
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
#if UNITY_EDITOR
        // 自杀
        if (Input.GetKey(KeyCode.D))
        {
            Player.Damage(999999, null);
        }
        // 随机杀死一个机器人
        if (Input.GetKeyDown(KeyCode.E))
        {
            var listPlayer = BattleManagerGroup.GetInstance().fishManager.GetAlivePlayer();
            if (listPlayer.Count > 1)
            {
                foreach (var note in listPlayer)
                {
                    if (note.fishType == FishBase.FishType.PlayerRobot)
                    {
                        note.Damage(999999, null);
                        break;
                    }
                }
            }
        }
#endif

        if (!Player) { return; }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.A))
        {
            PlayerRunSkill();
        }

        
#endif

        skillGuage.fillAmount = Player.fishSkill.currentGauge;
        skillBtn.interactable = Player.fishSkill.currentGauge >= 1f;
    }
    public void PlayerRunSkill()
    {
        Player.fishSkill.RunSkill();
    }

    public void CheckBattleEnd()
    {
        int alivePlayerNum = BattleManagerGroup.GetInstance().fishManager.GetAlivePlayer().Count;
        if (alivePlayerNum == 1)
        {
            BattleManagerGroup.GetInstance().BattleEnd();
        }

        if (Player.actionStep == FishBase.ActionType.Die)
        {
            BattleManagerGroup.GetInstance().GotoResult(alivePlayerNum + 1);
            var listPlayer = BattleManagerGroup.GetInstance().fishManager.GetAlivePlayerSort(cameraFollow.Target.position);
            cameraFollow.SetTarget(listPlayer[0].transform);
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

using UnityEngine;
using System.Collections.Generic;
using Google.Protobuf;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using System;

public class UIPopupMissionComplete : UIBase
{
    static public UIPopupMissionComplete Instance;
    //static readonly float ShowingTime = ;
    public Text textBody;
    Animator animator;

    bool canShowNext = true;
    float remainingTime = 0f;
    List<PBMission> pBMissions = new List<PBMission>();

    protected override void Awake()
    {
        Instance = this;
        base.Awake();
        animator = GetComponent<Animator>();
    }
    public void AddCompleteMission(PBMission pBMssion)
    {
        pBMissions.Add(pBMssion);
    }

    private void Update()
    {
        if (pBMissions.Count <= 0) { return; }
        if (!canShowNext) { return; }
        var actionData = MissionActionDataTableProxy.Instance.GetDataById(pBMissions[0].ActionId);
        textBody.text = string.Format(LanguageDataTableProxy.GetText(actionData.desc), pBMissions[0].Trigger);
        pBMissions.RemoveAt(0);
        animator.SetTrigger("Show");
        canShowNext = false;
    }
    public void ShowNext()
    {
        canShowNext = true;
    }
}
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagerGroup : MonoBehaviour
{

    enum GameStep 
    {
        None,
        Title,
        Battle,
        Result,
    }
    static ManagerGroup instance = null;

    public InGameUIPanel inGameUIPanel = null;
    public FishManager fishManager = null;
    public AquaticManager aquaticManager = null;
    public ShellManager shellManager = null;
    public PoisonRing poisonRing = null;
    public CameraFollow cameraFollow = null;

    public Text resultText = null;

    Animator animator = null;
    private bool isPause = false;
    private void Awake()
	{
        instance = this;
        animator = GetComponent<Animator>();

    }
    static public ManagerGroup GetInstance()
    { return instance; }

    public void GotoBattle()
    {
        //resultRoot.SetActive(false);
        //titleBtn.SetActive(false);
        //battleControl.SetActive(true) ;
        animator.SetTrigger("ReadyStart");

        poisonRing.gameObject.SetActive(true);
        poisonRing.Init();
        cameraFollow.Init();

        fishManager.Clean();
        inGameUIPanel.Init();
        fishManager.CreateEnemy();
        this.isPause = true;
    }
    public void SetPlayPoint()
    {
        animator.SetTrigger("PlayPoint");
    }
    public void GotoResult(int rank)
    {
        if (rank == 1)
        {
            this.isPause = true;
            animator.SetTrigger("Win");
        }
        else
        {
            animator.SetTrigger("Lose");
        }
        //resultRoot.SetActive(true);
        //battleControl.SetActive(false);
        resultText.text = string.Format( LanguageData.GetText("ResultText"), rank.ToString() );
    }

    public void SetBattleStart()
    {
        this.isPause = false;
    }

	private void Update()
	{
        if (isPause) { return; }
        fishManager.CustomUpdate();
        poisonRing.CustomUpdate();
    }
}

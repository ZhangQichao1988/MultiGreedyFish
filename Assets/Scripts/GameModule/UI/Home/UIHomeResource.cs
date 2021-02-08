using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHomeResource : UIBase
{
    static public UIHomeResource Instance{ private set; get; }
    public GameObject goGold;
    public Image imgGold;
    public Text textGold;

    public GameObject goDiamond;
    public Text textDiamond;

    private Animator animator;
    private AudioSource audioSource;

    bool isGoldAddCalc = false;
    float goldAddCalcRemainingTime = 0f;
    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        animator = GetComponent<Animator>();
    }
    public void SetActiveScene(string sceneName)
    {
        switch (sceneName)
        {
            case "Home":
            case "Shop/Shop":
            case "RankBonus/RankBonus":
            case "PlayerRanking/PlayerRanking":
            case "Mission/Mission":
                goGold.SetActive(true);
                goDiamond.SetActive(true);
                break;
            case "FishEditor":
            case "FishStatus":
                goGold.SetActive(true);
                goDiamond.SetActive(false);
                break;
            case "BattleResult":
            case "Option":
                goGold.SetActive(false);
                goDiamond.SetActive(false);
                break;
        }
        Apply();
    }

    private void Apply()
    {
        textGold.text = (PlayerModel.Instance.player.Gold - PlayerModel.Instance.gainGold).ToString();
        textDiamond.text = PlayerModel.Instance.player.Diamond.ToString();
    }

    public void UpdateAssets()
    {
        Apply();
    }
    public void StartGoldAddCalc()
    {
        animator.SetTrigger("GoldAdd");
        if (isGoldAddCalc) { return; }
        isGoldAddCalc = true;
        goldAddCalcRemainingTime = 1f;
        audioSource = SoundManager.PlaySE(1002);
    }
    private void Update()
    {
        if (isGoldAddCalc)
        {
            goldAddCalcRemainingTime -= Time.deltaTime;
            if (goldAddCalcRemainingTime < 0f)
            {
                goldAddCalcRemainingTime = 0f;
                isGoldAddCalc = false;
                audioSource.Stop();
            }

            PlayerModel.Instance.gainGold = (int)Mathf.Lerp(PlayerModel.Instance.gainGold, 0, 1f - goldAddCalcRemainingTime);
            textGold.text = (PlayerModel.Instance.player.Gold - PlayerModel.Instance.gainGold).ToString();
            
        }
    }
}

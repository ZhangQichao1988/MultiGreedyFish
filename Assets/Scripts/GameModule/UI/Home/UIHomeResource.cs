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

    bool isGoldAddCalc = false;
    float goldAddCalcRemainingTime = 0f;
    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }
    public void SetActiveScene(string sceneName)
    {
        switch (sceneName)
        {
            case "Home":
            case "Shop":
                goGold.SetActive(true);
                goDiamond.SetActive(true);
                break;
            case "FishEditor":
            case "FishStatus":
                goGold.SetActive(true);
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
        if (isGoldAddCalc) { return; }
        isGoldAddCalc = true;
        goldAddCalcRemainingTime = 1f;
    }
    private void Update()
    {
        if (isGoldAddCalc)
        {
            goldAddCalcRemainingTime = Mathf.Max(0f, goldAddCalcRemainingTime - Time.deltaTime);

            PlayerModel.Instance.gainGold = (int)Mathf.Lerp(PlayerModel.Instance.gainGold, 0, 1f - goldAddCalcRemainingTime);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHomeCommon : UIBase
{
    public GameObject goBack;
    private AudioSource bgmAudioSource;

    protected override void Awake()
    {
        base.Awake();
        bgmAudioSource = GetComponent<AudioSource>();
    }
    public void SetActiveByUIName(string uiname)
    {
        switch (uiname)
        {
            case "Home":
            case "BattleResult":
                goBack.SetActive(false);
                break;
            case "FishEditor":
            case "FishStatus":
            case "Shop/Shop":
            case "RankBonus/RankBonus":
            case "Option":
                goBack.SetActive(true);
                break;
        }
    }
    public void SetBgmValue(string uiname)
    {
        switch (uiname)
        {
            case "Home":
            case "FishEditor":
            case "FishStatus":
            case "Shop/Shop":
                bgmAudioSource.volume = 1;
                break;
            case "BattleResult":
                bgmAudioSource.volume = 0;
                break;
        }
    }
    public void OnBackBtn()
    {
        var homeScene = BlSceneManager.GetCurrentScene() as HomeScene;
        homeScene.BackPrescene();
    }
}

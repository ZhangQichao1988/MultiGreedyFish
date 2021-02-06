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
            case "PlayerRanking/PlayerRanking":
            case "Mission/Mission":
            case "Option":
                goBack.SetActive(true);
                break;
        }
    }
    public void SetBgmValue(string uiname)
    {
        switch (uiname)
        {
            case "BattleResult":
                bgmAudioSource.volume = 0;
                break;
            default:
                bgmAudioSource.volume = 1;
                break;
        }
    }
    public void OnBackBtn()
    {
        var homeScene = BlSceneManager.GetCurrentScene() as HomeScene;
        homeScene.BackPrescene();
    }
}

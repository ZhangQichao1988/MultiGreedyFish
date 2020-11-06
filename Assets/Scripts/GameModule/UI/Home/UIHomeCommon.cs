using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHomeCommon : UIBase
{
    public GameObject goBack;
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
                goBack.SetActive(true);
                break;
        }
    }
    public void OnBackBtn()
    {
        var homeScene = BlSceneManager.GetCurrentScene() as HomeScene;
        homeScene.BackPrescene();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHomeCommon : UIBase
{
    public GameObject goBack;
    public void SetActiveScene(string sceneName)
    {
        switch (sceneName)
        {
            case "Home":
                goBack.SetActive(false);
                break;
            case "FishEditor":
                goBack.SetActive(true);
                break;
            case "FishStatus":
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

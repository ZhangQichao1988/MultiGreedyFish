using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishEditor : MonoBehaviour
{
    public void GotoHome()
    {
        var homeScene = BlSceneManager.GetCurrentScene() as HomeScene;
        homeScene.GotoHomeScene("Home");
    }
}

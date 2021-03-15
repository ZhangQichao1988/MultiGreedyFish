using System;
using UnityEditor;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TimerModule;

public class UIPopupBattleTutorialCompleted : UIBase
{
    static public void Open()
    {
        var ui = UIBase.Open<UIPopupBattleTutorialCompleted>("ArtResources/UI/Prefabs/PopupBattleTutorialCompleted", UILayers.POPUP);
        
    }

    public void OnBtn()
    {
        BlSceneManager.LoadSceneByClass(SceneId.HOME_SCENE, typeof(HomeScene));
        Close();
    }
}

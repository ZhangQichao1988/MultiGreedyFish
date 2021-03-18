using System;
using UnityEditor;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TimerModule;

public class UIPopupBattleToHome : UIBase
{

    static public void Open()
    {
        var ui = UIBase.Open<UIPopupBattleToHome>("ArtResources/UI/Prefabs/PopupBattleToHome", UILayers.POPUP);        
    }

    public void OnBtn()
    {
        BlSceneManager.LoadSceneByClass(SceneId.HOME_SCENE, typeof(HomeScene));
        Close();
    }
}

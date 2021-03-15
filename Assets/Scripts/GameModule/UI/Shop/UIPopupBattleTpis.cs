using System;
using UnityEditor;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TimerModule;

public class UIPopupBattleTpis : UIBase
{
    public enum Type
    { 
        BadyFish    = 920,
        JellyFish,
        Shell,
        Shark,
        Tortoise,
        Skill,
        Aquatic,
    };

    public Image image;
    public Text text;

    private Action onClick;
    static public void Open(Type type, Action onClick)
    {
        var ui = UIBase.Open<UIPopupBattleTpis>("ArtResources/UI/Prefabs/PopupBattleTips", UILayers.POPUP);
        ui.Setup(type, onClick);
        
    }
    public void Setup(Type type, Action onClick)
    {
        text.text = LanguageDataTableProxy.GetText((int)type);
        this.onClick = onClick;
    }

    public void OnBtn()
    {
        if (onClick != null) { onClick(); }
        Close();
    }
}

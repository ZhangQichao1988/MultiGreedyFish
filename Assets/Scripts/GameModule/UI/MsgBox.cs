using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class MsgBox : UIBase
{
    public Text title;
    public Text content;

    public GameObject btnCancel;

    Action _callback;
    Action _cancelCallback;
    
    public static void Open(string title, string content, Action onSure = null)
    {
        var msgBox = UIBase.Open<MsgBox>("ArtResources/UI/Prefabs/Msg/MsgBox", UILayers.POPUP);
        msgBox.SetContent(title, content, onSure);
    }

    
    public static void OpenConfirm(string title, string content, Action onSure = null, Action onCancel = null)
    {
        var msgBox = UIBase.Open<MsgBox>("ArtResources/UI/Prefabs/Msg/MsgBox", UILayers.POPUP);
        msgBox.SetContent(title, content, onSure, onCancel);
    }

    public static void OpenTips(string content)
    {
        var msgBox = UIBase.Open<MsgBox>("ArtResources/UI/Prefabs/Msg/MsgTips", UILayers.POPUP);
        msgBox.SetContent(content);
        msgBox.AutoClose();
    }

    public static void ShowGettedItem(List<RewardItemVo> items)
    {
        if (items != null && items.Count > 0)
        {
            UIBase.Open("ArtResources/UI/Prefabs/Shop/ShopReward", UILayers.POPUP, items);
        }
    }

    public void SetContent(string txt_title, string txt_content, Action onSure, Action onCancel)
    {
        _callback = onSure;
        _cancelCallback = onCancel;
        SetContent(txt_title, txt_content);
        btnCancel.SetActive(true);
    }

    public void SetContent(string txt_content)
    {
        content.text = txt_content;
    }

    public void SetContent(string txt_title, string txt_content, Action onSure = null)
    {
        title.text = txt_title;
        content.text = txt_content;
        _callback = onSure;
    }

    public void AutoClose()
    {
        StartCoroutine(DelayClose());
    }

    public void OnSure()
    {
        _callback?.Invoke();
        base.Close();
    }

    public override void Close()
    {
        _cancelCallback?.Invoke();
        base.Close();
    }
}
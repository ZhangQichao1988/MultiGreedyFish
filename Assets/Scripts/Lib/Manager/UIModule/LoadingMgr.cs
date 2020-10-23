using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class LoadingMgr
{
    public enum LoadingType
    {
        Repeat = 0,
	    Progress = 1,
    }

    private static LoadingRepeat repeatUI;
    private static LoadingProgress progressUI;
    public static void Init()
    {
        GameObject progressLayerNode = BlUIManager.GetLayerNode("LOADING");
        repeatUI = new LoadingRepeat(progressLayerNode);
        progressUI = new LoadingProgress(progressLayerNode);
    }

    public static void Show(LoadingType type, string text = null)
    {
        if (type == LoadingType.Repeat)
        {
		    repeatUI.Show();
        }
        else if (type == LoadingType.Progress)
        {
            progressUI.Show(text);
        }
    }

    public static void Hide(LoadingType type)
    {
        if (type == LoadingType.Repeat)
        {
		    repeatUI.Hide();
        }
        else if (type == LoadingType.Progress)
        {
            progressUI.Hide();
        }
    }

    public static void SetProgress(float percent)
    {
        progressUI.SetProgress(percent);
    }
}
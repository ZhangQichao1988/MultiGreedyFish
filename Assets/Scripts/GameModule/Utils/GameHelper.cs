using UnityEngine;
using System;
using System.IO;

public class GameHelper
{
    public static void Repair()
    {
        PlayerPrefs.DeleteKey(UpdateFlowController.CURRENT_VERSION);
        if (Directory.Exists(AppConst.MasterSavedPath))
        {
            Directory.Delete(AppConst.MasterSavedPath, true);
        }
        PlayerPrefs.Save();
        Intro.Instance.Restart();
    }
}
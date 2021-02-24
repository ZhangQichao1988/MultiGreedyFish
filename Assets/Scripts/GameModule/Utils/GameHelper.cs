using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Networking;

public class GameHelper
{
    public class AdmobCustomData
    {
        public int eventId;
        public string arg1;
        public string arg2;

        public Dictionary<string, System.Object> toDictionary()
        {
            Dictionary<string, System.Object> result = new Dictionary<string, System.Object>();
            result["eventId"] = eventId;
            result["arg1"] = arg1;
            result["arg2"] = arg2;
            return result;
        }
    }

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

    public static string AdmobCustomGenerator(AdmobEvent adEvent, string parm1 = null, string parm2 = null)
    {
        return ((int)adEvent).ToString() + "." + parm1 + "." + parm2;
    }
}
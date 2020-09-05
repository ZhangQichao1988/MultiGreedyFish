using UnityEditor;
using UnityEngine;


public class UserEditorMenu
{
    [MenuItem("UserEditor/ClearPrefabs")]
    public static void ClearPlayerPrefabs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    public static string SERVER_TENCENT = "SERVER_TENCENT";
    public static string DUMMY_DATA = "DUMMY_DATA";

    [MenuItem("UserEditor/Server/Local")]
    public static void ServerLocal()
    {
        string symblos = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
        if (symblos.Contains(SERVER_TENCENT))
        {
            symblos = symblos.Replace(SERVER_TENCENT + ";", "").Replace(SERVER_TENCENT, "");
        }
        if (symblos.Contains(DUMMY_DATA))
        {
            symblos = symblos.Replace(DUMMY_DATA + ";", "").Replace(DUMMY_DATA, "");
        }
        MultiGreedyFish.Pipline.ProjectBuild.SetDefineSymbols(symblos);
    }


    [MenuItem("UserEditor/Server/Tencent")]
    public static void ServerTencent()
    {
        string symblos = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
        if (symblos.Contains(DUMMY_DATA))
        {
            symblos = symblos.Replace(DUMMY_DATA + ";", "").Replace(DUMMY_DATA, "");
        }
        if (!symblos.Contains(SERVER_TENCENT))
        {
            symblos = symblos + ";" +  SERVER_TENCENT + ";";
        }
        MultiGreedyFish.Pipline.ProjectBuild.SetDefineSymbols(symblos);
    }


    [MenuItem("UserEditor/Server/Dummy")]
    public static void ServerDummy()
    {
        string symblos = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
        if (symblos.Contains(SERVER_TENCENT))
        {
            symblos = symblos.Replace(SERVER_TENCENT + ";", "").Replace(SERVER_TENCENT, "");
        }
        if (!symblos.Contains(DUMMY_DATA))
        {
            symblos = symblos + ";" +  DUMMY_DATA + ";";
        }
        MultiGreedyFish.Pipline.ProjectBuild.SetDefineSymbols(symblos);
    }
}
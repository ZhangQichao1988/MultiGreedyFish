using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


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

    [MenuItem("UserEditor/FontChange")]
    public static void ChangeFont()
    {
        string fontPath = EditorUtility.OpenFilePanel("Select font", Application.dataPath, "ttf,otf");
        string[] prefabPaths = Directory.GetFiles(Application.dataPath, "*.prefab", SearchOption.AllDirectories);
        Dictionary<string, int> countFontRef = new Dictionary<string, int>();

        fontPath = fontPath.Replace(Application.dataPath, "Assets");
        Font changedFont = AssetDatabase.LoadAssetAtPath<Font>(fontPath) as Font;
        Debug.Log("NewFont:" + changedFont);


        for (int i = 0; i < prefabPaths.Length; ++i)
        {
            string path = prefabPaths[i];
            string relativePath = GetRelativeAssetPath(path);
            GameObject newPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(relativePath) as GameObject;
            if (null == newPrefab) 
            { 
                continue;
            }

            Text[] uiLabelChilds = newPrefab.GetComponentsInChildren<Text>(true);
            Debug.Log(relativePath);
            foreach (Text obj in uiLabelChilds)
            {
                try
                {
                    string fontName = obj.font != null ? obj.font.name : "";
                    
                    // if (fontName.Contains("font_num_"))
                    // {
                    //     continue;
                    // }
                    if (countFontRef.ContainsKey(fontName))
                    {
                        countFontRef[fontName] = countFontRef[fontName] + 1;
                    }
                    else
                    {
                        countFontRef.Add(fontName, 1);
                    }
                    obj.font = changedFont;
                    EditorUtility.SetDirty(obj);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("error :" + ex.Message);
                }
            }
        }


        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    private static string GetRelativeAssetPath(string _fullPath)
    {
        _fullPath = GetRightFormatPath(_fullPath);
        int idx = _fullPath.IndexOf("Assets");
        string assetRelativePath = _fullPath.Substring(idx);
        return assetRelativePath;
    }

    private static string GetRightFormatPath(string _path)
    {
        return _path.Replace("\\", "/");
    }
}
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

    [MenuItem("UserEditor/Server/LocalServer")]
    public static void ServerLocal()
    {
        SetMode("ESeverType.LOCAL_SERVER");
    }


    [MenuItem("UserEditor/Server/Offline")]
    public static void ServerDummy()
    {
        SetMode("ESeverType.OFFLINE");
    }

    [MenuItem("UserEditor/Server/TencentDev")]
    public static void ServerTencentDev()
    {
        SetMode("ESeverType.TENCENT_DEV");
    }

    [MenuItem("UserEditor/Server/TencentStable")]
    public static void ServerTencentStb()
    {
        SetMode("ESeverType.TENCENT_STABLE");
    }


    [MenuItem("UserEditor/Server/TencentProd")]
    public static void ServerTencentProd()
    {
        SetMode("ESeverType.TENCENT_PROD");
    }

    public static void SetMode(string mode)
    {
        var appFilePath = Path.Combine(Application.dataPath, "Scripts/GameModule/AppConst.cs");
        var lines = File.ReadAllLines(appFilePath);
        for (int i = 0; i <  lines.Length; i++)
        {
            if (lines[i].Contains("public static ESeverType DefaultServerType"))
            {
                lines[i] = "\tpublic static ESeverType DefaultServerType = " + mode + ";";
                File.WriteAllLines(appFilePath, lines); 
                PlayerPrefs.DeleteKey("SERVER_TYPE");
                PlayerPrefs.Save();
                AssetDatabase.Refresh();
                break;
            }
        }
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
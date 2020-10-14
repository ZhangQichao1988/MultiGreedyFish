/**
 * Jackpot
 * Copyright(c) 2014 KLab, Inc. All Rights Reserved.
 * Proprietary and Confidential - This source code is not for redistribution
 * 
 * Subject to the prior written consent of KLab, Inc(Licensor) and its terms and
 * conditions, Licensor grants to you, and you hereby accept nontransferable,
 * nonexclusive limited right to access, obtain, use, copy and/or download
 * a copy of this product only for requirement purposes. You may not rent,
 * lease, loan, time share, sublicense, transfer, make generally available,
 * license, disclose, disseminate, distribute or otherwise make accessible or
 * available this product to any third party without the prior written approval
 * of Licensor. Unauthorized copying of this product, including modifications
 * of this product or programs in which this product has been merged or included
 * with other software products is expressly forbidden.
 */
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Jackpot
{
    [CustomEditor(typeof(JackpotSettings))]
    public class JackpotSettingsEditor : Editor
    {
        const string settingsFile = "JackpotSettings";
        const string settingsFileExtension = ".asset";
        const string settingsFolderPath = "Assets/Jackpot/Jackpot.Core/Editor/Resources";
        static GUIContent labelAppControllerButton = new GUIContent("[iOS]Enable KJPAppController [?]", "Check to Enable KJPAppController in iOS");
        JackpotSettings currentSettings = null;
        bool appControllerButtonEnabled;
        string photoLibraryUsage = "Used for uploading screenshot";
        bool folding;

        [MenuItem("Jackpot/Settings")]
        public static void ShowSettings()
        {
            var settingsInstance = LoadSettings();
            if (settingsInstance == null)
            {
                settingsInstance = CreateSettings();
            }
            if (settingsInstance == null)
            {
                return;
            }
            Selection.activeObject = settingsInstance;
        }

        public override void OnInspectorGUI()
        {
            if (target == null)
            {
                Selection.activeObject = null;
                return;
            }
            currentSettings = (JackpotSettings) target;
            if (currentSettings == null)
            {
                return;
            }
            EditorGUILayout.BeginVertical();
            appControllerButtonEnabled = EditorGUILayout.ToggleLeft(labelAppControllerButton, currentSettings.IsAppControllerEnabled);
            if (appControllerButtonEnabled != currentSettings.IsAppControllerEnabled)
            {
                currentSettings.IsAppControllerEnabled = appControllerButtonEnabled;
                EditorUtility.SetDirty(currentSettings);
            }

            // URLスキーム入力フィールド
            if (folding = EditorGUILayout.Foldout(folding, "URL Scheme(iOS only)"))
            {
                // リスト表示
                var urlSchemes = currentSettings.UrlSchemes;
                for (var i = 0; i < urlSchemes.Count; i++)
                {
                    urlSchemes[i] = EditorGUILayout.TextField(urlSchemes[i]);
                }

                if (GUILayout.Button("追加"))
                {
                    urlSchemes.Add("");
                }
                if (GUILayout.Button("クリア"))
                {
                    urlSchemes.RemoveAt(urlSchemes.Count - 1);
                }
                currentSettings.UrlSchemes = urlSchemes;
                EditorUtility.SetDirty(currentSettings);
            }

            photoLibraryUsage = EditorGUILayout.TextField( "PhotoLibrary Usage",currentSettings.PhotoLibraryUsageDescription );
            if (photoLibraryUsage != currentSettings.PhotoLibraryUsageDescription)
            {
                currentSettings.PhotoLibraryUsageDescription = photoLibraryUsage;
                EditorUtility.SetDirty(currentSettings);
            }

            EditorGUILayout.EndVertical();
        }

        public static JackpotSettings LoadSettings()
        {
            return (JackpotSettings) AssetDatabase.LoadAssetAtPath(Path.Combine(settingsFolderPath, settingsFile + settingsFileExtension), typeof(JackpotSettings));
        }

        private static JackpotSettings CreateSettings()
        {
            var settings = (JackpotSettings) ScriptableObject.CreateInstance(typeof(JackpotSettings));
            if (settings == null)
            {
                return null;
            }
            if (!Directory.Exists(Path.Combine(Application.dataPath, "Jackpot/Jackpot.Core/Editor/Resources")))
            {
                AssetDatabase.CreateFolder("Assets/Jackpot/Jackpot.Core/Editor", "Resources");
            }
            AssetDatabase.CreateAsset(settings, Path.Combine(settingsFolderPath, settingsFile + settingsFileExtension));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return settings;
        }

        void OnDisable()
        {
            if (currentSettings != null)
            {
                EditorUtility.SetDirty(currentSettings);
                currentSettings = null;
            }
        }
    }
}

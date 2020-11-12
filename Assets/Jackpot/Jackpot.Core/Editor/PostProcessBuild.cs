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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
#if UNITY_EDITOR_OSX
using XcodeTools;
#endif
using Jackpot.Extensions;

namespace Jackpot
{
    public static class PostProcessBuild
    {
        /// <summary>
        /// ビルドターゲットをiOSに指定しているかを示します
        /// </summary>
        /// <remarks>
        /// Unity4.xでは<c>BuildTarget.iPhone</c>と宣言されてた識別子が、Unity5以降では<c>BuildTarget.iOS</c>にリネームされます。
        /// このインターフェースはその互換性維持の為のラッパーであり、将来的には廃止される予定です
        /// </remarks>
        /// <returns><c>true</c> if is iOS the specified target; otherwise, <c>false</c>.</returns>
        /// <param name="target">Target.</param>
        public static bool IsiOS(BuildTarget target)
        {
#if UNITY5_SCRIPTING_IN_UNITY4
            return target == BuildTarget.iPhone;
#else
            return target == BuildTarget.iOS;
#endif
        }

        [PostProcessBuild(90)]
        static void OnPostProcessBuild(BuildTarget target, string path)
        {
            if (IsiOS(target))
            {
                OnPostProcessBuildIos(target, path);
                return;
            }
            if (target == BuildTarget.Android)
            {
                OnPostProcessBuildAndroid(target, path);
                return;
            }
        }

        static void OnPostProcessBuildIos(BuildTarget target, string path)
        {
#if UNITY_EDITOR_OSX
            if (!IsiOS(target))
            {
                return;
            }

            var pbxProjectPath = PBXProject.GetPBXProjectPath(path);
            var project = new PBXProject();
            project.ReadFromFile(pbxProjectPath);

#if UNITY5_SCRIPTING_IN_UNITY4
            var jsonFile = "*.projmods_4.json";
#else
            var jsonFile = "*.projmods_5.json";
#endif

            var modFiles = Directory.GetFiles(
                               Path.Combine(Application.dataPath, "Jackpot"),
                               jsonFile,
                               SearchOption.AllDirectories
                           );
            foreach (var modFile in modFiles)
            {
                var mod = new XCMod(modFile);
                mod.ApplyTo(project);
            }
            var jackpotSettings = JackpotSettingsEditor.LoadSettings();
            if (jackpotSettings != null && jackpotSettings.IsAppControllerEnabled)
            {
#if UNITY_2019_3_OR_NEWER
                var projectGuid = project.GetUnityFrameworkTargetGuid();
#else
                var projectGuid = project.TargetGuidByName(PBXProject.GetUnityTargetName());
#endif
                project.AddBuildProperty(projectGuid, "GCC_PREPROCESSOR_DEFINITIONS", "JACKPOT_APP_CONTROLLER=1");
            }

            if (jackpotSettings != null && jackpotSettings.PhotoLibraryUsageDescription.Length > 0)
            {
                setInfoPlist(Path.Combine(path, "Info.plist"), jackpotSettings.PhotoLibraryUsageDescription);
            }

            project.WriteToFile(pbxProjectPath);

            if (jackpotSettings != null && jackpotSettings.UrlSchemes.Count > 0)	
            {	
                setUrlScheme(Path.Combine(path, "Info.plist"), jackpotSettings.UrlSchemes);
            }
#endif
        }

        static void OnPostProcessBuildAndroid(BuildTarget target, string path)
        {
            if (target != BuildTarget.Android)
            {
                return;
            }

#if UNITY_5_5_OR_NEWER
            ScriptingImplementation backend = PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android);
#else
            ScriptingImplementation backend = (ScriptingImplementation) PlayerSettings.GetPropertyInt("ScriptingBackend", BuildTargetGroup.Android);
#endif
            if (backend == ScriptingImplementation.IL2CPP)
            {
                return;
            }
            GenerateApkDigests(target, path);
        }

        /// <summary>
        /// APKファイルに紐づくダイジェスト情報(JSONファイル)を、apkの出力ディレクトリに書き込みます
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="path">Path.</param>
        static void GenerateApkDigests(BuildTarget target, string path)
        {
            var dictionary = new Dictionary<string, string>();
            var jdkPath = EditorPrefs.GetString("JdkPath");
            if (string.IsNullOrEmpty(jdkPath))
            {
                UnityEngine.Debug.LogWarning("JDK Path was empty. signature file was not created.");
                return;
            }
            if (string.IsNullOrEmpty(PlayerSettings.Android.keyaliasName))
            {
                UnityEngine.Debug.LogWarning("PlayerSettings.Android.keyaliasName was empty. signature file was not created.");
                return;
            }
            if (string.IsNullOrEmpty(PlayerSettings.Android.keystorePass))
            {
                UnityEngine.Debug.LogWarning("PlayerSettings.Android.keystorePass was empty. signature file was not created.");
                return;
            }
            if (string.IsNullOrEmpty(PlayerSettings.Android.keystoreName))
            {
                UnityEngine.Debug.LogWarning("PlayerSettings.Android.keystoreName was empty. signature file was not created.");
                return;
            }

            var info = ExternalProcess.CreateInfo(
                           jdkPath + "/bin/keytool",
                           string.Format(
                               "-list -v -alias {0} -keystore {1} -storepass {2}",
                               PlayerSettings.Android.keyaliasName,
                               PlayerSettings.Android.keystoreName,
                               PlayerSettings.Android.keystorePass
                           ),
                           Path.GetFullPath(Path.Combine(Application.dataPath, "../"))
                       );
            ExternalProcess.ProcessWithTimeout(info, TimeSpan.FromMinutes(2))
                .Unpack((stdout, stderr, isTimeout) =>
            {
                if (string.IsNullOrEmpty(stderr) && !isTimeout)
                {
                    var digestLines = stdout.Replace("\r\n", "\n").Split('\n').Where(x => x.Contains("MD5: ") || x.Contains("SHA1:") || x.Contains("SHA256:"));
                    foreach (string line in digestLines)
                    {
                        var digests = line.Split(new[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                        UnityEngine.Debug.Log(digests[0]);
                        UnityEngine.Debug.Log(digests[1]);
                        dictionary["keystore_" + digests[0].Replace(":", "").ToLower().Trim()] =
                                digests[1].Replace(":", "").ToLower().Trim();
                    }
                }
                else
                {
                    UnityEngine.Debug.LogWarning("PlayerSettings.Android.keystoreName was empty. The keystore signatures is not written.");
                }
            });
            var rootPath = Path.GetFullPath(Path.Combine(
                               Application.dataPath,
                               "../Temp/StagingArea/assets/bin/Data/Managed/"
                           ));
            const string assemblyCsharpDllFileName = "Assembly-CSharp.dll";
            if (!File.Exists(rootPath + assemblyCsharpDllFileName))
            {
                return;
            }
            var dll = File.ReadAllBytes(rootPath + assemblyCsharpDllFileName);
            var md5 = MD5.Create().ComputeHash(dll);
            var sha1 = SHA1.Create().ComputeHash(dll);
            var sha256 = SHA256.Create().ComputeHash(dll);
            dictionary["assembly_csharp_dll_md5"] = BitConverter.ToString(md5).ToLower().Replace("-", "");
            dictionary["assembly_csharp_dll_sha1"] = BitConverter.ToString(sha1).ToLower().Replace("-", "");
            dictionary["assembly_csharp_dll_sha256"] = BitConverter.ToString(sha256).ToLower().Replace("-", "");

            using (var stream = new FileStream(
                                    string.Format(
                                        "{0}{1}_{2}_signatures.json",
                                        path,
                                        PlayerSettings.productName,
                                        PlayerSettings.bundleVersion
                                    ),
                                    FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new StreamWriter(stream))
            {
                writer.WriteLine("{");
                foreach (var pair in dictionary)
                {
                    writer.WriteLine(string.Format("    \"{0}\": \"{1}\",", pair.Key, pair.Value));
                }
                writer.WriteLine(string.Format("    \"{0}\": \"{1}\"", "updated", UnixEpoch.FromDateTime(DateTime.Now)));
                writer.WriteLine("}");
                writer.Flush();
            }
        }

        /// <summary>
        /// 対象のplistに許可するURLスキームを書き込みます
        /// </summary>
        /// <param name="plistPath">Plist path.</param>
        /// <param name="urlSchemes">URL schemes.</param>
        static void setUrlScheme(string plistPath, IList<string> urlSchemes)
        {
#if UNITY_EDITOR_OSX
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);
            PlistElementArray dictElement = plist.root["LSApplicationQueriesSchemes"] as PlistElementArray;
            if (dictElement == null)
            {
                dictElement = plist.root.CreateArray("LSApplicationQueriesSchemes");
            }
            foreach (var scheme in urlSchemes)
            {
                dictElement.AddString(scheme);
            }
            plist.WriteToFile(plistPath);
#endif
        }

        /// <summary>
        /// plistにPhotosの利用を追記します
        /// </summary>
        /// <param name="plistPath">Plist path.</param>
        /// <param name="usageDescription">PhotoLibrary Usage Description.</param>
        static void setInfoPlist(string plistPath, string usageDescription)
        {
#if UNITY_EDITOR_OSX
            // Read plist
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            // Update value
            PlistElementDict rootDict = plist.root;
            rootDict.SetString("NSPhotoLibraryUsageDescription", usageDescription);

            // Write plist
            File.WriteAllText(plistPath, plist.WriteToString());
#endif
        }
    }
}

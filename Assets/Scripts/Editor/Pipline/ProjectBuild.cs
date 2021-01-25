using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;
using IFix.Editor;

namespace MultiGreedyFish.Pipline
{
    public class ProjectBuild
    {
        private static bool IsBuildIos = false;
        private static bool IsDevBuild = false;

        static void DealwithBuildArg()
        {
            IsBuildIos = System.Convert.ToBoolean(Function.GetValue("-iosBuild=", false));
            IsDevBuild = System.Convert.ToBoolean(Function.GetValue("-devBuild=", false));
            string strDefine = "";

            string enabledDebugMenu = Function.GetValue("-enabledDebugMenu=", false);
            bool enabledDebug = System.Convert.ToBoolean(enabledDebugMenu);
            if (enabledDebug)
            {
                strDefine = strDefine + "CONSOLE_ENABLE;";
            }

            string uServer = Function.GetValue("-useSever=", false);
            if (uServer != null)
            {
                UserEditorMenu.SetMode(uServer);
            }

            SetDefineSymbols(strDefine);
        }

        public static void SetDefineSymbols(string strDefine)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, strDefine);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, strDefine);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, strDefine);
        }

        static void Build()
        {
            DealwithBuildArg();
            
            if (!IsBuildIos)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            }
            else
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
            }
            
            IFixEditor.InjectAssemblys();
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
            {
                BuildForIPhone();
            }
            else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            {
                BuildForAndroid();
            }
        }

        static void BuildForIPhone()
        {
            string dir = "Achieve" + Path.DirectorySeparatorChar;
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
            Directory.CreateDirectory(dir);
            

            BuildOptions opt = BuildOptions.SymlinkLibraries;
            if (IsDevBuild)
            {
                opt = opt | BuildOptions.Development;
                EditorUserBuildSettings.development = true;
            }
            else
            {
                EditorUserBuildSettings.development = false;
            }
            // opt = opt | BuildOptions.Development | BuildOptions.ConnectWithProfiler;
            
            
            
            PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, 1);
            
            PlayerSettings.iOS.applicationDisplayName = Function.GetValue ("-productName=", true);
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS , Function.GetValue ("-bundleIdentifier=", true));
            PlayerSettings.iOS.buildNumber = Function.GetValue ("-buildNumber=", true);

            PlayerSettings.iOS.iOSManualProvisioningProfileID = Function.GetValue ("-provisionPID=", true);
            PlayerSettings.iOS.iOSManualProvisioningProfileType = Convert.ToBoolean( Function.GetValue ("-isDevelop=", true)) ?ProvisioningProfileType.Development : ProvisioningProfileType.Distribution;
            PlayerSettings.iOS.appleDeveloperTeamID = Function.GetValue ("-teamID=", true);
           
            PlayerSettings.iOS.appleEnableAutomaticSigning = false;
            PlayerSettings.bundleVersion = Function.GetValue ("-bundleVersion=", true);
            //string path = "/Users/dean/Documents/IWork/jenkinsTest/" + Function.projectName + ".apk";
            string path = dir + "xcodeProj";
            //BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.iOS, BuildOptions.None);
            BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.iOS, opt);
            
            Debug.Log("Build Suceed !");
            //
        }

        //在这里找出你当前工程所有的场景文件，假设你只想把部分的scene文件打包 那么这里可以写你的条件判断 总之返回一个字符串数组。
        static string[] GetBuildScenes()
        {
            List<string> names = new List<string>();
            foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
            {
                if (e == null)
                    continue;
                if (e.enabled)
                    names.Add(e.path);
            }
            return names.ToArray();
        }

        
        static void BuildForAndroid()
        {
            Debug.Log("Build Android Start!!");

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = Function.GetValue ("-keyStorePath=", true);
            PlayerSettings.Android.keystorePass = Function.GetValue ("-keyStorePass=", true);
            PlayerSettings.Android.keyaliasName = Function.GetValue ("-keyAliasName=", true);
            PlayerSettings.Android.keyaliasPass = Function.GetValue ("-keyAliasPass=", true);
            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;

            PlayerSettings.productName = Function.GetValue ("-productName=", true);
            // arm64 支持
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;
            
            PlayerSettings.applicationIdentifier = Function.GetValue ("-bundleIdentifier=", true);
            
            PlayerSettings.Android.bundleVersionCode = int.Parse(Function.GetValue("-buildNumber=", true));
            PlayerSettings.bundleVersion = Function.GetValue ("-bundleVersion=", true);





            string dir = "Achieve" + Path.DirectorySeparatorChar + "Android" + Path.DirectorySeparatorChar;

            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
            Directory.CreateDirectory(dir);
            string path = dir + Function.GetValue("-apk_name=") + ".apk";
            Debug.Log("show path: " + path);


            bool buildAsAAB = Convert.ToBoolean(Function.GetValue("-is_aab="));
            EditorUserBuildSettings.buildAppBundle = buildAsAAB;

            if (IsDevBuild)
            {
                EditorUserBuildSettings.development = true;
                BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.Android, BuildOptions.Development | BuildOptions.ConnectWithProfiler);
            }
            else
            {
                EditorUserBuildSettings.development = false;
                BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.Android, BuildOptions.None);
            }
            // BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.Android, BuildOptions.Development | BuildOptions.ConnectWithProfiler);
            Debug.Log("Build Android Suceed !");
        }
    }

    public class Function
    {

        //得到项目的名称
        public static string projectName
        {
            get
            {
                //在这里分析shell传入的参数
                //argPath = "OZTIME-" + BUILD_TIME + "OZPKGNAME-" + PKG_NAME
                foreach (string arg in System.Environment.GetCommandLineArgs())
                {
                    if (arg.StartsWith("OZARG"))
                    {
                        return arg.Split("@"[0])[1];
                    }
                }
                return "test";
            }
        }

        public static string[] projectArgList
        {
            get
            {
                //在这里分析shell传入的参数
                foreach (string arg in System.Environment.GetCommandLineArgs())
                {
                    if (arg.StartsWith("OZARG"))
                    {
                        return arg.Split("@"[0]);
                    }
                }
                return new string[] { "0", "Test" };
            }
        }

        public static string GetValue (string parameterName, bool required = false)
        {
            foreach (var arg in System.Environment.GetCommandLineArgs ()) {
                if (arg.ToLower ().StartsWith (parameterName.ToLower ())) {
                    var argValue = arg.Substring (parameterName.Length);
                    Debug.Log (parameterName + ":" + argValue);
                    return argValue;
                }
            }
            if (required) {
                throw new System.ArgumentException (parameterName);
            }
            return null;
        }

        public static void DeleteFolder(string dir)
        {
            foreach (string d in Directory.GetFileSystemEntries(dir))
            {
                if (File.Exists(d))
                {
                    FileInfo fi = new FileInfo(d);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                        fi.Attributes = FileAttributes.Normal;
                    File.Delete(d);
                }
                else
                {
                    DirectoryInfo d1 = new DirectoryInfo(d);
                    if (d1.GetFiles().Length != 0)
                    {
                        DeleteFolder(d1.FullName);////递归删除子文件夹
                    }
                    Directory.Delete(d);
                }
            }
        }

        public static void CopyDirectory(string sourcePath, string destinationPath)
        {
            DirectoryInfo info = new DirectoryInfo(sourcePath);
            Directory.CreateDirectory(destinationPath);
            foreach (FileSystemInfo fsi in info.GetFileSystemInfos())
            {
                string destName = Path.Combine(destinationPath, fsi.Name);
                if (fsi is System.IO.FileInfo)
                    File.Copy(fsi.FullName, destName);
                else
                {
                    Directory.CreateDirectory(destName);
                    CopyDirectory(fsi.FullName, destName);
                }
            }
        }
    }

}


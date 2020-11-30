using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

#if UNITY_IOS && UNITY_EDITOR  

using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;

namespace MultiGreedyFish.Pipline
{
    class BuildPostProcessor
    {
        // ios版本xcode工程维护代码  
        [PostProcessBuild]  
        public static void OnPostprocessBuild(BuildTarget BuildTarget, string path)  
        {  
            if (BuildTarget == BuildTarget.iOS)  
            {  
                string projPath = PBXProject.GetPBXProjectPath(path);  
                PBXProject proj = new PBXProject();  
                proj.ReadFromString(File.ReadAllText(projPath));  

                bool isDevMode = File.Exists(path + "/../ios-development-build");

                Debug.Log("Is DevMode " + isDevMode);


                // 获取当前项目名字  
                string target = proj.GetUnityMainTargetGuid();  

                // 对所有的编译配置设置选项  
                proj.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
                proj.SetBuildProperty(target, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
                // proj.SetBuildProperty(target, "Other Linker Flags", "$(inherited) -weak_framework CoreMotion -weak-lSystem -ObjC");
                proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC -lresolv");

                // 添加依赖库  
                // 语音sdk  
                proj.AddFrameworkToProject (target, "AdSupport.framework", false);
                proj.AddFrameworkToProject (target, "CoreTelephony.framework", false);
                proj.AddFrameworkToProject (target, "QuartzCore.framework", false);
                proj.AddFrameworkToProject (target, "CoreData.framework", false);
                proj.AddFrameworkToProject (target, "Security.framework", false);


                proj.AddFrameworkToProject (target, "SystemConfiguration.framework", false);
                proj.AddFrameworkToProject (target, "libc++.tbd", false);
                proj.AddFrameworkToProject (target, "libz.tbd", false);
                proj.AddFrameworkToProject (target, "libsqlite3.0.tbd", false);
                proj.AddFrameworkToProject (target, "libsqlite3.tbd", false);
                proj.AddFrameworkToProject (target, "CoreMotion.framework", false);
                proj.AddFrameworkToProject (target, "CoreFoundation.framework", false);
                proj.AddFrameworkToProject (target, "CFNetwork.framework", false);
                proj.AddFrameworkToProject (target, "CoreGraphics.framework", false);
                proj.AddFrameworkToProject (target, "Foundation.framework", false);
                proj.AddFrameworkToProject (target, "AuthenticationServices.framework", true);
                proj.AddFrameworkToProject (target, "UIKit.framework", false);
                proj.AddFrameworkToProject (target, "libresolv.tbd", false);
                proj.AddFrameworkToProject (target, "UserNotifications.framework", false);


                
                if (isDevMode)
                {
                    proj.AddCapability(target, PBXCapabilityType.PushNotifications);
                    proj.AddCapability(target, PBXCapabilityType.GameCenter);
                    proj.AddCapability(target, PBXCapabilityType.InAppPurchase);
                    // 修改 lapis.plist 添加 sign with appleid
                    string lapisPlistPath = path + "/Unity-iPhone/lapis.entitlements";  
                    PlistDocument plist_entit = new PlistDocument(); 
                    plist_entit.Create(); 
                    PlistElementArray lapisArray = plist_entit.root.CreateArray("com.apple.developer.applesignin"); 
                    lapisArray.AddString("Default"); 
                    plist_entit.WriteToFile(lapisPlistPath); 
                    
                    proj.AddFile("Unity-iPhone/lapis.entitlements", "lapis.entitlements");
                    proj.AddBuildProperty(target, "CODE_SIGN_ENTITLEMENTS", "Unity-iPhone/lapis.entitlements");
                }
                // 设置签名  
                //proj.SetBuildProperty (target, "CODE_SIGN_IDENTITY", "iPhone Distribution: _______________");  
                //proj.SetBuildProperty (target, "PROVISIONING_PROFILE", "********-****-****-****-************");   

                
                if (isDevMode)
                {
                    proj.AddFrameworkToProject (target, "StoreKit.framework", false);
                }
                else
                {
                    proj.RemoveFrameworkFromProject(target, "StoreKit.framework");
                }

                // 保存工程  
                proj.WriteToFile (projPath);  
                
                /**
                //替换 UnityAppController
                var fileName = "/UnityAppController.h";
                var fileName2 = "/UnityAppController.mm";
            
                string filePath = Application.dataPath  + "/.." + fileName;
                string ds = path + "/Classes" + fileName;
                Debug.Log("fp:"+filePath+" ds:"+ds);
                File.Delete(ds);
                File.Copy(filePath, ds);
                string filePath2 = Application.dataPath + "/.." + fileName2;
                string ds2 = path + "/Classes" + fileName2;
                File.Delete(ds2);
                File.Copy(filePath2, ds2);
                **/


                // 修改plist  
                string plistPath = path + "/Info.plist";  
                PlistDocument plist = new PlistDocument();  
                plist.ReadFromString(File.ReadAllText(plistPath));  
                PlistElementDict rootDict = plist.root;  

                // 语音所需要的声明，iOS10必须  
                rootDict.SetString("NSPhotoLibraryUsageDescription", "是否允许此游戏使用系统相册？");
                rootDict.SetString("NSPhotoLibraryAddUsageDescription", "是否允许此游戏使用系统相册附加功能？");
                rootDict.SetString("NSCameraUsageDescription", "允许访问相机");

                if (!isDevMode)
                {
                    // gamecenter disabled
                    PlistElementArray arr = rootDict["UIRequiredDeviceCapabilities"] as PlistElementArray;
                    if (arr != null)
                    {
                        foreach (var feat in arr.values)
                        {
                            if (feat.AsString() == "gamekit")
                            {
                                arr.values.Remove(feat);
                                break;
                            }
                        }
                    }
                }

                //微信白名单
                // PlistElementArray array = rootDict.CreateArray("LSApplicationQueriesSchemes");
                // array.AddString("weixin");

                string url_types = "CFBundleURLTypes";
                PlistElementArray bundleURLTypesArray = rootDict[url_types] as PlistElementArray;
                if (bundleURLTypesArray == null)
                {
                    bundleURLTypesArray = rootDict.CreateArray(url_types);
                }
                PlistElementDict dic = bundleURLTypesArray.AddDict(); 
                dic.SetString("CFBundleTypeRole", "Editor"); 
                if (!isDevMode)
                {
                    dic.SetString("CFBundleURLName", "com.klab.fishgame"); 
                    PlistElementArray dicArray = dic.CreateArray("CFBundleURLSchemes"); 
                    dicArray.AddString("com.klab.fishgame.dev"); 
                }
                else
                {
                    dic.SetString("CFBundleURLName", "com.klab.fishgame"); 
                    PlistElementArray dicArray = dic.CreateArray("CFBundleURLSchemes"); 
                    dicArray.AddString("com.klab.fishgame"); 
                }
                // 通知
                PlistElementArray backgroundModesArray = rootDict.CreateArray("UIBackgroundModes"); 
                backgroundModesArray.AddString("remote-notification");
                backgroundModesArray.AddString("fetch");
                
                PlistElementDict dictTmp = rootDict.CreateDict("NSAppTransportSecurity");
                dictTmp.SetBoolean( "NSAllowsArbitraryLoads", true);

                // 保存plist  
                plist.WriteToFile (plistPath); 
                if (!isDevMode)
                {
                    DisableCapability(projPath, "com.apple.GameCenter.iOS");
                } 
            }  
        } 
        
        private static void DisableCapability(string proPath, string capStr)
        {
            string[] resultLine = File.ReadAllLines(proPath);
            bool hasCenterTag = false;
            for (int i = 0; i < resultLine.Length; i++)
            {
                if (hasCenterTag)
                {
                    hasCenterTag = false;
                    if (resultLine[i].Contains("enabled = 1;"))
                    {
                        resultLine[i] = resultLine[i].Replace("enabled = 1;", "enabled = 0;");
                        continue;
                    }
                }
                if (resultLine[i].Contains(capStr + " ="))
                {
                    hasCenterTag = true;
                    continue;
                }
            }

            File.WriteAllLines(proPath, resultLine);
        }
    }
}
#endif
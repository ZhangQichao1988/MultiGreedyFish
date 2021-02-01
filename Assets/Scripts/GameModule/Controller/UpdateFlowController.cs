using UnityEngine;
using System;
using System.IO;
using NetWorkModule;

/// <summary>
/// 更新流程
/// </summary>
public class UpdateFlowController
{
    static string CURRENT_VERSION = "CURRENT_VERSION";
    static Action finishCb;
    static int retryTime = 3;
    public static void StartUpdateFlow(Action callback)
    {
        finishCb = callback;
        Debug.LogWarning(AppConst.HttpVersionPoint);
        NetWorkManager.SimpleGet<string>(AppConst.HttpVersionPoint, (str)=>{
            Debug.Log("Version Num " + str);
            if (PlayerPrefs.GetString(CURRENT_VERSION, "") != str)
            {
                retryTime = 3;
                DoUpdate(str.Trim());
            }
            else
            {
                finishCb();
            }
        }, ()=>{
            if (retryTime-- < 0)
            {
                MsgBox.Open("网络错误", "请检查网络", ()=>{
                    retryTime = 3;
                    StartUpdateFlow(finishCb);
                });
                return;
            }
            StartUpdateFlow(finishCb);
        });
    }

    //更新
    private static void DoUpdate(string currVersion)
    {
        Debug.LogWarning(string.Format(AppConst.HttpDownloadPoint, currVersion));
        NetWorkManager.SimpleGet<byte[]>(string.Format(AppConst.HttpDownloadPoint, currVersion), (bytes)=>{
            //保存 & 解压
            // File.WriteAllBytes(GetMasterSavedPath, bytes);
            if (Directory.Exists(AppConst.MasterSavedPath))
            {
                Directory.Delete(AppConst.MasterSavedPath, true);
            }
            ZipHelper.UnZip(bytes, AppConst.MasterSavedPath);

            PlayerPrefs.SetString(CURRENT_VERSION, currVersion);
            PlayerPrefs.Save();
            finishCb();
        }, ()=>{
            if (retryTime-- < 0)
            {
                 MsgBox.Open("网络错误", "请检查网络", ()=>{
                    retryTime = 3;
                    DoUpdate(currVersion);
                });
                return;
            }
            DoUpdate(currVersion);
        });
    }
}
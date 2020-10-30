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
    static int retryTime;
    public static void StartUpdateFlow(Action callback)
    {
        finishCb = callback;
        retryTime = 3;
        NetWorkManager.SimpleGet<string>(AppConst.HttpVersionPoint, (str)=>{
            Debug.Log("Version Num " + str);
            if (PlayerPrefs.GetString(CURRENT_VERSION, "") != str)
            {
                DoUpdate(str);
            }
            else
            {
                finishCb();
            }
        }, ()=>{
            if (retryTime-- < 0)
            {
                MsgBox.Open("网络错误", "请检查网络", ()=>{
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
        retryTime = 3;
        NetWorkManager.SimpleGet<byte[]>(AppConst.HttpDownloadPoint, (bytes)=>{
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
                    DoUpdate(currVersion);
                });
                return;
            }
            DoUpdate(currVersion);
        });
    }
}
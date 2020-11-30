using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FishEditor : UIBase
{
    public Transform transContent = null;

    public override void OnEnter(System.Object parms)
    {

        List<FishEditorItem> aryFishEditorItem = new List<FishEditorItem>( transContent.GetComponentsInChildren<FishEditorItem>());

        GameObject go;
        FishEditorItem fishEditorItem;
        foreach (var note in PlayerModel.Instance.player.AryPlayerFishInfo)
        {
            // 已有的鱼只是更新信息，不重新实例化
            fishEditorItem = aryFishEditorItem.Find((a) => a.pBPlayerFishLevelInfo.FishId == note.FishId);
            if (fishEditorItem != null)
            {
                fishEditorItem.Refash(note);
                continue;
            }

            // 新增的鱼实例化
            //if (note.FishLevel > 0)
            {
                var asset = ResourceManager.LoadSync<GameObject>(Path.Combine(AssetPathConst.uiRootPath, "FishEditorItem"));
                go = GameObjectUtil.InstantiatePrefab(asset.Asset, transContent.gameObject);
                fishEditorItem = go.GetComponent<FishEditorItem>();
                fishEditorItem.Init(note);
            }
        }
    }
    public void GotoHome()
    {
        var homeScene = BlSceneManager.GetCurrentScene() as HomeScene;
        homeScene.GotoSceneUI("Home");
    }

    private void OnDestroy()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FishEditor : UIBase
{
    public Transform transContent = null;

    public override void Init()
    {
        base.Init();

        GameObject go;
        Image image;
        foreach (var note in PlayerModel.Instance.player.AryPlayerFishInfo)
        {
            if (note.FishLevel > 0)
            {
                var asset = ResourceManager.LoadSync<GameObject>(Path.Combine(AssetPathConst.uiRootPath, "FishEditorItem"));
                go = GameObjectUtil.InstantiatePrefab(asset.Asset, transContent.gameObject);
                image = go.GetComponent<Image>();
                var spAsset = ResourceManager.LoadSync<Sprite>(string.Format(AssetPathConst.fishIconPath, note.FishId));
                image.sprite = spAsset.Asset;
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

using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class HomeScene : BaseScene
{
    private readonly string uiRootPath = "ArtResources/UI/Prefabs/";
    private readonly List<string> listUI = new List<string>() 
    {
        "Home",
        "FishEditor"
    };
    private Dictionary<string, GameObject> dicUI;

    private string mainUIPath = "ArtResources/UI/Prefabs/Home";
    private string fishEditorUIPath = "ArtResources/UI/Prefabs/FishEditor";
    public override void Init(object parms)
    {
        Resources.UnloadUnusedAssets();

        // Home相关UI预载
        dicUI = new Dictionary<string, GameObject>();
        foreach (var note in listUI)
        {
            m_sceneData.Add(new SceneData() { Resource = Path.Combine(uiRootPath, note), ResType = typeof(GameObject) });
        }
        
    }

    public override void Create()
    {
        CreateHomeScene("Home");
    }

    private void CreateHomeScene(string sceneName)
    {
        string uiPath = Path.Combine(uiRootPath, sceneName);
        var mainGo = cachedObject[uiPath] as GameObject;
        mainGo = GameObjectUtil.InstantiatePrefab(mainGo, null);
        dicUI.Add(sceneName, mainGo);
    }
    public void GotoFishEditor()
    {
        GotoHomeScene("FishEditor");
    }
    public void GotoHomeScene(string sceneName)
    {
        // 隐藏所有UI
        foreach (var note in dicUI.Values)
        {
            note.SetActive(false);
        }

        if (dicUI.ContainsKey(sceneName))
        {
            dicUI[sceneName].SetActive(true);
        }
        else
        {
            CreateHomeScene(sceneName);
        }
    }
}
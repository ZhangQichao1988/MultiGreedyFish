using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class HomeScene : BaseScene
{
    
    private readonly List<string> listUI = new List<string>() 
    {
        "Home",
        "FishEditor"
    };
    private Dictionary<string, GameObject> dicUI;
    private UIHomeCommon homeCommon;

    private string currentScene;
    private List<string> sceneHistory;

    public override void Init(object parms)
    {
        Resources.UnloadUnusedAssets();
        sceneHistory = new List<string>();
        // Home相关UI预载
        dicUI = new Dictionary<string, GameObject>();
        foreach (var note in listUI)
        {
            m_sceneData.Add(new SceneData() { Resource = Path.Combine(AssetPathConst.uiRootPath, note), ResType = typeof(GameObject) });
        }
        
    }

    public override void Create()
    {
        currentScene = "Home";
        CreateHomeScene(currentScene);
        sceneHistory.Add(currentScene);
        homeCommon = UIBase.Open<UIHomeCommon>(Path.Combine( AssetPathConst.uiRootPath, "HomeCommon" ), UIBase.UILayers.DEFAULT);
        homeCommon.SetActiveScene(currentScene);
    }

    private void CreateHomeScene(string sceneName)
    {
        string uiPath = Path.Combine(AssetPathConst.uiRootPath, sceneName);
        var mainGo = cachedObject[uiPath] as GameObject;
        mainGo = GameObjectUtil.InstantiatePrefab(mainGo, null);
        dicUI.Add(sceneName, mainGo);
    }
    public void GotoFishEditor()
    {
        GotoHomeScene("FishEditor");
    }
    public void GotoHomeScene(string sceneName, bool saveHistory = true)
    {
        if (saveHistory) { sceneHistory.Add(currentScene); }

        currentScene = sceneName;
        homeCommon.SetActiveScene(sceneName);
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
    public void BackPrescene()
    {
        GotoHomeScene(sceneHistory[sceneHistory.Count - 1], false);
    }
}
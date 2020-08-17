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

    private string mainUIPath = "ArtResources/UI/Prefabs/Home";
    private string fishEditorUIPath = "ArtResources/UI/Prefabs/FishEditor";
    public override void Init(object parms)
    {
        Resources.UnloadUnusedAssets();

        // Home相关UI预载
        dicUI = new Dictionary<string, GameObject>();
        foreach (var note in listUI)
        {
            m_sceneData.Add(new SceneData() { Resource = Path.Combine(AssetPathConst.uiRootPath, note), ResType = typeof(GameObject) });
        }
        
    }

    public override void Create()
    {
        GotoSceneUI("Home");
    }

    public void GotoFishEditor()
    {
        GotoSceneUI("FishEditor");
    }
}
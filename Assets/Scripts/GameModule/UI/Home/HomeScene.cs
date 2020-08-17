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
    private UIHomeCommon homeCommon;


    public override void Init(object parms)
    {
        base.Init(parms);
        foreach (var note in listUI)
        {
            m_sceneData.Add(new SceneData() { Resource = Path.Combine(AssetPathConst.uiRootPath, note), ResType = typeof(GameObject) });
        }
        
    }

    public override void Create()
    {
        homeCommon = UIBase.Open<UIHomeCommon>(Path.Combine(AssetPathConst.uiRootPath, "HomeCommon"), UIBase.UILayers.DEFAULT);
        GotoSceneUI("Home");
    }
    public void GotoFishEditor()
    {
        GotoSceneUI("FishEditor");
    }
    public override void GotoSceneUI(string uiName, bool saveHistory = true)
    {
        base.GotoSceneUI(uiName, saveHistory);
        homeCommon.SetActiveScene(uiName);
    }

}
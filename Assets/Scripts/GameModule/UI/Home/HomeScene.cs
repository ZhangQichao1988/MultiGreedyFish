using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class HomeScene : BaseScene
{
    
    private readonly List<string> listUI = new List<string>() 
    {
        "Home",
        "FishEditor",
        "FishStatus",
    };
    private UIHomeCommon homeCommon;
    private UIHomeResource homeResource;

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
        homeResource = UIBase.Open<UIHomeResource>(Path.Combine(AssetPathConst.uiRootPath, "HomeResource"), UIBase.UILayers.RESOURCE);
        GotoSceneUI("Home");
    }
    public void GotoFishEditor()
    {
        GotoSceneUI("FishEditor");
    }
    public override UIBase GotoSceneUI(string uiName, bool saveHistory = true)
    {
        var uiBase = base.GotoSceneUI(uiName, saveHistory);
        homeCommon.SetActiveScene(uiName);
        homeResource.SetActiveScene(uiName);
        return uiBase;
    }

    public override void Destory()
    {
        base.Destory();
        homeCommon.Close();
        homeResource.Close();
    }
}
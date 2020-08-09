using UnityEngine;
using System.IO;

public class HomeScene : BaseScene
{
    private string mainUIPath = "ArtResources/UI/Prefabs/Home";
    public override void Init(object parms)
    {
        Resources.UnloadUnusedAssets();
        // Other
        m_sceneData.Add(new SceneData(){ Resource = mainUIPath, ResType = typeof(GameObject) });
    }

    public override void Create()
    {
        var mainGo =  cachedObject[mainUIPath] as GameObject;
        GameObjectUtil.InstantiatePrefab(mainGo, null);
    }
}
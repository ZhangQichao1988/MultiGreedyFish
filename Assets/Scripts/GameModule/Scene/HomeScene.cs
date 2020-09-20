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
        "Shop/Shop",
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

        StageModel.Instance.AddListener(ShopEvent.ON_GETTED_ITEM, OnGettedItemNormal);
    }

    public void GotoFishEditor()
    {
        GotoSceneUI("FishEditor");
    }
    public override UIBase GotoSceneUI(string uiName, System.Object parms = null, bool saveHistory = true)
    {
        var uiBase = base.GotoSceneUI(uiName, parms, saveHistory);
        homeCommon.SetActiveByUIName(uiName);
        homeResource.SetActiveScene(uiName);
        return uiBase;
    }

    /// <summary>
    /// get item展示  金币刷新等
    /// </summary>
    /// <param name="res"></param>
    private void OnGettedItemNormal(System.Object res)
    {
        P11_Response response = res as P11_Response;
        MsgBox.ShowGettedItem(response.Content);

        if (response.IsTreasure)
        {
            //宝箱
        }
    }

    public override void Destory()
    {
        base.Destory();
        homeCommon.Close();
        homeResource.Close();
        ShopModel.Instance.Dispose();
    }
}
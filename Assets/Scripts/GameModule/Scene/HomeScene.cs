using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class HomeScene : BaseScene
{
    private string startSceneName = "Home";
    private readonly List<string> listUI = new List<string>() 
    {
        "BattleResult",
        "Home",
        "FishEditor",
        "FishStatus",
        "Shop/Shop",
        "RankBonus/RankBonus",
        "PlayerRanking/PlayerRanking",
        "Option",
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
        if (parms != null)
        {
            startSceneName = (string)parms;
        }
    }

    public override void Create()
    {
        homeCommon = UIBase.Open<UIHomeCommon>(Path.Combine(AssetPathConst.uiRootPath, "HomeCommon"), UIBase.UILayers.DEFAULT);
        homeResource = UIBase.Open<UIHomeResource>(Path.Combine(AssetPathConst.uiRootPath, "HomeResource"), UIBase.UILayers.RESOURCE);
        GotoSceneUI(startSceneName);

        ShopModel.Instance.AddListener(ShopEvent.ON_GETTED_ITEM, OnGettedItemNormal);
        BillingManager.Resume();
    }

    public override UIBase GotoSceneUI(string uiName, System.Object parms = null, bool saveHistory = true)
    {
        var uiBase = base.GotoSceneUI(uiName, parms, saveHistory);
        homeCommon.SetActiveByUIName(uiName);
        homeCommon.SetBgmValue(uiName); 
        homeResource.SetActiveScene(uiName);
        return uiBase;
    }

    /// <summary>
    /// get item展示  金币刷新等
    /// </summary>
    /// <param name="res"></param>
    public void OnGettedItemNormal(System.Object res)
    {
        RewardMapVo response = res as RewardMapVo;

        if (response.IsTreasure)
        {
            ShowTreasure.ShowGettedItem(response);
        }
        else 
        {
            MsgBox.ShowGettedItem(response.Content);
        }
        homeResource.UpdateAssets();
    }

    public override void Destory()
    {
        Debug.LogWarning("Desotry Item!!");
        base.Destory();
        homeCommon.Close();
        homeResource.Close();
        ShopModel.Instance.Dispose();
        SoundManager.Instance.UnloadSimSE();

    }
}
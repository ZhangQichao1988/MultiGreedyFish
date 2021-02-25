using System;
using UnityEditor;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class UIPopupGotoResGet : UIBase
{
    public enum ResType
    {
        DIAMOND,
        GOLD,
        CHIP
    };

    enum GotoType
    { 
        SHOP,
        MISSION,
        BATTLE,
        BATTLE_POINT_BONUS,
        RANKING_BONUS,
    };

    static readonly Dictionary<ResType, List<GotoType>> dicGotoParam = new Dictionary<ResType, List<GotoType>>
    {
        { ResType.DIAMOND, new List<GotoType>(){ GotoType.SHOP, GotoType.RANKING_BONUS, GotoType.BATTLE_POINT_BONUS } },
        { ResType.GOLD, new List<GotoType>(){ GotoType.SHOP, GotoType.MISSION, GotoType.BATTLE, GotoType.BATTLE_POINT_BONUS } },
        { ResType.CHIP, new List<GotoType>(){ GotoType.SHOP, GotoType.BATTLE_POINT_BONUS } },
    };

    public GameObject goGrid;
    public Text textTitle;

    private Action gotoAction;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="resType"></param>
    /// <param name="hasShop">是否包括商店</param>
    static public void Open(ResType resType, Action onClose, bool hasShop = true)
    {
        var ui = UIBase.Open<UIPopupGotoResGet>(Path.Combine(AssetPathConst.uiRootPath, "PopupGotoResGet"), UILayers.POPUP);
        ui.Init(resType, onClose, hasShop);
    }
    public void Init(ResType resType, Action onClose, bool hasShop = true)
    {
        gotoAction = onClose;
        gotoAction += Close;
        string strResName = "";
        switch (resType)
        {
            case ResType.DIAMOND: strResName = LanguageDataTableProxy.GetText(40001); break;
            case ResType.GOLD: strResName = LanguageDataTableProxy.GetText(40002); break;
            case ResType.CHIP: strResName = LanguageDataTableProxy.GetText(1206); break;
        }
        textTitle.text = string.Format(LanguageDataTableProxy.GetText(1200), strResName);

        List<GotoType> gotoTypes = dicGotoParam[resType];
        GameObject go;
        Button btn;
        Text text;
        var homeScene = BlSceneManager.GetCurrentScene() as HomeScene;
        foreach (var note in gotoTypes)
        {
            if (!hasShop && note == GotoType.SHOP) { continue; }
            var asset = ResourceManager.LoadSync<GameObject>(Path.Combine(AssetPathConst.uiRootPath, "PopupGotoResGetItem"));
            go = GameObjectUtil.InstantiatePrefab(asset.Asset, goGrid);
            btn = go.GetComponent<Button>();
            text = go.GetComponentInChildren<Text>();
            switch (note)
            {
                case GotoType.SHOP:
                    btn.onClick.AddListener(() => { homeScene.GotoSceneUI("Shop/Shop"); gotoAction(); });
                    text.text = LanguageDataTableProxy.GetText(1201);
                    break;
                case GotoType.MISSION:
                    btn.onClick.AddListener(() => { homeScene.GotoSceneUI("Mission/Mission"); gotoAction(); });
                    text.text = LanguageDataTableProxy.GetText(1202);
                    break;
                case GotoType.BATTLE:
                    btn.onClick.AddListener(() => { homeScene.GotoSceneUI("Home"); gotoAction(); });
                    text.text = LanguageDataTableProxy.GetText(1203);
                    break;
                case GotoType.BATTLE_POINT_BONUS:
                    btn.onClick.AddListener(() => { homeScene.GotoSceneUI("RankBonus/RankBonus"); gotoAction(); });
                    text.text = LanguageDataTableProxy.GetText(1204);
                    break;
                case GotoType.RANKING_BONUS:
                    btn.onClick.AddListener(() => { homeScene.GotoSceneUI("PlayerRanking/PlayerRanking"); gotoAction(); });
                    text.text = LanguageDataTableProxy.GetText(1205);
                    break;
            }
        }
    }
}

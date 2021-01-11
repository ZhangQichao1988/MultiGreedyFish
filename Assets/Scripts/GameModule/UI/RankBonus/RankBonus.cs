using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RankBonus : UIBase
{
    public Slider slider;
    public Text textTotalRankLevel;
    public RectTransform transGrid = null;
    public RectTransform transContent = null;

    List<RankBonusItem> aryFishEditorItem = null;
    List<RankBonusDataInfo> rankBonusDataInfos;

    public override void OnEnter(System.Object parms)
    {
        Refash();
    }
    public override void Init()
    {
        base.Init();

        aryFishEditorItem = new List<RankBonusItem>();

        GameObject go;
        RankBonusItem rankBonusItem;
        rankBonusDataInfos = RankBonusDataTableProxy.Instance.GetAll();
        foreach (var note in rankBonusDataInfos)
        {
            var asset = ResourceManager.LoadSync<GameObject>(Path.Combine(AssetPathConst.uiRootPath, "RankBonus/RankBonusItem"));
            go = GameObjectUtil.InstantiatePrefab(asset.Asset, transGrid.gameObject, false);
            rankBonusItem = go.GetComponent<RankBonusItem>();
            rankBonusItem.Init(note);
            aryFishEditorItem.Add(rankBonusItem);
        }
        var rect = slider.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(350f * rankBonusDataInfos.Count, rect.sizeDelta.y);
        slider.maxValue = rankBonusDataInfos[rankBonusDataInfos.Count - 1].rankLevel;
        Refash();
    }

    void Refash()
    {
        int playerTotalRankLevel = PlayerModel.Instance.GetTotalRankLevel();
        bool firstReach = false;
        RankBonusItem.Status status;

        foreach (var note in aryFishEditorItem)
        {
            if (playerTotalRankLevel >= note.dataInfo.rankLevel)
            {
                status = PlayerModel.Instance.player.GettedBoundsId.Contains(note.dataInfo.ID) ? RankBonusItem.Status.Getted : RankBonusItem.Status.NoGet;
            }
            else
            {
                if (!firstReach)
                {
                    firstReach = true;
                    status = RankBonusItem.Status.Next;
                }
                else
                {
                    status = RankBonusItem.Status.NoReach;
                }
            }
            note.Refash(status);
        }
        transContent.sizeDelta = new Vector2(transGrid.sizeDelta.x, 0);
        slider.value = playerTotalRankLevel;
        textTotalRankLevel.text = playerTotalRankLevel.ToString();
    }
    private void Update()
    {
        if (transContent.sizeDelta.x == 0)
        {
            transContent.sizeDelta = new Vector2(transGrid.sizeDelta.x + 100f, 0);
        }
    }
}

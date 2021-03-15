using UnityEngine;
using System.Collections.Generic;
using Google.Protobuf;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using System;
using UnityEngine.Audio;
using NetWorkModule;
using Firebase.Analytics;

public class UIPlayerRanking : UIBase
{
    public GameObject goRoot;
    public GameObject goWaringText;
    public GameObject goPlayerRankingGrid;

    public Text textPlayerName;
    public Text textTotalScore;
    public Text textRankingPercent;
    public Text textRewardCalcTime;

    public SimpleScrollingView scrollingView;

    List<UIPlayerRankingItem> listPlayerRankingItem = new List<UIPlayerRankingItem>();
    P19_Response res;
    public override void Init()
    {
        base.Init();
    }
    public override void OnEnter(object obj)
    {
        base.OnEnter(obj);
        FetchRanking(PBRankType.Weekly);
    }
    // 设置昵称
    public void FetchRanking(PBRankType type)
    {
        NetWorkHandler.GetDispatch().AddListener<P19_Response>(GameEvent.RECIEVE_P19_RESPONSE, OnRecvFetchRanking);
        NetWorkHandler.RequestGetRankList(type);

    }
    void OnRecvFetchRanking<T>(T response)
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P19_RESPONSE);
        res = response as P19_Response;
        if (res.Result.Code == NetWorkResponseCode.RANK_BOARD_NO_EXIST ||
            res.Result.Code == NetWorkResponseCode.NO_RANK_DATA)
        {
            goRoot.SetActive(false);
            goWaringText.SetActive(true);
            return;
        }
        else
        {
            goRoot.SetActive(true);
            goWaringText.SetActive(false);
        }
        var batchData = RankingBatchDataTableProxy.Instance.GetDataById(res.RankBatch);

        textPlayerName.text = PlayerModel.Instance.player.Nickname;
        textTotalScore.text = string.Format( LanguageDataTableProxy.GetText(601), PlayerModel.Instance.GetTotalRankLevel());
        if (res.Rank < 0)
        {
            textRankingPercent.text = LanguageDataTableProxy.GetText(615);
        }
        else
        {
            string rankStr = RankingRewardDataTableProxy.Instance.GetRankingStr(batchData.groupId, res.Rank, res.RankRate);
            textRankingPercent.text = string.Format(LanguageDataTableProxy.GetText(602), rankStr);
        }
        if (listPlayerRankingItem.Count <= 0)
        {
            scrollingView.Init(AssetPathConst.uiRootPath + "PlayerRanking/PlayerRankingItem");
            var listCell = scrollingView.Fill(res.RankPlayerList.Count);

            UIPlayerRankingItem uIPlayerRanking;
            for (int i = 0; i < listCell.Count; ++i)
            {
                uIPlayerRanking = listCell[i] as UIPlayerRankingItem;
                uIPlayerRanking.Init(i + 1);
                listPlayerRankingItem.Add(uIPlayerRanking);
            }
        }
        for (int i = 0; i < listPlayerRankingItem.Count; ++i)
        {
            listPlayerRankingItem[i].Setup( res.RankPlayerList[i]);
        }
        textRewardCalcTime.text =  string.Format(LanguageDataTableProxy.GetText(617), Clock.GetRemainingTimeStrWithWeekly());

        // 若获得奖励就弹窗
        if (res.RankReward != null)
        {
            FirebaseAnalytics.LogEvent( "get_ranking_bonus",  new Parameter( FirebaseAnalytics.ParameterIndex, res.RankReward.Rank.ToString()));
            UIBase.Open<UIPlayerRankingGetRewardDialog>(AssetPathConst.playerRankingGetRewardDialogPrefabPath, UIBase.UILayers.POPUP, res.RankReward);
        }
        
    }
    public void OpenRewardListDialog()
    {
        int groupId = RankingBatchDataTableProxy.Instance.GetGroupId(res.RankBatch);
        Debug.Assert(groupId >= 0, "UIPlayerRanking.OpenRewardListDialog()_0:" + res.RankBatch);
        UIBase.Open<UIPlayerRankingRewardList>(AssetPathConst.playerRankingRewardListPrefabPath, UILayers.POPUP, groupId);
    }
}
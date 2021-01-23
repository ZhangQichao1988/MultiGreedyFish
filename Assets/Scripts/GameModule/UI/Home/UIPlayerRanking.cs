using UnityEngine;
using System.Collections.Generic;
using Google.Protobuf;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using System;
using UnityEngine.Audio;

public class UIPlayerRanking : UIBase
{
    public GameObject goRoot;
    public GameObject goWaringText;
    public GameObject goPlayerRankingGrid;

    public Text textPlayerName;
    public Text textTotalScore;
    public Text textRankingPercent;

    public SimpleScrollingView scrollingView;

    List<UIPlayerRankingItem> listPlayerRankingItem = new List<UIPlayerRankingItem>();

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
        var res = response as P19_Response;
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
        textPlayerName.text = PlayerModel.Instance.player.Nickname;
        textTotalScore.text = string.Format( LanguageDataTableProxy.GetText(601), PlayerModel.Instance.GetTotalRankLevel());
        if (res.Rank < 0)
        {
            textRankingPercent.text = LanguageDataTableProxy.GetText(615);
        }
        else
        {
            textRankingPercent.text = string.Format(LanguageDataTableProxy.GetText(602), res.RankRate);
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

    }
}
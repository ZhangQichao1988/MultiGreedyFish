using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerRankingItem : SimpleScrollingCell
{
    public Text textRanking;
    public Text textPlayerId;
    public Text textName;
    public Text textScore;

    Image image;
    private void Awake()
    {
    }
    public void Init(int rankIndex)
    {
        image = GetComponent<Image>();
        image.color = rankIndex % 2 == 0 ? new Color(1f, 0.7f, 0.34f, 0.56f) : new Color(1f, 0.55f, 0f, 0.56f);
        textRanking.text = rankIndex.ToString();
    }
    public void Setup(RankPlayer rankPlayer)
    {
        textPlayerId.text = rankPlayer.PlayerId.ToString();
        textName.text = rankPlayer.Nickname;
        textScore.text = rankPlayer.Score.ToString();

    }
}

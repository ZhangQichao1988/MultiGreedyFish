using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleFishList : MonoBehaviour
{
    public Text textRemaining;
    public Text[] textsPlayerName;
    private uint runCnt = 0;
    private void Awake()
    {
        SetRemainingCnt(10);
    }

    // Update is called once per frame
    void Update()
    {
        // 性能优化
        if (runCnt++ % 3 != 0) { return; }

        var fishs = BattleManagerGroup.GetInstance().fishManager.GetAlivePlayer();
        fishs.Sort((a,b)=> { return (int)((b.battleLevel - a.battleLevel) * 100); });
        int plyaerRank = 0;
        FishBase player = null;
        for (int i = 0; i < fishs.Count; ++i)
        {
            ((PlayerBase)fishs[i]).battleLevelRanking = i;
            if (fishs[i].fishType == FishBase.FishType.Player)
            {
                player = fishs[i];
                plyaerRank = i;
            }
        }
        if (plyaerRank > 4)
        {
            fishs.Insert(4, player);
        }
        SetRemainingCnt(fishs.Count) ;

        for (int i = 0; i < textsPlayerName.Length; ++i)
        {
            if (i < fishs.Count)
            {
                textsPlayerName[i].text = string.Format("{0}.{1}", ((PlayerBase)fishs[i]).battleLevelRanking + 1, fishs[i].data.name);
                textsPlayerName[i].gameObject.SetActive(true);
                if (fishs[i].fishType == FishBase.FishType.Player)
                {
                    textsPlayerName[i].color = Color.green;
                }
                else
                {
                    textsPlayerName[i].color = new Color(1f,0.73f,0f);
                }
            }
            else
            {
                textsPlayerName[i].gameObject.SetActive(false);
            }
        }
    }
    void SetRemainingCnt(int value)
    {
        string str = LanguageDataTableProxy.GetText(23);
        str = string.Format(str, value);
        textRemaining.text = str;
    }
}

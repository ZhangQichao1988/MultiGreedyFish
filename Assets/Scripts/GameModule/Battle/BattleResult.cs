using UnityEngine;
using System.Collections.Generic;
using Google.Protobuf;
using UnityEngine.UI;

public class BattleResult : UIBase
{
    [SerializeField]
    Text textRewardGold;

    [SerializeField]
    Text textRewardGoldAdvert;

    [SerializeField]
    Text textRewardRank;

    protected override string uiName { get { return "BattleResult"; } }

    /// <summary>
    /// 普通报酬
    /// </summary>
    public void OnClickGetReward()
    {
        Close();

        // TODO 通信结束后执行来返回主界面
        BlSceneManager.LoadSceneByClass(SceneId.HOME_SCENE, typeof(HomeScene));
    }

    /// <summary>
    /// 双倍报酬
    /// </summary>
    public void OnClickGetRewardAdvert()
    {
        Close();

        // TODO 通信结束后执行来返回主界面
        BlSceneManager.LoadSceneByClass(SceneId.HOME_SCENE, typeof(HomeScene));
    }
    public void Setup(P5_Response response)
    {
        // TODO:明细显示时分离
        int gold = response.GainGold + response.GainRankLevelupBonusGold;
        int rankUp = response.GainRankLevel;

        Debug.Assert(textRewardGold != null, "BattleResult.Setup()_1");
        textRewardGold.text = string.Format( LanguageDataTableProxy.GetText(8), gold);

        Debug.Assert(textRewardGoldAdvert != null, "BattleResult.Setup()_2");
        textRewardGoldAdvert.text = string.Format(LanguageDataTableProxy.GetText(8), gold * ConfigTableProxy.Instance.GetDataByKey("BattleRewardGoldAdvertRate"));

        Debug.Assert(textRewardRank != null, "BattleResult.Setup()_3");
        textRewardRank.text = string.Format(LanguageDataTableProxy.GetText(9), rankUp);
    }
}
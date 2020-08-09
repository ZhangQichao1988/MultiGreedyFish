using UnityEngine;
using System.Collections.Generic;
using Google.Protobuf;
using UnityEngine.UI;

public class BattleResult : UIBase
{
    [SerializeField]
    Text textRewardGold;

    [SerializeField]
    Text textRewardRank;

    protected override string uiName { get { return "BattleResult"; } }

    public void OnClickNext()
    {
        Close();
        BlSceneManager.LoadSceneByClass(SceneId.HOME_SCENE, typeof(HomeScene));
    }
    public void Setup(int gold, int rankUp)
    {
        Debug.Assert(textRewardGold != null, "BattleResult.Setup()_1");
        textRewardGold.text = string.Format( LanguageDataTableProxy.GetText(8), gold);

        Debug.Assert(textRewardRank != null, "BattleResult.Setup()_2");
        textRewardRank.text = string.Format(LanguageDataTableProxy.GetText(9), rankUp);
    }
}
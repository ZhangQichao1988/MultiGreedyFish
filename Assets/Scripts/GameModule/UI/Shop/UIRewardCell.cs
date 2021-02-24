using UnityEngine;
using UnityEngine.UI;

public class UIRewardCell : SimpleScrollingCell
{
    public Text text;

    public Text numText;

    public Image images;

    RewardItemVo rewardData;


    public override void UpdateData(System.Object data)
    {
        rewardData = data as RewardItemVo;
        text.text = string.Format(LanguageDataTableProxy.GetText(204), rewardData.Name, rewardData.Amount);
        images.sprite = ResourceManager.LoadSync<Sprite>(AssetPathConst.itemIconPath + rewardData.ResIcon).Asset;
    }
}
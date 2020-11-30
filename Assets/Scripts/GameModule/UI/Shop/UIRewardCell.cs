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
        text.text = rewardData.Name;
        images.sprite = ResourceManager.LoadSync<Sprite>(AssetPathConst.itemIconPath + rewardData.ResIcon).Asset;
        numText.text = "x" + rewardData.Amount.ToString();
    }
}
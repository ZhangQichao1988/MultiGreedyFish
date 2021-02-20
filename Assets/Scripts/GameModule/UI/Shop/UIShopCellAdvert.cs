
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShopCellAdvert : UIShopCell
{
    
    public override void UpdateData(System.Object data)
    {
        string itemName = ItemDataTableProxy.GetItemName(1);
        textReward.text = string.Format( LanguageDataTableProxy.GetText(204), itemName, ConfigTableProxy.Instance.GetDataById(3101).intValue);

        Refresh();
    }

    public override void Refresh()
    {
        text.text = string.Format(LanguageDataTableProxy.GetText(205), PlayerModel.Instance.player.AdvertRewardRemainingCnt);
        buyBtn.interactable = PlayerModel.Instance.player.AdvertRewardRemainingCnt > 0;
        banObject.SetActive(!buyBtn.interactable);

    }

    public override void OnCellClick()
    {
        NetWorkHandler.GetDispatch().AddListener<P22_Response>(GameEvent.RECIEVE_P22_RESPONSE, OnRecvGetAdvertRemainingCnt);
        NetWorkHandler.RequestGetAdvertRemainingCnt();
    }
    void OnRecvGetAdvertRemainingCnt<T>(T response)
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P22_RESPONSE);
        Debug.Log("OnRecvGetAdvertRemainingCnt!");
        var res = response as P22_Response;
        if (res.Result.Code == NetWorkResponseCode.SUCEED)
        {
            PlayerModel.Instance.player.AdvertRewardRemainingCnt = res.AdvertRewardRemainingCnt;
            Refresh();
            var rewardVO = RewardMapVo.From(new List<ProductContent>() { new ProductContent(){ ContentId = 1, Amount = ConfigTableProxy.Instance.GetDataById(3101).intValue } });
            var homeScene = BlSceneManager.GetCurrentScene() as HomeScene;
            PlayerModel.Instance.UpdateAssets(rewardVO);
            homeScene.OnGettedItemNormal(rewardVO);
        }
        else
        {
            //todo l10n
            MsgBox.OpenTips(res.Result.Desc);
        }
    }

}
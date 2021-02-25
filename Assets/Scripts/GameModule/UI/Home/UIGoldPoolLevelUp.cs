using UnityEngine;
using System.Collections.Generic;
using Google.Protobuf;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using System;

public class UIGoldPoolLevelUp : UIBase
{
    public GameObject goBtnConfirm;
    public Text textTitle;
    public Text textLevelupUseDiamond;

    public Text textValue1;
    public Text textPlus1;

    public Text textValue2;
    public Text textPlus2;

    GoldPoolDataInfo nowLvData;

    public static void Open()
    {
        UIBase.Open<UIGoldPoolLevelUp>("ArtResources/UI/Prefabs/PopupGoldPoolLevelUp", UILayers.POPUP).Setup();
    }
    public void Setup()
    {
        nowLvData = GoldPoolDataTableProxy.Instance.GetDataById(PlayerModel.Instance.goldPoolLevel);
        textValue1.text = string.Format(LanguageDataTableProxy.GetText(503), nowLvData.gainPreSec);
        textValue2.text = nowLvData.maxGold.ToString();

        if (PlayerModel.Instance.goldPoolLevel >= GoldPoolDataTableProxy.Instance.GetAll().Count)
        {
            goBtnConfirm.SetActive(true);
            textTitle.text = LanguageDataTableProxy.GetText(503);
            textPlus1.gameObject.SetActive(false);
            textPlus2.gameObject.SetActive(false);
        }
        else
        {
            goBtnConfirm.SetActive(false);
            textLevelupUseDiamond.text = nowLvData.useDiamond.ToString();
            textTitle.text = string.Format(LanguageDataTableProxy.GetText(510), PlayerModel.Instance.goldPoolLevel + 1);

            var nextLvData = GoldPoolDataTableProxy.Instance.GetDataById(PlayerModel.Instance.goldPoolLevel + 1);
            textPlus1.text = "+" + string.Format(LanguageDataTableProxy.GetText(503), nextLvData.gainPreSec - nowLvData.gainPreSec);
            textPlus2.text = "+" + (nextLvData.maxGold - nowLvData.maxGold);
        }

    }

    public void OnClickLevelUp()
    {
        if (PlayerModel.Instance.player.Diamond >= nowLvData.useDiamond)
        {
            NetWorkHandler.GetDispatch().AddListener<P18_Response>(GameEvent.RECIEVE_P18_RESPONSE, OnRecvLevelUp);
            NetWorkHandler.RequestGoldPoolLevelUp();
        }
        else
        {
            UIPopupGotoResGet.Open(UIPopupGotoResGet.ResType.DIAMOND, () => { Close(); });
        }
    }

    void OnRecvLevelUp<T>(T response)
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P18_RESPONSE);
        var realResponse = response as P18_Response;
        if (realResponse.Result.Code == NetworkConst.CODE_OK)
        {
            UIHome.Instance.FetchGoldPool();
            PlayerModel.Instance.player.Diamond -= nowLvData.useDiamond;
            UIHomeResource.Instance.UpdateAssets();
            Close();
        }
    }

}
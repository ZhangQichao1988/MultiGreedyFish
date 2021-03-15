using UnityEngine;
using System.Collections.Generic;
using Google.Protobuf;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using System;
using Firebase.Analytics;

public class UIGoldPoolLevelUp : UIBase
{
    public GameObject goBtnLvUp;
    public Text textTitle;

    public Text textValue1;
    public Text textPlus1;

    public Text textValue2;
    public Text textPlus2;

    public Text textLvUpUse;
    public Text textLvUpValueUse;
    public Text textRecoverUse;
    public Text textRecoverValueUse;

    GoldPoolDataInfo nowLvData;
    int recoverUseDiamond;

    public static void Open()
    {
        UIBase.Open<UIGoldPoolLevelUp>("ArtResources/UI/Prefabs/PopupGoldPoolLevelUp", UILayers.POPUP).Setup();
    }
    public void Setup()
    {
        recoverUseDiamond = ConfigTableProxy.Instance.GetDataById(3001).intValue;
        nowLvData = GoldPoolDataTableProxy.Instance.GetDataById(PlayerModel.Instance.goldPoolLevel);
        textValue1.text = string.Format(LanguageDataTableProxy.GetText(503), nowLvData.gainPreSec);
        textValue2.text = nowLvData.maxGold.ToString();

        textRecoverUse.text = recoverUseDiamond.ToString();
        textRecoverValueUse.text = string.Format(LanguageDataTableProxy.GetText(505), nowLvData.maxGold);
        textLvUpValueUse.text = string.Format(LanguageDataTableProxy.GetText(506), nowLvData.maxGold);

        if (PlayerModel.Instance.goldPoolLevel >= GoldPoolDataTableProxy.Instance.GetAll().Count)
        {
            goBtnLvUp.SetActive(false);
            textTitle.text = LanguageDataTableProxy.GetText(510);
            textPlus1.gameObject.SetActive(false);
            textPlus2.gameObject.SetActive(false);
        }
        else
        {
            goBtnLvUp.SetActive(true);
            textLvUpUse.text = nowLvData.useDiamond.ToString();
            textTitle.text = string.Format(LanguageDataTableProxy.GetText(500), PlayerModel.Instance.goldPoolLevel + 1);

            var nextLvData = GoldPoolDataTableProxy.Instance.GetDataById(PlayerModel.Instance.goldPoolLevel + 1);
            if (nextLvData.gainPreSec - nowLvData.gainPreSec > 0)
            {
                textPlus1.text = "+" + string.Format(LanguageDataTableProxy.GetText(503), nextLvData.gainPreSec - nowLvData.gainPreSec);
            }
            else
            {
                textPlus1.gameObject.SetActive(false);
            }

            if (nextLvData.maxGold - nowLvData.maxGold > 0)
            {
                textPlus2.text = "+" + (nextLvData.maxGold - nowLvData.maxGold);
            }
            else
            {
                textPlus2.gameObject.SetActive(false);
            }
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
    public void OnClickRecover()
    {
        if (PlayerModel.Instance.player.Diamond >= recoverUseDiamond)
        {
            NetWorkHandler.GetDispatch().AddListener<P23_Response>(GameEvent.RECIEVE_P23_RESPONSE, OnRecvRecover);
            NetWorkHandler.RequestGoldPoolRecover();
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
            FirebaseAnalytics.LogEvent("gold_pool_level_up", new Parameter(FirebaseAnalytics.ParameterLevel, (PlayerModel.Instance.goldPoolLevel + 1).ToString()));
            UIHome.Instance.FetchGoldPool();
            PlayerModel.Instance.player.Diamond -= nowLvData.useDiamond;
            UIHomeResource.Instance.UpdateAssets();
            Close();
        }
    }
    void OnRecvRecover<T>(T response)
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P23_RESPONSE);
        var realResponse = response as P23_Response;
        if (realResponse.Result.Code == NetworkConst.CODE_OK)
        {
            UIHome.Instance.FetchGoldPool();
            PlayerModel.Instance.player.Diamond -= recoverUseDiamond;
            UIHomeResource.Instance.UpdateAssets();
            Close();
        }
    }

}
using UnityEngine;
using System.Collections.Generic;
using Google.Protobuf;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using System;
using UnityEngine.Audio;

public class UIOption : UIBase
{
    public struct LanguageSelectNote
    {
        public LanguageDataTableProxy.LanguageMode languageMode;
        public string languageValue;

    };
    public readonly List<LanguageSelectNote> languageModes = new List<LanguageSelectNote>() 
    {
        new LanguageSelectNote(){ languageMode = LanguageDataTableProxy.LanguageMode.CN, languageValue = "简体中文"},
        new LanguageSelectNote(){ languageMode = LanguageDataTableProxy.LanguageMode.TW, languageValue = "繁體中文"},
        new LanguageSelectNote(){ languageMode = LanguageDataTableProxy.LanguageMode.EN, languageValue = "English"},
        new LanguageSelectNote(){ languageMode = LanguageDataTableProxy.LanguageMode.JP, languageValue = "日本語"},
    };
    public DropdownPlus languageSelect;
    public AudioMixer audioMixer;
    public Text textPlayerID;
    public Text textNickName;
    public Slider sliderBgmValue;
    public Slider sliderSeValue;
    public Toggle isEco;
    public GameObject goLanguageSelectFullBg;

    string setNickName;

    public override void Init()
    {
        base.Init();
        goLanguageSelectFullBg.SetActive(false);
        textPlayerID.text = PlayerModel.Instance.playerId.ToString();
        textNickName.text = PlayerModel.Instance.player.Nickname;
        int languageModeIndex = (int)AppConst.languageMode;
        sliderBgmValue.value = AppConst.BgmValue;
        sliderSeValue.value = AppConst.SeValue;
        isEco.isOn = AppConst.IsEco == 1;
        isEco.onValueChanged.AddListener(isOn => 
        {
            AppConst.IsEco = isOn ? 1 : 0;
            UIHome.Instance.SetPowerMode();
        } );
        foreach (var note in languageModes)
        {
            languageSelect.options.Add(new Dropdown.OptionData(note.languageValue));
        }
        languageSelect.SetValueWithoutNotify(languageModeIndex);
        languageSelect.onClickFrame = () => 
        {
            OnClickFrame();
        };
    }
    public void OnClickFrame()
    {
        goLanguageSelectFullBg.SetActive(!goLanguageSelectFullBg.activeSelf);
    }
    public void SelectLangauge(int n)
    {
        AppConst.languageMode = languageModes[n].languageMode;
        PlayerPrefs.SetInt(AppConst.PlayerPrefabsOptionLangauge, n);
    }
    public void SetBgmValue(float n)
    {
        AppConst.BgmValue = n;
        UIHome.Instance.SetSoundValue();
    }
    public void SetSeValue(float n)
    {
        AppConst.SeValue = n;
        UIHome.Instance.SetSoundValue();
    }
    // 通知开关
    public void SetNoticeEnable(bool n)
    {
        Debug.Log(n);
        // TODO:通知开关设定
    }
    // 联系客服
    public void SendSupportMail()
    {
        Debug.Log("联系客服");
        // TODO:联系客服 修改
        MailSupport.RunMail("aaaa@sina.com", "title", "body");
    }
    // 服务规约
    public void OpenServiceAgreement()
    {
        Application.OpenURL(LanguageDataTableProxy.GetText(320));
    }
    // 设置昵称
    public void SetNickName(string value)
    {
        setNickName = value;
        NetWorkHandler.GetDispatch().AddListener<P9_Response>(GameEvent.RECIEVE_P9_RESPONSE, OnRecvChangeNickName);
        NetWorkHandler.RequestModifyNick(setNickName);

    }
    void OnRecvChangeNickName<T>(T response)
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P9_RESPONSE);
        var res = response as P9_Response;
        PlayerModel.Instance.player.Nickname = setNickName;
    }

    public override void Hide()
    {
        PlayerPrefs.SetFloat(AppConst.PlayerPrefabsOptionBgmValue, AppConst.BgmValue);
        PlayerPrefs.SetFloat(AppConst.PlayerPrefabsOptionSeValue, AppConst.SeValue);
        PlayerPrefs.SetInt(AppConst.PlayerPrefabsOptionIsEco, AppConst.IsEco);
        base.Hide();
    }
}
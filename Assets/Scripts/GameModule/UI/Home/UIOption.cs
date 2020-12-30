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
    public Dropdown languageSelect;
    public AudioMixer audioMixer;
    public Text textPlayerID;
    public Text textNickName;

    string setNickName;

    public override void Init()
    {
        base.Init();
        textPlayerID.text = PlayerModel.Instance.playerId.ToString();
        textNickName.text = PlayerModel.Instance.player.Nickname;
        int languageModeIndex = PlayerPrefs.GetInt(AppConst.PlayerPrefabsOptionLangauge, (int)AppConst.languageMode);
        foreach (var note in languageModes)
        {
            languageSelect.options.Add(new Dropdown.OptionData(note.languageValue));
        }
        languageSelect.SetValueWithoutNotify(languageModeIndex);
    }
    public void SelectLangauge(int n)
    {
        AppConst.languageMode = languageModes[n].languageMode;
        PlayerPrefs.SetInt(AppConst.PlayerPrefabsOptionLangauge, n);
    }
    public void SetBgmValue(float n)
    {
        audioMixer.SetFloat("BgmValue", n);
    }
    public void SetSeValue(float n)
    {
        audioMixer.SetFloat("SeValue", n);
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
        // TODO:联系客服
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
}
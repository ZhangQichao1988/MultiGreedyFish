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

    public override void Init()
    {
        base.Init();
        int languageModeIndex = PlayerPrefs.GetInt(AppConst.PlayerPrefabsOptionLangauge, (int)AppConst.languageMode);
        foreach (var note in languageModes)
        {
            languageSelect.options.Add(new Dropdown.OptionData(note.languageValue));
        }
        languageSelect.SetValueWithoutNotify(languageModeIndex);
    }
    public void Drop_LangaugeSelect(int n)
    {
        AppConst.languageMode = languageModes[n].languageMode;
        PlayerPrefs.SetInt(AppConst.PlayerPrefabsOptionLangauge, n);
    }
    public void Drop_BgmValue(float n)
    {
        audioMixer.SetFloat("BgmValue", n);
    }
    public void Drop_SeValue(float n)
    {
        audioMixer.SetFloat("SeValue", n);
    }
}
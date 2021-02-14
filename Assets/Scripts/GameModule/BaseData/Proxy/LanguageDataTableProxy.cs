using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// 基表代理
/// 每张基表对应一个proxy
/// </summary>
public class LanguageDataTableProxy : BaseDataTableProxy<LanguageDataTable, LanguageDataInfo, LanguageDataTableProxy>
{
    public LanguageDataTableProxy() : base("JsonData/LanguageData"){}

	public enum LanguageMode
	{
		CN,
		TW,
		EN,
		JP
	};

	static public string GetText(int Id, string defaultText = null)
	{
		var data = LanguageDataTableProxy.Instance.GetDataById(Id);
		// Debug.Assert(data != null, "not found id:" + Id);
		if (data == null)
		{
			return defaultText == null ? Id.ToString() : defaultText;
		}
		switch (AppConst.languageMode)
		{
			case LanguageMode.CN:
				return data.cn;
			case LanguageMode.TW:
				return data.tw;
			case LanguageMode.EN:
				return data.en;
			case LanguageMode.JP:
				return data.jp;
		}
		Debug.LogError("LangaugeData.GetText()_1");
		return "";

	}
}
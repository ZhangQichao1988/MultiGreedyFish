using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageData
{
	public enum LanguageMode
	{
		CN,
		TR,
		EN,
		JP
	};

	public struct LangaugeBaseData
	{
		public string cn;   // 简体
		public string tr;   // 繁体
		public string en;   // 英语
		public string jp;   // 日语
		public LangaugeBaseData(string cn, string tr, string en, string jp)
		{
			this.cn = cn;
			this.tr = tr;
			this.en = en;
			this.jp = jp;
		}
	}


	static readonly public Dictionary<string, LangaugeBaseData> dicLanguageBaseData = new Dictionary<string, LangaugeBaseData>()
		{
			{ "PlayerName", new LangaugeBaseData("江小鱼", "江小魚", "jiangxiaoyu", "魚ちゃん")},
			{ "ResultText", new LangaugeBaseData("第{0}名", "第{0}名", "No{0}", "第{0}名")},
			{ "LanguageAddLifeMax", new LangaugeBaseData("+{0}Max", "+{0}Max", "+{0}Max", "+{0}Max")},
			{ "LanguageAddLife", new LangaugeBaseData("+{0}", "+{0}", "+{0}", "+{0}")},
			{ "LanguageDamage", new LangaugeBaseData("-{0}", "-{0}", "-{0}", "-{0}")},
			{ "BattleReadyStart_1", new LangaugeBaseData("躲避周圍污染的海水", "躲避周圍污染的海水", "躲避周圍污染的海水", "躲避周圍污染的海水")},
			{ "BattleReadyStart_2", new LangaugeBaseData("吃掉其他玩家的魚\n成爲海底霸主！", "吃掉其他玩家的魚\n成爲海底霸主！", "吃掉其他玩家的魚\n成爲海底霸主！", "吃掉其他玩家的魚\n成爲海底霸主！")},
			{ "BattleReadyStart_3", new LangaugeBaseData("去剛吧！", "去剛吧！", "去剛吧！", "去剛吧！")},
			{ "BattlePlayPoint", new LangaugeBaseData("生死决战！", "生死决战！", "生死决战！", "生死决战！")},
		};

	static public string GetText(string Id)
	{
		LangaugeBaseData lbd = dicLanguageBaseData[Id];
		switch (AppConst.languageMode)
		{
			case LanguageMode.CN:
				return lbd.cn;
			case LanguageMode.TR:
				return lbd.tr;
			case LanguageMode.EN:
				return lbd.en;
			case LanguageMode.JP:
				return lbd.jp;
		}
		Debug.LogError("LangaugeData.GetText()_1");
		return "";

	}
}

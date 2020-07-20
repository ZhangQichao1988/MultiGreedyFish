using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Localization : MonoBehaviour
{
	private void Awake()
	{
		Text text = GetComponent<Text>();
		Debug.Assert(text != null, "LoLocalization.Awake()_1");
		text.text = LanguageData.GetText(text.text);
	}
}

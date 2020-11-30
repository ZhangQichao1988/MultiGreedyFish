using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Localization : MonoBehaviour
{
	public int id = -1;
	private void Awake()
	{
		Text text = GetComponent<Text>();
		Debug.Assert(text != null, "LoLocalization.Awake()_1" + gameObject.name);
		if(id == -1)
        {
			id = int.Parse(text.text);
		}
		text.text = LanguageDataTableProxy.GetText(id);
	}
}

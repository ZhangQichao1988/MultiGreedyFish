using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeGauge : MonoBehaviour
{
    public enum NumberType
    { 
        Life,
        Damage,
        MaxLife,
    };

    public struct NumberData
    {
        public NumberType numberType;
        public int value;
        public NumberData(NumberType numberType, int value)
        {
            this.numberType = numberType;
            this.value = value;
        }
    };

    public Slider slider = null;
    public Image valueImage = null;
    public Text valueText = null;
    public Text[] dmgExpTests = null;

    List<NumberData> listNumberDatas = new List<NumberData>();

    public void SetValue( int current, int max )
    {
        if (max > slider.maxValue)
        {
            ShowNumber(new NumberData( NumberType.MaxLife, max - (int)slider.maxValue));
        }
        else if (current > slider.value)
        {
            ShowNumber(new NumberData(NumberType.Life, current - (int)slider.value));
        }
        else if (current < slider.value)
        {
            ShowNumber(new NumberData(NumberType.Damage, (int)slider.value - current));
        }
        slider.maxValue = max;
        slider.value = current;
        valueImage.color = Color.HSVToRGB(Mathf.Lerp(0f, 0.3f, slider.normalizedValue), 1f, 1f);
        valueText.text = string.Format("{0}/{1}", current, max);
    }

    public void ShowNumber(NumberData numberData )
    {
        listNumberDatas.Add(numberData);
    }

	private void Update()
	{
        if (listNumberDatas.Count <= 0) { return; }
        Text text = GetText();
        if (text == null) { return; }
        Animator animator = text.GetComponent<Animator>();
        NumberData nd = listNumberDatas[0];
        listNumberDatas.RemoveAt(0);
        switch (nd.numberType)
        {
            case NumberType.MaxLife:
                text.text = string.Format(LanguageDataTableProxy.GetText(2), nd.value);
                text.color = new Color(1f, 0.6f, 0f);
                break;
            case NumberType.Damage:
                text.text = string.Format(LanguageDataTableProxy.GetText(3), nd.value);
                text.color = Color.red;
                break;
            case NumberType.Life:
                text.text = string.Format(LanguageDataTableProxy.GetText(2), nd.value);
                text.color = Color.green;
                break;
        }
        animator.SetTrigger("Restart");

    }

    private Text GetText()
    {
        foreach (Text text in dmgExpTests)
        {
            if (!text.enabled)
            {
                return text;
            }
        }
        return null;
    }
}

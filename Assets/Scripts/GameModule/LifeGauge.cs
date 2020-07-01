using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeGauge : MonoBehaviour
{
    public Slider slider = null;
    public Image valueImage = null;
    public Text valueText = null;

    public void SetValue( int current, int max )
    {
        slider.value = current;
        slider.maxValue = max;
        valueImage.color = Color.HSVToRGB(Mathf.Lerp(0f, 0.3f, slider.normalizedValue), 1f, 1f);
        valueText.text = string.Format("{0}/{1}", current, max);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    public GameObject dmgExpLocation = null;


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
        int effectId = BattleEffectManager.CreateEffect(1, dmgExpLocation.transform);
        Effect effect = EffectManager.GetEffect(effectId);
        //var asset = ResourceManager.LoadSync<GameObject>(Path.Combine(AssetPathConst.effectRootPath, "fx_dmgExp"));
        //GameObject go =  GameObjectUtil.InstantiatePrefab(asset.Asset as GameObject, null);
        //go.transform.position = dmgExpLocation.transform.position;
        Text text = effect.effectObject.GetComponent<Text>();
        Debug.Assert(text != null, "LifeGauge.Update()_1");
        Animator animator = text.GetComponent<Animator>();
        NumberData nd = numberData;
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
    }

}

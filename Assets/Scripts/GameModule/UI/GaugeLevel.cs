using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GaugeLevel : MonoBehaviour
{
    public Text textFishChip;

    public Slider sliderFishLevel;

    private void Awake()
    {
        sliderFishLevel = GetComponent<Slider>();
    }
    public void Refash(PBPlayerFishLevelInfo pBPlayerFishLevelInfo)
    {
        var fishLevelUpData = FishLevelUpDataTableProxy.Instance.GetDataById(pBPlayerFishLevelInfo.FishLevel);
        
        if (fishLevelUpData.useChip < 0)
        {
            textFishChip.text = pBPlayerFishLevelInfo.FishChip.ToString();
            sliderFishLevel.value = 1;
        }
        else
        {
            textFishChip.text = string.Format("{0}/{1}", fishLevelUpData.useChip, pBPlayerFishLevelInfo.FishChip);
            sliderFishLevel.value = (float)pBPlayerFishLevelInfo.FishChip / (float)fishLevelUpData.useChip;
        }

    }
}

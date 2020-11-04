using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GauageLevel : MonoBehaviour
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
        sliderFishLevel.value = (float)pBPlayerFishLevelInfo.FishChip / (float)fishLevelUpData.useChip;
        textFishChip.text = string.Format("{0}/{1}", pBPlayerFishLevelInfo.FishChip, fishLevelUpData.useChip);

    }
}

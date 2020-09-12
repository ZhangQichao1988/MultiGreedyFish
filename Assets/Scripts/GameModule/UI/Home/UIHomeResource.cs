using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHomeResource : UIBase
{
    public GameObject goGold;
    public Text textGold;

    public GameObject goDiamond;
    public Text textDiamond;
    public void SetActiveScene(string sceneName)
    {
        switch (sceneName)
        {
            case "Home":
            case "Shop":
                goGold.SetActive(true);
                goDiamond.SetActive(true);
                break;
            case "FishEditor":
            case "FishStatus":
                goGold.SetActive(true);
                goDiamond.SetActive(false);
                break;
        }
        Apply();
    }

    private void Apply()
    {
        textGold.text = PlayerModel.Instance.player.Gold.ToString();
        textDiamond.text = PlayerModel.Instance.player.Diamond.ToString();
    }
}

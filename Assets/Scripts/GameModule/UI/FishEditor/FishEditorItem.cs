using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FishEditorItem : MonoBehaviour
{
    Button button;
    Animator animator;
    Image bgImage;
    Material materialFishIcon;
    public Image imageFishIcon;

    public GaugeRank gaugeRank;
    public GaugeLevel gaugeLevel;

    public Text textFishName;
    public Text textFishLevel;
    public Text textLock;


    public GameObject goNew;

    FishDataInfo fishData;

    public PBPlayerFishLevelInfo pBPlayerFishLevelInfo;

    private void Awake()
    {
        bgImage = GetComponent<Image>();
        button = GetComponent<Button>();
        animator = GetComponent<Animator>();
        materialFishIcon = new Material(imageFishIcon.material);
        imageFishIcon.material = materialFishIcon;
    }
    public void Refash(PBPlayerFishLevelInfo pBPlayerFishLevelInfo)
    {
        this.pBPlayerFishLevelInfo = pBPlayerFishLevelInfo;

        gaugeRank.Refash(pBPlayerFishLevelInfo);
        gaugeLevel.Refash(pBPlayerFishLevelInfo);
        textFishName.text = LanguageDataTableProxy.GetText( fishData.name );
        if (pBPlayerFishLevelInfo.FishLevel > 0)
        {
            textFishLevel.text = string.Format("Lv.{0}", pBPlayerFishLevelInfo.FishLevel);
            textFishLevel.gameObject.SetActive(true);
            textLock.gameObject.SetActive(false);
            materialFishIcon.DisableKeyword("GRAY_SCALE");
            imageFishIcon.color = Color.white;
            bgImage.color = Color.white;
        }
        else
        {
            textFishLevel.gameObject.SetActive(false);
            textLock.gameObject.SetActive(true);
            materialFishIcon.EnableKeyword("GRAY_SCALE");
            imageFishIcon.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            bgImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);

        }
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(
            () =>
            {
                var homeScene = BlSceneManager.GetCurrentScene() as HomeScene;
                var ui = homeScene.GotoSceneUI("FishStatus") as UIFishStatus;
                ui.Setup(pBPlayerFishLevelInfo);
            });

        goNew.SetActive(gaugeLevel.sliderFishLevel.value >= 1);
    }
    public void Init(PBPlayerFishLevelInfo pBPlayerFishLevelInfo)
    {
        fishData = FishDataTableProxy.Instance.GetDataById(pBPlayerFishLevelInfo.FishId);
        Refash(pBPlayerFishLevelInfo);
        var spAsset = ResourceManager.LoadSync<Sprite>(string.Format(AssetPathConst.fishIconPath, pBPlayerFishLevelInfo.FishId));
        Debug.Assert(spAsset != null, "Not found IconData:" + pBPlayerFishLevelInfo.FishId);
        imageFishIcon.sprite = spAsset.Asset;

        spAsset = ResourceManager.LoadSync<Sprite>(string.Format(AssetPathConst.fishEditorItemBgPath, fishData.rare));
        bgImage.sprite = spAsset.Asset;


    }
    private void OnDestroy()
    {
        Destroy(materialFishIcon);
    }
}

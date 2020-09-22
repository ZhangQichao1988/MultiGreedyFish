using UnityEngine;
using System.Collections.Generic;
using Google.Protobuf;
using System.IO;
using System.Linq;
using UnityEngine.UI;

public class UIHome : UIBase
{
    // FaceIcon
    public Text textPlayerName;
    public Image imgPlayerFaceIcon;

    // 段位
    public Slider sliderRankProcess;
    public Text textRankLevel;

    // 资源
    public Text textGold;
    public Text textDiamond;

    public HomeFishControl fishControl;

    public Text textPlayerCnt;
    private string strPlayerCnt;
    private float playerCntCurrentTime;
    private float playerCntTargetTime;
    private int playerCnt;

    private FishDataInfo fishBaseData;
    private List<Button> listBtn;

    private bool battleRequestSuccess;

    public override void Init()
    {
        base.Init();
        textPlayerCnt.gameObject.SetActive(false);
        strPlayerCnt = LanguageDataTableProxy.GetText(60);
        listBtn = new List<Button>( GetComponentsInChildren<Button>() );
    }
    public override void OnEnter(System.Object parms)
    {
        PBPlayer pBPlayer = PlayerModel.Instance.player;
        OnGetPlayer(pBPlayer);

        // FaceIcon
        textPlayerName.text = pBPlayer.Nickname;
        imgPlayerFaceIcon.sprite = ResourceManager.LoadSync<Sprite>(string.Format(AssetPathConst.faceIconPath, pBPlayer.FaceIconId)).Asset;

        // 段位
        int totalRankLevel = PlayerModel.Instance.GetTotalRankLevel();
        textRankLevel.text = totalRankLevel.ToString();
        sliderRankProcess.value = RankBonusDataTableProxy.Instance.GetRankBonusProcess(totalRankLevel);

        // 资源
        textGold.text = pBPlayer.Gold.ToString();
        textDiamond.text = pBPlayer.Diamond.ToString();
    }
    private void OnGetPlayer(PBPlayer pBPlayer)
    {
        if (fishBaseData != null && fishBaseData.ID == pBPlayer.FightFish) { return; }

        fishBaseData = FishDataTableProxy.Instance.GetDataById(pBPlayer.FightFish);
        var asset = ResourceManager.LoadSync(Path.Combine(AssetPathConst.fishPrefabRootPath + fishBaseData.prefabPath), typeof(GameObject));
        GameObject go =  GameObjectUtil.InstantiatePrefab(asset.Asset as GameObject, fishControl.gameObject);
        fishControl.SetFishModel(go);
    }
    public void OnClickBattle()
    {
        battleRequestSuccess = false;
        NetWorkHandler.GetDispatch().AddListener<P4_Response>(GameEvent.RECIEVE_P4_RESPONSE, OnRecvBattle);
        NetWorkHandler.RequestBattle();

        textPlayerCnt.gameObject.SetActive(true);

        playerCnt = 1;
        textPlayerCnt.text = string.Format(strPlayerCnt, playerCnt);
        playerCntCurrentTime = 0f;
        playerCntTargetTime = Wrapper.GetRandom(0.5f, 1.5f);
        foreach (var btn in listBtn)
        {
            btn.interactable = false;
        }
    }
    public void OnClickFishSelect()
    {
        var homeScene = BlSceneManager.GetCurrentScene() as HomeScene;
        homeScene.GotoSceneUI("FishEditor");
    }

    public void OnClickAdsTest()
    {
        var homeScene = BlSceneManager.GetCurrentScene() as HomeScene;
        homeScene.GotoSceneUI("Shop/Shop");
    }

    void OnRecvBattle<T>(T response)
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P4_RESPONSE);
        var realResponse = response as P4_Response;
        if (realResponse.Result.Code == NetworkConst.CODE_OK)
        {
            battleRequestSuccess = true;
            StageModel.Instance.SetStartBattleRes(realResponse);
        }
        else
        {

            foreach (var btn in listBtn)
            {
                btn.interactable = true;
            }
        }

    }

    private void Update()
    {
        if (textPlayerCnt.gameObject.activeSelf)
        {
            playerCntCurrentTime += Time.deltaTime;
            if (playerCntCurrentTime > playerCntTargetTime && playerCnt <= 9)
            {
                playerCntCurrentTime = 0f;
                playerCntTargetTime = Wrapper.GetRandom(0.2f, 1f);
                ++playerCnt;
                textPlayerCnt.text = string.Format(strPlayerCnt, playerCnt);

            }
            else if (playerCntCurrentTime > 1f && playerCnt == 10 && battleRequestSuccess)
            {   // 进入战斗
                Close();
                BlSceneManager.LoadSceneByClass(SceneId.BATTLE_SCENE, typeof(BattleScene));
            }
        }
    }
}
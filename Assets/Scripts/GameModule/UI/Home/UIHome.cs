using UnityEngine;
using System.Collections.Generic;
using Google.Protobuf;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using TimerModule;
using System;

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

    public GameObject transGoldPool;
    public Slider sliderGoldPool;
    public Text textGoldPool;
    public Text textGoldPool_1;
    private float backupTime;
    private P14_Response goldPoolResponse;
    GoldPoolDataInfo goldPoolData = null;

    public Text textPlayerCnt;
    private string strPlayerCnt;
    private float playerCntCurrentTime;
    private float playerCntTargetTime;
    private int playerCnt;

    private FishDataInfo fishBaseData;
    private List<Button> listBtn;

    private bool battleRequestSuccess;

    protected override void Awake()
    {
        base.Awake();
        textGoldPool.gameObject.SetActive(false);
    }
    public override void Init()
    {
        base.Init();
        textPlayerCnt.gameObject.SetActive(false);
        strPlayerCnt = LanguageDataTableProxy.GetText(60);
        listBtn = new List<Button>(GetComponentsInChildren<Button>());
        listBtn.AddRange(UIHomeResource.Instance.gameObject.GetComponentsInChildren<Button>());

        NetWorkHandler.GetDispatch().AddListener<P14_Response>(GameEvent.RECIEVE_P14_RESPONSE, OnRecvGetGoldPool);
        NetWorkHandler.RequestFetchGoldPool();

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
    void OnRecvGetGoldPool<T>(T response)
    {
        NetWorkHandler.GetDispatch().RemoveListener(GameEvent.RECIEVE_P14_RESPONSE);
        Debug.Log("On Getted GoldPool!");
        textGoldPool.gameObject.SetActive(true);
        goldPoolResponse = response as P14_Response;
        backupTime = Time.realtimeSinceStartup;
        goldPoolData = GoldPoolDataTableProxy.Instance.GetDataById(goldPoolResponse.Level);
        GoldPoolUpdate();

        if (goldPoolResponse.CurrGold < goldPoolData.maxGold)
        {
            Animator animator = GetComponent<Animator>();
            animator.enabled = true;
        }


    }

    private void GoldPoolUpdate()
    {
        long nowTime = (long)(Time.realtimeSinceStartup - backupTime) + goldPoolResponse.CurrTime;
        //if () { return; }

        if (nowTime >= goldPoolResponse.NextAt && goldPoolResponse.CurrGold < goldPoolData.maxGold)
        {
            goldPoolResponse.NextAt += (int)ConfigTableProxy.Instance.GetDataById(3000).intValue;
            goldPoolResponse.NextAt = Math.Min(goldPoolResponse.FullAt, goldPoolResponse.NextAt);

            goldPoolResponse.CurrGold += goldPoolData.gainPreSec;
        }
        int disCurrGold = PlayerModel.Instance.gainGold + goldPoolResponse.CurrGold;
        sliderGoldPool.value = (float)disCurrGold / (float)goldPoolData.maxGold;
        bool isEnable = sliderGoldPool.value < 1f;
        //GameObjectUtil.SetActive(textGoldPool.gameObject, isEnable);
        GameObjectUtil.SetActive(textGoldPool_1.transform.parent.gameObject, isEnable);

        textGoldPool.text = string.Format("{0}/{1}",
                                                            disCurrGold,
                                                            goldPoolData.maxGold);
        if (isEnable)
        {
            textGoldPool_1.text = string.Format(LanguageDataTableProxy.GetText(61),
                                                                        goldPoolResponse.NextAt - nowTime,
                                                                        goldPoolData.gainPreSec);
        }

    }
    public void DropGoldFromPoolStart()
    {
        var asset = ResourceManager.LoadSync<GameObject>(AssetPathConst.uiRootPath + "HomePoolGold");
        int goldCnt = PlayerModel.Instance.gainGold;
        for (int i = 0; i < goldCnt; ++i)
        {
            GameObjectUtil.InstantiatePrefab(asset.Asset, transGoldPool);
        }
    }
    private void OnGetPlayer(PBPlayer pBPlayer)
    {
        if (fishBaseData != null && fishBaseData.ID == pBPlayer.FightFish) { return; }

        fishBaseData = FishDataTableProxy.Instance.GetDataById(pBPlayer.FightFish);
        var asset = ResourceManager.LoadSync(Path.Combine(AssetPathConst.fishPrefabRootPath + fishBaseData.prefabPath), typeof(GameObject));
        GameObject go = GameObjectUtil.InstantiatePrefab(asset.Asset as GameObject, fishControl.gameObject);
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
        if (goldPoolData != null)
        {
            GoldPoolUpdate();
        }

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
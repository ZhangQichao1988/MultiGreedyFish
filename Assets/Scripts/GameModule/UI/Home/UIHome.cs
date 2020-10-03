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

    public Slider sliderGoldPool;
    public Text textGoldPool;
    private float backupTime;
    private P14_Response goldPoolResponse;

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
        listBtn.AddRange(UIHomeResource.Instance.gameObject.GetComponentsInChildren<Button>());

        NetWorkHandler.GetDispatch().AddListener<P14_Response>(GameEvent.RECIEVE_P14_RESPONSE, OnRecvGetGoldPool);
        NetWorkHandler.RequestFetchGoldPool();

        Animator animator = GetComponent<Animator>();
        //animator.Play();
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
        goldPoolResponse = response as P14_Response;
        backupTime = Time.realtimeSinceStartup;
        GoldPoolUpdate();


    }

    private void GoldPoolUpdate()
    {
        long nowTime = (long)(Time.realtimeSinceStartup - backupTime) + goldPoolResponse.CurrTime;
        if (nowTime > goldPoolResponse.FullAt) { return; }

        var goldPoolData = GoldPoolDataTableProxy.Instance.GetDataById(goldPoolResponse.Level);
        if (nowTime >= goldPoolResponse.NextAt && nowTime < goldPoolResponse.FullAt)
        {
            goldPoolResponse.NextAt += (int)ConfigTableProxy.Instance.GetDataById(3000).floatValue;
            goldPoolResponse.NextAt = Math.Max(goldPoolResponse.FullAt, goldPoolResponse.NextAt);

            goldPoolResponse.CurrGold += goldPoolData.gainPreSec;
        }
        
        sliderGoldPool.value = (float)goldPoolResponse.CurrGold / (float)goldPoolData.maxGold;
        textGoldPool.text = string.Format("{0}/{1}\t还有{2}分钟恢复{3}金币", 
                                                                goldPoolResponse.CurrGold, 
                                                                goldPoolData.maxGold,
                                                                goldPoolResponse.NextAt - nowTime,
                                                                goldPoolData.gainPreSec);
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
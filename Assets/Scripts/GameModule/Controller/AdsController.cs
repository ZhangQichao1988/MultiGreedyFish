using UnityEngine;
using GoogleMobileAds.Api;
using System;
using System.Collections.Generic;
using Jackpot;

public class AdsController : MonoBehaviour
{
    public static int RewardRetryTimes = 3;
    public static int RewardWaitTime = 1;

    //使用测试的广告单元
#if UNITY_ANDROID
    // string appId = "ca-app-pub-3940256099942544/5224354917";
    string appId = "ca-app-pub-3796374498607875/8055947862";
#elif UNITY_IPHONE
    string appId = "ca-app-pub-3796374498607875/1231733180";
#else
    string appId = "unexpected_platform";
#endif

    private bool isInited;
    private bool isLoadFailed;

    public Action OnAdRewardGetted;
    void Start()
    {
        //todo : ios 等待申请
        MobileAds.Initialize(client=>{
            isInited = true;

#if CONSOLE_ENABLE
            MobileAds.SetRequestConfiguration((new RequestConfiguration.Builder()).SetTestDeviceIds(
                new List<string>(){
    #if UNITY_ANDROID
                    "90A076DBA55643D9651A1D3347C57C6A",
                    "636A92A693B6B3F0ADFDB471C1BBF70B",
                    "234"
    #elif UNITY_IOS
                    "f75d6701fabbd6600b397a1aaf3a7845",
                    "b303361fe28b19ad4944977e8505c2bb"
    #endif
                }
            ).build());
#endif
        });
        
    }

    RewardedAd rewardAd;
    public void PreLoad()
    {
        if (isInited)
        {
            Debug.Log("Started to preload");
            LoadingMgr.Show(LoadingMgr.LoadingType.Repeat);
            rewardAd = CreateAndLoadRewardedAd(appId);
            rewardAd.LoadAd(GetRequest());
        }
        else
        {
            Debug.Log("have not been Inited");
        }
    }

    AdRequest GetRequest()
    {
        return new AdRequest.Builder().Build();
    }

    public void Show()
    {
        if (rewardAd != null)
        {
            if (rewardAd.IsLoaded())
            {
                rewardAd.Show();
            }
            else if (isLoadFailed)
            {
                PreLoad();
            }
        }
        else
        {
            Debug.Log("have not been Loaded");
            PreLoad();
        }
    }

    private RewardedAd CreateAndLoadRewardedAd(string adUnitId)
    {
        RewardedAd rewardedAd = new RewardedAd(adUnitId);

        rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        rewardedAd.OnAdFailedToLoad += HandleRewardedAdLoadedFailed;
        rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        rewardedAd.OnAdClosed += HandleRewardedAdClosed;
        
        rewardedAd.SetServerSideVerificationOptions((new ServerSideVerificationOptions.Builder()).
                SetUserId(PlayerModel.Instance.player.PlayerId.ToString()).
                Build());

        return rewardedAd;
    }

    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardBasedVideoLoaded event received");
        isLoadFailed = false;
        MainThreadDispatcher.Post(()=>{
            LoadingMgr.Hide(LoadingMgr.LoadingType.Repeat);
            Show();
        });
    }

    public void HandleRewardedAdLoadedFailed(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardBasedVideoLoaded Loaded Failed");
        isLoadFailed = true;
        MainThreadDispatcher.Post(()=>{
            LoadingMgr.Hide(LoadingMgr.LoadingType.Repeat);
            MsgBox.OpenTips("Ads Load Failed");
        });
        
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        Debug.Log("HandleRewardBasedVideoLoaded Reward");
        MainThreadDispatcher.Post(()=>{
            OnAdRewardGetted?.Invoke();
            OnAdRewardGetted = null;
        });
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardBasedVideoLoaded Closed");
        rewardAd = null;
    }
}

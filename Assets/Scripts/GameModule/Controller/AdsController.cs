using UnityEngine;
using GoogleMobileAds.Api;
using System;
using System.Collections.Generic;

public class AdsController : MonoBehaviour
{
    public static int RewardRetryTimes = 3;
    public static int RewardWaitTime = 1;

    //使用测试的广告单元
#if UNITY_ANDROID
    // string appId = "ca-app-pub-3940256099942544/5224354917";
    string appId = "ca-app-pub-3796374498607875/8055947862";
#elif UNITY_IPHONE
    string appId = "ca-app-pub-3940256099942544/1712485313";
#else
    string appId = "unexpected_platform";
#endif

    private bool isInited;
    private bool isLoadFailed;

    public Action OnAdRewardGetted;
    void Start()
    {
        MobileAds.Initialize(client=>{
            isInited = true;

            MobileAds.SetRequestConfiguration((new RequestConfiguration.Builder()).SetTestDeviceIds(
                new List<string>(){
                    "90A076DBA55643D9651A1D3347C57C6A",
                    "7320F6D56BD75B826D440B2416692A0E",
                    "234"
                }
            ).build());
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
        LoadingMgr.Hide(LoadingMgr.LoadingType.Repeat);
        Show();
    }

    public void HandleRewardedAdLoadedFailed(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardBasedVideoLoaded Loaded Failed");
        isLoadFailed = true;
        LoadingMgr.Hide(LoadingMgr.LoadingType.Repeat);
        MsgBox.OpenTips("Ads Load Failed");
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        Debug.Log("HandleRewardBasedVideoLoaded Reward");
        OnAdRewardGetted?.Invoke();
        OnAdRewardGetted = null;
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardBasedVideoLoaded Closed");
        rewardAd = null;
    }
}

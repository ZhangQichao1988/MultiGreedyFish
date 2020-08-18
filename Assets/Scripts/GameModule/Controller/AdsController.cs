using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdsController : MonoBehaviour
{
    //使用测试的广告单元
#if UNITY_ANDROID
    string appId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
    string appId = "ca-app-pub-3940256099942544/1712485313";
#else
    string appId = "unexpected_platform";
#endif

    private bool isInited;
    private bool isLoadFailed;
    void Start()
    {
        MobileAds.Initialize(client=>{
            isInited = true;
        });
    }

    RewardedAd rewardAd;
    public void PreLoad()
    {
        if (isInited)
        {
            Debug.Log("Started to preload");
            rewardAd = CreateAndLoadRewardedAd(appId);
        }
        else
        {
            Debug.Log("have not been Inited");
        }
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

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        rewardedAd.LoadAd(request);
        return rewardedAd;
    }

    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardBasedVideoLoaded event received");
    }

    public void HandleRewardedAdLoadedFailed(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardBasedVideoLoaded Loaded Failed");
        isLoadFailed = true;
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        Debug.Log("HandleRewardBasedVideoLoaded Reward");
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardBasedVideoLoaded Closed");
    }
}

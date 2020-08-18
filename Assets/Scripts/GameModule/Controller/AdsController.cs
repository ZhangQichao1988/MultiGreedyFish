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
    void Start()
    {
        MobileAds.Initialize(client=>{
            Debug.Log(">>>>>>>>>>>>>>>>>>> ABMOD SDK has been Inited");
            isInited = true;
            PreLoad();
        });
    }

    RewardedAd rewardAd;
    public void PreLoad()
    {
        if (isInited)
        {
            rewardAd = CreateAndLoadRewardedAd(appId);
        }
        else
        {
            Debug.Log("have not been Inited");
        }
    }

    public void Show()
    {
        if (rewardAd != null && rewardAd.IsLoaded())
        {
            rewardAd.Show();
        }
        else
        {
            Debug.Log("have not been Loaded");
        }
    }

    private RewardedAd CreateAndLoadRewardedAd(string adUnitId)
    {
        RewardedAd rewardedAd = new RewardedAd(adUnitId);

        rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
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
        MonoBehaviour.print("HandleRewardBasedVideoLoaded event received");
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoLoaded Reward");
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoLoaded Closed");
    }
}

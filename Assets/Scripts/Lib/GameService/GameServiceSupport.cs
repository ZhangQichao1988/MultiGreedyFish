using UnityEngine;
using System;

#if UNITY_IOS
using UnityEngine.SocialPlatforms.GameCenter;
#elif UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
#endif


public class GameServiceSupport
{
    protected event Action currentSuccessAuthentication;
    protected event Action currentFailureAuthentication;

    public GameServiceSupport(bool isShowDefaultBanner = true)
    {
#if UNITY_ANDROID
            PlayGamesPlatform.DebugLogEnabled = UnityEngine.Debug.isDebugBuild;
            var builder = new PlayGamesClientConfiguration.Builder();


            // builder.EnableSavedGames();

            PlayGamesClientConfiguration config = builder.Build();
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.Activate();
#else
            GameCenterPlatform.ShowDefaultAchievementCompletionBanner(isShowDefaultBanner);
#endif
    }

    public void Authenticate(Action sucess,Action failed)
    {
        this.currentSuccessAuthentication = sucess;
        this.currentFailureAuthentication = failed;
        AuthenticateInternal();
    }

    void AuthenticateInternal()
    {
        UnityEngine.Social.localUser.Authenticate(AuthenticationFinishedEvent);
    }

    protected void AuthenticationFinishedEvent(bool success)
    {
        if (success)
        {
            if (currentSuccessAuthentication != null)
            {
                currentSuccessAuthentication();
            }
        }
        else
        {
            if (currentFailureAuthentication != null)
            {
                currentFailureAuthentication();
            }
        }
    }

    public string PlayerIdentifier()
    {
        var playerId = string.Empty;
        
        playerId = UnityEngine.Social.localUser.id;
        
        return playerId;
    }

    public void ShowLeaderboards()
    {
        UnityEngine.Social.ShowLeaderboardUI();
    }

    public void ShowAchievements()
    {
        UnityEngine.Social.ShowAchievementsUI();
    }

    public void ReportLeaderboardScore(
            long score,
            string boardId,
            Action<long, string> onSuccess,
            Action onError)
    {
        UnityEngine.Social.ReportScore(score, boardId, (bool success) =>
        {
            if (success)
            {
                onSuccess(score, boardId);
            }
            else
            {
                onError();
            }
        });
    }

    public void UnlockAchievement(string achievementId, Action<bool> callback)
    {
        UnityEngine.Social.ReportProgress(achievementId, 100.0f, callback);
    }

    public bool IsSignedIn()
    {
        var isSignedIn = false;
#if UNITY_ANDROID
        isSignedIn = UnityEngine.Social.localUser.authenticated;
#endif
        return isSignedIn;
    }
}
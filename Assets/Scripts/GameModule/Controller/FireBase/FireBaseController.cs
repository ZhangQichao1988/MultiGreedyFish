using UnityEngine;
using Firebase;
using System;
using Firebase.Analytics;
using Firebase.Crashlytics;
using Firebase.Extensions;

public class FireBaseController : MonoBehaviour
{
    private bool isInit;
    private string listeneredUserId;
    void Awake()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
		{
			var dependencyStatus = task.Result;
			if (dependencyStatus == Firebase.DependencyStatus.Available)
			{
				// Create and hold a reference to your FirebaseApp,
				// where app is a Firebase.FirebaseApp property of your application class.
				//   app = Firebase.FirebaseApp.DefaultInstance;

				// Set a flag here to indicate whether Firebase is ready to use by your app.
				InitFireBaseMessaging();
			}
			else
			{
				UnityEngine.Debug.LogError(System.String.Format(
				  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
				// Firebase Unity SDK is not safe to use here.
			}
		});
    }

    void InitFireBaseMessaging()
	{
        //push 
		Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
		Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;

        //analytics
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        FirebaseAnalytics.SetUserProperty(
            FirebaseAnalytics.UserPropertySignUpMethod,
            "Google");

        // Set default session duration values.
        FirebaseAnalytics.SetSessionTimeoutDuration(new TimeSpan(0, 30, 0));

        isInit = true;
        if (listeneredUserId != null)
        {
            SetUserId(listeneredUserId);
        }
	}

    public void SetUserId(string userId)
    {
        if (!isInit)
        {
            this.listeneredUserId = userId;
        }
        else
        {
            FirebaseAnalytics.SetUserId(userId);
            Crashlytics.SetUserId(userId);
        }
    }

    public void AnalyticsEvent(string eventName, string subName, int val = 0) 
    {
      // Log an event with no parameters.
        if (subName != null)
        {
            FirebaseAnalytics.LogEvent(eventName, subName, val);
        }
        else
        {
            FirebaseAnalytics.LogEvent(eventName);
        }
    }

	public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
	{
		UnityEngine.Debug.Log("Received Registration Token: " + token.Token);
		PlayerPrefs.SetString( "PUSH_TOKEN", token.Token );
	}

	public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
	{
		UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
	}
}
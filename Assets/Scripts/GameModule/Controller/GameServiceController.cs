using UnityEngine;
using System;
using System.Text;


public class GameServiceController
{
    static GameServiceSupport support;
    public static void Init()
    {
        support = new GameServiceSupport();
    }

    public static void Auth(Action succeed, Action failed)
    {
        support.Authenticate(()=>{
            succeed();
        }, ()=>{
            failed();
        });
    }

    public static bool IsSignIn()
    {
        return support.IsSignedIn();
    }

    public static void GetPlatformToken(Action<string> callback)
    {
        if (IsSignIn())
        {
            EncryptReceiveToken(callback);
            return;
        }

        support.Authenticate(() =>
            {
                EncryptReceiveToken(callback);
            },
            () =>
            {
                Debug.Log(" On Google Error");
                callback(null);
            }
        );
    }

    static void EncryptReceiveToken(Action<string> callback)
    {
        var token = support.PlayerIdentifier();
		if (token != null)
		{
			token = Convert.ToBase64String(CryptographyUtil.PublicEncrypt(Encoding.UTF8.GetBytes(token)));
		}

		callback(token);
    }
}
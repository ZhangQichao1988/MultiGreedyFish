using System.Text;
using System.IO;
using UnityEngine;
using System;

public enum ESeverType
{
    OFFLINE,
    LOCAL_SERVER,
    TENCENT_DEV,
    TENCENT_STABLE,
    TENCENT_PROD,
}

public class AppConst
{
    public static bool BundleMode;
    
    public static string RsaPublicKey = "<RSAKeyValue><Modulus>udy3x+fT95dgx/3SPYrIncR4LrSqPOEllKqID7Q/nQCLq/g/MkC2J0oO3HRl3rmgccxTecAXioemT2TV72w7NHRkw3JXn4AY+2moYy9Fx/ncLClYh1/+ieJmf9vJX8WyTe0+mFaUis4ShJCxtA6aUyaEqgKahFHgz1s0+1wvZyc=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

    public static bool EnableProtocolEncrypt = true;
    public static bool EnabledGameServices = true;


	public static ESeverType DefaultServerType = ESeverType.TENCENT_DEV;

	public static ESeverType ServerType
    {
        get
        {
            string typeStr = PlayerPrefs.GetString("SERVER_TYPE", DefaultServerType.ToString());
            return (ESeverType)Enum.Parse(typeof(ESeverType), typeStr);
        }
    }

    // public static string Host
    // {
    //     get
    //     {
    //         switch(ServerType)
    //         {
    //             case ESeverType.LOCAL_SERVER:
    //                 return "127.0.0.1";
    //             case ESeverType.TENCENT_DEV:
    //                 return "www.cad-crazyfish.top/dev";
    //             case ESeverType.TENCENT_STABLE:
    //                 return "www.cad-crazyfish.top/stable";
    //             case ESeverType.TENCENT_PROD:
    //                 return "www.cad-crazyfish.top";
    //             default:
    //                 return "127.0.0.1";
    //         }
    //     }
    // }
    public static string Host
    {
        get
        {
            switch(ServerType)
            {
                case ESeverType.LOCAL_SERVER:
                    return "127.0.0.1";
                case ESeverType.TENCENT_DEV:
                    return "81.68.85.172/dev";
                case ESeverType.TENCENT_STABLE:
                    return "81.68.85.172/stable";
                case ESeverType.TENCENT_PROD:
                    return "81.68.85.172";
                default:
                    return "127.0.0.1";
            }
        }
    }
    

    public static int ApiPort
    {
        get
        {
            switch(ServerType)
            {
                case ESeverType.LOCAL_SERVER:
                    return 8088;
                case ESeverType.TENCENT_DEV:
                    return 0;
                case ESeverType.TENCENT_STABLE:
                    return 0;
                case ESeverType.TENCENT_PROD:
                    return 0;
                default:
                    return 8088;
            }
        }
    }

    public static byte[] StartUpKey = Encoding.UTF8.GetBytes("hello world");
    public static string ApiPath = "/fishgame/call";
    public static string VersionPath = "/assets/version";
    public static string DownloadPath = "/assets/json-output.zip";

    public static string HttpEndPoint
    {
        get
        {
            return HttpHost + ApiPath;
        }
    }

    public static string HttpVersionPoint
    {
        get
        {
            return HttpHost + VersionPath + "?ra=" + UnityEngine.Random.Range(0, int.MaxValue).ToString();
        }
    }

    public static string HttpDownloadPoint
    {
        get
        {
            return HttpHost + DownloadPath + "?ra=" + UnityEngine.Random.Range(0, int.MaxValue).ToString();
        }
    }

    public static string HttpHost
    {
        get
        {
            string httpProtocol = "https://";
            if (System.Text.RegularExpressions.Regex.IsMatch(Host, @"[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"))
            {
                httpProtocol = "http://";
            }

            if (ServerType == ESeverType.LOCAL_SERVER)
            {
                return httpProtocol + Host + ":" + ApiPort;
            }
            else
            {
                return httpProtocol + Host;
            }
        }
    }

    public static string MasterSavedPath
    {
        get
        {
            return Path.Combine( Application.persistentDataPath, "masterData");
        }
    }

    // 本地化
    public static LanguageDataTableProxy.LanguageMode languageMode = LanguageDataTableProxy.LanguageMode.CN;

}

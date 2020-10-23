using System.Text;
using System.IO;
using UnityEngine;

public class AppConst
{
    public static bool BundleMode;
    
    public static string RsaPublicKey = "<RSAKeyValue><Modulus>udy3x+fT95dgx/3SPYrIncR4LrSqPOEllKqID7Q/nQCLq/g/MkC2J0oO3HRl3rmgccxTecAXioemT2TV72w7NHRkw3JXn4AY+2moYy9Fx/ncLClYh1/+ieJmf9vJX8WyTe0+mFaUis4ShJCxtA6aUyaEqgKahFHgz1s0+1wvZyc=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

    public static bool EnableProtocolEncrypt = true;
    public static bool EnabledGameServices = true;

    

#if SERVER_TENCENT
    public static string Host = "www.cad-crazyfish.top";
    public static int ApiPort = 0;
#else
    public static string Host = "127.0.0.1";
    public static int ApiPort = 8088;
#endif

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

#if SERVER_TENCENT
            return httpProtocol + Host;
#else
            return httpProtocol + Host + ":" + ApiPort;
#endif
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
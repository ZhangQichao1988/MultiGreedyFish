using System.Text;

public class AppConst
{
    public static bool BundleMode;
    
    public static string RsaPublicKey = "<RSAKeyValue><Modulus>udy3x+fT95dgx/3SPYrIncR4LrSqPOEllKqID7Q/nQCLq/g/MkC2J0oO3HRl3rmgccxTecAXioemT2TV72w7NHRkw3JXn4AY+2moYy9Fx/ncLClYh1/+ieJmf9vJX8WyTe0+mFaUis4ShJCxtA6aUyaEqgKahFHgz1s0+1wvZyc=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

    public static bool EnableProtocolEncrypt = true;
    public static bool EnabledGameServices = false;

    

#if SERVER_TENCENT
    public static string Host = "www.cad-crazyfish.top";
    public static int ApiPort = 0;
#else
    public static string Host = "127.0.0.1";
    public static int ApiPort = 8088;
#endif

    public static byte[] StartUpKey = Encoding.UTF8.GetBytes("hello world");
    public static string ApiPath = "/fishgame/call";

    public static string HttpEndPoint
    {
        get
        {
            string httpProtocol = "https://";
            if (System.Text.RegularExpressions.Regex.IsMatch(Host, @"[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"))
            {
                httpProtocol = "http://";
            }
#if SERVER_TENCENT
            return httpProtocol + Host + ApiPath;
#else
            return httpProtocol + Host + ":" + ApiPort + ApiPath;
#endif
        }
    }

    // 本地化
    public static LanguageDataTableProxy.LanguageMode languageMode = LanguageDataTableProxy.LanguageMode.CN;

}
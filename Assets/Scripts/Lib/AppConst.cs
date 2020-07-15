using System.Text;

public class AppConst
{
    public static bool BundleMode;
    
    public static string RsaPublicKey = "<RSAKeyValue><Modulus>udy3x+fT95dgx/3SPYrIncR4LrSqPOEllKqID7Q/nQCLq/g/MkC2J0oO3HRl3rmgccxTecAXioemT2TV72w7NHRkw3JXn4AY+2moYy9Fx/ncLClYh1/+ieJmf9vJX8WyTe0+mFaUis4ShJCxtA6aUyaEqgKahFHgz1s0+1wvZyc=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

    public static bool EnableProtocolEncrypt = false;

    public static string Host = "127.0.0.1";
    public static int ApiPort = 8088;

    public static byte[] StartUpKey = Encoding.UTF8.GetBytes("hello world");

    public static string HttpEndPoint
    {
        get
        {
            string httpProtocol = "https://";
            if (System.Text.RegularExpressions.Regex.IsMatch(Host, @"[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"))
            {
                httpProtocol = "http://";
            }

            return httpProtocol + Host + ":" + ApiPort;
        }
    }
}
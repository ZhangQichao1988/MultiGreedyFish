using System.Text;

public class AppConst
{
    public static bool BundleMode;

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
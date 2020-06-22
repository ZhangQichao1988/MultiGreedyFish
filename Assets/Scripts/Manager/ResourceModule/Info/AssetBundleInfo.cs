using System.Collections.Generic;

public class AssetBundleInfo : ResourceBundleInfo
{
    public string[] Dependencies;
    public int EncryptKey;

    public AssetBundleInfo()
    {
        Dependencies = new string[] { };
    }

    public override string GetFilePath()
    {
        return "";
    }

    public string GetFilePathNoLang()
    {
        return "";
    }

    public override string GetDownloadPath(string version, string part)
    {
        return "";
    }
    
}
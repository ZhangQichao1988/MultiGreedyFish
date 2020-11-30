
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
        return string.Format("AssetBundles/{1}", Name + PathUtility.ASSETBUNDLE_EXTENSION);
    }

    public string GetFilePathNoLang()
    {
        return "AssetBundles/" + Name + PathUtility.ASSETBUNDLE_EXTENSION;
    }

    public override string GetDownloadPath(string version, string part)
    {
        return string.Format("{0}/{1}/{2}", version, PathUtility.GetPlatformName(), part);
    }
    
}
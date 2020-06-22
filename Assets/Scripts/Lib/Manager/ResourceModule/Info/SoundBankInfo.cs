public class SoundBankInfo : ResourceBundleInfo
{
    public override string GetFilePath()
    {
        return string.Format("SoundBanks/{0}/{1}", PathUtility.GetPlatformName(), Name.Replace("\\", "/") + PathUtility.SOUNDBANK_EXTENSION);
    }

    public string GetFilePathWithoutPlatform()
    {
        return string.Format("SoundBanks/{0}", Name.Replace("\\", "/") + PathUtility.SOUNDBANK_EXTENSION);
    }
    
    public override string GetDownloadPath(string version, string part)
    {
        return string.Format("{0}/{1}/{2}", version, PathUtility.GetPlatformName(), part);
    }
}
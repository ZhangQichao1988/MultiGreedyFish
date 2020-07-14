
public class ResourceBundleInfo
{
    public string Version;
    public string Name;
    public ulong Size;
    public string Hash;
    public float BundleSize;

    

    public virtual string GetFilePath()
    {
        return string.Empty;
    }

    
    public virtual string GetFilePathByLang(string lang)
    {
        return string.Empty;
    }
    

    public virtual string GetDownloadPath(string version, string part)
    {
        return string.Empty;
    }
}
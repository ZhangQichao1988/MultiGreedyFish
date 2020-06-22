
public class MasterDataInfo : ResourceBundleInfo
{
    public override string GetFilePath()
    {
        return "MasterData/master_data.db";
    }
    public static string MASTER_FILE_PATH = "MasterData/master_data.db";

    public override string GetDownloadPath(string version, string part)
    {
        return string.Format("{0}/{1}", version, part);
    }
}

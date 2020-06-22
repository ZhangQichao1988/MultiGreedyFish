
public class ResourceBundleInfo
{
    public string Version;
    public string Name;
    public ulong Size;
    public string Hash;
    public int Flags;

    public bool HasFlag(eResBundleFlags flag)
    {
        if(flag != eResBundleFlags.local)
        {
            return (Flags & (int)flag) > 0;
        }
        else
        {
            return !((Flags & (int)eResBundleFlags.remote) > 0);
        }
    }

    public void AddFlag(eResBundleFlags flag)
    {
        if (flag != eResBundleFlags.local)
        {
            Flags |= (int)flag;
        }
        else
        {
            Flags &= ~(int)eResBundleFlags.remote;
            Flags &= ~(int)eResBundleFlags.optional;
        }
    }

    public void RemoveFlag(eResBundleFlags flag)
    {
        if (flag != eResBundleFlags.local)
        {
            Flags &= ~(int)flag;
        }
        else
        {
            Flags |= (int)eResBundleFlags.remote;
        }
    }

    public virtual string GetFilePath()
    {
        return string.Empty;
    }
    

    public virtual string GetDownloadPath(string version, string part)
    {
        return string.Empty;
    }
}
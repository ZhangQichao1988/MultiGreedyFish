using UnityEngine;


public class AssetBundleRef
{
    public AssetBundle Bundle { get; }

    public int RefCount { get; private set; }

    private DesStream m_DesStream;

    public AssetBundleRef(AssetBundle assetBundle)
    {
        Bundle = assetBundle;

        RefCount = 1;
    }

    public AssetBundleRef(AssetBundle assetBundle, DesStream desStream)
    {
        Bundle = assetBundle;
        m_DesStream = desStream;

        RefCount = 1;
    }

    public int IncRef()
    {
        return ++RefCount;
    }

    public int DecRef()
    {
        return --RefCount;
    }

    public void Unload(bool unloadAllLoadedObjects)
    {
        if(Bundle != null)
        {
            Bundle.Unload(unloadAllLoadedObjects);
        }
        if (m_DesStream != null)
        {
            m_DesStream.Dispose();
        }
    }
}
using System.Collections.Generic;
using System.Linq;

public class BundlesManifest
{
    public Dictionary<string, AssetBundleInfo> AssetBundleInfos
    {
        get;
        private set;
    }

    public BundlesManifest(ResourceBundleManifest manifest)
    {
        AssetBundleInfos = new Dictionary<string, AssetBundleInfo>();

        foreach (ResourceBundleInfo packageInfo in manifest.PackageInfos.Values)
        {
            AssetBundleInfo bundleInfo = packageInfo as AssetBundleInfo;

            if(bundleInfo != null)
            {
                AssetBundleInfos.Add(bundleInfo.Name, bundleInfo);
            }
        }           
    }

    public bool Exist(string assetBundleName)
    {
        return AssetBundleInfos.Keys.Contains(assetBundleName);
    }

    public string[] GetAllAssetBundles()
    {
        return AssetBundleInfos.Keys.ToArray();
    }

    public string[] GetAllLocalAssetBundles()
    {
        return AssetBundleInfos.Values.Where(info =>
        {
            return info.HasFlag(eResBundleFlags.local);
        }).Select(info => info.Name).ToArray();
    }

    public string[] GetAllRemoteAssetBundles()
    {
        return AssetBundleInfos.Values.Where(info =>
        {
            return info.HasFlag(eResBundleFlags.remote);
        }).Select(info => info.Name).ToArray();
    }

    public string[] GetAllPreloadAssetBundles()
    {
        return AssetBundleInfos.Values.Where(info =>
        {
            return info.HasFlag(eResBundleFlags.preload);
        }).Select(info => info.Name).ToArray();
    }

    public string[] GetAllOptionalAssetBundles()
    {
        return AssetBundleInfos.Values.Where(info =>
        {
            return info.HasFlag(eResBundleFlags.optional);
        }).Select(info => info.Name).ToArray();
    }

    public string GetAssetBundleFileName(string assetBundleName)
    {
        string AssetBundleFileName;

        if (GetAssetBundleFileName(assetBundleName, out AssetBundleFileName))
        {
            return AssetBundleFileName;
        }
        return string.Empty;
    }

    public bool GetAssetBundleFileName(string assetBundleName, out string assetBundlePath)
    {
        AssetBundleInfo assetBundleInfo;

        if (AssetBundleInfos.TryGetValue(assetBundleName, out assetBundleInfo))
        {
            assetBundlePath = assetBundleInfo.GetFilePath();

            return true;
        }
        assetBundlePath = string.Empty;

        return false;
    }

    public string GetAssetBundleLoadPath(string assetBundleName, bool resrve = false)
    {
        AssetBundleInfo assetBundleInfo;

        if (AssetBundleInfos.TryGetValue(assetBundleName, out assetBundleInfo))
        {
#if UNITY_ANDROID
            return PathUtility.GetPersistentDataPath();
#else

            if (resrve)
            {
                if (assetBundleInfo.HasFlag(eResBundleFlags.local))
                {
                    return PathUtility.GetPersistentDataPath();
                }
                else if (assetBundleInfo.HasFlag(eResBundleFlags.remote))
                {
                    return PathUtility.GetStreamingAssetsPath();
                }
            }
            else
            {
                if (assetBundleInfo.HasFlag(eResBundleFlags.local))
                {
                    return PathUtility.GetStreamingAssetsPath();
                }
                else if (assetBundleInfo.HasFlag(eResBundleFlags.remote))
                {
                    return PathUtility.GetPersistentDataPath();
                }
            }
#endif
        }
        return string.Empty;
    }

    public int GetAssetBundleEncKey(string assetBundleName)
    {
        AssetBundleInfo assetBundleInfo;

        if (AssetBundleInfos.TryGetValue(assetBundleName, out assetBundleInfo))
        {
            return assetBundleInfo.EncryptKey;
        }
        return 0;
    }

    public string[] GetDirectDependencies(string assetBundleName)
    {
        AssetBundleInfo info;

        if (AssetBundleInfos.TryGetValue(assetBundleName, out info))
        {
            return info.Dependencies.ToArray();
        }
        return null;
    }

    public string[] GetAllDependencies(string assetBundleName)
    {
        HashSet<string> allDependencies = new HashSet<string>();

        GetAllDependencies(assetBundleName, ref allDependencies);

        return allDependencies.ToArray();
    }

    private void GetAllDependencies(string assetBundleName, ref HashSet<string> allDependencies)
    {
        string[] directDependencies = GetDirectDependencies(assetBundleName);

        foreach (string directDependency in directDependencies)
        {
            if (!allDependencies.Contains(directDependency))
            {
                allDependencies.Add(directDependency);

                GetAllDependencies(directDependency, ref allDependencies);
            }
        }
    }

}

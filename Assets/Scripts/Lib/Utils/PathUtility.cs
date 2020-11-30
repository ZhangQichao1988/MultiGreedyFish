using UnityEditor;
using System.IO;
using UnityEngine;

public class PathUtility
{
    public const string ASSETBUNDLE_EXTENSION = ".bdl";
    public const string SOUNDBANK_EXTENSION = ".bnk";

    public static string GetPersistentDataURL()
    {
        return "file://" + Application.persistentDataPath + "/";
    }

    public static string GetPersistentDataPath()
    {
        return Application.persistentDataPath + "/";
    }

    public static string GetStreamingAssetsURL()
    {
        if (Application.isEditor)
            return "file://" + System.Environment.CurrentDirectory.Replace("\\", "/") + "/Assets/StreamingAssets/";
        else if (Application.isMobilePlatform || Application.isConsolePlatform)
            return Application.streamingAssetsPath;
        else // For standalone player.
            return "file://" + Application.streamingAssetsPath;
    }

    public static string GetStreamingAssetsPath()
    {
        if (Application.isEditor)
            return System.Environment.CurrentDirectory.Replace("\\", "/") + "/Assets/StreamingAssets/";
        else if (Application.isMobilePlatform || Application.isConsolePlatform)
        {
            string path = string.Empty;
            switch (Application.platform)
            {
                case RuntimePlatform.IPhonePlayer:
                    path = Application.dataPath + "/Raw/";
                    break;
                case RuntimePlatform.Android:
                    path = Application.dataPath + "!assets/";
                    break;
                default:
                    path = Application.dataPath + "/StreamingAssets/";
                    break;
            }
            return path;
        }
        else // For standalone player.
            return Application.streamingAssetsPath + "/AssetBundles/";
    }

    public static string GetPlatformName()
    {
#if UNITY_EDITOR
        return GetPlatformName(EditorUserBuildSettings.activeBuildTarget);
#else
        return GetPlatformName(Application.platform);
#endif
    }

#if UNITY_EDITOR
    public static string GetPlatformName(BuildTarget buildTarget)
    {
        switch (buildTarget)
        {
            case BuildTarget.Android:
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return "Android";
            case BuildTarget.StandaloneOSX:
            case BuildTarget.iOS:
                return "iOS";
            case BuildTarget.WebGL:
                return "WebGL";
            // Add more build targets for your own.
            // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
            default:
                return null;
        }
    }
#endif
    public static string GetPlatformName(RuntimePlatform platform)
    {
        switch (platform)
        {
            case RuntimePlatform.Android:
                return "Android";
            case RuntimePlatform.IPhonePlayer:
                return "iOS";
            case RuntimePlatform.WebGLPlayer:
                return "WebGL";
            case RuntimePlatform.WindowsPlayer:
                return "Windows";
            case RuntimePlatform.OSXPlayer:
                return "OSX";
            // Add more build targets for your own.
            // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
            default:
                return null;
        }
    }

    public static bool AssetBundleFileNameToAssetBundleName(string assetBundleFileName, out string assetBundleName)
    {
        if(assetBundleFileName.EndsWith(ASSETBUNDLE_EXTENSION))
        {
            assetBundleName = assetBundleFileName.Remove(assetBundleFileName.LastIndexOf(ASSETBUNDLE_EXTENSION));

            return true;
        }
        assetBundleName = string.Empty;

        return false;
    }

    public static bool SoundBankFileNameToSoundBankName(string soundBankFileName, out string soundBankName)
    {
        if (soundBankFileName.EndsWith(SOUNDBANK_EXTENSION))
        {
            soundBankName = soundBankFileName.Remove(soundBankFileName.LastIndexOf(SOUNDBANK_EXTENSION));

            return true;
        }
        soundBankName = string.Empty;

        return false;
    }

    public static bool ResourcePathToBundlePath(string resourcePath, out string assetBundleName, out string assetName)
    {
        if (resourcePath.Contains("/") && !resourcePath.EndsWith("/"))
        {
            assetBundleName = ResPathToABName(resourcePath);

            assetName = ResPathToAssetName(resourcePath);

            return !string.IsNullOrEmpty(assetBundleName) && !string.IsNullOrEmpty(assetName);
        }
        else
        {
            Debug.LogWarning("Invalid resource path: " + resourcePath);
        }
        assetBundleName = string.Empty;

        assetName = string.Empty;

        return false;
    }

    public static string ResPathToABName(string resourcePath)
    {
        string bundleAssetPath = ResPathToBundleAssetPath(resourcePath);

        return AssetPathToABName(bundleAssetPath);
    }

    public static string ResPathToAssetName(string resourcePath)
    {
        int lastIndesOfSlash = resourcePath.LastIndexOf('/');

        if (lastIndesOfSlash != -1)
        {
            return resourcePath.Remove(0, lastIndesOfSlash + 1);
        }
        return string.Empty;
    }

    public static string ResPathToBundleAssetPath(string resourcePath)
    {
        return "Assets/Products/Resources/" + resourcePath;
    }

    public static string AssetPathToABName(string assetPath)
    {
        int lastIndesOfSlash = assetPath.LastIndexOf('/');

        if (lastIndesOfSlash != -1)
        {
            return assetPath.Substring(0, lastIndesOfSlash).ToLower();
        }
        return string.Empty;
    }

    public static string CalculateTransformPath(Transform transform)
    {
        if (transform == null)
        {
            return string.Empty;
        }

        string path = transform.name;
        while (transform.parent != null)
        {
            if(transform.parent == transform.root)
            {
                break;
            }
            path = transform.parent.name + "/" + path;
            transform = transform.parent;
        }
        return path;
    }

    public static string GetExistAssetPath(string persistentPath, ref string assetPath, bool isAssetBundle = true)
    {
        if (File.Exists(persistentPath)) 
        {
            return persistentPath;
        }
        else
        {
            string pathInner = Path.Combine(GetStreamingAssetsPath(), assetPath);
            if (File.Exists(pathInner))
            {
                return pathInner;
            }
            else if (isAssetBundle)
            {
                // get the base resource path\
                return Path.Combine(GetStreamingAssetsPath(), assetPath);
            }
            else
            {
                return pathInner;
            }
        }
    }
}
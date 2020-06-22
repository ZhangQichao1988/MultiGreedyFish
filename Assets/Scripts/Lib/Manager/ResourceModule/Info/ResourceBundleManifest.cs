using ICSharpCode.SharpZipLib.GZip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml;
using UnityEngine;

public class ResourceBundleManifest
{
    private string _version;

    public string Version
    {
        get
        {
            return _version;
        }
        private set
        {
            _version = value;
            NumVersion = long.Parse(_version.Split('-')[0]);
        }
    }
    public long NumVersion
    {
        get;
        private set;
    }

    public Dictionary<string, ResourceBundleInfo> PackageInfos
    {
        get;
        private set;
    }

    public ResourceBundleManifest()
    {
        PackageInfos = new Dictionary<string, ResourceBundleInfo>();
    }

    public ResourceBundleManifest(string resVersion)
    {
        Version = resVersion;
        PackageInfos = new Dictionary<string, ResourceBundleInfo>();
    }

    public void SetVersion(string resVersion)
    {
        Version = resVersion;
    }
    

    public bool WriteToStreamOrigin(Stream stream)
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));

            XmlElement elementVersion = doc.CreateElement("Version");
            elementVersion.SetAttribute("Res", Version.ToString());
            doc.AppendChild(elementVersion);

            XmlElement elementInfos = doc.CreateElement("Infos");
            elementVersion.AppendChild(elementInfos);

            foreach (ResourceBundleInfo resourceBundleInfo in PackageInfos.Values)
            {
                XmlElement elementInfo = doc.CreateElement("Info");

                if(resourceBundleInfo is SoundBankInfo)
                {
                    elementInfo.SetAttribute("Type", "SoundBank");
                }
                else if(resourceBundleInfo is AssetBundleInfo)
                {
                    elementInfo.SetAttribute("Type", "AssetBundle");
                }
                else if(resourceBundleInfo is MasterDataInfo)
                {
                    elementInfo.SetAttribute("Type", "MasterData");
                }

                elementInfo.SetAttribute("Name", resourceBundleInfo.Name);
                elementInfo.SetAttribute("Size", resourceBundleInfo.Size.ToString());
                elementInfo.SetAttribute("Hash", resourceBundleInfo.Hash);
                elementInfo.SetAttribute("Flags", resourceBundleInfo.Flags.ToString());
                elementInfo.SetAttribute("Version", resourceBundleInfo.Version);

                AssetBundleInfo assetBundleInfo = resourceBundleInfo as AssetBundleInfo;
                if(assetBundleInfo != null)
                {
                    elementInfo.SetAttribute("EncKey", assetBundleInfo.EncryptKey.ToString());
                    foreach (string directDependency in assetBundleInfo.Dependencies)
                    {
                        XmlElement directDependencyElement = doc.CreateElement("Dependency");
                        directDependencyElement.SetAttribute("Name", directDependency);

                        elementInfo.AppendChild(directDependencyElement);
                    }
                }
                elementInfos.AppendChild(elementInfo);
            }

            doc.Save(stream);
        }
        catch (Exception e)
        {
            Debug.LogException(e);

            return false;
        }
        return true;
    }

    public bool WriteToStream(Stream stream)
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));

            XmlElement elementVersion = doc.CreateElement("Version");
            elementVersion.SetAttribute("Res", Version.ToString());
            doc.AppendChild(elementVersion);

            XmlElement elementInfos = doc.CreateElement("Infos");
            elementVersion.AppendChild(elementInfos);

            foreach (ResourceBundleInfo resourceBundleInfo in PackageInfos.Values)
            {
                XmlElement elementInfo = doc.CreateElement("Info");

                if(resourceBundleInfo is SoundBankInfo)
                {
                    elementInfo.SetAttribute("Type", "SoundBank");
                }
                else if(resourceBundleInfo is AssetBundleInfo)
                {
                    elementInfo.SetAttribute("Type", "AssetBundle");
                }
                else if(resourceBundleInfo is MasterDataInfo)
                {
                    elementInfo.SetAttribute("Type", "MasterData");
                }

                elementInfo.SetAttribute("Name", resourceBundleInfo.Name);
                elementInfo.SetAttribute("Size", resourceBundleInfo.Size.ToString());
                elementInfo.SetAttribute("Hash", resourceBundleInfo.Hash);
                elementInfo.SetAttribute("Flags", resourceBundleInfo.Flags.ToString());
                elementInfo.SetAttribute("Version", resourceBundleInfo.Version);

                AssetBundleInfo assetBundleInfo = resourceBundleInfo as AssetBundleInfo;
                if(assetBundleInfo != null)
                {
                    elementInfo.SetAttribute("EncKey", assetBundleInfo.EncryptKey.ToString());
                    foreach (string directDependency in assetBundleInfo.Dependencies)
                    {
                        XmlElement directDependencyElement = doc.CreateElement("Dependency");
                        directDependencyElement.SetAttribute("Name", directDependency);

                        elementInfo.AppendChild(directDependencyElement);
                    }
                }
                elementInfos.AppendChild(elementInfo);
            }

            using (GZipOutputStream gzipStream = new GZipOutputStream(stream))
            {
                doc.Save(gzipStream);
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);

            return false;
        }
        return true;
    }

    public bool ReadFromStream(Stream stream)
    {
        try
        {
            XmlDocument doc = new XmlDocument();

            using (GZipInputStream decompressStream = new GZipInputStream(stream))
            {
                doc.Load(decompressStream);
            }

            XmlNode node_root = doc.SelectSingleNode("Version");
            if (node_root != null)
            {
                XmlAttribute resVersionAttribute = node_root.Attributes["Res"];
                if (resVersionAttribute != null)
                {
                    Version = resVersionAttribute.Value;
                }
                else
                {
                    return false;
                }

                XmlNode infosRoot = node_root.SelectSingleNode("Infos");
                if (infosRoot != null)
                {
                    XmlNodeList nodeList = infosRoot.SelectNodes("Info");
                    if (nodeList != null && nodeList.Count > 0)
                    {
                        for (int i = 0; i < nodeList.Count; i++)
                        {
                            XmlNode node = nodeList[i];

                            XmlAttribute typeAttribute = node.Attributes["Type"];
                            if(typeAttribute != null)
                            {
                                ResourceBundleInfo resourceBundleInfo;

                                if (typeAttribute.Value == "AssetBundle")
                                {
                                    AssetBundleInfo assetBundleInfo = new AssetBundleInfo();
                                    resourceBundleInfo = assetBundleInfo;

                                    XmlAttribute EncAttribute = node.Attributes["EncKey"];
                                    if (EncAttribute != null)
                                    {
                                        assetBundleInfo.EncryptKey = int.Parse(EncAttribute.Value);
                                    }

                                    XmlNodeList dependenciesNodeList = node.SelectNodes("Dependency");
                                    if (dependenciesNodeList != null && dependenciesNodeList.Count > 0)
                                    {
                                        string[] dependencies = new string[dependenciesNodeList.Count];
                                        for (int n = 0; n < dependenciesNodeList.Count; n++)
                                        {
                                            XmlNode dependencyNode = dependenciesNodeList[n];
                                            if (dependencyNode != null)
                                            {
                                                XmlAttribute dependencyAttribute = dependencyNode.Attributes["Name"];
                                                if (dependencyAttribute != null)
                                                {
                                                    dependencies[n] = dependencyAttribute.Value;
                                                }
                                            }
                                        }
                                        assetBundleInfo.Dependencies = dependencies;
                                    }
                                }
                                else if(typeAttribute.Value == "SoundBank")
                                {
                                    resourceBundleInfo = new SoundBankInfo();
                                }
                                else if(typeAttribute.Value == "MasterData")
                                {
                                    resourceBundleInfo = new MasterDataInfo();
                                }
                                else
                                {
                                    Debug.LogError("Unknown resource bundle type!");

                                    return false;
                                }

                                XmlAttribute PathAttribute = node.Attributes["Name"];
                                if (PathAttribute != null)
                                {
                                    resourceBundleInfo.Name = PathAttribute.Value;
                                }
                                XmlAttribute SizeAttribute = node.Attributes["Size"];
                                if (SizeAttribute != null)
                                {
                                    resourceBundleInfo.Size = ulong.Parse(SizeAttribute.Value);
                                }
                                XmlAttribute HashAttribute = node.Attributes["Hash"];
                                if (HashAttribute != null)
                                {
                                    resourceBundleInfo.Hash = HashAttribute.Value;
                                }
                                XmlAttribute FlagsAttribute = node.Attributes["Flags"];
                                if (FlagsAttribute != null)
                                {
                                    resourceBundleInfo.Flags = int.Parse(FlagsAttribute.Value);
                                }
                                XmlAttribute VersionAttribute = node.Attributes["Version"];
                                if (VersionAttribute != null)
                                {
                                    resourceBundleInfo.Version = VersionAttribute.Value;
                                }

                                if (!PackageInfos.ContainsKey(resourceBundleInfo.Name))
                                {
                                    PackageInfos.Add(resourceBundleInfo.Name, resourceBundleInfo);
                                }
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    Debug.LogError("Info NodeList is empty!");

                    return false;
                }
            }
            else
            {
                Debug.LogError("Version Node not found!");

                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);

            return false;
        }
        return true;
    }

    public string GetPackageHash(string packageName)
    {
        ResourceBundleInfo info;

        if (PackageInfos.TryGetValue(packageName, out info))
        {
            return info.Hash;
        }
        return string.Empty;
    }

    public string GetPackageFilePath(string resourceBundleName)
    {
        string AssetBundleFileName;

        if (GetPackageFilePath(resourceBundleName, out AssetBundleFileName))
        {
            return AssetBundleFileName;
        }
        return string.Empty;
    }

    public bool GetPackageFilePath(string resourceBundleName, out string packageFilePath)
    {
        ResourceBundleInfo packageInfo;

        if (PackageInfos.TryGetValue(resourceBundleName, out packageInfo))
        {
            packageFilePath = packageInfo.GetFilePath();

            return true;
        }
        packageFilePath = string.Empty;

        return false;
    }

    public string GetPackageLoadPath(string packageName, bool resver = false)
    {
        ResourceBundleInfo packageInfo;

        if (PackageInfos.TryGetValue(packageName, out packageInfo))
        {
#if UNITY_ANDROID
            return PathUtility.GetPersistentDataPath();
#elif UNITY_IOS || UNITY_EDITOR
            if (resver)
            {
                if (packageInfo.HasFlag(eResBundleFlags.local))
                {
                    return PathUtility.GetPersistentDataPath();
                }
                else if (packageInfo.HasFlag(eResBundleFlags.remote))
                {
                    return PathUtility.GetStreamingAssetsPath();
                }
            }
            else
            {
                if (packageInfo.HasFlag(eResBundleFlags.local))
                {
                    return PathUtility.GetStreamingAssetsPath();
                }
                else if (packageInfo.HasFlag(eResBundleFlags.remote))
                {
                    return PathUtility.GetPersistentDataPath();
                }
            }
#endif
        }
        return string.Empty;
    }
}

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
    public string AppVersion
    {
        get;
        private set;
    }

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

    public ResourceBundleManifest(string appVersion, string resVersion)
    {
        AppVersion = appVersion;
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
            elementVersion.SetAttribute("App", AppVersion);
            elementVersion.SetAttribute("Res", Version.ToString());
            doc.AppendChild(elementVersion);

            XmlElement elementInfos = doc.CreateElement("Infos");
            elementVersion.AppendChild(elementInfos);

            foreach (ResourceBundleInfo resourceBundleInfo in PackageInfos.Values)
            {
                XmlElement elementInfo = doc.CreateElement("Info");

                elementInfo.SetAttribute("Type", "AssetBundle");

                elementInfo.SetAttribute("Name", resourceBundleInfo.Name);
                elementInfo.SetAttribute("Size", resourceBundleInfo.Size.ToString());
                elementInfo.SetAttribute("Hash", resourceBundleInfo.Hash);
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

    public void SetAppVersion(string appVersion)
    {
        AppVersion = appVersion;
    }

    public bool WriteToStream(Stream stream)
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));

            XmlElement elementVersion = doc.CreateElement("Version");
            elementVersion.SetAttribute("App", AppVersion);
            elementVersion.SetAttribute("Res", Version.ToString());
            doc.AppendChild(elementVersion);

            XmlElement elementInfos = doc.CreateElement("Infos");
            elementVersion.AppendChild(elementInfos);

            foreach (ResourceBundleInfo resourceBundleInfo in PackageInfos.Values)
            {
                XmlElement elementInfo = doc.CreateElement("Info");

                elementInfo.SetAttribute("Type", "AssetBundle");

                elementInfo.SetAttribute("Name", resourceBundleInfo.Name);
                elementInfo.SetAttribute("Size", resourceBundleInfo.Size.ToString());
                elementInfo.SetAttribute("Hash", resourceBundleInfo.Hash);
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

            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;
            // rijndaelCipher.Key = AppConst.rgbKey;
            // rijndaelCipher.IV = AppConst.rgbIV;
            using (CryptoStream encrypStream = new CryptoStream(stream, rijndaelCipher.CreateEncryptor(), CryptoStreamMode.Write))
            {
                using (GZipOutputStream gzipStream = new GZipOutputStream(encrypStream))
                {
                    doc.Save(gzipStream);
                }
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

            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;
            // rijndaelCipher.Key = AppConst.rgbKey;
            // rijndaelCipher.IV = AppConst.rgbIV;
            using (CryptoStream decryptStream = new CryptoStream(stream, rijndaelCipher.CreateDecryptor(), CryptoStreamMode.Read))
            {
                using (GZipInputStream decompressStream = new GZipInputStream(decryptStream))
                {
                    doc.Load(decompressStream);
                }
            }

            XmlNode node_root = doc.SelectSingleNode("Version");
            if (node_root != null)
            {
                XmlAttribute appVersionAttribute = node_root.Attributes["App"];
                if (appVersionAttribute != null)
                {
                    AppVersion = appVersionAttribute.Value;
                }
                else
                {
                    return false;
                }

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

                                AssetBundleInfo assetBundleInfo = new AssetBundleInfo();
                                ResourceBundleInfo resourceBundleInfo = assetBundleInfo;

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
}

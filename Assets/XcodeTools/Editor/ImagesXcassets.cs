/**
 * Jackpot
 * Copyright(c) 2014 KLab, Inc. All Rights Reserved.
 * Proprietary and Confidential - This source code is not for redistribution
 * 
 * Subject to the prior written consent of KLab, Inc(Licensor) and its terms and
 * conditions, Licensor grants to you, and you hereby accept nontransferable,
 * nonexclusive limited right to access, obtain, use, copy and/or download
 * a copy of this product only for requirement purposes. You may not rent,
 * lease, loan, time share, sublicense, transfer, make generally available,
 * license, disclose, disseminate, distribute or otherwise make accessible or
 * available this product to any third party without the prior written approval
 * of Licensor. Unauthorized copying of this product, including modifications
 * of this product or programs in which this product has been merged or included
 * with other software products is expressly forbidden.
 */
using System.Collections;
using System.IO;
using System.Text;

namespace XcodeTools
{
    using Json = XMiniJSON;

    public class ImagesXcassets
    {
        string appIconAssetName = "AppIcon.appiconset";
        string appIconAssetJsonPath;
        Hashtable appIconContents;

        public ImagesXcassets(string path)
        {
            appIconAssetJsonPath = Path.Combine(path, appIconAssetName + "/Contents.json");
            string appIconContentsJson = ReadFromFile(appIconAssetJsonPath);
            appIconContents = (Hashtable) Json.jsonDecode(appIconContentsJson);
        }

        string ReadFromFile(string path)
        {
            return ReadFromFile(path, Encoding.UTF8);
        }

        string ReadFromFile(string path, Encoding encoding)
        {
            string result = string.Empty;
            using (StreamReader reader = new StreamReader(path, encoding))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        public void SaveAll()
        {
            WriteToFile(appIconAssetJsonPath, Json.jsonEncode(appIconContents));
        }

        public void WriteToFile(string path, string source)
        {
            WriteToFile(path, source, Encoding.UTF8);
        }

        static void WriteToFile(string path, string source, Encoding encoding)
        {
            using (StreamWriter writer = new StreamWriter(path, false, encoding))
            {
                writer.Write(source);
            }
        }

        /// <summary>
        /// Gets the AppIcon PreRendered.
        /// </summary>
        /// <returns><c>true</c>, if app icon pre rendered was gotten, <c>false</c> otherwise.</returns>
        public bool GetAppIconPreRendered()
        {
            string propertiesKey = "properties";
            string preRenderedKey = "pre-rendered";

            if (appIconContents.ContainsKey(propertiesKey))
            {
                var properties = (Hashtable) appIconContents[propertiesKey];
                if (properties.ContainsKey(preRenderedKey))
                {
                    return (bool) properties[preRenderedKey];
                }
            }

            return false;
        }

        /// <summary>
        /// Sets the AppIcon PreRendered.
        /// </summary>
        /// <param name="value">If set to <c>true</c> value.</param>
        public void SetAppIconPreRendered(bool value)
        {
            string propertiesKey = "properties";
            string preRenderedKey = "pre-rendered";

            Hashtable properties = new Hashtable();
            if (appIconContents.ContainsKey(propertiesKey))
            {
                properties = (Hashtable) appIconContents[propertiesKey];
                appIconContents.Remove(propertiesKey);
            }

            if (properties.ContainsKey(preRenderedKey))
            {
                properties.Remove(preRenderedKey);
            }

            properties.Add(preRenderedKey, value);
            appIconContents.Add(propertiesKey, properties);
        }
    }
}

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
using System.Collections.Generic;
using UnityEditor.iOS.Xcode;

namespace XcodeTools
{
    public class PlistFile
    {
        public PlistDocument Plist { get; protected set; }
        public string Path { get; protected set; }

        public PlistFile()
        {
            Plist = new PlistDocument();
        }

        public PlistFile(string path) : this()
        {
            Path = path;
        }

        public virtual void WriteToFile(string path)
        {
            Plist.WriteToFileWithPlistDtd(path);
        }

        public void WriteToFile()
        {
            WriteToFile(Path);
        }

        /// <summary>
        /// Sets the string.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public void SetString(string key, string value)
        {
            Plist.root.SetString(key, value);
        }

        /// <summary>
        /// Sets the boolean.
        /// </summary>
        /// <returns>The boolean.</returns>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public void SetBoolean(string key, bool value)
        {
            Plist.root.SetBoolean(key, value);
        }

        /// <summary>
        /// Sets the dict.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="dict">Dict.</param>
        public void SetDictionary(string key, Dictionary<string, object> dict)
        {
            PlistElementDict dictElement = Plist.root.CreateDict(key);
            foreach (var kvp in dict)
            {
                if (kvp.Value.GetType() == typeof(bool))
                {
                    dictElement.SetBoolean(kvp.Key, bool.Parse(kvp.Value.ToString()));
                }
                if (kvp.Value.GetType() == typeof(string))
                {
                    dictElement.SetString(kvp.Key, kvp.Value.ToString());
                }
                if (kvp.Value.GetType() == typeof(int))
                {
                    dictElement.SetInteger(kvp.Key, int.Parse(kvp.Value.ToString()));
                }
            }
        }

        /// <summary>
        /// Sets the string array.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="values">Values.</param>
        public void SetStringArray(string key, string[] values)
        {
            PlistElementArray stringArray;
            if (Plist.root.values.ContainsKey(key))
            {
                stringArray = Plist.root.values[key].AsArray();
            }
            else {
                stringArray = Plist.root.CreateArray(key);
            }
            foreach (var value in values)
            {
                var hasSameValue = false;
                foreach (var item in stringArray.values)
                {
                    if (value.Equals(item.AsString()))
                    {
                        hasSameValue = true;
                        break;
                    }
                }
                if (!hasSameValue)
                {
                    stringArray.AddString(value);
                }
            }
        }
    }
}

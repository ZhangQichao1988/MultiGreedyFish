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
using System.Linq;
using UnityEditor.iOS.Xcode;

namespace XcodeTools
{
    public class InfoPlist : PlistFile
    {
        public InfoPlist() : base()
        {
        }

        public InfoPlist(string path) : base(path)
        {
            if (System.IO.File.Exists(path))
            {
                Plist.ReadFromFile(path);
            }
        }

        public override void WriteToFile(string path)
        {
            Plist.WriteToFile(path);
        }

        /// <summary>
        /// Sets the CFBundleURLTypes.
        /// </summary>
        /// <param name="urlName">URL name.</param>
        /// <param name="urlSchemes">URL schemes.</param>
        public void SetCFBundleURLTypes(string urlName, string[] urlSchemes)
        {
            var targetDictKey = "CFBundleURLTypes";
            var targetKey = "CFBundleURLName";
            var targetValKey = "CFBundleURLSchemes";

            PlistElementArray cfBundleUrlTypes;
            if (Plist.root.values.ContainsKey(targetDictKey))
            {
                cfBundleUrlTypes = Plist.root.values[targetDictKey].AsArray();
            }
            else
            {
                cfBundleUrlTypes = Plist.root.CreateArray(targetDictKey);
            }

            PlistElementDict targetItem = null;
            foreach (var item in cfBundleUrlTypes.values)
            {
                if (item.AsDict()[targetKey].AsString() == urlName)
                {
                    targetItem = item.AsDict();
                }
            }

            if (targetItem == null)
            {
                targetItem = cfBundleUrlTypes.AddDict();
            }

            targetItem.SetString(targetKey, urlName);

            var targetValues = targetItem.CreateArray(targetValKey);
            foreach (var scheme in urlSchemes)
            {
                targetValues.AddString(scheme);
            }
        }

        /// <summary>
        /// 指定keyのStringArrayに格納されている指定valueを削除する
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>成否</returns>
        public bool RemoveStringArrayValue(string key, string value)
        {
            if (!Plist.root.values.ContainsKey(key))
            {
                return false;
            }
            PlistElementArray capabilities = Plist.root[key].AsArray();
            
            PlistElement result = capabilities.values.FirstOrDefault(x => x.AsString() == value);
            if (result == null)
            {
                return false;
            }
            
            capabilities.values.Remove(result);
            return true;
        }
    }
}

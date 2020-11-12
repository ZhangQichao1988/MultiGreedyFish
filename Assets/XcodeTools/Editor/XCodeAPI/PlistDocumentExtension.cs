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
using System.IO;
using System.Xml.Linq;
using System.Reflection;
using UnityEditor.iOS.Xcode;

namespace XcodeTools
{
    public static class PlistDocumentExtension
    {
        static readonly FieldInfo Root;
        static readonly FieldInfo Version;
        static readonly FieldInfo DocumentType;
        static readonly MethodInfo WriteElement;

        static PlistDocumentExtension()
        {
            var type = typeof(PlistDocument);
            var binding = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
            Root = type.GetField("root", binding);
            Version = type.GetField("version", binding);
            WriteElement = type.GetMethod("WriteElement", binding);
        }

        public static string WriteToStringWithPlistDtd(this PlistDocument self)
        {
            var root = (PlistElementDict)Root.GetValue(self);
            var el = (XElement)WriteElement.Invoke(self, new object[] { root });
            var rootEl = new XElement("plist");
            var version = (string)Version.GetValue(self);

            rootEl.Add(new XAttribute("version", version));
            rootEl.Add(el);

            var doc = new XDocument();
            doc.Add(rootEl);

            var documentType = self.GetType().GetField("documentType", BindingFlags.NonPublic | BindingFlags.Instance);
            documentType.SetValue(self, new XDocumentType("plist", "-//Apple//DTD PLIST 1.0//EN", "http://www.apple.com/DTDs/PropertyList-1.0.dtd", null));
            return self.WriteToString();
        }

        public static void WriteToFileWithPlistDtd(this PlistDocument self, string path)
        {
            File.WriteAllText(path, self.WriteToStringWithPlistDtd());
        }
    }
}

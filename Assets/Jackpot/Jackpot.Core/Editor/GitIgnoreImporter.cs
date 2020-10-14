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
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

namespace Jackpot
{
    public class GitIgnoreImporter : AssetPostprocessor
    {
        public static void OnPostprocessAllAssets(string[] importAssets, string[] deleteAssets, string[] movedAssets, string[] movedFromPath)
        {
            var libraries = Directory.GetDirectories(Application.dataPath + "/Jackpot/");

            foreach (var library in libraries)
            {
                var outputPath = library + "/.gitignore";
                if (!File.Exists(outputPath))
                {
                    var sb = new StringBuilder ();
                    sb.AppendLine ("/Packages/");
                    sb.AppendLine ("/Packages.meta");

                    File.WriteAllText (outputPath, sb.ToString());

                    Debug.Log("Generate .gitignore " + outputPath);
                }
            }
        }
    }
}
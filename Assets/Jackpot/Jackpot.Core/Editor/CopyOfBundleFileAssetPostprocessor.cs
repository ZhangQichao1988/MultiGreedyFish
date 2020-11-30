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

namespace Jackpot
{
    /// <summary>
    /// Copy of bundle file asset postprocessor.
    /// </summary>
    public class CopyOfBundleFileAssetPostprocessor : AssetPostprocessor
    {

        /// <summary>
        /// Raises the postprocess all assets event.
        /// </summary>
        /// <param name="importedAssets">Imported assets.</param>
        /// <param name="deletedAssets">Deleted assets.</param>
        /// <param name="movedAssets">Moved assets.</param>
        /// <param name="movedFromPath">Moved from path.</param>
        static void OnPostprocessAllAssets 
        (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
        {
#if UNITY5_SCRIPTING_IN_UNITY4
        var sourceBundleFileName = @"Assets/Jackpot/Jackpot.Core/Editor/Bundle/KJMUnityPlugins_32.bundle";
#else
            var sourceBundleFileName = @"Assets/Jackpot/Jackpot.Core/Editor/Bundle/KJMUnityPlugins_64.bundle";
#endif
            var sourceBundleFullPath = sourceBundleFileName + "/Contents/MacOS/KJMUnityPlugins";
            var destBundleFileName = @"Assets/Plugins/KJMUnityPlugins.bundle";
            var isimportedBundleFile = false;

            foreach (string asset in importedAssets)
            {
                // 比較対象をsourceBundleFileName + "/Contents/MacOS/KJMUnityPlugins"としているのは
                // Bundleファイルがディレクトリとして判断され実際にimportedAssetsとして入ってくるファイル名が
                // 差分のファイル名となる為
                if (asset == sourceBundleFullPath)
                {
                    isimportedBundleFile = true;
                }
            }
            
            if (!Directory.Exists(destBundleFileName) && File.Exists(sourceBundleFullPath))
            {
                // プロジェクト内のファイルが変更された場合に、Plugins内にbundleがない場合はコピーする
                // Coreを含んだパッケージのImport時にエラーが発生した場合にコピー処理が動かなくなるため
                isimportedBundleFile = true;
            }
            
            if (isimportedBundleFile)
            {
                try
                {
                    // Assets/Plugins/KJMUnityPlugins.bundleファイルが存在する場合は一旦削除
                    DirectoryInfo dir = new DirectoryInfo(destBundleFileName);
                    if (dir.Exists)
                    {
                        dir.Delete(true);
                    }

                    DirectoryCopy(sourceBundleFileName, destBundleFileName);
                    
                    Debug.Log("destBundleFileName copy = " + destBundleFileName + ",\nsourceBundleFileName paste = " + sourceBundleFileName);
                }
                catch (DirectoryNotFoundException e)
                {
                    UnityEngine.Debug.LogError(e.Message);
                }
            }
        }


        /// <summary>
        /// Directories the copy.
        /// </summary>
        /// <param name="sourceDirName">Source dir name.</param>
        /// <param name="destDirName">Destination dir name.</param>
        /// <param name="copySubDirs">If set to <c>true</c> copy sub dirs.</param>
        static void DirectoryCopy(string sourceDirName, string destDirName)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }
            DirectoryInfo[] dirs = dir.GetDirectories();
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                if (!File.Exists(temppath))
                {
                    file.CopyTo(temppath, false);
                }
            }

            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath);
            }
        }
    }
}

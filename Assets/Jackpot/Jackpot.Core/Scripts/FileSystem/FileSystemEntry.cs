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

using System;

namespace Jackpot
{
    /// <summary>
    /// FileSystemで利用されるファイル管理情報を保持するクラスです
    /// </summary>
    public class FileSystemEntry
    {
        #region General Constants

        /// <summary>
        /// 現在のFileSystemEntryのフォーマットのバージョンを示します
        /// </summary>
        public static readonly int CurrentFormat = 1;

        /// <summary>
        /// 現在のFileSystemEntryのフォーマットのバージョンでのカラム数を示します
        /// </summary>
        public static readonly int CurrentFormatColumn = 4;

        /// <summary>
        /// The command update.
        /// </summary>
        public static readonly string CommandModify = "M";

        /// <summary>
        /// The command delete.
        /// </summary>
        public static readonly string CommandRemove = "R";

        #endregion

        #region Properties

        /// <summary>
        /// 保存されているファイルの相対パスを示します
        /// </summary>
        /// <value>The path.</value>
        public string Path { get; private set; }

        /// <summary>
        /// 保存されているファイルにひもづけるタグ文字列を示します
        /// </summary>
        /// <value>The tag.</value>
        public string Tag { get; private set; }

        /// <summary>
        /// 保存されているファイルのバージョンを数値で示します
        /// 数値に変換できない場合、InvalidOperationExceptionが投げられます。
        /// </summary>
        /// <value>The version.</value>
        public int Version
        {
            get
            {
                int version;
                if (!Int32.TryParse(VersionString, out version))
                {
                    var errorMsg = string.Format("Version cannot parse to integer. Version is {0}", VersionString);
                    throw new InvalidOperationException(errorMsg);
                }
                return version;
            }
        }

        /// <summary>
        /// 保存されているファイルのバージョンを文字列で示します
        /// </summary>
        /// <value>The version.</value>
        public string VersionString { get; private set; }

        #endregion

        #region Constructor

        public FileSystemEntry(string path, string tag, int version) : this(path, tag, version.ToString())
        {
        }

        public FileSystemEntry(string path, string tag, string version)
        {
            Path = path;
            Tag = tag;
            VersionString = version;
        }

        public FileSystemEntry(FileSystemEntry entry)
        {
            Path = entry.Path;
            Tag = entry.Tag;
            VersionString = entry.VersionString;
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return string.Format("[FileSystemEntry: Path={0}, Tag={1}, Version={2}]", Path, Tag, VersionString);
        }

        /// <summary>
        /// csv形式で出力します。
        /// </summary>
        /// <returns>The string for csv.</returns>
        public string ToCsvString()
        {
            return string.Format("{0},{1},{2},{3}", CommandModify, Path, Tag, VersionString);
        }

        /// <summary>
        /// 削除の行をcsv形式で出力します。
        /// </summary>
        /// <returns>The string for csv.</returns>
        public string ToRemoveCsvString()
        {
            return string.Format("{0},{1},,", CommandRemove, Path);
        }

        #endregion
    }
}

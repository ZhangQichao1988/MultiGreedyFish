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
using System.Collections;
using System.Collections.Generic;

namespace Jackpot
{
    /// <summary>
    /// FileSystemで管理するFileSystemEntryを保持するToC(Table of Contents)クラスです。
    /// </summary>
    /// <remarks>
    /// このクラスはスレッドセーフではありません。
    /// このクラスを利用するオブジェクト上で、適宜クリティカルセクションを定義し、実装する必要があります。
    /// </remarks>
    public class FileSystemToc : IEnumerable<FileSystemEntry>
    {

        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="Jackpot.FileSystemToc"/> with the specified path.
        /// </summary>
        /// <param name="path">Path.</param>
        public FileSystemEntry this[string path]
        {
            get
            {
                return new FileSystemEntry(entries[path]);
            }
            set
            {
                Modify(value);
            }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count { get { return entries.Count; } }

        #endregion

        #region Fields

        Dictionary<string, FileSystemEntry> entries;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.FileSystemToc"/> class.
        /// </summary>
        public FileSystemToc()
        {
            entries = new Dictionary<string, FileSystemEntry>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 指定したFileSystemEntryに修正または追加します
        /// </summary>
        /// <param name="entry">Entry.</param>
        public void Modify(FileSystemEntry entry)
        {
            if (entries.ContainsKey(entry.Path))
            {
                entries[entry.Path] = new FileSystemEntry(entry);
            }
            else
            {
                entries.Add(entry.Path, new FileSystemEntry(entry));
            }
        }

        /// <summary>
        /// 指定したpathを持つFileSystemEntryが存在するかを示します
        /// </summary>
        /// <param name="path">Path.</param>
        public bool Contains(string path)
        {
            return entries.ContainsKey(path);
        }

        /// <summary>
        /// 指定したpathを持つFileSystemEntryを削除します。存在しなかった場合はfalseを返却します
        /// </summary>
        /// <param name="path">Path.</param>
        public bool Remove(string path)
        {
            return entries.Remove(path);
        }

        /// <summary>
        /// すべてのFileSystemEntryを削除します
        /// </summary>
        public void Clear()
        {
            entries.Clear();
        }

        #endregion

        #region IEnumerable Implementation

        public IEnumerator<FileSystemEntry> GetEnumerator()
        {
            return ((IEnumerable<FileSystemEntry>) entries.Values).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) entries.Values).GetEnumerator();
        }

        #endregion

    }
}


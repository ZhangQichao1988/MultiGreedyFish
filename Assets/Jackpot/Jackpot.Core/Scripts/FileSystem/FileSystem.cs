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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Jackpot
{
    /// <summary>
    /// ファイルの管理機能を提供します。このクラスはスレッドセーフです。
    /// </summary>
    public partial class FileSystem
    {
        #region Constants

        /// <summary>
        /// FileSystem.Modify実行時にtag指定されなかった時に適用されるデフォルトのtagです
        /// </summary>
        public static readonly string DefaultTag = string.Empty;

        static readonly ILogger Logger = Jackpot.Logger.Get<FileSystem>();
        static readonly string TocFileName = "toc";
        static readonly string backupFileName = "toc.bak";

        #endregion

        #region Properties

        /// <summary>
        /// FileSystemの識別子を示します
        /// </summary>
        /// <value>The identifier.</value>
        public string Id
        {
            get { return id; }
        }

        /// <summary>
        /// FileSystemに使用する基準となるディレクトリを示します
        /// </summary>
        /// <value>The base path.</value>
        public string BasePath
        {
            get { return basePath; }
        }

        /// <summary>
        /// FileSystemに使用するtocFileのパスを示します
        /// </summary>
        /// <value>The tocFile path.</value>
        public string TocFilePath
        {
            get { return tocFilePath; }
        }

        #endregion

        #region Fields

        readonly string id;
        readonly string basePath;
        readonly string tocFilePath;
        readonly string backupFilePath;
        readonly ReaderWriterLockSlim slim;
        bool disposed;
        FileSystemToc toc;
        StreamWriter streamWriter;

        #endregion

        #region Constructor Destructor

        public FileSystem(string id)
        {
            this.id = id;
            basePath = Platform.GetPersistentDirectoryPath(id);
            tocFilePath = FullPathCore(TocFileName);
            backupFilePath = FullPathCore(backupFileName);
            slim = new ReaderWriterLockSlim();
            disposed = false;
            RefreshToc();

            bool isCreate = !File.Exists(tocFilePath);
            streamWriter = new StreamWriter(tocFilePath, true);
            if (isCreate)
            {
                AppendText(string.Format("{0},{1}", FileSystemEntry.CurrentFormat,
                    FileSystemEntry.CurrentFormatColumn));
            }
        }

        ~FileSystem()
        {
            Dispose(false);
        }

        #endregion

        #region Public Method

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// 指定したpathで管理されているファイルのバージョンを返却します。ファイルが存在しない場合は-1が返却されます。
        /// 数値に変換できない場合、InvalidOperationExceptionが投げられます。
        /// </summary>
        /// <param name="path">Path.</param>
        public int Version(string path)
        {
            ValidateDisposed();
            slim.EnterReadLock();
            try
            {
                if (!toc.Contains(path) || toc[path] == null)
                {
                    Logger.Error("[{0}] Cannot find entry.  path: {1}", Id, path);
                    return -1;
                }

                return toc[path].Version;
            }
            finally
            {
                slim.ExitReadLock();
            }
        }

        /// <summary>
        /// 指定したpathで管理されているファイルのバージョンを文字列で返却します。
        /// ファイルが存在しない場合はnullが返却されます。
        /// </summary>
        /// <param name="path">Path.</param>
        public string VersionString(string path)
        {
            ValidateDisposed();
            slim.EnterReadLock();
            try
            {
                if (!toc.Contains(path) || toc[path] == null)
                {
                    Logger.Error("[{0}] Cannot find entry.  path: {1}", Id, path);
                    return null;
                }

                return toc[path].VersionString;
            }
            finally
            {
                slim.ExitReadLock();
            }
        }

        /// <summary>
        /// 指定したpathで管理されているファイルのタグを返却します。ファイルが存在しない場合はnullが返却されます。
        /// </summary>
        /// <param name="path">Path.</param>
        public string Tag(string path)
        {
            ValidateDisposed();
            slim.EnterReadLock();
            try
            {
                if (!toc.Contains(path) || toc[path] == null)
                {
                    Logger.Error("[{0}] Cannot find entry.  path: {1}", Id, path);
                    return null;
                }

                return toc[path].Tag;
            }
            finally
            {
                slim.ExitReadLock();
            }
        }

        /// <summary>
        /// 指定したpathで管理されているファイルのフルパスを返却します。ファイルが存在しない場合はstring.Emptyが返却されます。
        /// </summary>
        /// <returns>The path.</returns>
        /// <param name="path">Path.</param>
        public string FullPath(string path)
        {
            return FullPath(path, true);
        }

        /// <summary>
        /// 指定したpathで管理されるファイルのフルパスを返却します。管理されていないファイルのフルパスを取得したい場合はfalseを指定します。
        /// </summary>
        /// <remarks>
        /// ファイルを保存する前等、FileSystem上でファイルを管理する前に、事前に保存パスを取得したい場合、
        /// 引数strictlyにfalseを指定することで、ファイルチェックを行わずにパスを取得します。
        /// </remarks>
        /// <returns>The path.</returns>
        /// <param name="path">Path.</param>
        /// <param name="strictly">If set to <c>true</c> strictly.</param>
        public string FullPath(string path, bool strictly)
        {
            ValidateDisposed();
            slim.EnterReadLock();
            try
            {
                if (strictly && !toc.Contains(path))
                {
                    Logger.Error("[{0}] Cannot find entry. path: {1}", Id, path);
                    return string.Empty;
                }

                return FullPathCore(path);
            }
            finally
            {
                slim.ExitReadLock();
            }
        }

        /// <summary>
        /// 指定したpathで管理されているファイルの容量を返却します。ファイルが存在しない場合は0が返却されます。
        /// </summary>
        /// <returns>The size.</returns>
        /// <param name="path">Path.</param>
        public long UsedSize(string path)
        {
            ValidateDisposed();
            slim.EnterReadLock();
            try
            {
                if (!toc.Contains(path))
                {
                    Logger.Error("[{0}] Cannot find entry. path: {1}", Id, path);
                    return 0L;
                }

                var entry = toc[path];
                var fullPath = FullPathCore(entry.Path);
                var info = new FileInfo(fullPath);
                if (!info.Exists)
                {
                    Logger.Error("[{0}]{1} Cannot find file. FullPath: {2}", Id, entry, fullPath);
                    return 0L;
                }

                return info.Length;
            }
            finally
            {
                slim.ExitReadLock();
            }
        }

        /// <summary>
        /// 指定したtagで管理されているファイルのリストを返します。
        /// </summary>
        /// <returns>The size by tag.</returns>
        /// <param name="tag">Tag.</param>
        public ICollection<FileSystemEntry> Entries(string tag)
        {
            ValidateDisposed();
            slim.EnterReadLock();
            try
            {
                var tagged = toc.Where(entry => entry.Tag == tag);
                var results = new List<FileSystemEntry>();
                foreach (var entry in tagged)
                {
                    if (tag == entry.Tag)
                    {
                        results.Add(entry);
                    }
                }

                return results;
            }
            finally
            {
                slim.ExitReadLock();
            }
        }

        /// <summary>
        /// 指定したtagで管理されているファイルの容量の合計を返却します。
        /// </summary>
        /// <returns>The size by tag.</returns>
        /// <param name="tag">Tag.</param>
        public long UsedSizeByTag(string tag)
        {
            ValidateDisposed();
            slim.EnterReadLock();
            try
            {
                var result = 0L;
                var tagged = toc.Where(entry => entry.Tag == tag);
                foreach (var entry in tagged)
                {
                    var fullPath = FullPathCore(entry.Path);
                    var info = new FileInfo(fullPath);
                    if (!info.Exists)
                    {
                        Logger.Error("[{0}]{1} Cannot find file. FullPath: {2}", Id, entry, fullPath);
                        continue;
                    }

                    result += info.Length;
                }

                return result;
            }
            finally
            {
                slim.ExitReadLock();
            }
        }

        /// <summary>
        /// 指定したtagで管理されているファイルの容量の合計を返却します。
        /// </summary>
        /// <returns>The size by tags.</returns>
        /// <param name="tags">Tags.</param>
        public long UsedSizeByTags(ICollection<string> tags)
        {
            ValidateDisposed();
            if (tags == null || tags.Count <= 0)
            {
                return 0L;
            }

            slim.EnterReadLock();
            try
            {
                var result = 0L;
                var tagged = toc.Where(entry => tags.Contains(entry.Tag));
                foreach (var entry in tagged)
                {
                    var fullPath = FullPathCore(entry.Path);
                    var info = new FileInfo(fullPath);
                    if (!info.Exists)
                    {
                        Logger.Error("[{0}]{1} Cannot find file. FullPath: {2}", Id, entry, fullPath);
                        continue;
                    }

                    result += info.Length;
                }

                return result;
            }
            finally
            {
                slim.ExitReadLock();
            }
        }

        /// <summary>
        /// FileSystemで管理されているすべてのファイルの容量の合計を返却します。
        /// </summary>
        /// <returns>The size all.</returns>
        public long UsedSizeAll()
        {
            ValidateDisposed();
            slim.EnterReadLock();
            try
            {
                var result = 0L;
                foreach (var entry in toc)
                {
                    var fullPath = FullPathCore(entry.Path);
                    var info = new FileInfo(fullPath);
                    if (!info.Exists)
                    {
                        Logger.Error("[{0}]{1} Cannot find file. FullPath: {2}", Id, entry, fullPath);
                        continue;
                    }

                    result += info.Length;
                }

                return result;
            }
            finally
            {
                slim.ExitReadLock();
            }
        }

        /// <summary>
        /// 指定したpathで管理されているファイルの最終アクセス時間(Utc)を返却します。ファイルが存在しない場合はnullが返却されます。
        /// </summary>
        /// <returns>The access time.</returns>
        /// <param name="path">Path.</param>
        public DateTime? LastAccessTime(string path)
        {
            ValidateDisposed();
            slim.EnterReadLock();
            try
            {
                if (!path.Contains(path))
                {
                    Logger.Error("[{0}] Cannot find entry. path: {1}", Id, path);
                    return null;
                }

                var entry = toc[path];
                var fullPath = FullPathCore(entry.Path);
                var info = new FileInfo(fullPath);
                if (!info.Exists)
                {
                    Logger.Error("[{0}]{1} Cannot find file. FullPath: {2}", Id, entry, fullPath);
                    return null;
                }

                return info.LastAccessTimeUtc;
            }
            finally
            {
                slim.ExitReadLock();
            }
        }

        /// <summary>
        /// 指定したtagで管理されているファイルの最終アクセス時間(Utc)を返却します。ファイルが存在しないものは含まれません。
        /// </summary>
        /// <returns>The access time by tag.</returns>
        /// <param name="tag">Tag.</param>
        public ICollection<LastAccessTimePathSet> LastAccessTimeByTag(string tag)
        {
            ValidateDisposed();
            slim.EnterReadLock();
            try
            {
                var tagged = toc.Where(entry => entry.Tag == tag);
                var results = new List<LastAccessTimePathSet>();
                foreach (var entry in tagged)
                {
                    var fullPath = FullPathCore(entry.Path);
                    var info = new FileInfo(fullPath);
                    if (!info.Exists)
                    {
                        Logger.Error("[{0}]{1} Cannot find file. FullPath: {2}", Id, entry, fullPath);
                        continue;
                    }

                    results.Add(new LastAccessTimePathSet(entry.Path, info.LastAccessTimeUtc));
                }

                return results;
            }
            finally
            {
                slim.ExitReadLock();
            }
        }

        /// <summary>
        /// 指定したtagで管理されているファイルの最終アクセス時間(Utc)を返却します。ファイルが存在しないものは含まれません。
        /// </summary>
        /// <returns>The access time by tags.</returns>
        /// <param name="tags">Tags.</param>
        public ICollection<LastAccessTimePathSet> LastAccessTimeByTags(ICollection<string> tags)
        {
            ValidateDisposed();
            var results = new List<LastAccessTimePathSet>();
            if (tags == null || tags.Count <= 0)
            {
                return results;
            }

            slim.EnterReadLock();
            try
            {
                var tagged = toc.Where(entry => tags.Contains(entry.Tag));
                foreach (var entry in tagged)
                {
                    var fullPath = FullPathCore(entry.Path);
                    var info = new FileInfo(fullPath);
                    if (!info.Exists)
                    {
                        Logger.Error("[{0}]{1} Cannot find file. FullPath: {2}", Id, entry, fullPath);
                        continue;
                    }

                    results.Add(new LastAccessTimePathSet(entry.Path, info.LastAccessTimeUtc));
                }

                return results;
            }
            finally
            {
                slim.ExitReadLock();
            }
        }

        /// <summary>
        /// FileSystemで管理されている総てのファイルの最終アクセス時間(Utc)を返却します。
        /// </summary>
        /// <returns>The access time all.</returns>
        public ICollection<LastAccessTimePathSet> LastAccessTimeAll()
        {
            ValidateDisposed();
            slim.EnterReadLock();
            try
            {
                var results = new List<LastAccessTimePathSet>();
                foreach (var entry in toc)
                {
                    var fullPath = FullPathCore(entry.Path);
                    var info = new FileInfo(fullPath);
                    if (!info.Exists)
                    {
                        Logger.Error("[{0}]{1} Cannot find file. FullPath: {2}", Id, entry, fullPath);
                        continue;
                    }

                    results.Add(new LastAccessTimePathSet(entry.Path, info.LastAccessTimeUtc));
                }

                return results;
            }
            finally
            {
                slim.ExitReadLock();
            }
        }

        /// <summary>
        /// FileSystemで管理されているファイルの総数を返却します。
        /// </summary>
        public int Count()
        {
            ValidateDisposed();
            slim.EnterReadLock();
            try
            {
                return toc.Count;
            }
            finally
            {
                slim.ExitReadLock();
            }
        }

        /// <summary>
        /// 指定したtagで管理されているファイルの総数を示します
        /// </summary>
        /// <returns>The by tag.</returns>
        /// <param name="tag">Tag.</param>
        public int CountByTag(string tag)
        {
            ValidateDisposed();
            slim.EnterReadLock();
            try
            {
                return toc.Count(entry => entry.Tag == tag);
            }
            finally
            {
                slim.ExitReadLock();
            }
        }

        /// <summary>
        /// 指定したtagで管理されているファイルの総数を示します
        /// </summary>
        /// <returns>The by tags.</returns>
        /// <param name="tags">Tags.</param>
        public int CountByTags(ICollection<string> tags)
        {
            ValidateDisposed();
            if (tags == null || tags.Count <= 0)
            {
                return 0;
            }

            slim.EnterReadLock();
            try
            {
                return toc.Count(entry => tags.Contains(entry.Tag));
            }
            finally
            {
                slim.ExitReadLock();
            }
        }

        /// <summary>
        /// 指定したpathのファイルが管理されているかを示します。ファイルの有無は確認しません
        /// </summary>
        /// <param name="path">Path.</param>
        public bool Contains(string path)
        {
            return Contains(path, false);
        }

        /// <summary>
        /// 指定したpathのファイルが管理されているかを示します。strictlyがtrueの場合ファイルの有無も確認します
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="strictly">If set to <c>true</c> strictly.</param>
        public bool Contains(string path, bool strictly)
        {
            ValidateDisposed();
            slim.EnterReadLock();
            try
            {
                if (!toc.Contains(path))
                {
                    return false;
                }

                if (!strictly)
                {
                    return true;
                }

                return File.Exists(FullPathCore(toc[path].Path));
            }
            finally
            {
                slim.ExitReadLock();
            }
        }

        /// <summary>
        /// バージョンが一致しない場合にtrueを返却します。
        /// </summary>
        /// <returns><c>true</c>, if update was verifyed, <c>false</c> otherwise.</returns>
        /// <param name="path">Path.</param>
        /// <param name="version">Version.</param>
        public bool VerifyNeedUpdate(string path, int version)
        {
            return VerifyNeedUpdate(path, version.ToString());
        }

        public bool VerifyNeedUpdate(string path, string version)
        {
            ValidateDisposed();
            slim.EnterReadLock();
            try
            {
                if (!toc.Contains(path))
                {
                    return true;
                }

                var entry = toc[path];
                if (entry.VersionString != version)
                {
                    return true;
                }

                return false;
            }
            finally
            {
                slim.ExitReadLock();
            }
        }

        /// <summary>
        /// 指定したpathのファイルを管理下に置きます。ファイルが存在しない場合、管理下に置かれず、falseが返却されます
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="version">Version.</param>
        public bool Modify(string path, int version)
        {
            return Modify(path, DefaultTag, version.ToString());
        }

        public bool Modify(string path, string version)
        {
            return Modify(path, DefaultTag, version);
        }

        /// <summary>
        /// 指定したpathのファイルをtag指定で管理下に置きます。ファイルが存在しない場合、管理下に置かれず、falseが返却されます
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="version">Version.</param>
        public bool Modify(string path, string tag, int version)
        {
            return Modify(path, tag, version.ToString());
        }

        public bool Modify(string path, string tag, string version)
        {
            ValidateDisposed();
            // pathとtagにカンマが入ってないかチェック(入っていたら例外スロー)
            ValidateComma(path);
            ValidateComma(tag);

            slim.EnterWriteLock();
            try
            {
                var fullPath = FullPathCore(path);
                if (!File.Exists(fullPath))
                {
                    Logger.Warn("[{0}] Cannot find file. fullPath: {1}", Id, fullPath);
                    return false;
                }

                var entry = new FileSystemEntry(path, tag, version);
                toc.Modify(entry);
                // streamに更新の行を追加
                AppendText(entry.ToCsvString());
                return true;
            }
            finally
            {
                slim.ExitWriteLock();
            }
        }

        /// <summary>
        /// 指定したpathで管理されているファイルを削除します。そもそも管理されていなかった場合にfalseを返却します
        /// </summary>
        /// <param name="path">Path.</param>
        public bool Remove(string path)
        {
            ValidateDisposed();
            slim.EnterWriteLock();
            try
            {
                var result = false;
                if (toc.Contains(path))
                {
                    var entry = toc[path];
                    result = toc.Remove(entry.Path);
                    // streamに削除の行を追加
                    AppendText(entry.ToRemoveCsvString());
                }

                var fullPath = FullPathCore(path);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }

                return result;
            }
            finally
            {
                slim.ExitWriteLock();
            }
        }

        /// <summary>
        /// 指定したtagで管理されているファイルを削除し、削除されたファイルのpath一覧を返却します
        /// </summary>
        /// <returns>The by tag.</returns>
        /// <param name="tag">Tag.</param>
        public ICollection<string> RemoveByTag(string tag)
        {
            ValidateDisposed();
            slim.EnterWriteLock();
            try
            {
                var removings = toc.Where(entry => entry.Tag == tag).ToList();
                var results = new List<string>();
                foreach (var removing in removings)
                {
                    if (toc.Remove(removing.Path))
                    {
                        results.Add(removing.Path);
                        // streamに削除の行を追
                        AppendText(removing.ToRemoveCsvString());
                    }

                    var fullPath = FullPathCore(removing.Path);
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                }

                return results;
            }
            finally
            {
                slim.ExitWriteLock();
            }
        }

        /// <summary>
        /// 指定したtagで管理されているファイルを削除し、削除されたファイルのpathとtagをセットで返却します
        /// </summary>
        /// <returns>The by tags.</returns>
        /// <param name="tags">Tags.</param>
        public ICollection<TaggedPathSet> RemoveByTags(ICollection<string> tags)
        {
            ValidateDisposed();
            var results = new List<TaggedPathSet>();
            if (tags == null || tags.Count <= 0)
            {
                return results;
            }

            slim.EnterWriteLock();
            try
            {
                var tagged = toc.Where(entry => tags.Contains(entry.Tag)).ToList();
                foreach (var entry in tagged)
                {
                    if (toc.Remove(entry.Path))
                    {
                        results.Add(new TaggedPathSet(entry.Tag, entry.Path));
                        // streamに削除の行を追加
                        AppendText(entry.ToRemoveCsvString());
                    }

                    var fullPath = FullPathCore(entry.Path);
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                }

                return results;
            }
            finally
            {
                slim.ExitWriteLock();
            }
        }

        /// <summary>
        /// FileSystemで管理管理されているファイルの中で、ファイルが存在しないものを削除し、削除されたファイルのpath一覧を返却します
        /// </summary>
        /// <remarks>
        /// ファイルの存在有無を確認し、正確なファイル数等を取得したい場合、事前にこのメソッドを実行してください。
        /// </remarks>
        /// <returns>The not exist files.</returns>
        public ICollection<string> RemoveNotExistFiles()
        {
            ValidateDisposed();
            slim.EnterWriteLock();
            try
            {
                return RemoveNotExistFilesCore();
            }
            finally
            {
                slim.ExitWriteLock();
            }
        }

        /// <summary>
        /// FileSystemで管理されているファイルを総て削除します
        /// </summary>
        public void Clear()
        {
            ValidateDisposed();
            slim.EnterWriteLock();
            try
            {
                foreach (var entry in toc)
                {
                    // streamに削除の行を追加
                    AppendText(entry.ToRemoveCsvString());

                    var fullPath = FullPathCore(entry.Path);
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                }

                toc.Clear();
            }
            finally
            {
                slim.ExitWriteLock();
            }
        }

        /// <summary>
        /// 現在管理している内容をストレージに保存します。保存に失敗した場合falseを返却します
        /// </summary>
        public bool Flush()
        {
            // TODO:I/Fだけ残し
            return true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Dispose the specified disposing.
        /// </summary>
        /// <param name="disposing">If set to <c>true</c> disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            slim.EnterWriteLock();
            try
            {
                if (disposing)
                {
                    if (streamWriter != null)
                    {
                        streamWriter.Close();
                        streamWriter = null;
                    }
                }

                DeleteOnPool(Id);
                toc = null;
                disposed = true;
            }
            finally
            {
                slim.ExitWriteLock();
            }
        }

        /// <summary>
        /// Dispose実行済みだった場合に例外をスローします
        /// </summary>
        void ValidateDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException("FileSystem");
            }
        }

        /// <summary>
        /// RemoveNotExistFilesの共通実装。スレッドセーフでないので、クリティカルセクションは呼び出し側で定義すること
        /// </summary>
        /// <returns>The not exist files core.</returns>
        ICollection<string> RemoveNotExistFilesCore()
        {
            var results = new List<string>();
            var copied = toc.ToList();
            foreach (var entry in copied)
            {
                var fullPath = FullPathCore(entry.Path);
                if (!File.Exists(fullPath))
                {
                    results.Add(entry.Path);
                    if (toc.Remove(entry.Path))
                    {
                        // streamに削除の行を追加
                        AppendText(entry.ToRemoveCsvString());
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// FullPathへの変換処理の共通実装
        /// </summary>
        /// <returns>The path core.</returns>
        /// <param name="path">Path.</param>
        string FullPathCore(string path)
        {
            var combined = string.IsNullOrEmpty(path) ? BasePath : Path.Combine(BasePath, path);
            return Path.GetFullPath(combined);
        }

        /// <summary>
        /// tocファイルの更新処理を行います
        /// </summary>
        void RefreshToc()
        {
            slim.EnterWriteLock();
            try
            {
                toc = new FileSystemToc();

                // tocファイルが開けなかったらバックアップから開く
                if (!LoadToc(tocFilePath))
                {
                    if (File.Exists(tocFilePath))
                    {
                        File.Delete(tocFilePath);
                    }

                    // バックアップファイルも読み込み失敗した場合は削除して処理終了
                    if (!LoadToc(backupFilePath))
                    {
                        if (File.Exists(backupFilePath))
                        {
                            File.Delete(backupFilePath);
                        }

                        return;
                    }
                }

                // 最新のフォーマットでtocファイル作成
                var newTocFilePath = FullPathCore("toc.new");
                // 一時ファイル(toc.new)を書き込み用に開き、現状の辞書の内容を出力する
                using (var stream = new FileStream(newTocFilePath, FileMode.Create, FileAccess.Write))
                {
                    using (var sw = new StreamWriter(stream))
                    {
                        // 先頭行にバージョンとカラム数書き込み
                        sw.WriteLine(string.Format("{0},{1}", FileSystemEntry.CurrentFormat,
                            FileSystemEntry.CurrentFormatColumn));
                        foreach (var entry in toc)
                        {
                            sw.WriteLine(entry.ToCsvString());
                        }
                    }
                };

                //ファイルの置き換え
                // toc を toc.bak に
                MoveFile(tocFilePath, backupFilePath);

                // toc.new を toc に
                MoveFile(newTocFilePath, tocFilePath);
            }
            finally
            {
                slim.ExitWriteLock();
            }
        }

        /// <summary>
        /// FileSystemに紐づくentry一覧(toc)を読み込み, entitiesに格納します。
        /// </summary>
        /// <remarks>
        /// </remarks>
        bool LoadToc(string tocPath)
        {
            if (toc != null)
            {
                toc.Clear();
            }

            if (!File.Exists(tocPath))
            {
                return false;
            }

            var compLoadToc = false;
            // tocファイルを読み込みモードで開いてFileSystemTocを生成
            using (var stream = new FileStream(tocPath, FileMode.Open, FileAccess.Read))
            {
                using (var streamReader = new StreamReader(stream))
                {
                    try
                    {
                        var columnNum = 0;

                        // 開いたファイルの1行目からカラム数取得
                        var firstLine = streamReader.ReadLine();
                        if (firstLine == null)
                        {
                            return false;
                        }

                        var firstLineContents = firstLine.Split(',');
                        if (firstLineContents.Length == 2)
                        {
                            columnNum = Int32.Parse(firstLineContents[1]);
                        }
                        // 1行目が読めなかったら処理を終了
                        else
                        {
                            return false;
                        }

                        // 2行目以降を順次読み
                        while (!streamReader.EndOfStream)
                        {
                            var currentLine = streamReader.ReadLine();
                            if (currentLine == null)
                            {
                                continue;
                            }

                            var lineContents = currentLine.Split(',');

                            // 要素数とカラム数が一致しているか判定
                            if (columnNum == lineContents.Length)
                            {
                                if (FileSystemEntry.CommandModify.Equals(lineContents[0]))
                                {
                                    toc.Modify(new FileSystemEntry(lineContents[1], lineContents[2], lineContents[3]));
                                }
                                // 削除の行の場合はtocから除外する
                                else if (FileSystemEntry.CommandRemove.Equals(lineContents[0]))
                                {
                                    toc.Remove(lineContents[1]);
                                }
                            }
                        }

                        compLoadToc = true;
                    }
                    catch (Exception e)
                    {
                        Logger.Info("[{0}] Falied to load {1} : {2}", Id, tocPath, e.Message);
                    }
                }
            };

            return compLoadToc;
        }

        /// <summary>
        /// ファイルを移動します。
        /// </summary>
        /// <param name="sourcefile">Sourcefile.</param>
        /// <param name="targetFile">Target file.</param>
        void MoveFile(string sourcefile, string targetFile)
        {
            // 移動元ファイルが無ければ何もしない
            if (!File.Exists(sourcefile))
            {
                return;
            }

            // 移動先ファイルが存在する場合は削除
            if (File.Exists(targetFile))
            {
                File.Delete(targetFile);
            }

            File.Move(sourcefile, targetFile);
        }

        /// <summary>
        /// 対象文字列に`,`が入っている場合は例外をスローします
        /// </summary>
        /// <returns><c>true</c>, if comma was validated, <c>false</c> otherwise.</returns>
        /// <param name="target">Target.</param>
        void ValidateComma(string target)
        {
            if (target.IndexOf(",", StringComparison.CurrentCulture) > 0)
            {
                throw new ArgumentException("Commma can not be used.");
            }
        }

        /// <summary>
        /// StreamWriterに指定の文字列を追記します
        /// スレッドセーフでないので、クリティカルセクションは呼び出し側で定義すること
        /// また、streamWriter.WriteLine()、streamWriter.Flush()の例外も呼び出し側で処理すること
        /// </summary>
        /// <param name="text">Text.</param>
        void AppendText(string text)
        {
            streamWriter.WriteLine(text);
            streamWriter.Flush();
        }

        #endregion
    }
}

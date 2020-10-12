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
using System;
using System.Collections;

namespace Jackpot
{
    /// <summary>
    /// バージョン番号を扱うクラスです
    /// </summary>
    public class Version
    {
        /// <summary>
        /// 内部プロパティのTuple
        /// </summary>
        readonly Tuple<int, int, int, int> versionTuple;

        /// <summary>
        /// バージョン番号の文字列
        /// </summary>
        readonly string versionString;

        #region properties
        /// <summary>
        /// Majorバージョン
        /// </summary>
        /// <value>The major.</value>
        public int Major 
        { 
            get 
            {
                return versionTuple.Item1;
            }
        }

        /// <summary>
        /// Minorバージョン
        /// </summary>
        /// <value>The minor.</value>
        public int Minor 
        { 
            get
            {
                return versionTuple.Item2;
            }
        }

        /// <summary>
        /// Revisionバージョン
        /// </summary>
        /// <value>The revision.</value>
        public int Revision 
        { 
            get
            {
                return versionTuple.Item3;
            }
        }

        /// <summary>
        /// Buildバージョン
        /// </summary>
        /// <value>The build.</value>
        public int Build 
        { 
            get
            {
                return versionTuple.Item4;
            }
        }
        #endregion

        #region public methods
        /// <summary>
        /// 引数versionStringを元に生成したVersionクラスを返却します
        /// </summary>
        /// <remarks>
        /// "."と":"をセパレータとして、引数versionStringを下記のように分割。
        /// ・分割した1つ目：Major
        /// ・分割した2つ目：Minor
        /// ・分割した3つ目：Revision
        /// ・分割した4つ目：Build
        /// 上記4要素を元にVersionクラスを生成いて返却します
        /// </remarks>
        /// <param name="versionString">Version string.</param>
        public static Version Of(string versionString)
        {
            var major = 0;
            var minor = 0;
            var revision = 0;
            var build = 0;
            var separators = new []{ ".", ":" };
            var splitted = versionString.Split(separators, System.StringSplitOptions.None);

            if (splitted.Length >= 1)
            {
                int.TryParse(splitted[0], out major);
            }

            if (splitted.Length >= 2)
            {
                int.TryParse(splitted[1], out minor);
            }
            else
            {
                return new Version(major);
            }

            if (splitted.Length >= 3)
            {
                int.TryParse(splitted[2], out revision);
            }
            else
            {
                return new Version(major, minor);
            }

            if (splitted.Length >= 4)
            {
                int.TryParse(splitted[3], out build);
            }
            else
            {
                return new Version(major, minor, revision);
            }

            return new Version(major, minor, revision, build);
        }

        /// <summary>
        /// 引数objとバージョンが一致するかどうかを判定します
        /// </summary>
        /// <remarks>
        /// Major->Minor->Revision->Buildの順でバージョン比較を行います
        /// </remarks>
        /// <param name="obj">Obj.</param>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var thatVersion = obj as Version;
            if (thatVersion == null)
            {
                if (!(obj is string))
                {
                    return false;
                }
                try
                {
                    thatVersion = Version.Of(obj as string);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return versionTuple.Equals(thatVersion.versionTuple);
        }

        /// <summary>
        /// バージョン比較について、比較演算子【<】を定義
        /// </summary>
        /// <param name="left">Left.</param>
        /// <param name="right">Right.</param>
        public static bool operator <(Version left, Version right) 
        {
            return ((IComparable) left.versionTuple).CompareTo(right.versionTuple) < 0;
        }

        /// <summary>
        /// バージョン比較について、比較演算子【<=】を定義
        /// </summary>
        /// <param name="left">Left.</param>
        /// <param name="right">Right.</param>
        public static bool operator <=(Version left, Version right) 
        {
            return ((IComparable) left.versionTuple).CompareTo(right.versionTuple) <= 0;
        }

        /// <summary>
        /// バージョン比較について、比較演算子【>】を定義
        /// </summary>
        /// <param name="left">Left.</param>
        /// <param name="right">Right.</param>
        public static bool operator >(Version left, Version right) 
        {
            return ((IComparable) left.versionTuple).CompareTo(right.versionTuple) > 0;
        }

        /// <summary>
        /// バージョン比較について、比較演算子【>=】を定義
        /// </summary>
        /// <param name="left">Left.</param>
        /// <param name="right">Right.</param>
        public static bool operator >=(Version left, Version right) 
        {
            return ((IComparable) left.versionTuple).CompareTo(right.versionTuple) >= 0;
        }

        /// <summary>
        /// バージョン比較について、比較演算子【==】を定義
        /// </summary>
        /// <param name="left">Left.</param>
        /// <param name="right">Right.</param>
        public static bool operator ==(Version left, Version right) 
        {
            return left.Equals(right);
        }

        /// <summary>
        /// バージョン比較について、比較演算子【!=】を定義
        /// </summary>
        /// <param name="left">Left.</param>
        /// <param name="right">Right.</param>
        public static bool operator !=(Version left, Version right) 
        {
            return !left.Equals(right);
        }
            
        /// <summary>
        /// ハッシュコードを返却します。
        /// </summary>
        /// <remarks>
        /// プロパティ値を元に決定されるハッシュコードを返却します。
        /// 参考：https://msdn.microsoft.com/en-us/library/ms182358.aspx
        /// </remarks>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// バージョン番号を{Major}.{Minor}.{Revision}.{Build}の形式の文字列で返却します
        /// </summary>
        public override string ToString()
        {
            return versionString;
        }

        #endregion

        #region private methods
        /// <summary>
        /// 引数major指定のコンストラクタ
        /// </summary>
        /// <param name="major">Major.</param>
        protected Version(int major) : this(major, 0, 0, 0, string.Format("{0}", major))
        {
        }

        /// <summary>
        /// 引数major,minor指定のコンストラクタ
        /// </summary>
        /// <param name="major">Major.</param>
        /// <param name="minor">Minor.</param>
        protected Version(int major, int minor) : this(major, minor, 0, 0, string.Format("{0}.{1}", major, minor))
        {
        }

        /// <summary>
        /// 引数major,minor,revision指定のコンストラクタ
        /// </summary>
        /// <param name="major">Major.</param>
        /// <param name="minor">Minor.</param>
        /// <param name="revision">Revision.</param>
        protected Version(int major, int minor, int revision) : this(major, minor, revision, 0, string.Format("{0}.{1}.{2}", major, minor, revision))
        {
        }

        /// <summary>
        /// 引数major,minor,revision,build指定のコンストラクタ
        /// </summary>
        /// <param name="major">Major.</param>
        /// <param name="minor">Minor.</param>
        /// <param name="revision">Revision.</param>
        /// <param name="build">Build.</param>
        protected Version(int major, int minor, int revision, int build) : this(major, minor, revision, build, string.Format("{0}.{1}.{2}.{3}", major, minor, revision, build))
        {
        }

        /// <summary>
        /// 引数major,minor,revision,build,versionString指定のprivateコンストラクタ
        /// </summary>
        /// <param name="major">Major.</param>
        /// <param name="minor">Minor.</param>
        /// <param name="revision">Revision.</param>
        /// <param name="build">Build.</param>
        /// <param name="versionString">VersionString.</param>
        Version(int major, int minor, int revision, int build, string versionString)
        {
            this.versionString = versionString;
            this.versionTuple = Tuple.Create(major, minor, revision, build);
        }
        #endregion
    }
}

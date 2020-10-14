#if UNITY_EDITOR

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Jackpot
{
    public class SetAArByVersion
    {
        /// <summary>
        /// 移動元パス
        /// </summary>
        const string SRC_DIR = @"Assets/Jackpot/Jackpot.Core/Editor/Plugins/Android/";

        /// <summary>
        /// 移動先パス
        /// </summary>
        const string DEST_DIR = @"Assets/Plugins/Android/";

        /// <summary>Unity5系用のファイルパス</summary>
        static readonly string unity5AarPath = SRC_DIR + @"JackpotCore.aar.5";

        /// <summary>Unity2017系用のファイルパス</summary>
        static readonly string unity2017AarPath = SRC_DIR + @"JackpotCore.aar.2017";

        /// <summary>リネーム先のファイルパス</summary>
        static readonly string renamePath = DEST_DIR + @"JackpotCore.aar";

        [MenuItem("Jackpot/Set AAR")]
        static void SetAAR()
        {
            SetAArByVersion setAAr = new SetAArByVersion();
            setAAr.SetupAar(setAAr.GetPath());
        }

        public string GetRenamePath()
        {
            return renamePath;
        }

        public string GetPath()
        {
#if UNITY_5
            return unity5AarPath;
#endif
#if UNITY_2017_1_OR_NEWER
            return unity2017AarPath;
#endif
        }

        public void SetupAar(string path)
        {
            // 社内にUnity5系の案件が無くなったのでこの処理は不要だけど、
            // 削除するとファイルがそのまま残ってしまうので処理を消して上書きする
        }
    }
}

#endif

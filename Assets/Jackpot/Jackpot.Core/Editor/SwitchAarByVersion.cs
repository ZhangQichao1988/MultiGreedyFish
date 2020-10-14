#if UNITY_EDITOR

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SwitchAarByVersion : AssetPostprocessor
{

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        // 社内にUnity5系の案件が無くなったのでこの処理は不要だけど、
        // 削除するとファイルがそのまま残ってしまうので処理を消して上書きする
    }
}

#endif

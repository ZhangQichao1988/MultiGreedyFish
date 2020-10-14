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
using System.Collections.Generic;
using UnityEditor;

namespace Jackpot
{
    /// <summary>
    /// インスペクタの各項目を畳み込む為のジェネリクスヘルパーです
    /// </summary>
    /// <remarks>
    /// このクラスは、インスペクタに表示するフィールドがリスト、辞書、ハッシュセットなどのコレクションを持つ時に
    /// 各要素を畳み込みで表示できるよう補助します
    /// <code>
    /// public class Bar : MonoBehaviour
    /// {
    ///     public List<Foo> fooList = new List<Foo>();
    ///
    ///     // ...
    /// }
    ///
    /// [CustomEditor(typeof(Bar))]
    /// public class BarEditor : Editor
    /// {
    ///     FoldoutCache<Foo> foldouts = new FoldoutCache<Foo>();
    ///
    ///     public override void OnInspectorGUI()
    ///     {
    ///         var list = (target as Bar).fooList;
    ///
    ///         foreach (var foo in list)
    ///         {
    ///            if (foldouts.Foldout(foo))
    ///            {
    ///               // Foo インスタンスについてのインスペクタ描画をここに書く
    ///            }
    ///         }
    ///
    ///         foldouts.Flush(); // 終わりに Flush() を呼び、次回描画時の為に内部を更新させます
    ///     }
    /// }
    /// </code>
    /// </remarks>
    public class FoldoutCache<T>
    {
        /// <summary>
        /// デフォルトでの項目の開閉を示します
        /// </summary>
        /// <value><c>true</c> の場合畳み込みは開いた状態、 <c>false</c> の場合は閉じた状態として表示します</value>
        public bool IsDefaultOpen { get; private set; }

        readonly Dictionary<T, bool> byLastUpdate = new Dictionary<T, bool>();

        readonly Dictionary<T, bool> toNextUpdate = new Dictionary<T, bool>();

        public FoldoutCache() : this(false)
        {
        }

        public FoldoutCache(bool defaultSetting)
        {
            IsDefaultOpen = defaultSetting;
        }

        /// <summary>
        /// 前回描画時からの項目の畳み込み指定を引き継いで、インスペクタにGUIを表示します
        /// </summary>
        /// <param name="key">表示する項目の本体</param>
        /// <param name="content">インスペクタに表示されるテキスト</param>
        /// <remarks>
        /// 前回描画時に無いキーがあった場合、デフォルトの開閉指定を用いり、新たに設定を追加します
        /// </remarks>
        public bool Foldout(T key, string content)
        {
            var isFold = byLastUpdate.ContainsKey(key) ? byLastUpdate[key] : IsDefaultOpen;
            isFold = EditorGUILayout.Foldout(isFold, content);
            toNextUpdate.Add(key, isFold);
            return isFold;
        }

        /// <summary>
        /// 項目の畳み込み指定を次回の描画用に引き継ぎます
        /// </summary>
        public void Flush()
        {
            byLastUpdate.Clear();
            foreach (var setting in toNextUpdate)
            {
                byLastUpdate.Add(setting.Key, setting.Value);
            }
            toNextUpdate.Clear();
        }
    }
}

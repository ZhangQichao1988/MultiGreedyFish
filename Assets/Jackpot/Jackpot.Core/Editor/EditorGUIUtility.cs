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
using UnityEngine;
using UnityEditor;

namespace Jackpot
{
    public static class EditorGUIUtility
    {
        public static bool DrawHeader(string text, bool force = false)
        {
            return DrawHeader(text, text, force);
        }

        public static bool DrawHeader(string text, string key, bool force = false)
        {
            bool state = EditorPrefs.GetBool(key, true);

            GUILayout.Space(3f);

            if (!force && !state)
                GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);

            GUILayout.BeginHorizontal();
            GUI.changed = false;

            text = "<b><size=11>" + text + "</size></b>";

            if (state)
                text = "\u25BC " + text;
            else
                text = "\u25BA " + text;

            if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f)))
                state = !state;

            if (GUI.changed)
                EditorPrefs.SetBool(key, state);

            GUILayout.Space(2f);
            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;

            if (!force && !state)
                GUILayout.Space(3f);

            return state;
        }

        public static bool DrawFoldout(string text, string key)
        {
            bool state = EditorPrefs.GetBool(key, false);

            state = EditorGUILayout.Foldout(state, text);

            if (GUI.changed)
                EditorPrefs.SetBool(key, state);

            return state;
        }

        public static void BeginContents()
        {
            EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(10f));
            GUILayout.Space(10f);
            GUILayout.BeginVertical();
            GUILayout.Space(2f);
        }

        public static void EndContents()
        {
            GUILayout.Space(3f);
            GUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(3f);
        }

    }
}


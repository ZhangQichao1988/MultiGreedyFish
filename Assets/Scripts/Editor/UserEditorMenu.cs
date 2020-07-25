using UnityEditor;
using UnityEngine;


public class UserEditorMenu
{
    [MenuItem("UserEditor/ClearPrefabs")]
    public static void ClearPlayerPrefabs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
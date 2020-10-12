using UnityEditor;

namespace XcodeTools.XcodeUtility
{
    public static class XcodeDefaultSettingsExtensions
    {
        public static void SetAutomaticallySign(bool value)
        {
            EditorPrefs.SetBool("DefaultiOSAutomaticallySignBuild", value);
        }

        public static bool GetAutomaticallySign()
        {
            return EditorPrefs.GetBool("DefaultiOSAutomaticallySignBuild", true);
        }
    }
}

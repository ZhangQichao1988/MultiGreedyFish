using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(ResourceManager))]
internal sealed class ResourceManagerInspector : UnityEditor.Editor
{
	private ReorderableList m_AssetsReorderableList;
	private bool m_AssetsFoldout;

	private ReorderableList m_BundlesReorderableList;
	private bool m_BundlesFoldout;

	private void OnEnable()
	{
		m_AssetsReorderableList = new ReorderableList(AssetsCacher.AssetRefs, typeof(AssetRef), false, true, false, false);
		m_AssetsReorderableList.elementHeight = 20;
		m_AssetsReorderableList.drawElementCallback = DrawAssetRefElement;
		m_AssetsReorderableList.drawHeaderCallback = DrawAssetRefHead;

		m_BundlesReorderableList = new ReorderableList(AssetBundleLoader.AssetBundleRefs, typeof(AssetBundleRef), false, true, false, false);
		m_BundlesReorderableList.elementHeight = 20;
		m_BundlesReorderableList.drawElementCallback = DrawAssetBundleRefElement;
		m_BundlesReorderableList.drawHeaderCallback = DrawAssetBundleRefHead;
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		m_AssetsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_AssetsFoldout, "Assets:" + AssetsCacher.AssetRefs.Count);
		if(m_AssetsFoldout)
		{
			m_AssetsReorderableList.DoLayoutList();
		}
		EditorGUILayout.EndFoldoutHeaderGroup();

		m_BundlesFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_BundlesFoldout, "Bundles:" + AssetBundleLoader.AssetBundleRefs.Count);
		if(m_BundlesFoldout)
		{
			m_BundlesReorderableList.DoLayoutList();
		}
		EditorGUILayout.EndFoldoutHeaderGroup();

		serializedObject.ApplyModifiedProperties();
	}

	void DrawAssetRefElement(Rect position, int index, bool isActive, bool isFocused)
	{
		AssetRef assetRef = AssetsCacher.AssetRefs[index];

		EditorGUI.BeginDisabledGroup(true);

		Rect assetRect = new Rect(position.x, position.y, position.width - 20, position.height);
		Rect refCountRect = new Rect(position.x + position.width - 20, position.y, 20, position.height);

		EditorGUI.ObjectField(assetRect, assetRef.Asset, typeof(Object), false);
		EditorGUI.TextField(refCountRect, assetRef.RefCount.ToString());

		EditorGUI.EndDisabledGroup();
	}

	private void DrawAssetRefHead(Rect rect)
	{
		EditorGUI.LabelField(rect, "Assets In Cache");
	}

	void DrawAssetBundleRefElement(Rect position, int index, bool isActive, bool isFocused)
	{
		AssetBundleRef assetBundleRef = AssetBundleLoader.AssetBundleRefs[index];

		EditorGUI.BeginDisabledGroup(true);

		Rect assetRect = new Rect(position.x, position.y, position.width - 20, position.height);
		Rect refCountRect = new Rect(position.x + position.width - 20, position.y, 20, position.height);

		EditorGUI.ObjectField(assetRect, assetBundleRef.Bundle, typeof(Object), false);
		EditorGUI.TextField(refCountRect, assetBundleRef.RefCount.ToString());

		EditorGUI.EndDisabledGroup();
	}

	private void DrawAssetBundleRefHead(Rect rect)
	{
		EditorGUI.LabelField(rect, "AssetBundles Loaded");
	}
}
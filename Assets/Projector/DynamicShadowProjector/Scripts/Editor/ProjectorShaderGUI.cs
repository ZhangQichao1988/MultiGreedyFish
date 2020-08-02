//
// ProjectorShaderGUI.cs
//
// Dynamic Shadow Projector
//
// Copyright 2019 NYAHOON GAMES PTE. LTD. All Rights Reserved.
//

using UnityEngine;
using UnityEditor;

namespace DynamicShadowProjector {
	public class ProjectorShaderGUI : ShaderGUI {
		private enum ProjectorType {
			UnityProjector,
			CustomProjector
		}
		static private string ProjectorTypeToKeyword(ProjectorType type)
		{
			switch (type) {
			case ProjectorType.CustomProjector:
				return "FSR_RECEIVER";
			case ProjectorType.UnityProjector:
				return "";
			}
			return null;
		}
		public override void OnGUI (MaterialEditor materialEditor, MaterialProperty[] properties)
		{
			base.OnGUI (materialEditor, properties);
			Material material = materialEditor.target as Material;
			ProjectorType currentType = ProjectorType.UnityProjector;
			if (material.IsKeywordEnabled ("FSR_RECEIVER")) {
				currentType = ProjectorType.CustomProjector;
			}
			float oldLabelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 150;
			ProjectorType newType = (ProjectorType)EditorGUILayout.EnumPopup("Projector Type", currentType);
			EditorGUIUtility.labelWidth = oldLabelWidth;
			if (newType != currentType) {
				Undo.RecordObject (material, "Change Projector Type");
				string keyword = ProjectorTypeToKeyword (currentType);
				if (!string.IsNullOrEmpty (keyword)) {
					material.DisableKeyword (keyword);
				}
				keyword = ProjectorTypeToKeyword (newType);
				if (!string.IsNullOrEmpty (keyword)) {
					material.EnableKeyword (keyword);
				}
			}
			bool forLWRP = material.IsKeywordEnabled("FSR_PROJECTOR_FOR_LWRP");
#if UNITY_2019_3_OR_NEWER
			bool newLWRP = EditorGUILayout.Toggle("Build for Universal RP", forLWRP);
#else
			bool newLWRP = EditorGUILayout.Toggle("Build for LWRP", forLWRP);
#endif
			if (newLWRP != forLWRP)
			{
				Undo.RecordObject(material, "Change Target Renderpipeline");
				if (newLWRP)
				{
					material.EnableKeyword("FSR_PROJECTOR_FOR_LWRP");
				}
				else
				{
					material.DisableKeyword("FSR_PROJECTOR_FOR_LWRP");
				}
			}
		}
	}
}

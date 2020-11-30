Shader "DynamicShadowProjector/FastShadowReceiver/Dynamic/Mipmapped Shadow For Lightmap Subtractive" {
	Properties {
		_ClipScale ("Near Clip Sharpness", Float) = 100
		_Alpha ("Shadow Darkness", Range (0, 1)) = 1.0
		_AmbientColor ("Ambient Color", Color) = (0.3, 0.3, 0.3, 1.0)
		_Offset ("Offset", Range (-1, -10)) = -1.0
	}
	Subshader {
		Tags {"Queue"="Transparent-1" "IgnoreProjector"="True"}
		UsePass "DynamicShadowProjector/Projector/Mipmapped Shadow For Lightmap Subtractive/PASS"
	}
	CustomEditor "DynamicShadowProjector.ProjectorShaderGUI"
}

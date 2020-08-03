Shader "DynamicShadowProjector/FastShadowReceiver/Dynamic/Mipmapped Shadow For Lightmap Shadowmask" {
	Properties {
		_ClipScale ("Near Clip Sharpness", Float) = 100
		_Alpha ("Shadow Darkness", Range (0, 1)) = 1.0
		_ShadowMaskSelector ("Shadowmask Channel", Vector) = (1,0,0,0) 
		_Offset ("Offset", Range (-1, -10)) = -1.0
	}
	Subshader {
		Tags {"Queue"="Transparent-1" "IgnoreProjector"="True"}
		UsePass "DynamicShadowProjector/Projector/Mipmapped Shadow For Lightmap Shadowmask/PASS"
	}
	CustomEditor "DynamicShadowProjector.ProjectorShaderGUI"
}

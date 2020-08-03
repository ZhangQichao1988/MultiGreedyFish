Shader "DynamicShadowProjector/Projector/Light With Linear Falloff" {
	Properties {
		[NoScaleOffset] _LightTex ("Cookie", 2D) = "gray" {}
		_ClipScale ("Near Clip Sharpness", Float) = 100
		_Alpha ("Light Intensity", Range (0, 1)) = 1.0
		_Offset ("Offset", Range (-1, -10)) = -1.0
	}
	Subshader {
		Tags {"Queue"="Transparent-1"}
		Pass {
			ZWrite Off
			ColorMask RGB
			Blend DstColor One
			Offset -1, [_Offset]

			HLSLPROGRAM
			#pragma vertex DSPProjectorVertLightLinearFalloff
			#pragma fragment DSPProjectorFragLight
			#pragma shader_feature _ FSR_PROJECTOR_FOR_LWRP
			#pragma multi_compile _ FSR_RECEIVER 
			#pragma multi_compile_fog
			#include "DSPProjector.cginc"
			ENDHLSL
		}
	}
	CustomEditor "DynamicShadowProjector.ProjectorShaderGUI"
}

Shader "DynamicShadowProjector/Projector/Shadow With Linear Falloff" {
	Properties {
		[NoScaleOffset] _ShadowTex ("Cookie", 2D) = "gray" {}
		_ClipScale ("Near Clip Sharpness", Float) = 100
		_Alpha ("Shadow Darkness", Range (0, 1)) = 1.0
		_Ambient ("Ambient", Range (0.01, 1)) = 0.3
		_Offset ("Offset", Range (-1, -10)) = -1.0
	}
	Subshader {
		Tags {"Queue"="Transparent-1"}
		Pass {
			Name "PASS"
			ZWrite Off
			ColorMask RGB
			Blend DstColor Zero
			Offset -1, [_Offset]

			HLSLPROGRAM
			#pragma vertex DSPProjectorVertLinearFalloff
			#pragma fragment DSPProjectorFrag
			#pragma shader_feature _ FSR_PROJECTOR_FOR_LWRP
			#pragma multi_compile _ FSR_RECEIVER 
			#pragma multi_compile_fog
			#define DSP_USE_AMBIENT
			#include "DSPProjector.cginc"
			ENDHLSL
		}
	}
	CustomEditor "DynamicShadowProjector.ProjectorShaderGUI"
}

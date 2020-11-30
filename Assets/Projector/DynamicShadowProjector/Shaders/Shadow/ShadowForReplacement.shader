Shader "Hidden/DynamicShadowProjector/Shadow/Replacement" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	}
	HLSLINCLUDE
	#include "DSPShadow.cginc"
	ENDHLSL

	SubShader {
		Tags { "RenderType"="Opaque" }
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode Off }
			Blend One OneMinusSrcAlpha
			HLSLPROGRAM
			#pragma multi_compile _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma vertex DSPShadowVertStandard
			#pragma fragment DSPShadowFragStandard
			ENDHLSL
		}
	}
	SubShader {
		Tags { "RenderType"="Transparent" }
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode Off }
			Blend One OneMinusSrcAlpha
			HLSLPROGRAM
			#pragma vertex DSPShadowVertTrans
			#pragma fragment DSPShadowFragTrans
			ENDHLSL
		}
	} 
	SubShader {
		Tags { "RenderType"="TransparentCutout" }
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode Off }
			Blend One OneMinusSrcAlpha
			HLSLPROGRAM
			#pragma vertex DSPShadowVertTrans
			#pragma fragment DSPShadowFragTrans
			ENDHLSL
		}
	} 
}

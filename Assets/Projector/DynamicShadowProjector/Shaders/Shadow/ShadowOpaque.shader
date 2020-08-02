Shader "DynamicShadowProjector/Shadow/Opaque" {
	SubShader {
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode Off }
		
			HLSLPROGRAM
			#pragma vertex DSPShadowVertOpaque
			#pragma fragment DSPShadowFragOpaque
			#include "DSPShadow.cginc"
			ENDHLSL
		}
	}
}

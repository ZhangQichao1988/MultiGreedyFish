﻿Shader "Projector For LWRP/Light" 
{
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		[NoScaleOffset] _ShadowTex ("Cookie", 2D) = "gray" {}
		[NoScaleOffset] _FalloffTex ("FallOff", 2D) = "white" {}
		_Offset ("Offset", Range (-1, -10)) = -1.0
	}
	SubShader
	{
		Tags {"Queue"="Transparent-1"}
        // Shader code
		Pass
        {
			ZWrite Off
			Fog { Color (1, 1, 1) }
			ColorMask RGB
			Blend DstColor One
			Offset -1, [_Offset]

			HLSLPROGRAM
			#pragma vertex p4lwrp_vert_projector
			#pragma fragment p4lwrp_frag_projector_light
			#pragma shader_feature FSR_PROJECTOR_FOR_LWRP
			#pragma multi_compile _ FSR_RECEIVER
			#pragma multi_compile_fog
			#include "P4LWRP.cginc"
			ENDHLSL
		}
	} 
}

#ifndef UNIVERSAL_UNLIT_INPUT_INCLUDED
#define UNIVERSAL_UNLIT_INPUT_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

CBUFFER_START(UnityPerMaterial)
float4 _BaseMap_ST;
half4 _BaseColor;
half4 _MulColor;

half _Cutoff;

#ifdef _EFFECT1
float4 _EffectMap1_ST;
half4 _EffectColor1;
float4 _EffectMove1;
#endif

#ifdef _EFFECT2
float4 _EffectMap2_ST;
half4 _EffectColor2;
float4 _EffectMove2;
#endif

#ifdef _SHADOW
float _ShadowRange;
#endif

CBUFFER_END

#endif

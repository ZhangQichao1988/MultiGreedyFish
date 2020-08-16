#ifndef UNIVERSAL_PARTICLES_UNLIT_INPUT_INCLUDED
#define UNIVERSAL_PARTICLES_UNLIT_INPUT_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Particles.hlsl"


CBUFFER_START(UnityPerMaterial)
float4 _SoftParticleFadeParams;
float4 _CameraFadeParams;
float4 _BaseMap_ST;
//half _IsSingleChannel;
half2 _BaseMapUVScroll;
half4 _BaseColor;
half _BaseColorPower;
half4 _EmissionColor;
half4 _BaseColorAddSubDiff;
half _Cutoff;
half _DistortionStrengthScaled;
half _DistortionBlend;
float Curvature;


#if defined (_NOISE)
TEXTURE2D_X(_NoiseMap); SAMPLER(sampler_NoiseMap);
float4 _NoiseMap_ST;
half2 _NoiseMapUVScroll;
half _NoisePower;
half _FadeAlongU;
#endif

#if defined (_BLENDMAP)
TEXTURE2D_X(_BlendMap); SAMPLER(sampler_BlendMap);
float4 _BlendMap_ST;
half2 _BlendMapUVScroll;
#endif

#if defined (_ALPHAMAP)
TEXTURE2D_X(_AlphaMap); SAMPLER(sampler_AlphaMap);
float4 _AlphaMap_ST;
half2 _AlphaMapUVScroll;
#endif

#if defined(_DISSOLVE)
TEXTURE2D_X(_DissolveMap); SAMPLER(sampler_DissolveMap);
half _DissSize;
half4 _DissColor, _DissAddColor;
half _DissProcess;
half _DissSmooth;

#endif

CBUFFER_END

#define SOFT_PARTICLE_NEAR_FADE _SoftParticleFadeParams.x
#define SOFT_PARTICLE_INV_FADE_DISTANCE _SoftParticleFadeParams.y

#define CAMERA_NEAR_FADE _CameraFadeParams.x
#define CAMERA_INV_FADE_DISTANCE _CameraFadeParams.y

half4 SampleAlbedo(float2 uv, float2 uvBlendMap, float2 uvAlphaMap, float3 blendUv, half4 color, float4 particleColor, float4 projectedPosition, TEXTURE2D_PARAM(albedoMap, sampler_albedoMap))
{
#if defined(_SINGLE_CHANNEL)
    half4 albedo = half4(color.rgb, BlendTexture(TEXTURE2D_ARGS(albedoMap, sampler_albedoMap), uv, blendUv).r);
#else
    half4 albedo = BlendTexture(TEXTURE2D_ARGS(albedoMap, sampler_albedoMap), uv, blendUv) * color;
#endif
#if defined(_BASECOLOR_POWER)
    albedo *= _BaseColorPower;
#endif

#if defined(_BLENDMAP)
    half3 blend = BlendTexture(TEXTURE2D_ARGS(_BlendMap, sampler_BlendMap), uvBlendMap, blendUv).rgb;
    albedo = half4(albedo.rgb + blend, albedo.a);
#endif

#if defined(_ALPHAMAP)
    half alpha = BlendTexture(TEXTURE2D_ARGS(_AlphaMap, sampler_AlphaMap), uvAlphaMap, blendUv).r;
    albedo.a *= alpha;
#endif


    // No distortion Support
    half4 colorAddSubDiff = half4(0, 0, 0, 0);
#if defined (_COLORADDSUBDIFF_ON)
    colorAddSubDiff = _BaseColorAddSubDiff;
#endif
    albedo = MixParticleColor(albedo, particleColor, colorAddSubDiff);

    AlphaDiscard(albedo.a, _Cutoff);


    albedo.rgb = AlphaModulate(albedo.rgb, albedo.a);

#if defined(_SOFTPARTICLES_ON)
    albedo = SOFT_PARTICLE_MUL_ALBEDO(albedo, SoftParticles(SOFT_PARTICLE_NEAR_FADE, SOFT_PARTICLE_INV_FADE_DISTANCE, projectedPosition));
#endif

#if defined(_FADING_ON)
    ALBEDO_MUL *= CameraFade(CAMERA_NEAR_FADE, CAMERA_INV_FADE_DISTANCE, projectedPosition);
#endif


    return albedo;
}

#endif // UNIVERSAL_PARTICLES_PBR_INCLUDED

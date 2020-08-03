#if !defined(DSP_MIPMAPPEDSHADPW_CGINC_INCLUDED)
#define DSP_MIPMAPPEDSHADPW_CGINC_INCLUDED

#include "DSPProjector.cginc"

struct DSP_V2F_PROJECTOR_LIGHTMAPUV {
	float4 uvShadow         : TEXCOORD0;
	half4  lightmapuv_alpha : TEXCOORD1;
	UNITY_FOG_COORDS(2)
	float4 pos : SV_POSITION;
};


#if UNITY_VERSION < 201800 && !defined(SHADOWS_SHADOWMASK)
UNITY_DECLARE_TEX2D(unity_ShadowMask);
#endif

float _SizeScale;

DSP_V2F_PROJECTOR DSPProjectorVertMipmap(float4 vertex : POSITION, float3 normal : NORMAL)
{
	DSP_V2F_PROJECTOR o;

		float z;
		float y;

	fsrTransformVertex(vertex, o.pos, o.uvShadow);
	z = o.uvShadow.z;
	o.uvShadow.z = _DSPMipLevel * z;
	o.alpha.x = _ClipScale * z;
	o.alpha.y = DSPCalculateDiffuseShadowAlpha(vertex, normal);
	UNITY_TRANSFER_FOG(o, o.pos);
	return o;
}

DSP_V2F_PROJECTOR_LIGHTMAPUV DSPProjectorVertMipmapForLightmap(float4 vertex : POSITION, float3 normal : NORMAL, float2 lightmapUV : TEXCOORD1)
{
	DSP_V2F_PROJECTOR_LIGHTMAPUV o;
	fsrTransformVertex(vertex, o.pos, o.uvShadow);
	float z = o.uvShadow.z;
	o.uvShadow.z = _DSPMipLevel * z;
	o.lightmapuv_alpha.xy = lightmapUV * unity_LightmapST.xy + unity_LightmapST.zw;
	o.lightmapuv_alpha.z = _ClipScale * z;
	o.lightmapuv_alpha.w = -dot(normal, fsrProjectorDir());
	UNITY_TRANSFER_FOG(o, o.pos);
	return o;
}

fixed4 DSPGetMipmappedShadowTex(float4 uvShadow)
{
	float3 uv;
	uv.xy = saturate(uvShadow.xy/uvShadow.w);
	uv.z = uvShadow.z;
	fixed4 ret = tex2Dlod(_ShadowTex, uv.xyzz);
	ret = pow(ret, _SizeScale);
	return ret;
}

fixed3 DSPCalculateSubtractiveShadow(fixed3 shadowColor, half2 lightmapUV, half ndotl, fixed falloff)
{
	half4 bakedColorTex = UNITY_SAMPLE_TEX2D(unity_Lightmap, lightmapUV);
	FSR_LIGHTCOLOR3 bakedColor = DecodeLightmap(bakedColorTex);
	FSR_LIGHTCOLOR3 mainLight = FSR_MAINLIGHTCOLOR * saturate(ndotl);
	FSR_LIGHTCOLOR3 ambientColor = max(bakedColor - mainLight, _AmbientColor.rgb);
	shadowColor = lerp(fixed3(1,1,1), shadowColor, saturate(_Alpha * falloff));
	return saturate((shadowColor * (bakedColor - ambientColor) + ambientColor) / bakedColor);
}

fixed3 DSPCalculateShadowmaskShadow(fixed3 shadowColor, half2 lightmapUV, half ndotl, fixed falloff)
{
	fixed4 shadowmask = UNITY_SAMPLE_TEX2D(unity_ShadowMask, lightmapUV);
	shadowmask = saturate(dot(shadowmask, _ShadowMaskSelector));
	half4 bakedColorTex = UNITY_SAMPLE_TEX2D(unity_Lightmap, lightmapUV);
	FSR_LIGHTCOLOR3 bakedColor = DecodeLightmap(bakedColorTex);
	FSR_LIGHTCOLOR3 mainLight = FSR_MAINLIGHTCOLOR * saturate(shadowmask*ndotl);
	fixed3 shadow = saturate((_Alpha * falloff * mainLight)/(bakedColor + mainLight));
	return lerp(fixed3(1,1,1), shadowColor, shadow);
}

fixed4 DSPProjectorFragMipmap(DSP_V2F_PROJECTOR i) : SV_Target
{
	fixed4 shadow = DSPGetMipmappedShadowTex(i.uvShadow);
	return DSPCalculateFinalShadowColor(shadow, i);
}

fixed4 DSPProjectorFragMipmapForLightmapSubtractive(DSP_V2F_PROJECTOR_LIGHTMAPUV i) : SV_Target
{
	fixed4 shadow = DSPGetMipmappedShadowTex(i.uvShadow);
	shadow.rgb = DSPCalculateSubtractiveShadow(shadow.rgb, i.lightmapuv_alpha.xy, i.lightmapuv_alpha.w, saturate(i.lightmapuv_alpha.z));

	UNITY_APPLY_FOG_COLOR(i.fogCoord, shadow, fixed4(1,1,1,1));
	return shadow;
}

fixed4 DSPProjectorFragMipmapForLightmapShadowmask(DSP_V2F_PROJECTOR_LIGHTMAPUV i) : SV_Target
{
	fixed4 shadow = DSPGetMipmappedShadowTex(i.uvShadow);
	shadow.rgb = DSPCalculateShadowmaskShadow(shadow.rgb, i.lightmapuv_alpha.xy, i.lightmapuv_alpha.w, saturate(i.lightmapuv_alpha.z));

	UNITY_APPLY_FOG_COLOR(i.fogCoord, shadow, fixed4(1,1,1,1));
	return shadow;
}

#endif // !defined(DSP_MIPMAPPEDSHADPW_CGINC_INCLUDED)

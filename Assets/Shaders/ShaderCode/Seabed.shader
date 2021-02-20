Shader "Custom/Seabed"
{
    Properties
    {
        _BaseMap("Texture", 2D) = "white" {}
        _BaseColor("Color", Color) = (1, 1, 1, 1)
        _Cutoff("AlphaCutout", Range(0.0, 1.0)) = 0.1

        _EffectMap1("EffectMap1", 2D) = "white" {}
        _EffectColor1("EffectColor1", Color) = (1, 1, 1, 1)
        _EffectMove1("_EffectMove1", Vector) = (1, 1, 0, 0)

        _EffectMap2("EffectMap2", 2D) = "white" {}
        _EffectColor2("EffectColor2", Color) = (1, 1, 1, 1)
        _EffectMove2("_EffectMove2", Vector) = (1, 1, 0, 0)

        _ShadowRange("ShadowRange", Float) = 0.0

            // BlendMode
            [HideInInspector] _Surface("__surface", Float) = 0.0
            [HideInInspector] _Blend("__blend", Float) = 0.0
            [HideInInspector] _AlphaClip("__clip", Float) = 0.0
            [HideInInspector] _SrcBlend("Src", Float) = 1.0
            [HideInInspector] _DstBlend("Dst", Float) = 0.0
            [HideInInspector] _ZWrite("ZWrite", Float) = 1.0
            [HideInInspector] _Cull("__cull", Float) = 2.0

            // Editmode props
            [HideInInspector] _QueueOffset("Queue offset", Float) = 0.0

            // ObsoleteProperties
            [HideInInspector] _MainTex("BaseMap", 2D) = "white" {}
            [HideInInspector] _Color("Base Color", Color) = (0.5, 0.5, 0.5, 1)
            [HideInInspector] _SampleGI("SampleGI", float) = 0.0 // needed from bakedlit
    }
        SubShader
            {
                Tags { "RenderType" = "Opaque" "IgnoreProjector" = "True" "RenderPipeline" = "UniversalPipeline" }
                LOD 100


                Blend SrcAlpha OneMinusSrcAlpha
                ZWrite On
                //Cull [_Cull]

                Pass
                {
                    Name "Unlit"
                    HLSLPROGRAM
                // Required to compile gles 2.0 with standard srp library
                #pragma prefer_hlslcc gles
                #pragma exclude_renderers d3d11_9x

                #pragma vertex vert
                #pragma fragment frag
                #pragma shader_feature _ALPHATEST_ON
                #pragma shader_feature _ALPHAPREMULTIPLY_ON

                #pragma multi_compile _EFFECT1
                #pragma multi_compile _EFFECT2 
                #pragma multi_compile _SHADOW 

                #pragma multi_compile _NOISE
                // -------------------------------------
                // Unity defined keywords
                #pragma multi_compile_fog
                #pragma multi_compile_instancing
                 
                #include "UnlitInput.hlsl"

                struct Attributes
                {
                    float4 positionOS       : POSITION;
                    float2 uv               : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct Varyings
                {
                    float2 uv        : TEXCOORD0;
                    float2 effectUv1        : TEXCOORD1;
                    float2 effectUv2        : TEXCOORD2;
                    float2 noiseUv          : TEXCOORD3;
                    float fogCoord : TEXCOORD4;
                    float3 vertexWS        : TEXCOORD5;
                    float4 vertex : SV_POSITION;


                    UNITY_VERTEX_INPUT_INSTANCE_ID
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                Varyings vert(Attributes input)
                {
                    Varyings output = (Varyings)0;

                    UNITY_SETUP_INSTANCE_ID(input);
                    UNITY_TRANSFER_INSTANCE_ID(input, output);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                    output.vertex = vertexInput.positionCS;
                    output.vertexWS = vertexInput.positionWS;
                    output.uv = TRANSFORM_TEX(input.uv, _BaseMap);

                    _EffectMap1_ST.zw += _EffectMove1.xy * _Time.y;
                    output.effectUv1 = TRANSFORM_TEX(input.uv, _EffectMap1);

                    _EffectMap2_ST.zw += _EffectMove2.xy * _Time.y;
                    output.effectUv2 = TRANSFORM_TEX(input.uv, _EffectMap2);

                    output.fogCoord = ComputeFogFactor(vertexInput.positionCS.z);

                    return output;
                }

                half4 frag(Varyings input) : SV_Target
                {
                    UNITY_SETUP_INSTANCE_ID(input);
                    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                    half2 uv = input.uv;
                    half4 texColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);

                    half noise = SAMPLE_TEXTURE2D(_NoiseMap, sampler_NoiseMap, input.noiseUv);

                    half4 effectColor1 = SAMPLE_TEXTURE2D(_EffectMap1, sampler_EffectMap1, input.effectUv1);
                    effectColor1 *= _EffectColor1;

                    half4 effectColor2 = SAMPLE_TEXTURE2D(_EffectMap2, sampler_EffectMap2, input.effectUv2);
                    effectColor2 *= _EffectColor2;

                    half3 color = texColor * _BaseColor.rgb + effectColor1 * effectColor2;
                    half shadow = smoothstep(_ShadowRange, _ShadowRange - 2, abs(input.vertexWS.x));
                    color *= shadow;
                    shadow = smoothstep(_ShadowRange, _ShadowRange - 2, abs(input.vertexWS.z));
                    color *= shadow;

                    half alpha = texColor.a * _BaseColor.a;
                    AlphaDiscard(alpha, _Cutoff);

    #ifdef _ALPHAPREMULTIPLY_ON
                    color *= alpha;
    #endif

                    color = MixFog(color, input.fogCoord);

                    return half4(color, alpha);
                }
                ENDHLSL
            }
                //Pass
                //{
                //    Name "ShadowCaster"
                //    Tags{"LightMode" = "ShadowCaster"}

                //    ZWrite On
                //    ZTest LEqual
                //    Cull[_Cull]

                //    HLSLPROGRAM
                //        // Required to compile gles 2.0 with standard srp library
                //        #pragma prefer_hlslcc gles
                //        #pragma exclude_renderers d3d11_9x
                //        #pragma target 2.0

                //        // -------------------------------------
                //        // Material Keywords
                //        #pragma shader_feature _ALPHATEST_ON

                //        //--------------------------------------
                //        // GPU Instancing
                //        #pragma multi_compile_instancing
                //        #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

                //        #pragma vertex ShadowPassVertex
                //        #pragma fragment ShadowPassFragment

                //        #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
                //        #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
                //        ENDHLSL
                //    }
                //Pass
                //{
                //    Tags{"LightMode" = "DepthOnly"}

                //    ZWrite On
                //    ColorMask 0

                //    HLSLPROGRAM
                //    // Required to compile gles 2.0 with standard srp library
                //    #pragma prefer_hlslcc gles
                //    #pragma exclude_renderers d3d11_9x
                //    #pragma target 2.0

                //    #pragma vertex DepthOnlyVertex
                //    #pragma fragment DepthOnlyFragment

                //    // -------------------------------------
                //    // Material Keywords
                //    #pragma shader_feature _ALPHATEST_ON

                //    //--------------------------------------
                //    // GPU Instancing
                //    #pragma multi_compile_instancing

                //    #include "UnlitInput.hlsl"
                //    #include "DepthOnlyPass.hlsl"
                //    ENDHLSL
                //}

                // This pass it not used during regular rendering, only for lightmap baking.
                Pass
                {
                    Name "Meta"
                    Tags{"LightMode" = "Meta"}

                    Cull Off

                    HLSLPROGRAM
                    // Required to compile gles 2.0 with standard srp library
                    #pragma prefer_hlslcc gles
                    #pragma exclude_renderers d3d11_9x
                    #pragma vertex UniversalVertexMeta
                    #pragma fragment UniversalFragmentMetaUnlit

                    #include "UnlitInput.hlsl"
                    #include "UnlitMetaPass.hlsl"

                    ENDHLSL
                }
            }
                FallBack "Hidden/Universal Render Pipeline/FallbackError"
                    //CustomEditor "UnityEditor.Rendering.Universal.ShaderGUI.UnlitShader"
}

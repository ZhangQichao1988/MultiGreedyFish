



Shader "Custom/PoisonRing"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BaseColor("Color", Color) = (1, 1, 1, 1)

        _NoiseTex("NoiseTexture", 2D) = "white" {}
        _NoisePow("NoisePow", Float) = 0.0
        _RotSpeed("RotSpeed", Float) = 0.0
        _SafeAreaRange("SafeAreaRange", Float) = 0.0
        _SafeGradAreaRange("SafeGradAreaRange", Float) = 0.0
        /*_HeightTiling("HeightTiling", Float) = 0.0
        _Height("Height", Float) = 0.0*/
            
    }
    SubShader
    {
        Tags{"RenderType" = "Opaque" "IgnoreProjector" = "True" "PreviewType" = "Plane" "PerformanceChecks" = "False" "RenderPipeline" = "UniversalPipeline"}
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            //#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
//#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Version.hlsl"


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                //half length : TEXCOORD2;
                float3 positionWS : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;

            half _SafeAreaRange;
            half _SafeGradAreaRange;
            half4 _BaseColor;
            fixed _RotSpeed;
            fixed _NoisePow;
            /*float _HeightTiling;
            fixed _Height;*/
            v2f vert (appdata v)
            {
                v2f o;
                //v.vertex.y = sin((v.vertex.x + v.vertex.z) * _HeightTiling) * _Height;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.positionWS = mul(UNITY_MATRIX_M, v.vertex);
                 
                
                o.uv = TRANSFORM_TEX(v.uv, _NoiseTex);
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o; 
            } 

            fixed4 frag(v2f i) : SV_Target
            {
                half len = length(i.positionWS);

                float clipCut = smoothstep(_SafeGradAreaRange, _SafeAreaRange, len);
                clip(clipCut - 0.001);
                

                fixed2 uvOffset = (tex2D(_NoiseTex, i.uv).r - 0.5) * _NoisePow;
                 
                float2 uv = float2(i.positionWS.x, i.positionWS.z);
                uv = TRANSFORM_TEX(uv, _MainTex) + uvOffset;
                fixed angle = _RotSpeed * _Time.y;
                float c = cos(angle);
                float s = sin(angle);
                uv = mul(float2x2(c, -s, s, c), uv);
                
                fixed4 col = tex2D(_MainTex, uv);

                col *= _BaseColor;
                col.a *= clipCut;

                // apply fog
                return col;
            }
            ENDCG
        }
    }
}

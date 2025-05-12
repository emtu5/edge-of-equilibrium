// does this work..????

Shader "Instanced/GridTestParticleShader_URP"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Color("Color", Color) = (0.25, 0.5, 0.5, 1)
        _DensityRange("Density Range", Range(0,500000)) = 1.0
        _Size("Size", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode"="UniversalForward" }
            Blend SrcAlpha OneMinusSrcAlpha
            // ZWrite Off


            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:setup


            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // TEXTURE2D(_MainTex);
            // SAMPLER(sampler_MainTex);
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;


            float _Glossiness;
            float _Metallic;
            float4 _Color;
            float _DensityRange;
            float _Size;

            struct Particle
            {
                float pressure;
                float density;
                float3 currentForce;
                float3 velocity;
                float3 position;
            };

            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                StructuredBuffer<Particle> _particlesBuffer;
            #endif

            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                void setup() { }
            #endif


            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
                float2 uv       : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                float4 localPos = IN.vertex;
                localPos.xyz *= _Size;

                float3 particlePos = float3(0.0, 0.0, 0.0);
                #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                    particlePos = _particlesBuffer[unity_InstanceID].position;
                #endif

                localPos.xyz += particlePos;

                float4 worldPos = mul(unity_ObjectToWorld, localPos);
                OUT.position = TransformWorldToHClip(worldPos.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 Frag(Varyings IN) : SV_Target
            {
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                half3 finalColor = _Color.rgb * texColor.rgb;
                return half4(finalColor, .3);
            }

            ENDHLSL
        }
    }

    FallBack "Universal Forward"
}

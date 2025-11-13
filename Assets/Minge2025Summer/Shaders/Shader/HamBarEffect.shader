Shader "Custom/HamBarEffect"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Amplitude ("Amplitude", Range(0, 0.1)) = 0.02
        _Frequency ("Frequency", Range(1, 100)) = 20
        _Speed ("Scroll Speed", Range(-10, 10)) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 3_0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;
            float _Amplitude;
            float _Frequency;
            float _Speed;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 Frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                float wave = sin(uv.y * _Frequency + _Time.y * _Speed);
                float horizontalOffset = wave * _Amplitude;
                float2 distortedUV = float2(uv.x + horizontalOffset, uv.y);
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, distortedUV);
                return color;
            }

            ENDHLSL
        }
    }
}

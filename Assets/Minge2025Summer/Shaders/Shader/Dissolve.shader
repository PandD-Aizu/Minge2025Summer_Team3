Shader "Custom/Dissolve"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        
        [Header(Shape)]
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _NoiseTiling ("Noise Tiling", Float) = 1.0
        _NoiseStrength ("Noise Strength", Range(0, 1)) = 0.2
        _ScrollSpeed ("Noise Scroll Speed", Vector) = (0.1, 0.1, 0, 0)
        
        [Header(Warping)]
        _WarpStrength ("Noise Warp Strength", Range(0, 0.5)) = 0.1
        _WarpTiling ("Noise Warp Tiling", Float) = 0.3
        _WarpScrollSpeed ("Noise Warp Scroll Speed", Vector) = (0.05, 0.05, 0, 0)
        
        [Header(Colors and Transition)]
        [HDR] _EdgeColor ("Edge Color (The band)", Color) = (0.1, 0, 0.2, 1)
        [HDR] _CoverColor ("Cover Color (The fill)", Color) = (0, 0, 0, 1)
        _EdgeWidth ("Edge Thickness", Range(0, 0.5)) = 0.1
        _EdgeBlur ("Edge Blur", Range(0.001, 0.5)) = 0.05
        
        [Header(Control)]
        _Progress ("Progress (0 = Color, 1 = Covered)", Range(0, 1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

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
            
            sampler2D _NoiseTex;

            CBUFFER_START(UnityPerMaterial)
                float4 _EdgeColor;
                float4 _CoverColor;
                float _NoiseTiling;
                float _NoiseStrength;
                float4 _ScrollSpeed;

                float _WarpStrength;
                float _WarpTiling;
                float4 _WarpScrollSpeed;
            
                float _EdgeWidth;
                float _EdgeBlur;
                float _Progress;
            CBUFFER_END

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float2 warpOffset = _Time.y * _WarpScrollSpeed.xy;
                float2 warpUV = (IN.uv * _WarpTiling) + warpOffset;

                float warpNoise = tex2D(_NoiseTex, warpUV).r * 3.14159 * 2.0;
                float2 warpVector = float2(cos(warpNoise), sin(warpNoise)) * _WarpStrength;

                float2 scrollOffset = _Time.y * _ScrollSpeed.xy;
                float2 noiseUV = (IN.uv * _NoiseTiling) + scrollOffset + warpVector;

                float noise = tex2D(_NoiseTex, noiseUV).r;
                float radial = distance(IN.uv, float2(0.5, 0.5)) * 2.0;
                float dissolveMap = radial + (noise - 0.5) * _NoiseStrength;

                float threshold_inner = lerp(2.0, -1.0 - _EdgeWidth - _EdgeBlur, _Progress);
                float threshold_outer = threshold_inner + _EdgeWidth;

                float inner_mix = smoothstep(threshold_inner - _EdgeBlur, threshold_inner + _EdgeBlur, dissolveMap);
                float outer_mix = smoothstep(threshold_outer - _EdgeBlur, threshold_outer + _EdgeBlur, dissolveMap);

                half4 color = half4(0, 0, 0, 0);
                color = lerp(color, _EdgeColor, inner_mix);
                color = lerp(color, _CoverColor, outer_mix);

                return color;
            }
            
            ENDHLSL
        }
    }
}
Shader "Unlit/PixelationEffect"
{
    Properties
    {
        _PixelationColor ("Pixelation Color", Color) = (1,1,1,1)
        _Intensity ("Tint Intensity", Range(0,1)) = 0.0
        _PixelSize ("Pixel Size (Screen Pixels)", Range(1,512)) = 8
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            
            SAMPLER(sampler_BlitTexture);

            float4 _PixelationColor;
            float  _Intensity;
            float  _PixelSize;

            float2 QuantizeUV(float2 uv)
            {
                float2 screenSize = _ScreenParams.xy;
                float2 blockCount = max(1.0, screenSize / _PixelSize);
                return floor(uv * blockCount) / blockCount;
            }

            half4 Frag (Varyings i) : SV_Target
            {
                float2 qUV = QuantizeUV(i.texcoord);
                half4 baseCol = SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, qUV);
                
                half3 tinted = lerp(baseCol.rgb, _PixelationColor.rgb, saturate(_Intensity) * _PixelationColor.a);

                return half4(tinted, baseCol.a);
            }
            ENDHLSL
        }
    }
}
